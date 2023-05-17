using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test1_Input : TestBase
{
    Unit player;
    Selector selector;

    float initPlayerMoveSpeed;

    Vector3 initPlayerPos;

    protected override void Awake()
    {
        base.Awake();
        player = FindObjectOfType<Unit>();
        selector = FindObjectOfType<Selector>();
    }
    private void Start()
    {
        initPlayerPos = player.transform.position;
        initPlayerMoveSpeed = player.moveSpeed;
        //Debug.Log(initPlayerPos);
    }
    protected override void Test1(InputAction.CallbackContext _)
    {
        player.TargetPos = new Vector3(8, 0, 0);

    }

    protected override void Test2(InputAction.CallbackContext _)
    {
        player.transform.position = Vector3.zero;
        player.TargetPos = player.transform.position;
        player.moveSpeed = initPlayerMoveSpeed;
    }

    protected override void Test3(InputAction.CallbackContext obj)
    {
        if (player.moveSpeed != 0)
        {
            player.moveSpeed = 0;
        }
        else
        {
            player.moveSpeed = initPlayerMoveSpeed;
        }
    }

    protected override void Test4(InputAction.CallbackContext obj)
    {
        selector.CheckSelect();
    } 
}
