using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Nurse : Unit
{
    [Header("Nurse특성")]
    public float healRange = 3.0f;    //회복물체 추적 반경
    public float interval = 5f;

    WaitForSeconds healInterval;

    public float healAmount = 10f;   //힐량
    public float HealAmount
    {
        get => healAmount;
        private set
        {
            healAmount = value;
        }
    }
    bool onAction = false;   //패시브활동 여부

    protected override void Awake()
    {
        base.Awake();
        healInterval = new WaitForSeconds(interval);
    }

    protected override void Start()
    {
        base.Start();
    }

    /// <summary>
    /// 선택상관없이 Update()에서 실행될 함수
    /// </summary>
    protected override void OnUpdate()
    {
        //안움직이고, 패시브활동중이 아니면,
        if (!onMove && !onAction)
        {
            Unit Target = FindPatient(healRange);
            if (Target != null)
            {
                TargetPos = Target.transform.position;
                this.transform.LookAt(TargetPos);
                onMove = true;
                anim.SetBool("onMove", onMove);
                if((TargetPos-transform.position).sqrMagnitude < healRange*healRange)
                {
                    OnStop();
                }
            }
        }
    }
    /// <summary>
    /// [패시브스킬] 주변 환자 찾기
    /// </summary>
    /// <param name="radius">환자를 찾을 범위</param>
    /// <returns></returns>
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
        // currentHp < maxHp 인 유닛이 있을 때,
        if (targets.Count > 0)
        {
            Debug.Log("target 존재");
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
        }
        StartCoroutine(OnHeal(patient));
        return patient;
    }

    IEnumerator OnHeal(Unit target)
    {
        if (target!=null)
        {
            while (target.HP < target.maxHp && (target.transform.position - transform.position).sqrMagnitude < healRange*healRange)
            {
                onAction = true;
                this.anim.SetBool("Action", onAction);
                Debug.Log($"Heal중임 : +{healAmount}");
                target.HP += healAmount;
                yield return healInterval;
            }
            target = null;
            onAction = false;
        }
        else
        {
            onAction = false;
            StopCoroutine(OnHeal(target));
        }
        this.anim.SetBool("Action", onAction);
    }

    private void OnDrawGizmos()
    {
        Handles.color = Color.green;
        Handles.DrawWireDisc(transform.position, transform.up, healRange);
        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, transform.up, stopDistanceSqr);
    }
}
