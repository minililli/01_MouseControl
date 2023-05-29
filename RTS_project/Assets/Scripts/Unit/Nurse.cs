using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Nurse : Unit
{
    public float healRange = 3.0f;    //회복물체 추적 반경
    public float interval = 0.5f;
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
    bool chase = false;
    protected override void Awake()
    {
        base.Awake();
        healInterval = new WaitForSeconds(interval);
    }

    protected override void Start()
    {
        base.Start();
        StartCoroutine(OnHeal(FindPatient(healRange)));
    }

    /// <summary>
    /// 선택상관없이 Update()에서 실행될 함수
    /// </summary>
    protected override void OnUpdate()
    {

        FindPatient(healRange);

        if (this.TargetPos == transform.position && target != null)
        {
            chase = true;
            TargetPos = this.target.transform.position;
            transform.LookAt(TargetPos);
            
        }
    }

    private Unit FindPatient(float radius)
    {
        Unit patient = null;
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, LayerMask.GetMask("Unit"));
        List<Unit> targets = new List<Unit>(colliders.Length);
        foreach (var col in colliders)
        {
            Unit tmp = col.GetComponent<Unit>();
            if (tmp != this && tmp.HP < tmp.maxHp)
            {
                targets.Add(tmp);
                Debug.Log(tmp);
            }
        }
        if (targets.Count > 0)
        {

            float result = (transform.position - targets[0].transform.position).sqrMagnitude;
            int calIndex = 0;
            for (int i = 0; i < targets.Count; i++)
            {
                float cal = (transform.position - targets[i].transform.position).sqrMagnitude;
                if (result > cal)
                {
                    result = cal;
                    calIndex = i;
                }
            }
            patient = targets[calIndex];
            target = patient.gameObject;
        }
        return patient;
    }

    IEnumerator OnHeal(Unit patient)
    {
        if (chase)
        {
            while (patient.HP < patient.maxHp)
            {
                Debug.Log($"Heal중임 : +{healAmount}");
                patient.HP += healAmount;
                yield return healInterval;
            }
        }
        else
        {
            StopCoroutine(OnHeal(patient));
        }
    }

    private void OnDrawGizmos()
    {
        Handles.color = Color.green;
        Handles.DrawWireDisc(transform.position, transform.up, healRange);
    }
}
