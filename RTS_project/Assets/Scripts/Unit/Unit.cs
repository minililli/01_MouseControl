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
    List<GameObject> unitsList;                   // Selector의 유닛 리스트 받아오기


    public Action<GameObject> onSelectUnit;       //마우스 좌클릭. 선택된 유닛이 있음을 알리는 델리게이트
    public Action<GameObject> onUnselectUnit;     //마우스 좌클릭. 선택에서 빠진 유닛이 있음을 알리는 델리게이트

    public float rotateSpeed = 180.0f;              //회전 속도
    float originMoveSpeed;                          //초기화 속도
    public float moveSpeed = 2.0f;                  //유닛 이동속도

    public bool onSelected = false;                 //선택여부 확인용 변수

    float stopDistance = 0.01f;                     // 멈춤을 인정하는 거리제곱
    bool onMove = false;                            // 이동중임을 나타내는 변수
    Vector3 targetPos;                              // 이동하려는 위치(목표지점)
    public Vector3 TargetPos                        // 이동하려는 위치(목표지점)프로퍼티
    {
        get => targetPos;
        set
        {
            value.y = transform.position.y;
            targetPos = value;
        }
    }

    //---Indicator
    Transform indicator;
    Transform bottomIndicator;
    //-----------
    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        controller = FindObjectOfType<PlayerController>();
        selector = FindObjectOfType<Selector>();
        //mat = GetComponent<MeshRenderer>().material;

        indicator = transform.GetChild(0);
        bottomIndicator = transform.GetChild(0).GetChild(0);

    }
    void Start()
    {
        selector.onChangedUnits += OnUnitsSelected;         //유닛리스트 함수 등록

        controller.onCancel += OnUnitsCancel;
        controller.onSetDestination += OnSetDestination;      //목적지 설정 델리게이트 수신시 실행할 함수등록

        //변수 초기화
        unitsList = null;
        onMove = false;
        TargetPos = transform.position;
        originMoveSpeed = moveSpeed;
        bottomIndicator.gameObject.SetActive(false);

    }


    private void OnUnitsSelected(List<GameObject> UnitsList)
    {
        if (UnitsList != null)
        {
            unitsList = UnitsList;

            foreach (GameObject unit in unitsList)
            {
                if (unit == this.gameObject && !onSelected)
                {
                    onSelected = true;
                    anim.SetBool("Click", true);
                    bottomIndicator.gameObject.SetActive(true);          //바닥 선택 표시
                    Debug.Log($"Unit포함됨:{unit}");
                }
                else if (unit == this.gameObject && onSelected && !controller.dragging)
                {
                    onSelected = false;
                    anim.SetBool("Click", false);
                    bottomIndicator.gameObject.SetActive(false);          //바닥 선택 표시제거
                    Debug.Log($"Unit해제됨:{unit}");
                }
            }

        }
        RefreshUnitsList();

    }



    /* private void OnUnitSelected(GameObject obj)
     {
         if (obj == this.gameObject)
         {
             if (!onSelected)
             {
                 onSelected = true;
                 anim.SetBool("Click", true);
                 bottomIndicator.gameObject.SetActive(true);          //바닥 선택 표시
                 selectedUnit = obj;
             }
             else
             {
                 onSelected = false;
                 anim.SetBool("Click", false);
                 bottomIndicator.gameObject.SetActive(false);             //바닥 선택 표시 제거
                 selectedUnit = null;
             }
         }
     }*/

    private void OnUnitsCancel()
    {
        unitsList.Clear();
        InitializeUnit();
    }

    private void InitializeUnit()
    {
        moveSpeed = originMoveSpeed;
        rigid.velocity = Vector3.zero;
        onSelected = false;

        //Indicator 설정 ---------------------------
        anim.SetBool("Click", false);
        bottomIndicator.gameObject.SetActive(false);
    }

    private void OnSetDestination(Vector3 Destination)
    {
        if (unitsList != null && !controller.dragging)
        {
            if (unitsList.Count > 0)
            {
                foreach (var obj in unitsList)
                {
                    if (this.gameObject == obj)
                    {
                        TargetPos = Destination;
                        this.transform.LookAt(TargetPos);
                        onMove = true;
                        //Debug.Log(Destination);
                    }
                }
            }
            else
            {
                onMove = false;
            }
        }
    }
    void Update()
    {
        if (onSelected)
        {
            if (TargetPos != transform.position)
            {
                transform.Translate(Time.deltaTime * moveSpeed * Vector3.forward);

                if (unitsList.Count == 1)
                {
                    stopDistance = 0.25f;
                }
                else
                {
                    stopDistance = 1.25f;
                }

                if ((TargetPos - transform.position).sqrMagnitude < stopDistance)
                {
                    TargetPos = transform.position;
                    onMove = false;
                }
            }
        }
        else
        {
            if ((TargetPos - transform.position).sqrMagnitude > stopDistance)
            {
                if (onMove)
                {
                    transform.Translate(Time.deltaTime * moveSpeed * transform.forward, Space.World);

                    if ((TargetPos - transform.position).sqrMagnitude < stopDistance)
                    {
                        TargetPos = transform.position;
                        onMove = false;
                    }
                }
                else
                {
                    transform.Translate(Time.deltaTime * moveSpeed * transform.forward, Space.World);
                    if ((TargetPos - transform.position).sqrMagnitude < stopDistance)
                    {
                        TargetPos = transform.position;
                        onMove = false;
                    }
                }
            }

        }
    }



    public void RefreshUnitsList()
    {
        if (!unitsList.Contains(this.gameObject))
        {
            onSelected = false;
            anim.SetBool("Click", onSelected);
            bottomIndicator.gameObject.SetActive(false);          //바닥 선택 표시제거
        }
        else
        {
            foreach (var unit in unitsList)
            {
                if (unit == this.gameObject)
                {
                    onSelected = true;
                    anim.SetBool("Click", onSelected);
                    bottomIndicator.gameObject.SetActive(true);
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
