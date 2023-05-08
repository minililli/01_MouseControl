using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;

public class Unit : MonoBehaviour
{
    PlayerController controller;

    public float rotateSpeed = 180.0f;
    public float moveSpeed = 2.0f;
    bool onSelected = false;

    Vector3 targetPos;
    public Vector3 TargetPos
    {
        get => targetPos;
        set
        {
            value.y = transform.position.y;
            targetPos = value;
            //transform.LookAt(targetPos);
        }
    }
    Vector3 targetDir;

    void Start()
    {
        TargetPos = transform.position;
        controller = FindObjectOfType<PlayerController>();

        controller.onSelectUnit += (unit) => onSelected = true;
        controller.onSetDestination += SetDestination;
    }

    void Update()
    {
        if (TargetPos != transform.position)
        {
            transform.Translate(Time.deltaTime * moveSpeed * targetDir);
            if ((TargetPos - transform.position).sqrMagnitude < 0.01f)
            {
                TargetPos = transform.position;
            }
        }
    }
    void SetDestination(Vector3 Destination)
    {
        Debug.Log(Destination);
        transform.LookAt(TargetPos, Vector3.up);
        TargetPos = Destination;
        targetDir = (TargetPos - transform.position).normalized;

        Debug.Log($"델리게이트 성공 : {Destination}");
    }
}
