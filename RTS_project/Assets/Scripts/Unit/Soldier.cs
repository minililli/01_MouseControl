using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : Unit, IBattle
{
    //----------------------------Soldier 스탯
    
    float UcurrentHp;
    float UcurrentMp;
    float UcurrentPower;
    float UattackSpeed;
    float Udefence;
    float UcurrentMoveSpeed = 2.0f;           //유닛 이동속도
    [Header("Soldier 특성")]
    public float UattackInterval=1.0f;
    public float UattackRange;


    //-----------------스탯관련-------------------------------------------
    public float AttackPower { get => UcurrentPower; set => UcurrentPower = value; }
    public float DefencePower { get => Udefence; set => Udefence=value; }

    public float AttackInterval => UattackInterval;

    public float AttackRange => UattackRange;

    public void Attack()
    {
        throw new System.NotImplementedException();
    }

    public void GetDamage()
    {
        throw new System.NotImplementedException();
    }

    public void OnBuff()
    {
        throw new System.NotImplementedException();
    }
}
