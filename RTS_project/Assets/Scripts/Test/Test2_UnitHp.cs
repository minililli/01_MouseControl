using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test2_UnitHp : TestBase
{
    Soldier[] soldiers;
    Nurse nurse;
    protected override void Awake()
    {
        base.Awake();
        soldiers = FindObjectsOfType<Soldier>();
        nurse = FindObjectOfType<Nurse>();
    }
    protected override void Test1(InputAction.CallbackContext _)
    {
        foreach (var w in soldiers)
        {
            w.HP = 1;
        }
    }
}
