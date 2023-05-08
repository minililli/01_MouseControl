using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    PlayerInputActions inputActions;

    //------------------------------------------���콺 �Է�ó���� ���� ����
    Vector2 startClickPos;      //���콺 ��Ŭ�� ���۽� ��ġ
    Vector2 endClickPos;        //���콺 ��Ŭ���� ������ �� ��ġ
    Vector2 currentMousePos;    //���� ���콺 ��ġ
    /// <summary>
    /// �巡�� ���� Ȯ�ο� ����
    /// </summary>
    bool dragging = false;
    /// <summary>
    /// ���� ���ÿ��� Ȯ�ο� ����
    /// ture �� ����, false�� ���þ���
    /// </summary>
    bool isSelectedUnit = false;
    
    // �巡�׽� �ʿ��� ���� ���ؿ� ����---------------------------------------
    float minX;
    float minY;
    float maxX;
    float maxY;

    float width;
    float height;
   
    //���� ��������Ʈ------------------------------------------------------
    public Action<Unit> onSelectUnit;       //���콺 ��Ŭ��. ���õ� ������ ������ �˸��� ��������Ʈ
    public Action<Unit> onUnselectUnit;     //���콺 ��Ŭ��. ���ÿ��� ���� ������ ������ �˸��� ��������Ʈ
    public Action<Vector3> onSetDestination; //���콺 ��Ŭ��. ���� ���� �� ���� �̼��ý� ��ġ ������ �˸��� ��������Ʈ
    public Action<Unit> onAttackTarget;     //���콺 ��Ŭ��. ���ּ��� �� (��)���� ���ý� ����Ÿ���������� �˸��� ��������Ʈ
    
    //���� ����-------------------------------------------------------------
    int Unitsize = 12;      //�ִ� ���ð����� ���ּ�
    List<Unit> UnitList;    //���õ� ���ֵ��� (�δ�_�ִ� UnitSize)
    Unit unit;              //���õ� ������ ���� ����

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
                Debug.Log("�巡������");
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
            Debug.Log("�巡�׿Ϸ�");
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
