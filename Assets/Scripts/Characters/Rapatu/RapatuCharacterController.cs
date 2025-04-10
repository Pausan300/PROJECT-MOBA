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
    public GameObject m_WIndicatorPrefab;
    public GameObject m_WExplosion;

    public float m_MAXRangeW = 1000;
    public float m_AutoJumpSecondsW = 2;
    public float m_JumpWAirDuration = 1f;
    public float m_GetUpTimeW = 1f;
    public float m_CanDoOtherJumpTimeW = 0.4f;

    [Header("FIRST JUMP W")]
    [UnityEngine.Range(0f, 100f)]
    public float m_PercentageSkillPowerWFirstJump = 50;
    [UnityEngine.Range(0f, 100f)]
    public float m_PercentageAdditionalLifeWFirstJump = 5;
    [UnityEngine.Range(0f, 100f)]
    public float m_PercentageSlowsDownWFirstJump = 40;
    public float m_TimeSlowsDownWFirstJump = 2;
    public float m_DamageRangeWFirstJump = 250;

    [Header("SECOND JUMP W")]
    [UnityEngine.Range(0f, 100f)]
    public float m_PercentageSkillPowerWSecondJump = 30;
    [UnityEngine.Range(0f, 100f)]
    public float m_PercentageAdditionalLifeWSecondJump = 3;
    [UnityEngine.Range(0f, 100f)]
    public float m_PercentageSlowsDownWSecondJump = 30;
    public float m_TimeSlowsDownWSecondJump = 2;
    public float m_DamageRangeWSecondJump = 250;

    [UnityEngine.Range(0f, 100f)]
    public float m_PercentageExtraCountdownW = 30f;



    float m_TimerW;
    bool m_WSkillStarted = false;
    bool m_LoadingJumpW = false;
    GameObject m_WIndicator;
    bool m_JumpingW = false;
    Vector3 m_JumpWStartPosition;
    Vector3 m_JumpWEndPosition;
    float m_WJumpSpeed;
    bool m_SaverCanDoW = true;
    bool m_OtherJumpW = true;
    bool m_CanDoOtherJumpW = true;

    [Header("E SKILL")]

    public GameObject m_HealingAreaEPrefab;
    [Tooltip("Cuantas veces cura")]
    public int m_HealingTimes = 4;
    [Tooltip("Cada cuanto cura")]
    public float m_HealingTimeSpace = 1f;
    public float m_HealingAreaRadius = 500;
    [UnityEngine.Range(0f, 100f)]
    public float m_PercentageSkillPowerE = 50;
    [UnityEngine.Range(0f, 100f)]
    public float m_PercentageAdditionalLifeE = 7;
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
    public float m_GetUpTimeR = 1f;
    GameObject m_RIndicator;
    float m_TimerR;
    bool m_CanStopR = false;
    bool m_LoadingJumpR = false;
    bool m_JumpingR = false;
    bool m_SaverCanDoR = true;
    bool m_RSkillStarted = false;
    Vector3 m_JumpRStartPosition;
    Vector3 m_JumpREndPosition;

    Coroutine m_RSkillCoroutine;
    float m_RJumpSpeed;
    bool m_RStopJumping = false;

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
        if (m_WSkill.GetUsingSkill() && m_WSkillStarted)
        {
            WHoldTimeCheck();
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

                if (m_WSkill.GetUsingSkill() && !m_WSkillStarted)
                    StartWSkill();
            }
        }
    }

    #region Q Skill
    //Q SKILL
    protected override void QSkill()
    {
        if (m_ESkill.GetUsingSkill())
            return;
        if (m_RSkill.GetUsingSkill())
            return;
    }
    #endregion
    #region W Skill
    //W SKILL
    protected override void WSkill()
    {
        if (m_RSkill.GetUsingSkill())
            return;
        if (m_ESkill.GetUsingSkill())
            return;
        if (!m_SaverCanDoW)
            return;

        m_WSkill.SetUsingSkill(true);

        if (GetUseSkillGizmos())
        {
            m_WSkillStarted = false;
            if (GetShowingGizmos())
            {
                m_SkillIndicatorUI.ClearDeletableSkillIndicatorUI();
                m_SkillIndicatorUI.ClearNormalSkillIndicatorUI();
                m_SkillIndicatorUI.ClearTargetSkillIndicatorUI();
            }
            m_SkillIndicatorUI.CreateCircleSkillIndicator(m_WSkill.m_IndicatorUIObject, m_MAXRangeW, transform, false);
            SetShowingGizmos(true);
        }
        else
            StartWSkill();


    }

    void StartWSkill()
    {
        SetShowingGizmos(false);
        StopAttacking();
        SetDisabled(true);
        SetAnimatorTrigger("IsUsingW");
        if (!GetIsLookingForPosition())
            StopMovement();
        m_TimerW = 0;
        GetCharacterUI().SetCastingUIAbilityText("Cargando salto");
        GetCharacterUI().HideCastingTime();
        GetCharacterUI().UpdateCastingUI(0, 1);
        GetCharacterUI().ShowCastingUI();
        m_LoadingJumpW = true;
        m_WSkillStarted = true;

        m_WIndicator = Instantiate(m_WIndicatorPrefab, transform.position, Quaternion.identity);
        m_WIndicator.transform.localScale = new Vector3(m_DamageRangeWFirstJump / 100, m_WIndicator.transform.localScale.y, m_DamageRangeWFirstJump / 100);
    }
    void WHoldTimeCheck()
    {

        if (m_LoadingJumpW)
        {
            m_TimerW += Time.deltaTime;
            if (m_TimerW >= m_AutoJumpSecondsW)
            {
                Debug.Log("Saltando por tiempo");
                StartJumpingW();
            }
            GetCharacterUI().UpdateCastingUI(m_TimerW, m_AutoJumpSecondsW);


            if (GetUseSkillGizmos())
            {
                if (Input.GetMouseButtonUp(0))
                {
                    Debug.Log("Saltando por activacion");
                    StartJumpingW();
                }
            }
            else
            {
                if (Input.GetKeyUp(m_WSkillKey) || !Input.GetKey(m_WSkillKey))
                {
                    Debug.Log("Saltando por activacion");
                    StartJumpingW();
                }
            }

        }

        if (Vector3.Distance(transform.position, GetPositionWithMouse()) <= m_MAXRangeW / 100)
        {
            m_WIndicator.transform.position = GetPositionWithMouse();
        }
        else
        {
            Vector3 l_Direction = (GetPositionWithMouse() - transform.position).normalized;
            m_WIndicator.transform.position = transform.position + l_Direction * m_MAXRangeW / 100;
        }

        if (m_JumpingW)
        {
            transform.position = Vector3.MoveTowards(transform.position, m_JumpWEndPosition, m_WJumpSpeed * Time.deltaTime);

            if (transform.position == m_JumpWEndPosition)
            {
                m_JumpingW = false;
            }
        }
        else
        {
            LookAt(m_WIndicator.transform.position);

        }

        if (m_CanDoOtherJumpW)
        {
            if (Input.GetMouseButtonDown(0))
                m_OtherJumpW = true;
        }



    }
    void StartJumpingW()
    {
        GetCharacterUI().HideCastingUI();
        m_LoadingJumpW = false;
        SetAnimatorTrigger("WJump");
        StartCoroutine(JumpingW());

    }
    public void StartJumpWAnim()
    {
        if (m_WIndicator == null)
            return;

        if (!m_WSkill.GetUsingSkill())
            return;

        m_JumpWEndPosition = m_WIndicator.transform.position;
        m_JumpWStartPosition = transform.position;
        float l_Distance = Vector3.Distance(m_JumpWStartPosition, m_JumpWEndPosition);
        m_WJumpSpeed = l_Distance / m_JumpWAirDuration;
        m_JumpingW = true;
    }
    void DoLandsDamageWFirstJump()
    {
        float l_Damage = (m_WSkill.GetAttribute("Daño base", GetWSkillLevel())) + (m_PercentageSkillPowerWFirstJump / 100) * GetCharacterStats().GetAbilityPower() + (m_PercentageAdditionalLifeWFirstJump / 100) * GetCharacterStats().GetBonusHealth();
        StartCoroutine(DoLandsDamageW(l_Damage, m_PercentageSlowsDownWFirstJump, m_TimeSlowsDownWFirstJump, m_DamageRangeWFirstJump));
    }
    void DoLandsDamageWSecondJump()
    {
        float l_Damage = (m_WSkill.GetAttribute("Daño base", GetWSkillLevel())) + (m_PercentageSkillPowerWSecondJump / 100) * GetCharacterStats().GetAbilityPower() + (m_PercentageAdditionalLifeWSecondJump / 100) * GetCharacterStats().GetBonusHealth();
        StartCoroutine(DoLandsDamageW(l_Damage, m_PercentageSlowsDownWSecondJump, m_TimeSlowsDownWSecondJump, m_DamageRangeWSecondJump));
    }
    IEnumerator DoLandsDamageW(float Damage, float PercentageSlowsDown, float TimeSlowsDown, float DamageRange)
    {

        GameObject l_Explosion = Instantiate(m_WExplosion, transform.position, Quaternion.identity);
        l_Explosion.transform.localScale = new Vector3(DamageRange / 100, DamageRange / 100, DamageRange / 100);
        List<Collider> l_CollidersHit = new List<Collider>();
        Collider[] l_HitColliders = Physics.OverlapSphere(l_Explosion.transform.position, l_Explosion.transform.localScale.x / 2.0f, m_DamageLayerMask);
        foreach (Collider Entity in l_HitColliders)
        {
            if (!l_CollidersHit.Contains(Entity) && Entity.TryGetComponent(out ITakeDamage Enemy))
            {
                if (Entity.TryGetComponent(out BuffableEntity Buffs))
                {
                    Debug.LogError("You need to add buff T_T");
                }
                Debug.Log("TAKEN " + Damage + " DAMAGE");
                Enemy.TakeDamage(0, Damage, m_CharacterStats.GetPlayerName());
                l_CollidersHit.Add(Entity);
            }
        }

        yield return new WaitForSeconds(0.5f);

        Destroy(l_Explosion);
    }

    IEnumerator JumpingW()
    {


        while (!m_JumpingW)
        {
            yield return null;
        }

        m_WIndicator.SetActive(false);

        while (m_JumpingW)
            yield return null;
        DoLandsDamageWFirstJump();

        m_WIndicator.transform.localScale = new Vector3(m_DamageRangeWSecondJump / 100, m_WIndicator.transform.localScale.y, m_DamageRangeWSecondJump / 100);
        m_WIndicator.SetActive(true);

        m_OtherJumpW = false;
        m_CanDoOtherJumpW = true;

        yield return new WaitForSeconds(m_CanDoOtherJumpTimeW);


        if (m_OtherJumpW)
        {
            while (!m_JumpingW)
            {
                yield return null;
            }

            m_WIndicator.SetActive(false);

            while (m_JumpingW)
                yield return null;

            DoLandsDamageWSecondJump();

            SetAnimatorTrigger("WJumpStop");

            yield return new WaitForSeconds(m_GetUpTimeW);

            EndWSkill(true);
        }
        else
        {
            SetAnimatorTrigger("WJumpStop");
            m_WIndicator.SetActive(false);

            yield return new WaitForSeconds(m_GetUpTimeW - m_CanDoOtherJumpTimeW);

            EndWSkill(false);
        }

    }
    void EndWSkill(bool ExtraJump)
    {
        base.WSkill();
        if (ExtraJump)
            m_WSkill.SetTimer(m_WSkill.GetCd() + m_WSkill.GetCd() * (m_PercentageExtraCountdownW / 100));
        m_WSkill.SetUsingSkill(false);
        SetDisabled(false);
        if (!GetIsLookingForPosition())
            StopMovement();
        Destroy(m_WIndicator);
        StartCoroutine(RepeatWSaver());
    }
    IEnumerator RepeatWSaver()
    {
        m_SaverCanDoW = false;
        yield return null; // Esperar 1 frame por seguridad
        m_SaverCanDoW = true;

    }
    #endregion
    #region E Skill

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


        float l_HealLifeValue = (m_ESkill.GetAttribute("Curación", GetESkillLevel())) + (m_PercentageSkillPowerE / 100) * GetCharacterStats().GetAbilityPower() + (m_PercentageAdditionalLifeE / 100) * GetCharacterStats().GetBonusHealth();

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
    #endregion
    #region R Skill
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
        m_LoadingJumpR = true;
        m_RSkillStarted = true;
        StartCoroutine(RSkillCanStop());

        m_RIndicator = Instantiate(m_RIndicatorPrefab, transform.position, Quaternion.identity);
    }
    void RHoldTimeCheck()
    {

        if (m_LoadingJumpR)
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
                m_LoadingJumpR = false;
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

        if (m_JumpingR)
        {
            transform.position = Vector3.MoveTowards(transform.position, m_JumpREndPosition, m_RJumpSpeed * Time.deltaTime);

            if (transform.position == m_JumpREndPosition)
            {
                m_JumpingR = false;
            }
        }
        else
        {
            LookAt(m_RIndicator.transform.position);

        }
    }

    void StartJumpingR()
    {
        GetCharacterUI().HideCastingUI();
        m_LoadingJumpR = false;
        m_RStopJumping = false;
        SetAnimatorTrigger("RJump");
        m_RSkillCoroutine = StartCoroutine(JumpingR());

    }

    public void StartJumpRAnim()
    {
        if (m_RIndicator == null)
            return;

        if (!m_RSkill.GetUsingSkill())
            return;

        m_JumpREndPosition = m_RIndicator.transform.position;
        m_JumpRStartPosition = transform.position;
        float l_Distance = Vector3.Distance(m_JumpRStartPosition, m_JumpREndPosition);
        m_RJumpSpeed = l_Distance / m_JumpRAirDuration;
        m_JumpingR = true;
    }


    IEnumerator JumpingR()
    {
        //int l_JumpsNum = 1;
        int l_JumpsNum = (int)m_RSkill.GetAttribute("Saltos", GetRSkillLevel());
        Debug.Log("Va a hacer " + m_RSkill.GetAttribute("Saltos", GetRSkillLevel()) + " saltos");
        bool l_StopJumpAnim = false;

        for (int i = 0; i < l_JumpsNum; i++)
        {
            while (!m_JumpingR)
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

            while (m_JumpingR)
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

        yield return new WaitForSeconds(m_GetUpTimeR);

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
        StartCoroutine(ResetTiggersSaverR());
    }
    IEnumerator ResetTiggersSaverR()
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
    #endregion

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
