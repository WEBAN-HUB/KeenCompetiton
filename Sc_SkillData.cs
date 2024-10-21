using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillData", menuName = "Scriptable Object/SkillData")]
public class Sc_SkillData : ScriptableObject
{
    public enum ESkillType
    {
        Attack,
        Support
    }

    [Header("# SkillStat")]
    public ESkillType skillType;
    public int ID;
    public string skillName;
    public int addAttackDamage;
    public int addArmor;
    public int addSpeed;
    public int addHealth;
    public int coolTime;
    public GameObject effect;
    public int duration;

    [TextArea]
    public string skillDesc;

    [Header("# Graphic")]
    public Sprite icon;
}
