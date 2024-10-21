using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Sc_FighterData;

public class Sc_Fighter : MonoBehaviour
{
    public Sc_FighterData fighterData;

    [Header("# Component")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    [Header("# Stat")]
    public int ID;
    private int maxHealth;
    public int health;
    public int damage;
    public int armor;
    public int speed;
    public EFighterPosition fighterPosition;
    public EFighterState fighterState;
    public int actionFigures;
    public bool isAlive;

    [Header("# Graphic")]
    public Sprite baseSprite;


    public bool testIsLeft;

    // fighter의 초기화를 진행한다.
    public void Init(Sc_FighterData inSc_FighterData, bool isLeft)
    {
        fighterData = inSc_FighterData;
        ID = fighterData.ID;
        maxHealth = fighterData.health;
        health = fighterData.health;
        damage = fighterData.damage;
        armor = fighterData.armor;
        speed = fighterData.speed;
        fighterPosition = fighterData.fighterPosition;
        fighterState = fighterData.fighterState;
        baseSprite = fighterData.mainSprite;
        animator.runtimeAnimatorController = fighterData.animCon;
        // XOR 연산에 결과를 뒤집은 값을 flipX에 넣어준다.
        /*true true = true
        false true = false
        true false = false
        false flase = true*/
        spriteRenderer.flipX = !(fighterData.isFlip ^ isLeft);

        spriteRenderer.gameObject.transform.localPosition = isLeft ? fighterData.positionForBattle : new Vector3(fighterData.positionForBattle.x*-1,fighterData.positionForBattle.y,fighterData.positionForBattle.z);
        spriteRenderer.gameObject.transform.localScale = fighterData.scaleForBattle;
        actionFigures = 0;
        isAlive = true;
    }

    /// <summary>
    /// 캐릭터의 속도를 더한다.
    /// 100을 넘기게 되면 true를 리턴한다.
    /// </summary>
    /// <returns></returns>
    public bool AddSpeed()
    {
        actionFigures += speed;
        if(actionFigures >= 100)
        {
            actionFigures -= 100;
            return true;
        }
        return false;
    }

}
