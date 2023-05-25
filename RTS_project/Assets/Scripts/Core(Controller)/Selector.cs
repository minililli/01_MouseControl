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
        controller.onDragEnd = (pos) => endClickPos = pos;
        controller.onDragging += OnDrag;
        controller.onClickUnit += OnSelect;
        controller.onCancel += OnSelectCancel;

    }

    public void OnDrag(Vector3 cursorPos)    //5/18
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

        //float centerX = (maxX - minX) * 0.5f;
        //float centerY = (minY - maxY) * 0.5f;
        //float centerZ = (minZ - maxZ) * 0.5f;
        //
        //Vector3 Center = new Vector3(centerX, centerY, centerZ);
        foreach (var obj in allUnits)
        {
            if (minX < obj.transform.position.x && obj.transform.position.x < maxX && obj.transform.position.z > maxZ && obj.transform.position.z < minZ)
            {
                OnDragSelect(obj.gameObject);
            }
        }

    }


    public void OnSelect(GameObject SelectUnit)
    {
        if (selectedUnitsList.Count <= unitSize)
        {
            if(SelectedUnitsList.Contains(SelectUnit))    //선택된 유닛을 포함하면
            {
                OnUnSelect(SelectUnit);
            }
            else
            {
                SelectedUnitsList.Add(SelectUnit);
                onChangedUnits?.Invoke(selectedUnitsList);
            }
        }

        else
        {
            Debug.Log("Full");
        }
    }

    public void OnDragSelect(GameObject dragUnit)
    {
      if(selectedUnitsList.Count <= unitSize)
        {
            if(!SelectedUnitsList.Contains(dragUnit))
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

    public void OnSelectCancel()
    {
        selectedUnitsList.Clear();
        onChangedUnits?.Invoke(selectedUnitsList);
    }
}
