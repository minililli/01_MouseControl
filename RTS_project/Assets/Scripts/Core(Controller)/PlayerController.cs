using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class PlayerController : MonoBehaviour
{
    PlayerInputActions inputActions;
    Camera controllerCamera;
    //------------------------------------------마우스 입력처리에 대한 변수
    Vector3 startClickPos;      //마우스 좌클릭 시작시 위치
    Vector3 endClickPos;        //마우스 좌클릭이 끝났을 시 위치
    Vector3 currentMousePos;    //현재 마우스 위치

    /// <summary>
    /// 드래그 여부 확인용 변수
    /// </summary>
    public bool dragging = false;

    /// <summary>
    /// 유닛 선택여부 확인용 변수
    /// ture 면 선택, false면 선택안함
    /// </summary>
    bool onSelect = false;

    //사용된 델리게이트------------------------------------------------------

    public Action<GameObject> onClickUnit;  //마우스 좌클릭. 유닛 선택을 알리는 델리게이트
    public Action<Vector3> onSetDestination; //마우스 우클릭. 유닛 선택 후 유닛 미선택시 위치 지정을 알리는 델리게이트
    public Action<Unit> onAttackTarget;     //마우스 우클릭. 유닛선택 후 (적)유닛 선택시 공격타겟지정임을 알리는 델리게이트
    public Action<Vector3> onDragStart; //드래그시작시 startPos 전달하는 델리게이트
    public Action<Vector3> onDragging; //드래그중임을 알리는 델리게이트
    public Action<Vector3> onDragEnd; //드래그종료시 EndPos 전달하는 델리게이트
    public Action onCancel;   //선택 취소를 알리는 델리게이트

    Unit unit;              //선택된 유닛을 담을 변수

    //마우스 위치를 따라올 플레이어카메라
    Camera playerCamera;    

    // 드래그시 필요한 지점 분해용 변수---------------------------------------
    float minX;
    float minY;
    float maxX;
    float maxY;

    float width;
    float height;


    private void Awake()
    {
        inputActions = new PlayerInputActions();
        unit = gameObject.GetComponent<Unit>();
        controllerCamera = transform.GetComponentInChildren<Camera>();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.LClick.performed += OnLeftClick;
        inputActions.Player.LClick.canceled += OnLeftEndClick;
        inputActions.Player.RClick.performed += OnRightClick;
        inputActions.Player.Attack.performed += OnAttack;
        inputActions.Player.CancelESC.performed += OnCancel;
    }


    private void OnDisable()
    {
        inputActions.Player.CancelESC.performed -= OnCancel;
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
    private void Update()
    {
        // 게임오브젝트가 선택되어지고 있음을 알려줌
        if (dragging) 
        {
            onDragging?.Invoke(Input.mousePosition);
        }
    }

    private void OnLeftClick(InputAction.CallbackContext _)
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out RaycastHit hitinfo))
        {
            if (hitinfo.collider.gameObject.GetComponent<Unit>())
            {
                dragging = false;
                GameObject hitObj = hitinfo.collider.gameObject;

                onClickUnit?.Invoke(hitObj);
                Debug.Log($"hitObject : {hitObj.name}");
            }

            else
            {
                startClickPos = hitinfo.point;
                onDragStart?.Invoke(startClickPos);
                Debug.Log("드래그 중임");
                dragging = true;
            }
        }

    }

    private void OnLeftEndClick(InputAction.CallbackContext _)
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        Physics.Raycast(ray, out RaycastHit hitinfo);
        if (dragging)
        {
            dragging = false;
            endClickPos = hitinfo.point;
            onDragEnd?.Invoke(endClickPos);
            //Debug.Log("드래그완료");
        }
    }

    private void OnRightClick(InputAction.CallbackContext _)
    {
        Vector3 screenPos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hitinfo))
        {
            // Debug.Log(hitinfo.point);
            if (hitinfo.collider.gameObject.layer == 3)
            {
                onSetDestination?.Invoke(hitinfo.point);
            }
            else if (hitinfo.collider.gameObject.layer == LayerMask.GetMask("Enemy"))
            {
                Unit enemy = hitinfo.collider.gameObject.GetComponent<Unit>();
                onAttackTarget?.Invoke(enemy);
            }
        }

    }
    private void OnCancel(InputAction.CallbackContext _)
    {
        onCancel?.Invoke();
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


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(startClickPos, 1f);
        Gizmos.DrawSphere(endClickPos, 1f);
    }
}
