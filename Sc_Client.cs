using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class Sc_Client : MonoBehaviour
{
    public InputField ipInputField, portInputField, nickInputField;
    public string ipInput, portInput, nickInput;
    public string clientName;
    bool socketReady;
    TcpClient socket;
    NetworkStream stream;
    StreamWriter writer;
    StreamReader reader;

    public static Sc_Client Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }


    public void OnJoinButton()
    {
        Sc_Chat.Instance.ClearMessageAndUser();
        ipInput = ipInputField.text;
        portInput = portInputField.text;
        nickInput = nickInputField.text;
        ConnectToServer();
    }

    public void OnHostButton(string ip, string port, string nick)
    {
        ipInput = ip;
        portInput = port;
        nickInput = nick;
        ConnectToServer();
    }

    public void ConnectToServer()
    {
        // 이미 연결되어있다면 리턴
        if (socketReady)
            return;

        // 기본 ip, 포트번호 지정
        string ip = ipInput == "" ? "127.0.0.1" : ipInput;
        int port = portInput == "" ? 7777 : int.Parse(portInput);

        try
        {
            socket = new TcpClient(ip, port);
            stream = socket.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);
            socketReady = true;
        }
        catch(Exception e)
        {
            Sc_Chat.Instance.ShowMessage($"소켓에러 : {e.Message}");
        }
    }


    // Update is called once per frame
    void Update()
    {
        if(socketReady && stream.DataAvailable)
        {
            string data = reader.ReadLine();
            if (data != null)
                OnIncomingData(data);
        }
    }

    void OnIncomingData(string data)
    {
        if(data == "%NAME")
        {
            clientName = nickInput == "" ? "Guest" + UnityEngine.Random.Range(1000, 10000) : nickInput;
            Send($"&NAME|{clientName}");
            return;
        }
        else if(data.Split("|")[0] == "%USER")
        {
            Sc_Chat.Instance.ShowUserName(data.Split("|")[1].Replace(",","\n"));
            Sc_Chat.Instance.ShowMessage(data.Split("|")[2]);
            Sc_RoomDeck.Instance.roomDeckInfo = Sc_Serializer.Deserialize<RoomDeck>(Convert.FromBase64String(data.Split("|")[3]));
            Sc_RoomDeck.Instance.OnDeckChanged();
        }
        else if (data.Split("|")[0] == "%Deck")
        {
            Sc_RoomDeck.Instance.roomDeckInfo = Sc_Serializer.Deserialize<RoomDeck>(Convert.FromBase64String(data.Split("|")[1]));
            Sc_RoomDeck.Instance.OnDeckChanged();
        }
        else if(data.Split("|")[0] == "%GameStart")
        {
            Sc_RoomDeck.Instance.roomDeckInfo = Sc_Serializer.Deserialize<RoomDeck>(Convert.FromBase64String(data.Split("|")[1]));
            GameObject.FindObjectOfType<Sc_Title>().ShowGamePlayUIFromRoom();
            Sc_GamePlay.Instance.GameStart();
        }
        else
        {
            Sc_Chat.Instance.ShowMessage(data);
        }
    }

    void Send(string data)
    {
        if (!socketReady)
            return;
        writer.WriteLine(data);
        writer.Flush();
    }

    public void OnSendButton(InputField SendInput)
    {
        #if (UNITY_EDITOR || UNITY_STANDALONE)
        if (!Input.GetButtonDown("Submit")) return;
        SendInput.ActivateInputField();
        #endif
        if (SendInput.text.Trim() == "") return;

        string message = SendInput.text;
        SendInput.text = "";
        Send(message);
    }

    /// <summary>
    /// 방에서 전투 참가용 덱을 눌렀을 경우
    /// </summary>
    /// <param name="leftRight">0 = Left, 1 = Right</param>
    public void OnDeckButton(int leftRight)
    {
        // 자신의 덱 정보를 직렬화
        byte[] data = Sc_Serializer.Serialize(Sc_PlayerInfo.Instance.deckInfo);
        // 직렬화 된 데이터를 string 데이터로 변환한 후 송신
        Send($"&Deck|{leftRight}|{Convert.ToBase64String(data)}");
    }

    /// <summary>
    /// 방에서 전투 참가를 취소하는 버튼을 눌렀을 경우
    /// </summary>
    /// <param name="newRoomdeck">새로 바꿀 방의 덱 정보</param>
    public void OnCancelButton(RoomDeck newRoomdeck)
    {
        // 자신 방의 덱 정보를 직렬화
        byte[] data = Sc_Serializer.Serialize(newRoomdeck);
        // 직렬화 된 데이터를 string 데이터로 변환한 후 송신
        Send($"&RoomDeck|{Convert.ToBase64String(data)}");
    }

    /// <summary>
    /// 양쪽 덱이 준비 완료되고 호스트가 게임 시작 버튼을 눌렀을 경우
    /// </summary>
    public void OnGameStart()
    {
        Send($"&GameStart");
    }


    private void OnApplicationQuit()
    {
        CloseSocket();
    }

    public void CloseSocket()
    {
        if (!socketReady)
            return;

        writer.Close();
        reader.Close();
        socket.Close();
        socketReady = false;
    }
}
