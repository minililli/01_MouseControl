using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    PlayerInputActions inputActions;

    //------------------------------------------마우스 입력처리에 대한 변수
    Vector2 startClickPos;      //마우스 좌클릭 시작시 위치
    Vector2 endClickPos;        //마우스 좌클릭이 끝났을 시 위치
    Vector2 currentMousePos;    //현재 마우스 위치
    /// <summary>
    /// 드래그 여부 확인용 변수
    /// </summary>
    bool dragging = false;
    /// <summary>
    /// 유닛 선택여부 확인용 변수
    /// ture 면 선택, false면 선택안함
    /// </summary>
    bool isSelectedUnit = false;
    
    // 드래그시 필요한 지점 분해용 변수---------------------------------------
    float minX;
    float minY;
    float maxX;
    float maxY;

    float width;
    float height;
   
    //사용된 델리게이트------------------------------------------------------
    public Action<Unit> onSelectUnit;       //마우스 좌클릭. 선택된 유닛이 있음을 알리는 델리게이트
    public Action<Unit> onUnselectUnit;     //마우스 좌클릭. 선택에서 빠진 유닛이 있음을 알리는 델리게이트
    public Action<Vector3> onSetDestination; //마우스 우클릭. 유닛 선택 후 유닛 미선택시 위치 지정을 알리는 델리게이트
    public Action<Unit> onAttackTarget;     //마우스 우클릭. 유닛선택 후 (적)유닛 선택시 공격타겟지정임을 알리는 델리게이트
    
    //유닛 관련-------------------------------------------------------------
    int Unitsize = 12;      //최대 선택가능한 유닛수
    List<Unit> UnitList;    //선택된 유닛들목록 (부대_최대 UnitSize)
    Unit unit;              //선택된 유닛을 담을 변수

    Camera playerCamera;
    

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        unit = gameObject.GetComponent<Unit>();
    }
   
    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.LClick.performed += OnLeftClick;
        inputActions.Player.LClick.canceled += OnLeftEndClick;
        inputActions.Player.RClick.performed += OnRightClick;
        inputActions.Player.Attack.performed += OnAttack;
    }

    private void OnDisable()
    {
        inputActions.Player.Attack.performed -= OnAttack;
        inputActions.Player.RClick.performed -= OnRightClick;
        inputActions.Player.LClick.canceled -= OnLeftEndClick;
        inputActions.Player.LClick.performed -= OnLeftClick;
        inputActions.Player.Disable();
    }

    void UnitActionsOnEnable()
    {
        inputActions.Unit.Enable();
        inputActions.Unit.Attack.performed += OnAttack;
        inputActions.Unit.Stop.performed += OnStop;
        inputActions.Unit.Patrol.performed += OnPatrol;
        inputActions.Unit.Hold.performed += Hold_performed;
    }
    void UnitActionsOnDisable()
    {
        inputActions.Unit.Hold.performed -= Hold_performed;
        inputActions.Unit.Patrol.performed -= OnPatrol;
        inputActions.Unit.Stop.performed -= OnStop;
        inputActions.Unit.Attack.performed -= OnAttack;
        inputActions.Unit.Enable();
    }

    private void Start()
    {
        onSelectUnit += OnSelectUnits;
        onUnselectUnit += OnUnselectUnits;
    }
    private void Update()
    {
        currentMousePos = Input.mousePosition;
        Vector2 delta = currentMousePos - startClickPos;
        //Debug.Log(currentMousePos);
        if (dragging)
        {
            float minX = startClickPos.x;
            float minY = startClickPos.y;
            float maxX = currentMousePos.x;
            float maxY = currentMousePos.y;

            float width = maxX - minX;
            float height = maxY - minY;

            Vector2 node2= new (maxX, minY);
            Vector2 node3 = new (minX, maxY);

        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(startClickPos, 0.5f);
        Gizmos.DrawWireSphere(endClickPos, 0.5f);

    }
    private void OnLeftClick(InputAction.CallbackContext _)
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        startClickPos = mousePos;
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out RaycastHit hitinfo))
        {
            if (hitinfo.collider.gameObject.CompareTag("Unit"))
            {
                dragging = false;
                Unit hitObj = hitinfo.collider.gameObject.GetComponent<Unit>();
                onSelectUnit?.Invoke(hitObj);
                Debug.Log($"hitObject : {hitObj.name}");
            }
            else
            {
                Debug.Log("드래그중임");
                dragging = true;
            }
        }

    }

    private void OnLeftEndClick(InputAction.CallbackContext _)
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        Vector3 screenPos = Camera.main.WorldToScreenPoint(mousePos);
        endClickPos = mousePos;

        if (dragging)
        {
            dragging = false;
            Debug.Log("드래그완료");
            Debug.Log($"Rect Node:{startClickPos},({endClickPos.x},{startClickPos.y}),({startClickPos.x},{endClickPos.y}),{endClickPos}");
        }
    }

    private void OnRightClick(InputAction.CallbackContext _)
    {
        Vector3 screenPos = Mouse.current.position.ReadValue();
        Vector3 ViewPortPos = Camera.main.ScreenToViewportPoint(screenPos);
        Vector3 ViewToWorldPos = Camera.main.ViewportToWorldPoint(new Vector3(ViewPortPos.x, ViewPortPos.y, Camera.main.nearClipPlane));
        
            onSetDestination?.Invoke(ViewToWorldPos);
        Ray ray = Camera.main.ScreenPointToRay(ViewToWorldPos);
        if (Physics.Raycast(ray, out RaycastHit hitinfo))
        {
           /* if (hitinfo.collider.gameObject.layer == LayerMask.GetMask("Land"))
            {
                onSetDestination?.Invoke(ViewToWorldPos);
            }
            else if(hitinfo.collider.gameObject.layer == LayerMask.GetMask("Enemy"))
            {
                Unit enemy = hitinfo.collider.gameObject.GetComponent<Unit>();
                onAttackTarget?.Invoke(enemy);
            }*/
        }
        //Debug.Log(screenPos);
        //Debug.Log(ViewPortPos);
        Debug.Log($"Player Controller:{ViewToWorldPos}");

        
        //onSetDestination?.Invoke(screenPos);
    }

    private void OnAttack(InputAction.CallbackContext _)
    {
        throw new NotImplementedException();
    }


    private void OnPatrol(InputAction.CallbackContext _)
    {
        throw new NotImplementedException();
    }

    private void OnStop(InputAction.CallbackContext _)
    {
        throw new NotImplementedException();
    }

    private void Hold_performed(InputAction.CallbackContext obj)
    {
        throw new NotImplementedException();
    }


    void OnSelectUnits(Unit targetUnit)
    {
        isSelectedUnit = true;
        UnitList = new List<Unit>(Unitsize);
        UnitList.Add(targetUnit);
    }
    void OnUnselectUnits(Unit targetUnit)
    {
        UnitList.Remove(targetUnit);
    }
}
