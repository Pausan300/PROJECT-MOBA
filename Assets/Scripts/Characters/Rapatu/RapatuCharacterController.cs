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
    public RapatuEHealingArea m_HealingAreaE;
    [Tooltip("Cuantas veces cura")]
    public int m_HealingTimes = 4;
    [Tooltip("Cada cuanto cura")]
    public float m_HealingTimeSpace = 1f;
    public float m_XCSkillPower = 50;
    public float m_XCAdditionalLife = 7;

    [Header("R SKILL")]
    public float m_Delete;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        m_HealingAreaE.gameObject.SetActive(false);
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
    }

    //W SKILL
    protected override void WSkill()
    {

    }



    //E SKILL
    protected override void ESkill()
    {
        base.ESkill();
        m_ESkill.SetUsingSkill(true);
        SetAnimatorTrigger("IsUsingE");
        StartCoroutine(DisableForDuration(m_ESkill.m_SkillDisabledTime));
        StartCoroutine(ESkillCoroutine());

    }
    IEnumerator ESkillCoroutine()
    {
        yield return new WaitForSeconds(m_ESkill.m_SkillDisabledTime);
        m_HealingAreaE.gameObject.SetActive(true);
        m_ESkill.SetUsingSkill(false);
        m_HealingAreaE.transform.localScale = new Vector3(1 * ((m_ESkill.GetAttribute("Radio", GetESkillLevel()) / 100) * 2), m_HealingAreaE.transform.localScale.y, 1 * ((m_ESkill.GetAttribute("Radio", GetESkillLevel()) / 100) * 2));

        float l_HealLifeValue = (m_ESkill.GetAttribute("Curación", GetESkillLevel())) + (m_XCSkillPower / 100) * GetCharacterStats().GetAbilityPower() + (m_XCAdditionalLife / 100) * GetCharacterStats().GetBonusHealth();

        m_HealingAreaE.Heal(l_HealLifeValue);
        for (int i = 0; i < m_HealingTimes - 1; i++)
        {
            yield return new WaitForSeconds(m_HealingTimeSpace);
            m_HealingAreaE.Heal(l_HealLifeValue);

        }


        m_HealingAreaE.gameObject.SetActive(false);
    }
    //R SKILL
    protected override void RSkill()
    {

    }

    public override void LevelUpRpc()
    {
        base.LevelUpRpc();
    }
    protected override void StartAttacking()
    {
        SetIsAttacking(true);


    }
    protected override void StopAttacking()
    {
        if (GetIsAttacking())
        {
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
