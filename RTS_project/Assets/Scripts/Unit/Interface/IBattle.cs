using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattle
{
    float AttackPower { get; set; }
    float DefencePower { get; set; }
    float AttackInterval { get; }
    float AttackRange { get; }

    void Attack();
    void GetDamage();

    void OnBuff();


}
