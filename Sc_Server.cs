using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using System.IO;
using UnityEngine.UI;
using System.Threading;
using System.Data;
using System.Threading.Tasks;
using System.Drawing;

public class Sc_Server : MonoBehaviour
{
    public InputField portInputField, roomInputField, nickInputField;
    public string portInput, roomInput, nickInput;

    public List<ServerClient> clients;
    List<ServerClient> disconList;

    TcpListener server;
    private Thread listenerThread;
    public bool serverStarted;
    const int MAXPORTNUM = 65535;

    public static Sc_Server Instance;

    public RoomDeck serverRoomDeck;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void OnHostButton()
    {
        Sc_Chat.Instance.ClearMessageAndUser();
        portInput = portInputField.text;
        roomInput = roomInputField.text;
        nickInput = nickInputField.text;
        ServerCreate();
        GameObject.FindObjectOfType<Sc_Client>().OnHostButton(GetInternalIP(),portInput,nickInput);
    }

    public void OnDisconUserRefreshDeck(string userName)
    {
        RoomDeck newRoomDeckInfo = null;
        if (serverRoomDeck.leftUserName == userName)
        {
            newRoomDeckInfo = new RoomDeck(serverRoomDeck, true);
        }
        else if (serverRoomDeck.rightUserName == userName)
        {
            newRoomDeckInfo = new RoomDeck(serverRoomDeck, false);
        }

        if (newRoomDeckInfo != null)
        {
            serverRoomDeck = newRoomDeckInfo;
        }
    }

    public void ServerCreate()
    {
        clients = new List<ServerClient>();
        disconList = new List<ServerClient>();

        try
        {
            serverRoomDeck = new RoomDeck();

            int port = portInput == "" ? 7777 : int.Parse(portInput) > MAXPORTNUM ? MAXPORTNUM : int.Parse(portInput);
            server = new TcpListener(IPAddress.Any, port);
            server.Start();
            serverStarted = true;
            StartListening();
            Sc_Chat.Instance.ShowMessage($"서버가 {port}에서 시작되었습니다.");
            Sc_Chat.Instance.ShowMessage($"현재 Local IP : {GetInternalIP()}");
            Sc_Chat.Instance.ShowMessage($"현재 Public IP : {GetPublicIP()}");
        }
        catch (Exception e)
        {
            Sc_Chat.Instance.ShowMessage($"Socket error: {e.Message}");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!serverStarted)
            return;
    
        foreach(ServerClient c in clients)
        {
            // 클라이언트가 여전히 연결되었나?
            if(!IsConnected(c.tcpClient))
            {
                c.tcpClient.Close();
                disconList.Add(c);
                continue;
            }
            // 클라이언트로부터 체크 메시지를 받는다.
            else
            {
                NetworkStream s = c.tcpClient.GetStream();
                if(s.DataAvailable)
                {
                    string data = new StreamReader(s,true).ReadLine();
                    if (data != null)
                        OnIncomingData(c, data);
                }
            }
        }

        for (int i = 0; i < disconList.Count; i++)
        {
            OnDisconUserRefreshDeck(disconList[i].clientName);
            clients.Remove(disconList[i]);
            UserListUpdate($"{disconList[i].clientName} 연결이 끊어졌습니다.");
            disconList.RemoveAt(i);
        }
    }

    bool IsConnected(TcpClient c)
    {
        try
        {
            if(c != null && c.Client != null && c.Client.Connected)
            {
                if (c.Client.Poll(0, SelectMode.SelectRead))
                    return !(c.Client.Receive(new byte[1], SocketFlags.Peek) == 0);

                return true;
            }
            else
            {
                return false;
            }
        }
        catch
        {
            return false;
        }
    }

    void StartListening()
    {
        if(serverStarted)
        {
            // 비동기 리스닝 시작
            server.BeginAcceptTcpClient(AcceptTcpClient, server);
        }
    }

    async void AcceptTcpClient(IAsyncResult ar)
    {
        TcpListener listener = (TcpListener)ar.AsyncState;
        clients.Add(new ServerClient(listener.EndAcceptTcpClient(ar)));
        StartListening();

        // 메세지를 연결된 모두에게 보냄
        await BroadcastAsync("%NAME", new List<ServerClient>() { clients[clients.Count - 1]});
    }

    async void OnIncomingData(ServerClient c, string data)
    {
        if(data.Contains("&NAME"))
        {
            c.clientName = data.Split('|')[1];
            UserListUpdate($"<color=#00FF00>{c.clientName}</color>이 연결되었습니다.");
            return;
        }
        else if (data.Split("|")[0] == "&Deck")
        {
            //left 덱
            if(data.Split("|")[1] == "0")
            {
                // 만약 지금 덱이 활성화 되어 있지 않다면
                if(!serverRoomDeck.isActiveLeftDeck)
                {
                    serverRoomDeck.isActiveLeftDeck = true;
                    serverRoomDeck.leftDeckInfo = Sc_Serializer.Deserialize<DeckInfo>(Convert.FromBase64String(data.Split("|")[2]));
                    serverRoomDeck.leftUserName = c.clientName;
                }
            }
            //right 덱
            else if(data.Split("|")[1] == "1")
            {
                // 만약 지금 덱이 활성화 되어 있지 않다면
                if (!serverRoomDeck.isActiveRightDeck)
                {
                    serverRoomDeck.isActiveRightDeck = true;
                    serverRoomDeck.rightDeckInfo = Sc_Serializer.Deserialize<DeckInfo>(Convert.FromBase64String(data.Split("|")[2]));
                    serverRoomDeck.rightUserName = c.clientName;
                }
            }

            Broadcast($"%Deck|{Convert.ToBase64String(Sc_Serializer.Serialize(serverRoomDeck))}",clients);
        }
        else if(data.Split("|")[0] == "&RoomDeck")
        {
            RoomDeck newRoomDeck = Sc_Serializer.Deserialize<RoomDeck>(Convert.FromBase64String(data.Split("|")[1]));
            // 현재 룸덱에서 취소를 신청한 클라이언트가 전투 참가를 하고 있는지 확인
            if (serverRoomDeck.leftUserName == c.clientName || serverRoomDeck.rightUserName == c.clientName)
            {
                // 취소를 신청한 클라이언트가 아닌 반대편 유저 정보가 같은지 확인, 다르면 취소 버튼을 동시에 누른 경우기 때문에 처리하지 않는다.
                if(serverRoomDeck.leftUserName == newRoomDeck.leftUserName || serverRoomDeck.rightUserName == newRoomDeck.rightUserName)
                {
                    serverRoomDeck = newRoomDeck;
                }
            }
            Broadcast($"%Deck|{Convert.ToBase64String(Sc_Serializer.Serialize(serverRoomDeck))}", clients);
        }
        else if (data.Split("|")[0] == "&GameStart")
        {
            Broadcast($"%GameStart|{Convert.ToBase64String(Sc_Serializer.Serialize(serverRoomDeck))}", clients);
        }
        else
        {
            await BroadcastAsync($"<color=#00FF00>{c.clientName}</color>: {data}", clients);
        }
    }

    void UserListUpdate(string data)
    {
        string userList = "";
        foreach (var cl in clients)
        {
            userList = userList == "" ? cl.clientName : userList + "," + cl.clientName;
        }

        Broadcast($"%USER|{userList}|{data}|{Convert.ToBase64String(Sc_Serializer.Serialize(serverRoomDeck))}", clients);
    }

    async Task BroadcastAsync(string data, List<ServerClient> cl)
    {
        foreach(var c in cl)
        {
            try
            {
                StreamWriter writer = new StreamWriter(c.tcpClient.GetStream());
                await writer.WriteLineAsync(data);
                await writer.FlushAsync();
            }
            catch (Exception e)
            {
                Sc_Chat.Instance.ShowMessage($"유저 {c.clientName} 쓰기 에러 : {e.Message}");
            }
        }
    }

    void Broadcast(string data, List<ServerClient> cl)
    {
        foreach (var c in cl)
        {
            try
            {
                StreamWriter writer = new StreamWriter(c.tcpClient.GetStream());
                writer.WriteLine(data);
                writer.Flush();
            }
            catch (Exception e)
            {
                Sc_Chat.Instance.ShowMessage($"유저 {c.clientName} 쓰기 에러 : {e.Message}");
            }
        }
    }



    public static string GetInternalIP()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());

        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }

        return "IPv4 주소를 찾을 수 없습니다.";
    }

    private static string GetPublicIP()
    {
        string publicIp = new WebClient().DownloadString("http://ipinfo.io/ip").Trim();

        //null경우 Get Internal IP를 가져오게 한다.
        if (String.IsNullOrWhiteSpace(publicIp))
        {
            publicIp = GetInternalIP();
        }

        return publicIp;
    }

    public void StopServer()
    {
        if (serverStarted)
        {
            Broadcast($"서버가 종료되었습니다.", clients);

            // 모든 클라이언트 연결 닫기
            foreach (var client in clients)
            {
                client.tcpClient.Close();
            }
            // TcpListener 닫기
            server.Stop();
            serverStarted = false;
            // 리소스 정리
            clients.RemoveAll(item => true); ;
            disconList.RemoveAll(item => true); ;


            foreach (var client in clients)
            {
                Debug.Log(client.clientName);
            }
        }
    }
}

public class ServerClient
{
    public TcpClient tcpClient;
    public string clientName;

    public ServerClient(TcpClient clientSocket)
    {
        clientName = "Guest";
        tcpClient = clientSocket;
    }
}
