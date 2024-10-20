using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Sc_FighterData;

[Serializable]
public class RoomDeck
{
    public bool isActiveLeftDeck;
    public string leftUserName;
    public DeckInfo leftDeckInfo;

    public bool isActiveRightDeck;
    public DeckInfo rightDeckInfo;
    public string rightUserName;

    public RoomDeck()
    {
        isActiveLeftDeck = false;
        isActiveRightDeck = false;
        leftUserName = "";
        rightUserName = "";
    }

    /// <summary>
    /// 현재 룸덱 정보를 받고 반대편 방향의 데이터를 비우는 생성자
    /// </summary>
    /// <param name="inRoomDeck">현재 룸덱</param>
    /// <param name="isLeft">true = 왼쪽 덱이라면</param>
    public RoomDeck(RoomDeck inRoomDeck, bool isLeft)
    {
        if(isLeft)
        {
            isActiveLeftDeck = false;
            leftDeckInfo = null;
            leftUserName = "";

            isActiveRightDeck = inRoomDeck.isActiveRightDeck;
            rightDeckInfo = inRoomDeck.rightDeckInfo;
            rightUserName = inRoomDeck.rightUserName;
        }
        else
        {
            isActiveLeftDeck = inRoomDeck.isActiveLeftDeck;
            leftDeckInfo = inRoomDeck.leftDeckInfo;
            leftUserName = inRoomDeck.leftUserName;

            isActiveRightDeck = false;
            rightDeckInfo = null;
            rightUserName = "";
        }
    }
}
public class Sc_RoomDeck : MonoBehaviour 
{
    public RoomDeck roomDeckInfo;

    public GameObject leftShowDeck;
    public Animator[] animLeftFighters;
    public Text leftText;
    public GameObject leftButton;

    public GameObject rightShowDeck;
    public Animator[] animRightFighters;
    public Text rightText;
    public GameObject rightButton;

    public static Sc_RoomDeck Instance;

    public GameObject fightStartButton;
    public GameObject fightCancelButton;

    public int[] fighterIDListForRightDeck;

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


    public void OnDeckChanged()
    {

        leftShowDeck.SetActive(roomDeckInfo.isActiveLeftDeck);
        if (roomDeckInfo.isActiveLeftDeck)
        {
            for (int i = 0; i < animLeftFighters.Length; i++)
            {

                // 달리기 애니메이션을 위한 애니메이션 컨트롤러 할당
                animLeftFighters[i].runtimeAnimatorController = Sc_FighterDataLib.Instance
                    .GetFighterRuntimeAnimCon(roomDeckInfo.leftDeckInfo.fighterID[i]);
                animLeftFighters[i].SetInteger("AnimState", (int)EFighterState.RUN);

                // UI 크기 조정
                animLeftFighters[i].gameObject.transform.localPosition = Sc_FighterDataLib.Instance.GetFighterUIPosition(roomDeckInfo.leftDeckInfo.fighterID[i]);
                animLeftFighters[i].gameObject.transform.localScale = Sc_FighterDataLib.Instance.GetFighterUIScale(roomDeckInfo.leftDeckInfo.fighterID[i]);
                animLeftFighters[i].GetComponent<SpriteRenderer>().flipX = Sc_FighterDataLib.Instance.GetIsFlip(roomDeckInfo.leftDeckInfo.fighterID[i]);
            }
            leftText.text = roomDeckInfo.leftUserName;
        }
        else
        {
            leftText.text = "";
        }

        rightShowDeck.SetActive(roomDeckInfo.isActiveRightDeck);
        if (roomDeckInfo.isActiveRightDeck)
        {
            for (int i = 0; i < animRightFighters.Length; i++)
            {
                // 달리기 애니메이션을 위한 애니메이션 컨트롤러 할당
                animRightFighters[i].runtimeAnimatorController = Sc_FighterDataLib.Instance
                    .GetFighterRuntimeAnimCon(roomDeckInfo.rightDeckInfo.fighterID[i]);
                animRightFighters[i].SetInteger("AnimState", (int)EFighterState.RUN);

                // 오른쪽 덱 표시일 경우 캐릭터에 따라 위치 조정이 필요하다.
                // 위치 조정이 필요한 ID를 확인하고 위치 조정을 실시한다. 
                if (Array.Exists(fighterIDListForRightDeck, ID => ID == roomDeckInfo.rightDeckInfo.fighterID[i]))
                {
                    Vector3 localPosition = Sc_FighterDataLib.Instance.GetFighterUIPosition(roomDeckInfo.rightDeckInfo.fighterID[i]);
                    localPosition.x = localPosition.x * -1;
                    animRightFighters[i].gameObject.transform.localPosition = localPosition;
                }
                else
                {
                    animRightFighters[i].gameObject.transform.localPosition = Sc_FighterDataLib.Instance.GetFighterUIPosition(roomDeckInfo.rightDeckInfo.fighterID[i]);
                }
                animRightFighters[i].gameObject.transform.localScale = Sc_FighterDataLib.Instance.GetFighterUIScale(roomDeckInfo.rightDeckInfo.fighterID[i]);
                animRightFighters[i].GetComponent<SpriteRenderer>().flipX = !Sc_FighterDataLib.Instance.GetIsFlip(roomDeckInfo.rightDeckInfo.fighterID[i]);
            }
            rightText.text = roomDeckInfo.rightUserName;
        }
        else
        {
            rightText.text = "";
        }
    }
    private void Update()
    {
        if (roomDeckInfo == null)
            return;

        if(roomDeckInfo.isActiveLeftDeck || roomDeckInfo.rightUserName == Sc_Client.Instance.clientName)
        {
            leftButton.SetActive(false);
        }
        else
        {
            leftButton.SetActive(true);
        }

        if(roomDeckInfo.isActiveRightDeck || roomDeckInfo.leftUserName == Sc_Client.Instance.clientName)
        {
            rightButton.SetActive(false);
        }
        else
        {
            rightButton.SetActive(true);
        }

        if(roomDeckInfo.isActiveLeftDeck && roomDeckInfo.isActiveRightDeck && Sc_Server.Instance.serverStarted)
        {
            fightStartButton.SetActive(true);
        }
        else
        {
            fightStartButton.SetActive(false);
        }

        if(roomDeckInfo.leftUserName == Sc_Client.Instance.clientName || roomDeckInfo.rightUserName == Sc_Client.Instance.clientName)
        {
            fightCancelButton.SetActive(true);
        }
        else
        {
            fightCancelButton.SetActive(false);
        }
    }


    public void OnBtnEntryCancel()
    {
        RoomDeck newRoomDeckInfo = null;
        if (roomDeckInfo.leftUserName == Sc_Client.Instance.clientName)
        {
            newRoomDeckInfo = new RoomDeck(roomDeckInfo,true);
        }
        else if(roomDeckInfo.rightUserName == Sc_Client.Instance.clientName)
        {
            newRoomDeckInfo = new RoomDeck(roomDeckInfo, false);
        }

        if(newRoomDeckInfo != null)
        {
            Sc_Client.Instance.OnCancelButton(newRoomDeckInfo);
        }
    }

    public void OnBtnLeftDeck()
    {
        Sc_Client.Instance.OnDeckButton(0);
    }

    public void OnBtnRightDeck()
    {
        Sc_Client.Instance.OnDeckButton(1);
    }
}
