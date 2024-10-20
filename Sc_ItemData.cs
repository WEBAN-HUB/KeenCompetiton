using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Object/ItemData")]
public class Sc_ItemData : ScriptableObject
{
    [Header("# ItemStat")]
    public int ID;
    public Sc_FighterData.EFighterPosition[] equipTarget;
    public string itemName;
    public int addAttackDamage;
    public int addsArmor;
    public int addsSpeed;
    public int addsHealth;

    [TextArea]
    public string itemDesc;

    [Header("Graphic")]
    public Sprite icon;
}
