using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Nurse : Unit
{
    [Header("Nurse특성")]
    [Tooltip("healRange = SphereCollider.raidus")]
    public float healRange = 3.0f;    //회복물체 추적 반경
    public float interval = 5f;

    Coroutine healCoroutine = null;
    WaitForSeconds healInterval;
    Unit healTarget=null;
    public float healAmount = 10f;   //힐량
    public float HealAmount
    {
        get => healAmount;
        private set
        {
            healAmount = value;
        }
    }
    /// <summary>
    /// 개별능력 여부 확인용 변수(애니메이션 관여)
    /// </summary>
    bool onAction = false;

    protected override void Awake()
    {
        base.Awake();

        healInterval = new WaitForSeconds(interval);
    }


    protected override void OnUpdate()
    {
        if (!onMove && !onAction)
        {
            healTarget = FindPatient(healRange);
            if (healTarget != null && !onMove)
            {
                TargetPos = healTarget.transform.position;
                transform.LookAt(TargetPos);
                healCoroutine = StartCoroutine(OnHeal(healTarget));
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
            }
        }
        // currentHp < maxHp 인 유닛이 있을 때,
        if (targets.Count > 0)
        {
            float result = (transform.position - targets[0].transform.position).sqrMagnitude;
            int index = 0;
            for (int i = 0; i < targets.Count; i++)
            {
                float dis = (transform.position - targets[i].transform.position).sqrMagnitude;
                if (result > dis)
                {
                    result = dis;
                    index = i;
                }
            }
            patient = targets[index];
        }
        return patient;
    }


    IEnumerator OnHeal(Unit target)
    {
        if (target != null)
        {
            while (target.HP < target.maxHp && (target.transform.position - transform.position).sqrMagnitude < healRange * healRange)
            {
                transform.LookAt(target.transform.position);
                onAction = true;
                anim.SetBool("onAction", onAction);
                Debug.Log($"Heal중임 : +{healAmount}");
                target.HP += healAmount;
                yield return healInterval;
            }
            target = null;
        }
        onAction = false;
        anim.SetBool("onAction", onAction);
        StopCoroutine(OnHeal(target));
    }


    private void OnDrawGizmos()
    {
        Handles.color = Color.green;
        Handles.DrawWireDisc(transform.position, transform.up, healRange);
        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, transform.up, stopDistanceSqr);
    }
}
