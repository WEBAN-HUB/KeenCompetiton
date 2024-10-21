using DG.Tweening;
using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using static Sc_FighterData;

public class Sc_Title : MonoBehaviour
{
    [Header("# Component")]
    // UI 화면 이동 관련 변수
    public GameObject startUI;
    public GameObject mainUI;
    public GameObject unitSettingUI;
    public GameObject hostUI;
    public GameObject joinUI;
    public GameObject roomUI;
    public GameObject gamePlayUI;

    // 화면 움직임 효과 관련 변수
    public Transform backCloud;
    public RectTransform title;
    public Text txtAnyKey;
    // 파이터들 보여주기 위한 변수들
    public Animator[] showFighters;
    public Animator[] showOpponentFighters;
    public Text[] selectFightersName;

    [Header("# Unit Change")]
    public GameObject unitSelectUI;
    public GameObject unitSelectFrontFighter;
    public GameObject unitSelectMiddleFighter;
    public GameObject unitSelectBackFighter;
    public GameObject unitSelectScrollViewContent;
    public Sc_UnitChangeButton PF_unitButton;
    public List<Sc_UnitChangeButton> unitButtonList;

    [Header("# Value")]
    public Sc_PlayerInfo playerInfo;
    public List<GameObject> fromUIList;
    public List<GameObject> toUIList;
    private float parallaxScale;
    bool isStartCanvas;
    bool isShowingFighter;

    private void Awake()
    {
        parallaxScale = 0.5f;
        isStartCanvas = true;
        isShowingFighter = false;
        fromUIList = new List<GameObject>();
        toUIList = new List<GameObject>();
        unitButtonList = new List<Sc_UnitChangeButton>();
    }

    private void Start()
    {
        // Title 위아래로 움직이는 효과
        title.DOAnchorPosY(title.anchoredPosition.y + 10f, 3f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
        // Txt_AnyKey 알파값 반복 효과
        txtAnyKey.DOFade(0, 2f).SetLoops(-1, LoopType.Yoyo);

        playerInfo = Sc_PlayerInfo.Instance;
    }

    private void Update()
    {
        if (Input.anyKeyDown && isStartCanvas)
        {
            fromUIList.Add(startUI);
            toUIList.Add(mainUI);
            StartCoroutine(ChangeUI(1.5f));
        }

        if(mainUI.activeSelf && isShowingFighter == false)
        {
            ShowFightersUI();
        }
    }

    private void LateUpdate()
    {
        MoveBackCloud();
    }


    /// <summary>
    /// UI를 변경하는 함수, 1.5f초 동안 UI가 사라지다가 다음 UI로 변경된다.
    /// </summary> 
    /// <param name="delay">Fade할 시간</param>
    private IEnumerator ChangeUI(float delay)
    {
        // 변환 전 UI를 delay 시간만큼 fade out
        foreach (var baseUi in fromUIList)
        {
            foreach (var text in baseUi.GetComponentsInChildren<Text>())
            {
                if (text.gameObject.name.Contains("fixed_A"))
                    continue;
                text.DOFade(0, delay);
            }

            foreach (var image in baseUi.GetComponentsInChildren<Image>())
            {
                if (image.gameObject.name.Contains("fixed_A"))
                    continue;
                image.DOFade(0, delay);
            }

            foreach(var rawImage in baseUi.GetComponentsInChildren<RawImage>())
            {
                if (rawImage.gameObject.name.Contains("fixed_A"))
                    continue;
                rawImage.DOFade(0, delay);
            }

            foreach(var btn in baseUi.GetComponentsInChildren<Button>())
            {
                btn.enabled = false;
            }
        }

        yield return new WaitForSeconds(delay);

        foreach (var baseUi in fromUIList)
        {
            baseUi.gameObject.SetActive(false);
        }

        // 변환 후 UI를 delay 시간만큼 fade in
        foreach (var baseUi in toUIList)
        {
            baseUi.gameObject.SetActive(true);
            foreach (var text in baseUi.GetComponentsInChildren<Text>())
            {
                if (text.gameObject.name.Contains("fixed_A"))
                    continue;
                text.DOFade(1, (isStartCanvas ? 0.01f : delay));
            }

            foreach (var image in baseUi.GetComponentsInChildren<Image>())
            {
                if (image.gameObject.name.Contains("fixed_A"))
                    continue;
                image.DOFade(1, (isStartCanvas ? 0.01f : delay));
            }

            foreach (var rawImage in baseUi.GetComponentsInChildren<RawImage>())
            {
                if (rawImage.gameObject.name.Contains("fixed_A"))
                    continue;
                rawImage.DOFade(1, (isStartCanvas ? 0.01f : delay));
            }

            foreach (var btn in baseUi.GetComponentsInChildren<Button>())
            {
                btn.enabled = true;
            }
        }

        // 처음 화면에서 이동하는 경우 초기 세팅
        if (isStartCanvas)
        {
            isStartCanvas = false;
            if (playerInfo == null)
                playerInfo = Sc_PlayerInfo.Instance;
        }

        // UI 리스트를 비운다
        fromUIList.Clear();
        toUIList.Clear();
    }

    /// <summary>
    /// 뒷 배경의 구름을 계속 움직이게 하는 함수
    /// </summary> 
    private void MoveBackCloud()
    {
        // 배경의 구름을 움직이게 하는 효과
        backCloud.position = backCloud.position + (Vector3.left * parallaxScale * Time.deltaTime);
        foreach (var layer in backCloud.GetComponentsInChildren<Transform>())
        {
            if (layer != backCloud && layer.transform.position.x < -21)
            {
                layer.transform.position = new Vector3(21.75f, layer.position.y, layer.position.z);
            }
        }
    }

    /// <summary>
    /// 현재 덱 구성을 UI에 보여주는 함수
    /// </summary> 
    public void ShowFightersUI()
    {
        isShowingFighter = true;
        for (int i = 0; i < showFighters.Length;i++)
        {
            // 달리기 애니메이션을 위한 애니메이션 컨트롤러 할당
            showFighters[i].runtimeAnimatorController = Sc_FighterDataLib.Instance
                .GetFighterRuntimeAnimCon(playerInfo.deckInfo.fighterID[i]);
            showFighters[i].SetInteger("AnimState", (int)EFighterState.RUN);

            // UI 크기 조정
            showFighters[i].gameObject.transform.localPosition = Sc_FighterDataLib.Instance.GetFighterUIPosition(playerInfo.deckInfo.fighterID[i]);
            showFighters[i].gameObject.transform.localScale = Sc_FighterDataLib.Instance.GetFighterUIScale(playerInfo.deckInfo.fighterID[i]);
            showFighters[i].GetComponent<SpriteRenderer>().flipX = Sc_FighterDataLib.Instance.GetIsFlip(playerInfo.deckInfo.fighterID[i]);
        }
    }

    /// <summary>
    /// 덱 구성 UI로 이동하는 함수
    /// </summary> 
    /// <param name="isShow"> true : 덱 구성 UI로 이동 false : 메인 UI로 이동</param>
    public void ShowUnitSettingUI(bool isShow)
    {
        if (isShow)
        {
            fromUIList.Add(mainUI);
            toUIList.Add(unitSettingUI);
            for(int i = 0; i < selectFightersName.Length;i++)
            {
                selectFightersName[i].text = Sc_FighterDataLib.Instance.GetName(playerInfo.deckInfo.fighterID[i]);
            }
        }
        else
        {
            toUIList.Add(mainUI);
            fromUIList.Add(unitSettingUI);
        }
        StartCoroutine(ChangeUI(0.5f));
    }

    /// <summary>
    /// Host UI로 이동하는 함수
    /// </summary> 
    /// <param name="isShow"> true : Host UI로 이동 false : 메인 UI로 이동</param>
    public void ShowHostUI(bool isShow)
    {
        if (isShow)
        {
            fromUIList.Add(mainUI);
            toUIList.Add(hostUI);
        }
        else
        {
            toUIList.Add(mainUI);
            fromUIList.Add(hostUI);
        }
        StartCoroutine(ChangeUI(0.5f));
    }

    /// <summary>
    /// Main UI에서 Join UI로 이동
    /// </summary> 
    /// <param name="isShow"> true : Join UI로 이동 false : 메인 UI로 이동</param>
    public void ShowJoinUI(bool isShow)
    {
        if (isShow)
        {
            fromUIList.Add(mainUI);
            toUIList.Add(joinUI);
        }
        else
        {
            toUIList.Add(mainUI);
            fromUIList.Add(joinUI);
        }
        StartCoroutine(ChangeUI(0.5f));
    }

    /// <summary>
    /// host UI에서 room UI로 이동
    /// </summary> 
    public void ShowRoomUIFromHost()
    {
        fromUIList.Add(hostUI);
        toUIList.Add(roomUI);
        StartCoroutine(ChangeUI(0.01f));
    }

    /// <summary>
    /// host UI에서 room UI로 이동
    /// </summary> 
    public void ShowRoomUIFromJoin()
    {
        fromUIList.Add(joinUI);
        toUIList.Add(roomUI);
        StartCoroutine(ChangeUI(0.01f));
    }

    /// <summary>
    /// room UI에서 main UI로 이동
    /// </summary> 
    public void ShowMainUIFromRoom()
    {
        fromUIList.Add(roomUI);
        toUIList.Add(mainUI);
        StartCoroutine(ChangeUI(0.01f));
        if(GameObject.FindObjectOfType<Sc_Server>().serverStarted)
        {
            GameObject.FindObjectOfType<Sc_Server>().StopServer();
        }
        GameObject.FindObjectOfType<Sc_Client>().CloseSocket();
    }

    /// <summary>
    /// room UI에서 main UI로 이동
    /// </summary> 
    public void ShowGamePlayUIFromRoom()
    {
        fromUIList.Add(roomUI);
        toUIList.Add(gamePlayUI);
        StartCoroutine(ChangeUI(0.01f));
    }

    /// <summary>
    /// unitSettingUI에서 unitSelectUI로 이동
    /// </summary> 
    public void ShowUnitSelectUI(int position)
    {
        fromUIList.Add(unitSettingUI);
        toUIList.Add(unitSelectUI);
        int idx = 0;

        switch (position)
        {
            case 0:
                unitSelectFrontFighter.SetActive(true);
                unitSelectMiddleFighter.SetActive(false);
                unitSelectBackFighter.SetActive(false);

                if(unitButtonList.Count > 0)
                {
                    foreach(var btn in unitButtonList)
                    {
                        btn.gameObject.SetActive(false);
                    }
                }
                foreach(var fighter in Sc_FighterDataLib.Instance.fighterDic)
                {
                    if(fighter.Value.fighterPosition == Sc_FighterData.EFighterPosition.FRONT)
                    {
                        if(unitButtonList.Count > 0 && idx < unitButtonList.Count)
                        {
                            unitButtonList[idx].gameObject.SetActive(true);
                            unitButtonList[idx].InitUnitChangeButton(fighter.Value.ID);
                            idx++;
                        }
                        else
                        {
                            idx++;
                            Sc_UnitChangeButton newButton = Instantiate<Sc_UnitChangeButton>(PF_unitButton, unitSelectScrollViewContent.transform);
                            newButton.InitUnitChangeButton(fighter.Value.ID);
                            unitButtonList.Add(newButton);
                        }
                    }
                }
                break;
            case 1:
                unitSelectMiddleFighter.SetActive(true);
                unitSelectFrontFighter.SetActive(false);
                unitSelectBackFighter.SetActive(false);

                if (unitButtonList.Count > 0)
                {
                    foreach (var btn in unitButtonList)
                    {
                        btn.gameObject.SetActive(false);
                    }
                }
                foreach (var fighter in Sc_FighterDataLib.Instance.fighterDic)
                {
                    if (fighter.Value.fighterPosition == Sc_FighterData.EFighterPosition.MIDDLE)
                    {
                        if (unitButtonList.Count > 0 && idx < unitButtonList.Count)
                        {
                            unitButtonList[idx].gameObject.SetActive(true);
                            unitButtonList[idx].InitUnitChangeButton(fighter.Value.ID);
                            idx++;
                        }
                        else
                        {
                            idx++;
                            Sc_UnitChangeButton newButton = Instantiate<Sc_UnitChangeButton>(PF_unitButton, unitSelectScrollViewContent.transform);
                            newButton.InitUnitChangeButton(fighter.Value.ID);
                            unitButtonList.Add(newButton);
                        }
                    }
                }
                break;
            case 2:
                unitSelectBackFighter.SetActive(true);
                unitSelectFrontFighter.SetActive(false);
                unitSelectMiddleFighter.SetActive(false);
                if (unitButtonList.Count > 0)
                {
                    foreach (var btn in unitButtonList)
                    {
                        btn.gameObject.SetActive(false);
                    }
                }
                foreach (var fighter in Sc_FighterDataLib.Instance.fighterDic)
                {
                    if (fighter.Value.fighterPosition == Sc_FighterData.EFighterPosition.BACK)
                    {
                        if (unitButtonList.Count > 0 && idx < unitButtonList.Count)
                        {
                            unitButtonList[idx].gameObject.SetActive(true);
                            unitButtonList[idx].InitUnitChangeButton(fighter.Value.ID);
                            idx++;
                        }
                        else
                        {
                            idx++;
                            Sc_UnitChangeButton newButton = Instantiate<Sc_UnitChangeButton>(PF_unitButton, unitSelectScrollViewContent.transform);
                            newButton.InitUnitChangeButton(fighter.Value.ID);
                            unitButtonList.Add(newButton);
                        }
                    }
                }
                break;
        }

        if (idx < unitButtonList.Count)
        {
            for(int i = idx; i < unitButtonList.Count;i++)
            {
                unitButtonList[i].gameObject.SetActive(false);
            }
        }

        StartCoroutine(ChangeUI(0.01f));
    }

    /// <summary>
    /// unitSettingUI에서 unitSelectUI로 이동
    /// </summary> 
    public void ShowUnitSettingUIFromUnitSelect()
    {
        fromUIList.Add(unitSelectUI);
        toUIList.Add(unitSettingUI);
        StartCoroutine(ChangeUI(0.01f));
    }


    /// <summary>
    /// 게임 종료 함수
    /// </summary>
    public void DoExit()
    {
        Sc_PlayerInfo.Instance.SaveIntArray("fighterID",Sc_PlayerInfo.Instance.deckInfo.fighterID);

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit(); // 어플리케이션 종료
        #endif
    }
}
