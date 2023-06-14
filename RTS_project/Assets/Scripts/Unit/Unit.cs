using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Rendering;

public class Unit : MonoBehaviour, IUnitStatus
{
    [Header("유닛관련특성")]
    //Material mat;
    protected PlayerController controller;
    protected Animator anim;
    protected Rigidbody rigid;
    protected Selector selector;
    // Selector의 유닛 리스트 받아올 유닛리스트
    protected List<GameObject> unitsList;


    public Action<GameObject> onSelectUnit;       //마우스 좌클릭. 선택된 유닛이 있음을 알리는 델리게이트
    public Action<GameObject> onUnselectUnit;     //마우스 좌클릭. 선택에서 빠진 유닛이 있음을 알리는 델리게이트
    // ---------------------------------- 이동관련 변수
    public bool onSelected = false;                 //선택여부 확인용 변수
    public bool onStop = false;                     //정지여부 확인용 변수
    protected bool onMove = false;                  // 이동중임을 나타내는 변수
    protected float stopDistanceSqr = 1f;        // 멈춤을 인정하는 거리제곱

    Vector3 targetPos;                              // 이동하려는 위치(목표지점)
    public Vector3 TargetPos                        // 이동하려는 위치(목표지점)프로퍼티
    {
        get => targetPos;
        protected set
        {
            value.y = this.transform.position.y;
            targetPos = value;
        }
    }


    // Stat관련 ------------------------------------------------------------

    public float currentHp;
    public float maxHp = 100f;
    float currentMp;
    public float maxMp = 100f;
    float currentPower;
    float attackSpeed;
    float defence;
    float currentMoveSpeed = 2.0f;           //유닛 이동속도

    float originMoveSpeed;                  //기존 MoveSpeed 속도초기화 변수
    //-------------------------IUnitStatus-------------------------------
    public float HP
    {
        get => currentHp;
        set
        {
            currentHp = value;
            Mathf.Clamp(currentHp, 0f, maxHp);
            if (currentHp <= 0f)
            {
                OnDie();
            }
            if (currentHp > maxHp)
            {
                currentHp = maxHp;
            }
        }
    }
    public float MP
    {
        get => currentMp;
        set
        {
            Mathf.Clamp(currentMp, 0f, maxMp);
        }
    }

    public float Power
    {
        get => currentPower;
        set
        {
            currentPower = value;
        }
    }
    public float AttackSpeed
    {
        get => attackSpeed;
        set
        {
            attackSpeed = value;
        }
    }
    public float Defence
    {
        get => defence;
        set
        {
            defence = value;
        }
    }
    public float MoveSpeed
    {
        get => currentMoveSpeed;
        set
        {
            currentMoveSpeed = value;
        }
    }

    //-------------------------------------Indicator--------------------------
    Transform indicator;
    Transform bottomIndicator;
    //------------------------------------------------------------------------

    protected virtual void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        controller = FindObjectOfType<PlayerController>();
        selector = FindObjectOfType<Selector>();
        //mat = GetComponent<MeshRenderer>().material;

        indicator = transform.GetChild(0);
        bottomIndicator = indicator.GetChild(0);

    }
    protected virtual void OnEnable()
    {
        //스태터스 초기화
        InitializeUnitStatus();
        Debug.Log($"currentHp : {currentHp}");
    }

    protected virtual void Start()
    {
        selector.onChangedUnits += OnUnitsSelected;         //유닛리스트 함수 등록
        controller.onCancel += OnUnitsCancel;               //유닛취소 함수 등록
        controller.onSetDestination += OnSetDestination;    //목적지 설정 델리게이트 수신시 실행할 함수등록


        //변수 초기화
        unitsList = null;
        onMove = false;
        TargetPos = transform.position;

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
                    indicator.GetComponent<SpriteRenderer>().color = Color.green;
                    bottomIndicator.gameObject.SetActive(true);          //바닥 선택 표시
                    Debug.Log($"Unit포함됨:{unit}");
                }
                else if (unit == this.gameObject && onSelected && !controller.dragging)
                {
                    onSelected = false;
                    indicator.GetComponent<SpriteRenderer>().color = Color.red;
                    bottomIndicator.gameObject.SetActive(false);          //바닥 선택 표시제거
                    Debug.Log($"Unit해제됨:{unit}");
                }
            }

        }

        RefreshUnitsList();
    }


    private void OnUnitsCancel()
    {
        unitsList.Clear();
        RefreshUnit();
    }

    private void RefreshUnit()
    {
        onSelected = false;
        //Indicator 설정 ---------------------------
        //anim.SetBool("Click", false);
        bottomIndicator.gameObject.SetActive(false);
    }
    /// <summary>
    /// 유닛 스텟 초기화하는 함수
    /// </summary>
    protected virtual void InitializeUnitStatus()
    {
        HP = maxHp;
        MP = maxMp;
        originMoveSpeed = currentMoveSpeed;
        MoveSpeed = originMoveSpeed;
        AttackSpeed = attackSpeed;
        Defence = defence;
    }
    /// <summary>
    /// 목적지 설정
    /// </summary>
    /// <param name="Destination">우클릭 한 지점(목적지)</param>
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
                        anim.SetBool("onMove", onMove);
                    }
                }
            }
            else  //선택된 유닛이 없으면,
            {
                onMove = false;
            }
        }
    }
    void Update()
    {
        OnUpdate();

        if (onSelected)
        {
            if (TargetPos != transform.position)
            {
                transform.Translate(Time.deltaTime * currentMoveSpeed * transform.forward, Space.World);

                if (unitsList.Count >0 && unitsList.Count < 4)
                {
                    stopDistanceSqr = 1f;
                }
                else
                {
                    stopDistanceSqr = 4.0f;
                }

                if ((TargetPos - transform.position).sqrMagnitude < stopDistanceSqr)
                {
                  
                    OnStop();
                }
            }

            OnSelcted();
        }
        else
        {
            if ((TargetPos - transform.position).sqrMagnitude > stopDistanceSqr)
            {
                if (onMove)
                {
                    transform.Translate(Time.deltaTime * currentMoveSpeed * transform.forward, Space.World);

                    if ((TargetPos - transform.position).sqrMagnitude < stopDistanceSqr)
                    {
                        OnStop(); //유닛 움직임 제어
                    }
                }
                else
                {
                    transform.Translate(Time.deltaTime * currentMoveSpeed * transform.forward, Space.World);
                    if ((TargetPos - transform.position).sqrMagnitude < stopDistanceSqr)
                    {
                        OnStop(); //유닛 움직임 제어
                    }
                }
            }
        }

    }
    /// <summary>
    /// 멈추는 함수
    /// </summary>
    protected void OnStop()
    {
        TargetPos = transform.position;
        onMove = false;
        anim.SetBool("onMove", onMove);
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        rigid.inertiaTensor = Vector3.zero;
    }

    private void OnCollisionEnter(Collision collision) 
    {
    }

    /// <summary>
    /// 유닛리스트 갱신하는 함수
    /// </summary>
    public void RefreshUnitsList()
    {
        if (!unitsList.Contains(this.gameObject))
        {
            onSelected = false;
            indicator.GetComponent<SpriteRenderer>().color = Color.red;
            bottomIndicator.gameObject.SetActive(false);          //바닥 선택 표시제거

        }
        else
        {
            foreach (var unit in unitsList)
            {
                if (unit == this.gameObject)
                {
                    onSelected = true;
                    indicator.GetComponent<SpriteRenderer>().color = Color.green;
                    bottomIndicator.gameObject.SetActive(true);
                }

            }
        }
    }
    /// <summary>
    /// 죽었을 때 실행될 함수
    /// </summary>
    private void OnDie()
    {
        OnOverrideDie();
    }

    /// <summary>
    /// 선택되었을 때 업데이트에서 실행할 덮어쓰기용 함수
    /// </summary>
    protected virtual void OnSelcted()
    {

    }
    /// <summary>
    /// 선택상관없이 업데이트에서 실행할 덮어쓰기용 함수
    /// </summary>
    protected virtual void OnUpdate()
    {

    }
    /// <summary>
    /// 죽을 떄 자식에서 실행될 함수
    /// </summary>
    protected virtual void OnOverrideDie()
    {

    }

    //------------------테스트용  ----------------------------
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 from = transform.position;
        Vector3 to = TargetPos;

        Gizmos.DrawLine(from, to);

        //stopDistance 알아보기
        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, transform.up, stopDistanceSqr);

    }
}
