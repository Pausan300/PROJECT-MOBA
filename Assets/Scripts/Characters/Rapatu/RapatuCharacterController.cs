using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RapatuCharacterController : CharacterMaster
{
    [Header("--- RAPATU ---")]
    [Header("PASSIVE SKILL")]

    [Header("Q SKILL")]
    public float m_QAdditionalDamage;

    [Header("W SKILL")]
    public float m_WAdditionalDamageSplinter;

    [Header("E SKILL")]
    public float m_ERange;

    [Header("R SKILL")]
    public float m_RAdditionalDamage;

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
        StopSkills();
        m_QSkill.SetUsingSkill(true);
        StartCoroutine(DisableForDuration(m_QSkill.m_SkillDisabledTime));
        m_QSkill.SetUsingSkill(false);
    }

    //W SKILL
    protected override void WSkill()
    {
        Debug.LogError("Hi W");
        StopSkills();
        m_WSkill.SetUsingSkill(true);
        StartCoroutine(DisableForDuration(m_WSkill.m_SkillDisabledTime));
        m_WSkill.SetUsingSkill(false);

    }



    //E SKILL
    protected override void ESkill()
    {
        Debug.LogError("Hi E");
        StopSkills();
        m_ESkill.SetUsingSkill(true);
        StartCoroutine(DisableForDuration(m_ESkill.m_SkillDisabledTime));
        m_ESkill.SetUsingSkill(false);

    }
    
    //R SKILL
    protected override void RSkill()
    {
        Debug.LogError("Hi R");
        StopSkills();
        m_RSkill.SetUsingSkill(true);
        StartCoroutine(DisableForDuration(m_RSkill.m_SkillDisabledTime));
        m_RSkill.SetUsingSkill(false);

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
