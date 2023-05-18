using System;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    //Material mat;
    PlayerController controller;
    Animator anim;
    Rigidbody rigid;
    Selector selector;
    List<GameObject> unitsList;
    float stopDistance = 0.01f;

    public Action<GameObject> onSelectUnit;       //마우스 좌클릭. 선택된 유닛이 있음을 알리는 델리게이트
    public Action<GameObject> onUnselectUnit;     //마우스 좌클릭. 선택에서 빠진 유닛이 있음을 알리는 델리게이트

    public float rotateSpeed = 180.0f;
    float originMoveSpeed;                          //초기화 속도
    public float moveSpeed = 2.0f;                  //유닛 이동속도
    GameObject selectedUnit;                        //선택된 유닛
    bool onSelected = false;                        //선택여부 확인용 변수

    Vector3 targetPos;                              //이동하려는 위치
    public Vector3 TargetPos
    {
        get => targetPos;
        set
        {
            value.y = transform.position.y;
            targetPos = value;
        }
    }

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        controller = FindObjectOfType<PlayerController>();
        selector = FindObjectOfType<Selector>();
        //mat = GetComponent<MeshRenderer>().material;
    }
    void Start()
    {

        controller.onClickUnit += OnUnitSelected;           //유닛 선택시 실행될 함수 등록
        selector.onChangedUnits += OnUnitsSelected;         //유닛리스트 함수 등록
        controller.onSetDestination += OnSetDestination;      //목적지 설정 델리게이트 수신시 실행할 함수등록

        //변수 초기화
        TargetPos = transform.position;
        originMoveSpeed = moveSpeed;

    }

    private void OnUnitsSelected(List<GameObject> UnitsList)
    {
        unitsList = UnitsList;
        foreach (GameObject unit in unitsList)
        {
            if (unit == this.gameObject)
            {
                this.onSelected = true;
                Debug.Log($"Unit포함됨:{unit}");
            }
            //transform.SetParent(null);
        }
    }

    private void OnUnitSelected(GameObject obj)
    {
        this.selectedUnit = obj;
        if (obj == this.gameObject)
        {
            if (!onSelected)
            {
                onSelected = true;
                anim.SetBool("Click", onSelected);
                onSelectUnit?.Invoke(this.gameObject);
            }
            else
            {
                onSelected = false;
                anim.SetBool("Click", onSelected);
                onUnselectUnit?.Invoke(this.gameObject);
                selectedUnit = null;
            }
        }
    }
    private void OnSetDestination(Vector3 Destination)
    {
        if (unitsList.Count > 0)
        {
            foreach (var obj in unitsList)
            {
                if (this.gameObject == obj)
                {
                    TargetPos = Destination;
                    this.transform.LookAt(TargetPos);
                    //Debug.Log(Destination);
                }
            }
        }
        else
        {
            //선택된 것이 없을 땐, 아무것도 하지 않음.
        }
    }
    void Update()
    {
        if (selectedUnit)
        {
            if (TargetPos != transform.position)
            {
                transform.Translate(Time.deltaTime * moveSpeed * Vector3.forward);

                if (unitsList.Count == 1)
                {
                    stopDistance = 0.01f;
                }
                else
                {
                    stopDistance = 4.0f;
                }
                if ((TargetPos - transform.position).sqrMagnitude < stopDistance)
                {
                    TargetPos = transform.position;
                }
            }

            else if (TargetPos != transform.position)
            {
                transform.Translate(Time.deltaTime * moveSpeed * transform.forward, Space.World);

                if ((TargetPos - transform.position).sqrMagnitude < stopDistance)
                {
                    TargetPos = transform.position;
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 from = transform.position;
        Vector3 to = TargetPos;

        Gizmos.DrawLine(from, to);
    }

}
