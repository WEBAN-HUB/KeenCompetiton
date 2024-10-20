using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Turn
{
    public bool isLeft;
    public int position;
    public int ID;
    public Turn(bool isLeft, int position, int ID)
    {
        this.isLeft = isLeft;
        this.position = position;
        this.ID = ID;
    }
}


public class Sc_GamePlay : MonoBehaviour
{
    public GameObject canvasUI;
    public Image[] turnImage;
    public GameObject lookAt;

    public Sc_Fighter[] leftFighters;
    public Sc_Fighter[] rightFighters;

    public Sc_RoomDeck roomDeck = null;
    public Sc_FighterDataLib dataLib = null;

    public static Sc_GamePlay Instance;

    public bool isStartedGame;

    public List<Turn> turnList;

    public Text leftTextName;
    public Text rightTextName;
    public Text nowText;
    public Text nextText;
    public Text nextNextText;
    public Text gameWinnerText;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        isStartedGame = false;
        turnList = new List<Turn>();

    }


    public void GameStart()
    {
        if (roomDeck == null)
            roomDeck = Sc_RoomDeck.Instance;
        if (dataLib == null)
            dataLib = Sc_FighterDataLib.Instance;

        canvasUI.SetActive(true);
        for (int i = 0; i < 3; i++)
        {
            leftFighters[i].gameObject.SetActive(true);
            leftFighters[i].Init(dataLib.fighterDic[roomDeck.roomDeckInfo.leftDeckInfo.fighterID[i]],true);
            rightFighters[i].gameObject.SetActive(true);
            rightFighters[i].Init(dataLib.fighterDic[roomDeck.roomDeckInfo.rightDeckInfo.fighterID[i]], false);
        }

        while(turnList.Count < 10)
        {
            for (int i = 0; i < 3; i++)
            {
                if (leftFighters[i].isAlive)
                {
                    if (leftFighters[i].AddSpeed())
                    {
                        turnList.Add(new Turn(true, i, leftFighters[i].ID));
                        if(turnList.Count == 1)
                        {
                            turnImage[0].sprite = dataLib.GetIcon(leftFighters[i].ID);
                            nowText.text = roomDeck.leftText.text;
                        }
                        else if(turnList.Count == 2)
                        {
                            turnImage[1].sprite = dataLib.GetIcon(leftFighters[i].ID);
                            nextText.text = roomDeck.leftText.text;
                        }
                        else if( turnList.Count == 3)
                        {
                            turnImage[2].sprite = dataLib.GetIcon(leftFighters[i].ID);
                            nextText.text = roomDeck.leftText.text;
                        }
                    }
                }

                if(rightFighters[i].isAlive)
                {
                    if (rightFighters[i].AddSpeed())
                    {
                        turnList.Add(new Turn(false, i, rightFighters[i].ID));
                        if (turnList.Count == 1)
                        {
                            turnImage[0].sprite = dataLib.GetIcon(rightFighters[i].ID);
                            nowText.text = roomDeck.rightText.text;
                        }
                        else if (turnList.Count == 2)
                        {
                            turnImage[1].sprite = dataLib.GetIcon(rightFighters[i].ID);
                            nextText.text = roomDeck.rightText.text;
                        }
                        else if (turnList.Count == 3)
                        {
                            turnImage[2].sprite = dataLib.GetIcon(rightFighters[i].ID);
                            nextText.text = roomDeck.rightText.text;
                        }
                    }
                }
            }
        }
        


        leftTextName.text = roomDeck.leftText.text;
        rightTextName.text = roomDeck.rightText.text;
        StartCoroutine(PlayTurn(1f));
        isStartedGame = true;
    }

    private void Update()
    {
        if (!isStartedGame)
            return;

        if(turnList.Count < 10)
        {
            for (int i = 0; i < 3; i++)
            {
                if (leftFighters[i].isAlive)
                {
                    if (leftFighters[i].AddSpeed())
                    {
                        turnList.Add(new Turn(true, i, leftFighters[i].ID));
                    }
                }

                if (rightFighters[i].isAlive)
                {
                    if (rightFighters[i].AddSpeed())
                    {
                        turnList.Add(new Turn(false, i, rightFighters[i].ID));
                    }
                }
            }
        }

        if (!leftFighters[0].isAlive && !leftFighters[1].isAlive && !leftFighters[2].isAlive)
        {
            gameWinnerText.gameObject.SetActive(true);
            gameWinnerText.text = "Winner\n" + rightTextName.text;
        }

        if (!rightFighters[0].isAlive && !rightFighters[1].isAlive && !rightFighters[2].isAlive)
        {
            gameWinnerText.gameObject.SetActive(true);
            gameWinnerText.text = "Winner\n" + leftTextName.text;
        }
    }



    IEnumerator PlayTurn(float delay)
    {
        yield return new WaitForSeconds(delay);
        Turn nowTurn = turnList[0];
        Turn nextTurn = turnList[1];
        Turn nextNextTurn = turnList[2];

        Sc_Fighter turnFighter = null, opponentFighter = null;

        

        if (nowTurn.isLeft)
        {
            turnFighter = leftFighters[nowTurn.position];
            turnFighter.animator.SetTrigger("Run");
            turnImage[0].sprite = dataLib.GetIcon(turnFighter.ID);
            nowText.text = leftTextName.text;

            for(int i = 0; i < 3; i++)
            {
                if(rightFighters[i].isAlive)
                {
                    opponentFighter = rightFighters[i];
                    break;
                }
            }
            if(turnFighter.fighterPosition == Sc_FighterData.EFighterPosition.BACK)
            {
                turnFighter.gameObject.transform.DOMoveX(opponentFighter.gameObject.transform.position.x - 2, 2).SetEase(Ease.OutQuad);
            }
            else
            {
                turnFighter.gameObject.transform.DOMoveX(opponentFighter.gameObject.transform.position.x - 1, 2).SetEase(Ease.OutQuad);
            }
        }
        else
        {
            turnFighter = rightFighters[nowTurn.position];
            turnFighter.animator.SetTrigger("Run");
            turnImage[0].sprite = dataLib.GetIcon(turnFighter.ID);
            nowText.text = rightTextName.text;

            for (int i = 0; i < 3; i++)
            {
                if (leftFighters[i].isAlive)
                {
                    opponentFighter = leftFighters[i];
                    break;
                }
            }
            if (turnFighter.fighterPosition == Sc_FighterData.EFighterPosition.BACK)
            {
                turnFighter.gameObject.transform.DOMoveX(opponentFighter.gameObject.transform.position.x + 2, 2).SetEase(Ease.OutQuad);
            }
            else
            {
                turnFighter.gameObject.transform.DOMoveX(opponentFighter.gameObject.transform.position.x + 1, 2).SetEase(Ease.OutQuad);
            }
        }
        float turnFighterOriginX = turnFighter.transform.position.x;

        lookAt.transform.parent = turnFighter.transform;
        lookAt.transform.localPosition = Vector3.zero;

        if(nextTurn.isLeft)
        {
            turnImage[1].sprite = dataLib.GetIcon(leftFighters[nextTurn.position].ID);
            nextText.text = leftTextName.text;
        }
        else
        {
            turnImage[1].sprite = dataLib.GetIcon(rightFighters[nextTurn.position].ID);
            nextText.text = rightTextName.text;
        }

        if (nextNextTurn.isLeft)
        {
            turnImage[2].sprite = dataLib.GetIcon(leftFighters[nextNextTurn.position].ID);
            nextNextText.text = leftTextName.text;
        }
        else
        {
            turnImage[2].sprite = dataLib.GetIcon(rightFighters[nextNextTurn.position].ID);
            nextNextText.text = rightTextName.text;
        }
        yield return new WaitForSeconds(2);

        turnFighter.animator.SetTrigger("Attack");
        opponentFighter.animator.SetTrigger("Hurt");
        opponentFighter.spriteRenderer.DOColor(Color.white,0.15f).SetLoops(4, LoopType.Yoyo);
        opponentFighter.health -= (turnFighter.damage - opponentFighter.armor);

        yield return new WaitForSeconds(1f);

        if(opponentFighter.health < 0)
        {
            opponentFighter.isAlive = false;
            opponentFighter.animator.SetTrigger("Death");
            opponentFighter.animator.StopPlayback();
            turnList.RemoveAll(item => item.isLeft != nowTurn.isLeft && opponentFighter.ID == item.ID);
        }
        turnFighter.spriteRenderer.flipX = !turnFighter.spriteRenderer.flipX;
        turnFighter.animator.SetTrigger("Run");
        turnFighter.gameObject.transform.DOMoveX(turnFighterOriginX, 2).SetEase(Ease.Linear);

        yield return new WaitForSeconds(2);
        turnFighter.spriteRenderer.flipX = !turnFighter.spriteRenderer.flipX;
        turnFighter.animator.SetTrigger("Idle");

        turnList.RemoveAt(0);
        StartCoroutine(PlayTurn(1));
    }

}
