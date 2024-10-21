using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sc_UnitChangeButton : MonoBehaviour
{
    public Sc_FighterData.EFighterPosition position;
    public int fighterID;
    public Image Icon;
    public Text textCharacterInfo;
    public Text textStatInfo;

    public void InitUnitChangeButton(int ID)
    {
        fighterID = ID;
        Icon.sprite = Sc_FighterDataLib.Instance.GetIcon(ID);
        textCharacterInfo.text = Sc_FighterDataLib.Instance.GetName(ID) + "\n" + Sc_FighterDataLib.Instance.GetInfo(ID);
        textStatInfo.text = Sc_FighterDataLib.Instance.GetStat(ID);
        position = Sc_FighterDataLib.Instance.GetEFighterPosition(ID);
    }

    public void OnBtnSelect()
    {
        switch (position)
        {
            case Sc_FighterData.EFighterPosition.FRONT:
                Sc_PlayerInfo.Instance.deckInfo.fighterID[0] = fighterID;
                GameObject.FindObjectOfType<Sc_Title>().ShowFightersUI();
                break;
            case Sc_FighterData.EFighterPosition.MIDDLE:
                Sc_PlayerInfo.Instance.deckInfo.fighterID[1] = fighterID;
                GameObject.FindObjectOfType<Sc_Title>().ShowFightersUI();
                break;
            case Sc_FighterData.EFighterPosition.BACK:
                Sc_PlayerInfo.Instance.deckInfo.fighterID[2] = fighterID;
                GameObject.FindObjectOfType<Sc_Title>().ShowFightersUI();
                break;
        }
    }
}
