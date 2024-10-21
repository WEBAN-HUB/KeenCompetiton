using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_ItemDataLib : MonoBehaviour
{
    public static Sc_ItemDataLib Instance;

    /// <summary>
    /// Key : ID, value : 아이템 데이터
    /// </summary>
    public Dictionary<int, Sc_ItemData> itemDataDic;
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
        Sc_ItemData[] itemDatas = Resources.LoadAll<Sc_ItemData>("ItemData");
        itemDataDic = new Dictionary<int, Sc_ItemData>();
        foreach(var item in itemDatas)
        {
            itemDataDic.Add(item.ID,item);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
