using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sc_FighterDataLib : MonoBehaviour
{
    public static Sc_FighterDataLib Instance;

    /// <summary>
    /// Key : ID, value : 파이터 데이터
    /// </summary>
    public Dictionary<int, Sc_FighterData> fighterDic;

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

        Sc_FighterData[] fighterDatas = Resources.LoadAll<Sc_FighterData>("FighterData");
        fighterDic = new Dictionary<int, Sc_FighterData>();
        foreach (var data in fighterDatas)
        {
            fighterDic.Add(data.ID,data);
        }
    }

    /// <summary>
    /// 입력받은 파이터 ID의 runtimeAnimatorController을 리턴한다
    /// </summary>
    /// <param name="ID">파이터의 ID</param>
    public RuntimeAnimatorController GetFighterRuntimeAnimCon(int ID)
    {
        return fighterDic[ID].animCon;
    }
    /// <summary>
    /// 입력받은 파이터 ID의 positionForUI를 리턴한다
    /// </summary>
    /// <param name="ID">파이터의 ID</param>
    public Vector3 GetFighterUIPosition(int ID)
    {
        return fighterDic[ID].positionForUI;
    }
    /// <summary>
    /// 입력받은 파이터 ID의 scaleForUI를 리턴한다
    /// </summary>
    /// <param name="ID">파이터의 ID</param>
    public Vector3 GetFighterUIScale(int ID)
    {
        return fighterDic[ID].scaleForUI;
    }
    /// <summary>
    /// 입력받은 파이터 ID의 isFlip을 리턴한다
    /// </summary>
    /// <param name="ID">파이터의 ID</param>
    public bool GetIsFlip(int ID)
    {
        return fighterDic[ID].isFlip;
    }

    public Sprite GetIcon(int ID)
    {
        return fighterDic[ID].icon;
    }

    public string GetInfo(int ID)
    {
        return fighterDic[ID].fighterDesc;
    }

    public string GetStat(int ID)
    {
        string stat = "";
        stat += $"Health : {fighterDic[ID].health}\n";
        stat += $"Armor : {fighterDic[ID].armor}\n";
        stat += $"Damage : {fighterDic[ID].damage}\n";
        stat += $"Speed : {fighterDic[ID].speed}\n";
        return stat;
    }
    public Sc_FighterData.EFighterPosition GetEFighterPosition(int ID)
    {
        return fighterDic[ID].fighterPosition;
    }

    public string GetName(int ID)
    {
        return fighterDic[ID].fighterName;
    }
}
