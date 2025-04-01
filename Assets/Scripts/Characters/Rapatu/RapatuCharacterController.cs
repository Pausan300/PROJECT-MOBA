using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RapatuCharacterController : CharacterMaster
{
    [Header("--- RAPATU ---")]
    [Header("PASSIVE SKILL")]

    [Header("Q SKILL")]

    [Header("W SKILL")]

    [Header("E SKILL")]

    [Header("R SKILL")]
    public float m_Delete;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }
    protected override void Update()
    {
        if (!IsSpawned || !HasAuthority)
        {
            return;
        }

        base.Update();


    }


    //Q SKILL
    protected override void QSkill()
    {
        Debug.LogError("Hi Q");
    }

    //W SKILL
    protected override void WSkill()
    {
        Debug.LogError("Hi W");

    }



    //E SKILL
    protected override void ESkill()
    {
        Debug.LogError("Hi E");
        base.ESkill();
        StartCoroutine(DisableForDuration(m_ESkill.m_SkillDisabledTime));
        SetAnimatorTrigger("IsUsingE");
    }

    //R SKILL
    protected override void RSkill()
    {
        Debug.LogError("Hi R");

    }

    public override void LevelUpRpc()
    {
        base.LevelUpRpc();
        Debug.LogError("Hi LVUP");
    }
    protected override void StartAttacking()
    {
        SetIsAttacking(true);
        Debug.LogError("Hi Start Attack");


    }
    protected override void StopAttacking()
    {
        if (GetIsAttacking())
        {
            Debug.LogError("Hi Stop Attack");
            SetIsAttacking(false);
        }
    }
    IEnumerator DisableForDuration(float Duration)
    {
        SetDisabled(true);
        yield return new WaitForSeconds(Duration);
        SetDisabled(false);
    }
    protected override void PerformAutoAttack()
    {
        if (!IsSpawned || !HasAuthority)
        {
            return;
        }
    }
}
