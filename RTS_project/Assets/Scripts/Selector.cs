using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : MonoBehaviour
{
    PlayerController controller;

    //드래그선택 관련 -------------------------------------------------------------
    Vector3 startClickPos;
    Vector3 endClickPos;

    //유닛 관련-------------------------------------------------------------
    Unit[] units;           //게임씬 내 모든 유닛
    int unitSize = 12;      //최대 선택가능한 유닛수

    List<GameObject> unitsList;    //선택된 유닛들목록 (무리_최대 UnitSize)

    public Action<List<GameObject>> onChangedUnits;
    public List<GameObject> UnitsList
    {
        get => unitsList;

    }

    bool onSelect = false;  //선택한 유닛 유무 확인 변수

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        units = FindObjectsOfType<Unit>();
        unitsList = new List<GameObject>(unitSize);
    }

    private void Start()
    {
        controller.onDragStart = (pos) => startClickPos = pos;
        controller.onDragEnd = (pos) => endClickPos = pos;
        foreach (Unit unit in units)
        {
            unit.onSelectUnit += OnSelect;
            unit.onUnselectUnit += OnUnSelect;
        }
    }

    public void OnDrag(Vector3 startPos, Vector3 endPos)    //5/18
    {
        float minX = startClickPos.x;
        float minY = startClickPos.y;
        float minZ = startClickPos.z;
        float maxX = endClickPos.x;
        float maxY = endClickPos.y;
        float maxZ = endClickPos.z;

        float centerX = (maxX - minX) * 0.5f;
        float centerY = (minY - maxY) * 0.5f;
        float centerZ = (minZ - maxZ) * 0.5f;

        Vector3 Center = new Vector3(centerX, centerY, centerZ);

        Collider[] dragColliders = Physics.OverlapBox(Center, transform.localScale * 0.5f, transform.rotation, 6);

        foreach(var col in dragColliders)
        {
            OnSelect(col.gameObject);
        }
    }


    public void OnSelect(GameObject SelectedUnit)
    { 
        if (unitsList.Count <= unitSize)
        {
            if (!UnitsList.Contains(SelectedUnit))
            {
                UnitsList.Add(SelectedUnit);
                onChangedUnits?.Invoke(unitsList);
            }
            else
            {
                //아무것도 하지 않음.
            }
        }
        else
        {
            // 리스트가 꽉찼으면, 아무것도 하지 않음.
        }
    }

    /// <summary>
    /// 드래그 시 리스트에 넣을 유닛들
    /// </summary>
    /// <param name="SelectedUnits"></param>
    public void OnSelect(GameObject[] SelectedUnits)
    {
        foreach (var obj in SelectedUnits)
        {
            if (!UnitsList.Contains(obj))
            {
                UnitsList.Add(obj);
                onChangedUnits?.Invoke(unitsList);
            }
        }
    }


    public void OnUnSelect(GameObject obj)
    {
        UnitsList.Remove(obj);
        onChangedUnits?.Invoke(unitsList);
    }


    //Test용 코드--------------------------------------------------
    public void CheckSelect()
    {
        Debug.Log($"{unitsList.Count}");
        foreach (var obj in unitsList)
        {
            Debug.Log($"UnitList : {obj.name}");
        }
    }

    public void RefreshSelect()
    {
        unitsList.Clear();
    }
}
