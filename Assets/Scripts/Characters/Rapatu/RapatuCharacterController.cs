using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class RapatuCharacterController : CharacterMaster
{
    [Header("--- RAPATU ---")]



    [Header("PASSIVE SKILL")]

    [Header("Q SKILL")]

    [Header("W SKILL")]

    [Header("E SKILL")]

    public GameObject m_HealingAreaEPrefab;
    [Tooltip("Cuantas veces cura")]
    public int m_HealingTimes = 4;
    [Tooltip("Cada cuanto cura")]
    public float m_HealingTimeSpace = 1f;
    public float m_HealingAreaRadius = 500;
    public float m_PercentageSkillPower = 50;
    public float m_PercentageAdditionalLife = 7;
    RapatuEHealingArea m_HealingAreaE;
    bool m_CanStopE = false;
    public float m_AmimDelayE = 0.5f;
    public float m_ECurrentHealingTime = 0;
    public float m_EMaxHealingTime = 0;

    [Header("R SKILL")]
    public float m_RangeR = 1000;
    public float m_AutoJumpSecondsR = 2;

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

        if (m_RSkill.GetUsingSkill())
        {
            RHoldTimeCheck();
        }
        if (m_ESkill.GetUsingSkill() && m_CanStopE)
        {
            m_ECurrentHealingTime += Time.deltaTime;
            GetCharacterUI().UpdateCastingUI(m_ECurrentHealingTime, m_EMaxHealingTime);
            if (Input.GetKeyDown(m_ESkillKey))
            {
                StartCoroutine(StopESkill());
            }
        }

    }


    //Q SKILL
    protected override void QSkill()
    {
        if (m_ESkill.GetUsingSkill())
            return;
        if (m_RSkill.GetUsingSkill())
            return;
    }

    //W SKILL
    protected override void WSkill()
    {
        if (m_ESkill.GetUsingSkill())
            return;
        if (m_RSkill.GetUsingSkill())
            return;
    }



    //E SKILL
    protected override void ESkill()
    {
        if (m_ESkill.GetUsingSkill())
            return;
        if (m_RSkill.GetUsingSkill())
            return;

        StopAttacking();
        base.ESkill();
        m_ESkill.SetUsingSkill(true);
        SetAnimatorTrigger("IsUsingE");
        SetDisabled(true);
        m_CanStopE = false;
        if (!GetIsLookingForPosition())
            StopMovement();
        StartCoroutine(ESkillCoroutine());

        GetCharacterUI().SetCastingUIAbilityText("Curando");
        GetCharacterUI().HideCastingTime();
        GetCharacterUI().UpdateCastingUI(0, 1);

    }
    IEnumerator StopESkill()
    {
        SetAnimatorTrigger("EStop");
        if (!GetIsLookingForPosition())
            StopMovement();
        SetDisabled(false);
        GetCharacterUI().HideCastingUI();
        StopCoroutine(ESkillCoroutine());
        Destroy(m_HealingAreaE.gameObject);
        yield return new WaitForSeconds(0.1f);
        m_CanStopE = false;
        m_ESkill.SetUsingSkill(false);
    }

    IEnumerator ESkillCoroutine()
    {
        yield return new WaitForSeconds(m_AmimDelayE);
        m_HealingAreaE = Instantiate(m_HealingAreaEPrefab, transform.position, Quaternion.identity).GetComponent<RapatuEHealingArea>();
        m_HealingAreaE.SetHealingAria(transform, m_HealingAreaRadius / 100);
        m_ECurrentHealingTime = 0;
        m_EMaxHealingTime = m_HealingTimes - 1 * m_HealingTimeSpace;
        m_CanStopE = true;
        GetCharacterUI().ShowCastingUI();

        float l_HealLifeValue = (m_ESkill.GetAttribute("Curación", GetESkillLevel())) + (m_PercentageSkillPower / 100) * GetCharacterStats().GetAbilityPower() + (m_PercentageAdditionalLife / 100) * GetCharacterStats().GetBonusHealth();

        m_HealingAreaE.Heal(l_HealLifeValue);
        for (int i = 0; i < m_HealingTimes - 1; i++)
        {
            yield return new WaitForSeconds(m_HealingTimeSpace);
            if (!m_HealingAreaE.IsDestroyed())
                m_HealingAreaE.Heal(l_HealLifeValue);

        }

        if (!m_HealingAreaE.IsDestroyed())
            StartCoroutine(StopESkill());
    }
    //R SKILL
    protected override void RSkill()
    {
        if (m_RSkill.GetUsingSkill())
            return;
        if (m_ESkill.GetUsingSkill())
            return;

        StopAttacking();
        m_RSkill.SetUsingSkill(true);
        SetDisabled(true);
        SetAnimatorTrigger("IsUsingR");
        if (!GetIsLookingForPosition())
            StopMovement();
        m_TimerR = 0;
        GetCharacterUI().SetCastingUIAbilityText("Cargando salto");
        GetCharacterUI().HideCastingTime();
        GetCharacterUI().UpdateCastingUI(0, 1);
        GetCharacterUI().ShowCastingUI();
    }
    float m_TimerR;
    public float m_MinTimeToBeRedy = 1f;
    bool m_Jumping = false;
    void RHoldTimeCheck()
    {
        if (!m_Jumping)
        {
            m_TimerR += Time.deltaTime;
            Debug.LogError("Cargando...");
            if (m_TimerR >= m_AutoJumpSecondsR)
            {
                StartJumpingR();
            }
            GetCharacterUI().UpdateCastingUI(m_TimerR, m_AutoJumpSecondsR);
        }
    }
    void StartJumpingR()
    {
        base.RSkill();
        GetCharacterUI().HideCastingUI();
        m_Jumping = true;
        SetAnimatorTrigger("RJump");
        JumpR();
    }
    public void EndJump()
    {
        Debug.LogError("Floor");
        EndRSkill();

    }

    void JumpR()
    {
        Debug.LogError("Jumping");
    }
    void EndRSkill()
    {
        SetAnimatorTrigger("RJumpStop");
        m_RSkill.SetUsingSkill(false);
        SetDisabled(false);
        m_Jumping = false;
    }

    public override void LevelUpRpc()
    {
        base.LevelUpRpc();
    }

    IEnumerator DisableForDuration(float Duration)
    {
        SetDisabled(true);
        yield return new WaitForSeconds(Duration);
        SetDisabled(false);
    }
}
