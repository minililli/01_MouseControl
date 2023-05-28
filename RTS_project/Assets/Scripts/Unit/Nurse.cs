using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Nurse : Unit
{
    public float healRange = 5.0f;    //회복물체 추적 반경
    public float interval = 0.2f;
    public LayerMask targetLayer;
    WaitForSeconds healInterval;
    GameObject target;          //힐 대상
    float healAmount = 10f;   //힐량
    public float HealAmount
    {
        get => healAmount;
        private set
        {
            healAmount = value;
        }
    }

    private void Awake()
    {
        healInterval = new WaitForSeconds(interval);
    }

    private void Start()
    {
        
    }
    private void Update()
    {
        if(!onMove)
        {
            FindPatient(healRange);
        }
    }
    private Unit FindPatient(float radius)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, targetLayer);
        float distSqr = Vector3.SqrMagnitude(transform.position - colliders[0].transform.position);
        foreach (var col in colliders)
        {
            float distanceSqr = Vector3.SqrMagnitude(transform.position - col.transform.position);
            if (distSqr < distanceSqr)
            {
                target = colliders[0].gameObject;
            }
            else
            {
                target = col.gameObject;
            }
        }
        Unit unit = target.GetComponent<Unit>();
        return unit;
    }

    IEnumerator OnHealUnit(Unit target)
    {
        while (target != null && target.HP < target.maxHp)
        {
            TargetPos = target.transform.position;
            if ((TargetPos - transform.position).sqrMagnitude < 1f)
            {
                TargetPos = transform.position;
            }
            yield return null;
            StartCoroutine(Heal(target));
        }
    }

    IEnumerator Heal(Unit target)
    {
        while (target.HP < target.maxHp)
        {
            target.HP += healAmount;
            yield return healInterval;
        }
    }

    private void OnDrawGizmos()
    {
        Handles.color = Color.green;
        Handles.DrawWireDisc(transform.position, transform.up, healRange);
    }
}
