using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_SkillDataLib : MonoBehaviour
{
    public static Sc_SkillDataLib Instance;

    /// <summary>
    /// Key : ID, value : 스킬 데이터
    /// </summary>
    public Dictionary<int, Sc_SkillData> skillDataDic;
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

        Sc_SkillData[] skillDatas = Resources.LoadAll<Sc_SkillData>("SkillData");
        skillDataDic = new Dictionary<int, Sc_SkillData>();
        foreach(var skill in skillDatas)
        {
            skillDataDic.Add(skill.ID, skill);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
