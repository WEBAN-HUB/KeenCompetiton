using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Fighter", menuName = "Scriptable Object/FighterData")]
public class Sc_FighterData : ScriptableObject
{
    public enum EFighterPosition { FRONT, MIDDLE, BACK}
    public enum EFighterState { IDLE = 0, CIDLE, RUN, JUMP, ATTACK, HURT, DEATH}

    [Header("# Stat")]
    public EFighterPosition fighterPosition;
    public EFighterState fighterState;
    public int ID;
    public string fighterName;
    public int health;
    public int damage;
    public int armor;
    public int speed;
    public Sc_SkillData skill;

    [Header("# position")]
    public Vector3 positionForUI;
    public Vector3 scaleForUI;
    public Vector3 positionForBattle;
    public Vector3 scaleForBattle;
    public bool isFlip;

    [TextArea]
    public string fighterDesc;

    [Header("# Graphic")]
    public Sprite mainSprite;
    public Sprite icon;
    public RuntimeAnimatorController animCon;
}

