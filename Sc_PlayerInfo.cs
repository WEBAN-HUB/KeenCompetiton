using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_PlayerInfo : MonoBehaviour
{
    public static Sc_PlayerInfo Instance;
    public DeckInfo deckInfo;

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
        int[] fighterID;
        int[] itemID = new int[3] { 0, 0, 0 };

        fighterID = LoadIntArray("fighterID");
        //TODO 저장이 되어 있다면 덱 정보, 아이템 정보를 세팅한다.
        if (fighterID == null)
        {
            fighterID = new int[3] { 3, 4, 5 };
        }
        deckInfo = new DeckInfo(fighterID, itemID);
    }

    public void SaveIntArray(string key, int[] array)
    {
        string arrayString = string.Join(",", array);
        PlayerPrefs.SetString(key, arrayString);
        PlayerPrefs.Save();
    }

    public int[] LoadIntArray(string key)
    {
        string arrayString = PlayerPrefs.GetString(key, "");
        if (string.IsNullOrEmpty(arrayString)) return null;

        string[] stringArray = arrayString.Split(',');
        int[] intArray = new int[stringArray.Length];
        for (int i = 0; i < stringArray.Length; i++)
        {
            intArray[i] = int.Parse(stringArray[i]);
        }
        return intArray;
    }
}

[Serializable]
public class DeckInfo
{
    // fighter, item ID 배열, 인덱스 0 = front, 1 = middle, 2 = back
    public int[] fighterID;
    public int[] itemID;

    public DeckInfo(int[] inFighterID, int[] inItemID)
    {
        fighterID = inFighterID;
        itemID = inItemID;
    }

    public void InitDeck(DeckInfo inputDeck)
    {
        fighterID = inputDeck.fighterID;
        itemID = inputDeck.itemID;
    }
}