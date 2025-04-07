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
    float m_CancelDelayE = 0.5f;
    public float m_ECurrentHealingTime = 0;
    public float m_EMaxHealingTime = 0;

    bool m_SaverCanDoE = true;

    [Header("R SKILL")]

    public GameObject m_RIndicatorPrefab;
    GameObject m_RIndicator;
    public float m_RangeR = 1000;
    public float m_AutoJumpSecondsR = 2;
    float m_TimerR;
    bool m_Jumping = false;
    bool m_SaverCanDoR = true;
    bool m_RSkillStarted = false;

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

        if (m_RSkill.GetUsingSkill() && m_RSkillStarted)
        {
            RHoldTimeCheck();
        }
        if (m_ESkill.GetUsingSkill() && m_CanStopE)
        {
            m_ECurrentHealingTime += Time.deltaTime;
            GetCharacterUI().UpdateCastingUI(m_ECurrentHealingTime, m_EMaxHealingTime);
            if (Input.GetKeyDown(m_ESkillKey))
            {
                StopESkill();
            }
        }


        if (GetUseSkillGizmos())
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (m_ESkill.GetUsingSkill())
                    StartCoroutine(ESkillCoroutine());

                if (m_RSkill.GetUsingSkill() && !m_RSkillStarted)
                    StartRSkill();
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
        if (!m_SaverCanDoE)
            return;



        m_ESkill.SetUsingSkill(true);
        if (GetUseSkillGizmos())
        {
            if (GetShowingGizmos())
            {
                m_SkillIndicatorUI.ClearDeletableSkillIndicatorUI();
                m_SkillIndicatorUI.ClearNormalSkillIndicatorUI();
                m_SkillIndicatorUI.ClearTargetSkillIndicatorUI();
            }
            m_SkillIndicatorUI.CreateCircleSkillIndicator(m_ESkill.m_IndicatorUIObject, m_HealingAreaRadius, transform, false);
            SetShowingGizmos(true);
        }
        else
            StartCoroutine(ESkillCoroutine());

    }


    IEnumerator ESkillCoroutine()
    {
        SetShowingGizmos(false);
        m_ESkill.SetUsingSkill(false);

        StopAttacking();
        SetAnimatorTrigger("IsUsingE");
        SetDisabled(true);
        m_CanStopE = false;
        if (!GetIsLookingForPosition())
            StopMovement();
        StartCoroutine(ESkillCanStop());

        GetCharacterUI().SetCastingUIAbilityText("Curando");
        GetCharacterUI().HideCastingTime();
        GetCharacterUI().UpdateCastingUI(0, 1);



        m_HealingAreaE = Instantiate(m_HealingAreaEPrefab, transform.position, Quaternion.identity, transform).GetComponent<RapatuEHealingArea>();
        m_HealingAreaE.SetHealingAria(transform, m_HealingAreaRadius / 100);
        m_ECurrentHealingTime = 0;
        m_EMaxHealingTime = m_HealingTimes - 1 * m_HealingTimeSpace;
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
            StopESkill();
    }
    void StopESkill()
    {
        StartCoroutine(RepeatESaver());
        SetAnimatorTrigger("EStop");
        if (!GetIsLookingForPosition())
            StopMovement();
        SetDisabled(false);
        GetCharacterUI().HideCastingUI();
        StopCoroutine(ESkillCoroutine());
        StopCoroutine(ESkillCanStop());
        Destroy(m_HealingAreaE.gameObject);
        base.ESkill();
        m_CanStopE = false;

    }
    IEnumerator ESkillCanStop()
    {
        yield return new WaitForSeconds(m_CancelDelayE);
        m_CanStopE = true;
    }
    IEnumerator RepeatESaver()
    {
        m_SaverCanDoE = false;
        yield return new WaitForSeconds(0.5f);
        m_SaverCanDoE = true;

    }

    //R SKILL
    protected override void RSkill()
    {
        if (m_RSkill.GetUsingSkill())
            return;
        if (m_ESkill.GetUsingSkill())
            return;
        if (!m_SaverCanDoR)
            return;

        m_RSkill.SetUsingSkill(true);

        if (GetUseSkillGizmos())
        {
            m_RSkillStarted = false;
            if (GetShowingGizmos())
            {
                m_SkillIndicatorUI.ClearDeletableSkillIndicatorUI();
                m_SkillIndicatorUI.ClearNormalSkillIndicatorUI();
                m_SkillIndicatorUI.ClearTargetSkillIndicatorUI();
            }
            m_SkillIndicatorUI.CreateCircleSkillIndicator(m_RSkill.m_IndicatorUIObject, m_RangeR, transform, false);
            SetShowingGizmos(true);
        }
        else
            StartRSkill();



    }
    void StartRSkill()
    {
        SetShowingGizmos(false);
        m_RSkillStarted = true;
        StopAttacking();
        SetDisabled(true);
        SetAnimatorTrigger("IsUsingR");
        if (!GetIsLookingForPosition())
            StopMovement();
        m_TimerR = 0;
        GetCharacterUI().SetCastingUIAbilityText("Cargando salto");
        GetCharacterUI().HideCastingTime();
        GetCharacterUI().UpdateCastingUI(0, 1);
        GetCharacterUI().ShowCastingUI();

        m_RIndicator = Instantiate(m_RIndicatorPrefab, transform.position, Quaternion.identity, transform);
    }
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

            if (Input.GetKeyDown(m_RSkillKey) && m_TimerR >= 0.2f)
            {
                Debug.LogError("Cancel R");
                StartCoroutine(RepeatRSaver());
                GetCharacterUI().HideCastingUI();
                base.RSkill();
                m_RSkill.SetTimer(m_RSkill.GetCd() / 2.0f);
                m_RSkill.SetUsingSkill(false);
                SetDisabled(false);
                SetAnimatorTrigger("RJumpStop");
                Destroy(m_RIndicator);
            }

        }

    }
    IEnumerator RepeatRSaver()
    {
        m_SaverCanDoR = false;
        yield return new WaitForSeconds(0.5f);
        m_SaverCanDoR = true;

    }
    void StartJumpingR()
    {

        GetCharacterUI().HideCastingUI();
        m_Jumping = true;
        SetAnimatorTrigger("RJump");
        StartCoroutine(JumpR());

    }
    public void EndJump()
    {
        Debug.LogError("Floor");
        EndRSkill();

    }

    IEnumerator JumpR()
    {
        Debug.LogError("Jumping");
        yield return new WaitForSeconds(1f);
        EndJump();
    }
    void EndRSkill()
    {
        base.RSkill();
        SetAnimatorTrigger("RJumpStop");
        m_RSkill.SetUsingSkill(false);
        SetDisabled(false);
        m_Jumping = false;

        Destroy(m_RIndicator); // Destruir en el ultimo salto
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
