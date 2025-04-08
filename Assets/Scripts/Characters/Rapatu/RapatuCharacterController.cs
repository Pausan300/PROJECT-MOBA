using NUnit.Framework;
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
    float m_ECurrentHealingTime = 0;
    float m_EMaxHealingTime = 0;
    bool m_ESkillStarted = false;

    bool m_SaverCanDoE = true;
    Coroutine m_ESkillCoroutine;

    [Header("R SKILL")]

    public GameObject m_RIndicatorPrefab;
    public float m_MAXRangeR = 1000;
    public float m_AutoJumpSecondsR = 2;
    public float m_JumpRAirDuration = 1f;
    public float m_GetUpTime = 1f;
    GameObject m_RIndicator;
    float m_TimerR;
    bool m_CanStopR = false;
    bool m_LoadingJump = false;
    bool m_Jumping = false;
    bool m_SaverCanDoR = true;
    bool m_RSkillStarted = false;
    Vector3 m_JumpRStartPosition;
    Vector3 m_JumpREndPosition;

    Coroutine m_RSkillCoroutine;
    float m_RJumpSpeed;

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
                Debug.Log("E Canceled");
                StopESkill();
            }
        }


        if (GetUseSkillGizmos())
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (m_ESkill.GetUsingSkill() && !m_ESkillStarted)
                    m_ESkillCoroutine = StartCoroutine(ESkillCoroutine());

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

            m_ESkillStarted = false;
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
            m_ESkillCoroutine = StartCoroutine(ESkillCoroutine());

    }


    IEnumerator ESkillCoroutine()
    {
        m_ESkillStarted = true;
        SetShowingGizmos(false);

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
        if (m_ESkillCoroutine != null)
        {
            StopCoroutine(m_ESkillCoroutine);
            m_ESkillCoroutine = null;
        }
        Destroy(m_HealingAreaE.gameObject);
        base.ESkill();
        m_CanStopE = false;
        m_ESkill.SetUsingSkill(false);

    }
    IEnumerator ESkillCanStop()
    {
        yield return null;// Esperar 1 frame por seguridad
        m_CanStopE = true;
    }
    IEnumerator RepeatESaver()
    {
        m_SaverCanDoE = false;
        yield return null; // Esperar 1 frame por seguridad
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
            m_SkillIndicatorUI.CreateCircleSkillIndicator(m_RSkill.m_IndicatorUIObject, m_MAXRangeR, transform, false);
            SetShowingGizmos(true);
        }
        else
            StartRSkill();



    }
    void StartRSkill()
    {
        SetShowingGizmos(false);
        m_CanStopR = false;
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
        m_LoadingJump = true;
        m_RSkillStarted = true;
        StartCoroutine(RSkillCanStop());

        m_RIndicator = Instantiate(m_RIndicatorPrefab, transform.position, Quaternion.identity);
    }
    void RHoldTimeCheck()
    {

        if (m_LoadingJump)
        {
            m_TimerR += Time.deltaTime;
            if (m_TimerR >= m_AutoJumpSecondsR)
            {
                Debug.Log("Saltando por tiempo");
                StartJumpingR();
            }
            GetCharacterUI().UpdateCastingUI(m_TimerR, m_AutoJumpSecondsR);

            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("Saltando por activacion");
                StartJumpingR();
            }

            if (Input.GetKeyDown(m_RSkillKey) && m_CanStopR)
            {
                Debug.Log("R Canceled");
                StartCoroutine(RepeatRSaver());
                GetCharacterUI().HideCastingUI();
                base.RSkill();
                m_RSkill.SetTimer(m_RSkill.GetCd() / 2.0f);
                m_RSkill.SetUsingSkill(false);
                SetDisabled(false);
                SetAnimatorTrigger("RJumpStop");
                Destroy(m_RIndicator);
                m_RSkillStarted = false;
                if (!GetIsLookingForPosition())
                    StopMovement();
                m_LoadingJump = false;
            }



        }
        else
        {
            if (Input.GetKeyDown(m_RSkillKey) && m_CanStopR)
            {
                Debug.Log("R Canceled");
                m_RStopJumping = true;
            }
        }

        if (Vector3.Distance(transform.position, GetPositionWithMouse()) <= m_MAXRangeR / 100)
        {
            m_RIndicator.transform.position = GetPositionWithMouse();
        }
        else
        {
            Vector3 direction = (GetPositionWithMouse() - transform.position).normalized;
            m_RIndicator.transform.position = transform.position + direction * m_MAXRangeR / 100;
        }

        if (m_Jumping)
        {
            transform.position = Vector3.MoveTowards(transform.position, m_JumpREndPosition, m_RJumpSpeed * Time.deltaTime);

            if (transform.position == m_JumpREndPosition)
            {
                m_Jumping = false;
            }
        }
        else
        {
            LookAt(m_RIndicator.transform.position);

        }
    }

    bool m_RStopJumping = false;
    void StartJumpingR()
    {
        GetCharacterUI().HideCastingUI();
        m_LoadingJump = false;
        m_RStopJumping = false;
        SetAnimatorTrigger("RJump");
        m_RSkillCoroutine = StartCoroutine(JumpingR());

    }

    public void StartJumpRAnim()
    {
        if (m_RIndicator == null)
            return;

        m_JumpREndPosition = m_RIndicator.transform.position;
        m_JumpRStartPosition = transform.position;
        float l_Distance = Vector3.Distance(m_JumpRStartPosition, m_JumpREndPosition);
        m_RJumpSpeed = l_Distance / m_JumpRAirDuration;
        m_Jumping = true;
    }


    IEnumerator JumpingR()
    {
        //int l_JumpsNum = 1;
        int l_JumpsNum = (int)m_RSkill.GetAttribute("Saltos", GetRSkillLevel());
        Debug.Log("Va a hacer " + m_RSkill.GetAttribute("Saltos", GetRSkillLevel()) + " saltos");
        bool l_StopJumpAnim = false;

        for (int i = 0; i < l_JumpsNum; i++)
        {
            while (!m_Jumping)
            {
                if (m_RStopJumping)
                {
                    SetAnimatorTrigger("RJumpCancel");
                    EndRSkill();
                }
                yield return null;
            }

            m_RIndicator.SetActive(false);

            if (i == l_JumpsNum - 1)
            {
                if (!l_StopJumpAnim)
                {
                    SetAnimatorTrigger("RJumpStop");
                    l_StopJumpAnim = true;
                }
            }

            if (m_RStopJumping)
            {
                if (!l_StopJumpAnim)
                {
                    SetAnimatorTrigger("RJumpStop");
                    l_StopJumpAnim = true;
                }
                m_RIndicator.SetActive(false);
            }

            while (m_Jumping)
                yield return null;

            if (i != l_JumpsNum - 1 && !m_RStopJumping)
                m_RIndicator.SetActive(true);

            if (m_RStopJumping)
            {
                if (!l_StopJumpAnim)
                {
                    SetAnimatorTrigger("RJumpStop");
                    l_StopJumpAnim = true;
                }
                EndRSkill();
            }

        }

        yield return new WaitForSeconds(m_GetUpTime);

        m_RSkillCoroutine = null;
        EndRSkill();
    }
    void EndRSkill()
    {
        if (m_RSkillCoroutine != null)
            StopCoroutine(m_RSkillCoroutine);
        m_RSkillCoroutine = null;

        base.RSkill();
        m_RSkill.SetUsingSkill(false);
        SetDisabled(false);
        if (!GetIsLookingForPosition())
            StopMovement();
        Destroy(m_RIndicator);
        StartCoroutine(RepeatRSaver());
        StartCoroutine(ResetTiggersSaver());
    }
    IEnumerator ResetTiggersSaver()
    {
        yield return new WaitForSeconds(5);
        ResetAnimatorTrigger("RJumpStop");
        ResetAnimatorTrigger("RJumpCancel");
    }
    IEnumerator RSkillCanStop()
    {
        yield return null;// Esperar 1 frame por seguridad
        m_CanStopR = true;
    }
    IEnumerator RepeatRSaver()
    {
        m_SaverCanDoR = false;
        yield return null; // Esperar 1 frame por seguridad
        m_SaverCanDoR = true;

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
