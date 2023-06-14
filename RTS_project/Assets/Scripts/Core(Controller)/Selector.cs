using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class Selector : MonoBehaviour
{
    PlayerController controller;

    //드래그선택 관련 -------------------------------------------------------------
    Vector3 startClickPos;
    Vector3 endClickPos;

    //유닛 관련-------------------------------------------------------------
    Unit[] allUnits;           //게임씬 내 모든 유닛
    List<Unit> AllUnitsList;    //게임씬 내 모든 유닛 리스트 


    public Action<List<GameObject>> onChangedUnits;

    bool onDrag = false;  // 드래그 확인 변수
    List<GameObject> selectedUnitsList;    //선택된 유닛들목록 (무리_최대 UnitSize)
    int unitSize = 12;      //최대 선택가능한 유닛수
    public List<GameObject> SelectedUnitsList
    {
        get => selectedUnitsList;

    }


    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        allUnits = FindObjectsOfType<Unit>();
        selectedUnitsList = new List<GameObject>(unitSize);
    }

    private void Start()
    {
        controller.onDragStart = (pos) => startClickPos = pos;
        controller.onDragEnd = (pos) =>
        {
            endClickPos = pos;
        };

        controller.onDragging += OnDrag;
        controller.onClickUnit += OnSelect;
        controller.onCancel += OnSelectCancel;

    }
    /// <summary>
    /// 드래그 중일 때의 처리될 유닛 확인용 함수
    /// </summary>
    /// <param name="cursorPos">현재 마우스 커서의 위치</param>
    public void OnDrag(Vector3 cursorPos)
    {
        onDrag = true;
        float minX = startClickPos.x;
        float minY = startClickPos.y;
        float minZ = startClickPos.z;

        Ray ray = Camera.main.ScreenPointToRay(cursorPos);
        Physics.Raycast(ray, out RaycastHit hitinfo);
        Vector3 currentMousePos = hitinfo.point;

        float maxX = currentMousePos.x;
        float maxY = currentMousePos.y;
        float maxZ = currentMousePos.z;


        OnSelectCancel();

        // 게임 내 모든 유닛 찾아놓기
        foreach (var obj in allUnits)
        {
            if (minX < obj.transform.position.x && obj.transform.position.x < maxX && obj.transform.position.z > maxZ && obj.transform.position.z < minZ)
            {
                OnDragSelect(obj.gameObject);
            }
        }

    }
    /// <summary>
    /// 선택했을 때 처리될 함수
    /// </summary>
    /// <param name="SelectUnit">선택된 유닛 </param>

    public void OnSelect(GameObject SelectUnit)
    {
        if (selectedUnitsList.Count <= unitSize)
        {
            //선택된 유닛을 포함하면(이중선택에 대한 처리)
            if (SelectedUnitsList.Contains(SelectUnit))
            {
                OnUnSelect(SelectUnit);
            }
            else
            {
                SelectedUnitsList.Add(SelectUnit);
                onChangedUnits?.Invoke(selectedUnitsList);
            }
        }
        //유닛리스트의 용량이 부족할 때
        else
        {
            Debug.Log("Full");
        }
    }
    /// <summary>
    /// 유닛 다중 선택처리하는 함수
    /// </summary>
    /// <param name="dragUnit"> 드래그한 유닛 </param>
    public void OnDragSelect(GameObject dragUnit)
    {
        if (selectedUnitsList.Count <= unitSize)
        {
            if (!SelectedUnitsList.Contains(dragUnit))
            {
                SelectedUnitsList.Add(dragUnit);
                onChangedUnits?.Invoke(selectedUnitsList);
            }
        }
    }


    public void OnUnSelect(GameObject obj)
    {
        selectedUnitsList.Remove(obj);
        Debug.Log($"{obj.name}해제");
        onChangedUnits?.Invoke(selectedUnitsList);
    }

    public void OnSelectCancel()
    {
        selectedUnitsList.Clear();
        onChangedUnits?.Invoke(selectedUnitsList);
    }

    //Test용 코드--------------------------------------------------
    public void CheckSelect()
    {
        Debug.Log($"{selectedUnitsList.Count}");
        foreach (var obj in selectedUnitsList)
        {
            Debug.Log($"UnitList : {obj.name}");
        }
    }

    public void DeleteUnits(Unit unit)
    {
        AllUnitsList.Remove(unit);
    }


}
