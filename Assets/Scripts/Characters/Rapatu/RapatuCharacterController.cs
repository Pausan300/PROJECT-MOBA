using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class RapatuCharacterController : CharacterMaster
{
    [Header("--- RAPATU ---")]



    [Header("PASSIVE SKILL")]

    [Header("Q SKILL")]
    public Transform m_TonguePositionFace;
    public GameObject m_TonguePrefab;

    public float m_TongueSpeedQ = 450;
    public float m_TongueRangeQ = 900;
    public float m_TongueHitboxWidthQ = 80;
    public float m_QChannelingTime = 0.15f;
    public float m_QReactivationTime = 1.5f;

    [Header("Q1")]
    public float m_Q1DamageZoneRange = 350f;
    public ImmobilizeBuff m_QImmobilizeBuff;
    public float m_Q1ImmobilizeBuffTime = 1.0f;
    [UnityEngine.Range(0f, 100f)]
    public float m_PercentageSkillPowerQ1 = 50f;

    [Header("Q2")]
    public float m_Q2ThrowingRange = 350;
    public StunBuff m_QStunBuff;
    public float m_Q2StunBuffTime = 0.5f;
    [UnityEngine.Range(0f, 100f)]
    public float m_PercentageSkillPowerQ2 = 50f;
    public LayerMask m_Q2CanColisioneMask;
    public float m_Q2AnimationTime = 1f;

    Vector3 m_Q2ThrowingTargetPos;
    bool m_Q2Throwing = false;

    float m_Q2ColisionsOffset = 0.2f;




    RapatuTongue m_TongueController;
    Transform m_TongueTarget;

    float m_QTimer = 0;
    bool m_QChanneling;
    bool m_TongueDetectEnemy = false;
    bool m_LookAtMouseQ = false;
    bool m_QSkillStarted = false;
    bool m_MoveTongueQ = false;
    bool m_SaverCanDoQ = true;
    bool m_MoveTongueReverseQ = false;
    bool m_CanReactiveQ = false;
    bool m_QReactived = false;

    float m_QMoveEnemysSpeed = 5f;
    List<GameObject> m_EnemysTrappedQ;
    bool m_QAbsorbing = false;


    Vector3 m_TongueTargetPos;
    Vector3 m_TongueInitPos;


    [Header("W SKILL")]
    public GameObject m_WIndicatorPrefab;
    public GameObject m_WExplosion;

    public float m_MAXRangeW = 1000;
    public float m_AutoJumpSecondsW = 2;
    public float m_CanDoOtherJumpTimeW = 0.4f;
    public float m_WChannelingTime = 0.15f;
    public SpeedBuff m_WSlowsDownBuff;

    [Tooltip("Duración del salto (idealmente con animación)")]
    public float m_JumpWAirDuration = 1f;
    [Tooltip("Tiempo en levantarse del salto (idealmente con animación)")]
    public float m_GetUpTimeW = 1f;

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


    bool m_WChanneling = false;
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
    [Tooltip("Cuantas veces cura en total")]
    public int m_HealingTimes = 4;
    [Tooltip("Cada cuantos segundos cura")]
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
    [Tooltip("Duración del salto (idealmente con animación)")]
    public float m_JumpRAirDuration = 1f;
    [Tooltip("Tiempo en levantarse del salto (idealmente con animación)")]
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

        if (m_QSkill.GetUsingSkill())
        {
            QUpdate();
        }

        if (m_RSkill.GetUsingSkill() && m_RSkillStarted)
        {
            RUpdate();
        }
        if (m_WSkill.GetUsingSkill() && m_WSkillStarted)
        {
            WUpdate();
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
                    StartCoroutine(StartWSkill());
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (m_QSkill.GetUsingSkill() && !m_QSkillStarted)
                StartCoroutine(StartQSkill());
        }

        QSkillThrowing();
    }

    #region Q Skill
    //Q SKILL
    protected override void QSkill()
    {
        if (m_QSkill.GetUsingSkill() || m_WSkill.GetUsingSkill() || m_ESkill.GetUsingSkill() || m_RSkill.GetUsingSkill())
            return;

        if (!m_SaverCanDoQ)
            return;


        m_QSkill.SetUsingSkill(true);


        m_QSkillStarted = false;
        if (GetShowingGizmos())
        {
            m_SkillIndicatorUI.ClearDeletableSkillIndicatorUI();
            m_SkillIndicatorUI.ClearNormalSkillIndicatorUI();
            m_SkillIndicatorUI.ClearTargetSkillIndicatorUI();
        }
        m_SkillIndicatorUI.CreateArrowSkillIndicator(m_QSkill.m_IndicatorUIObject, m_TongueHitboxWidthQ, m_TongueRangeQ, transform.position, true);
        SetShowingGizmos(true);

    }

    IEnumerator StartQSkill()
    {
        SetShowingGizmos(false);

        m_TongueDetectEnemy = false;
        m_QSkillStarted = true;


        GetCharacterUI().SetCastingUIAbilityText("Sacando la lengua");
        GetCharacterUI().HideCastingTime();
        GetCharacterUI().UpdateCastingUI(0, 1);
        GetCharacterUI().ShowCastingUI();
        m_QChanneling = true;

        m_QAbsorbing = false;
        m_MoveTongueQ = false;
        m_MoveTongueReverseQ = false;
        m_LookAtMouseQ = true;
        m_QTimer = 0;

        StopAttacking();
        SetAnimatorTrigger("IsUsingQ");
        SetDisabled(true);
        if (!GetIsLookingForPosition())
            StopMovement();


        yield return new WaitForSeconds(m_QChannelingTime);

        SetDisabled(false);
        m_QChanneling = false;
        GetCharacterUI().HideCastingUI();
        m_MoveTongueQ = true;
        m_LookAtMouseQ = false;

        Vector3 l_Direction = (GetPositionWithMouse() - transform.position).normalized;
        m_TongueController = Instantiate(m_TonguePrefab, m_TonguePositionFace.position, Quaternion.Euler(0f, l_Direction.y, 0)).GetComponent<RapatuTongue>();
        m_TongueController.SetTongue(this);

        m_TongueController.GetDamageZone().SetActive(false);

        m_TongueController.GetTongueLineRenderer().gameObject.transform.localPosition = Vector3.zero;
        m_TongueController.GetTongueLineRenderer().positionCount = 2;
        m_TongueController.GetTongueLineRenderer().SetPosition(0, Vector3.zero);
        m_TongueController.GetTongueEndPos().position = m_TonguePositionFace.position;
        m_TongueController.GetTongueLineRenderer().SetPosition(1, Vector3.zero);

        float l_TongueHitboxWidth = m_TongueHitboxWidthQ / 100;
        m_TongueController.GetTongueLineRenderer().startWidth = l_TongueHitboxWidth;
        m_TongueController.GetTongueEndPos().localScale = new Vector3(l_TongueHitboxWidth, l_TongueHitboxWidth, l_TongueHitboxWidth);

        m_TongueTargetPos = m_TongueController.GetTongueEndPos().position + (l_Direction * m_TongueRangeQ);
        m_TongueInitPos = m_TongueController.GetTongueEndPos().position;
    }

    private void QSkillThrowing()
    {
        if (m_Q2Throwing && m_EnemysTrappedQ.Count != 0)
        {
            Vector3 l_TargetPos = m_Q2ThrowingTargetPos;
            bool l_AllArrived = true;

            for (int i = m_EnemysTrappedQ.Count - 1; i >= 0; i--)
            {
                GameObject l_Enemy = m_EnemysTrappedQ[i];
                Vector3 l_MoveTarget = new Vector3(l_TargetPos.x, l_Enemy.transform.position.y, l_TargetPos.z);
                Vector3 l_Direction = (l_MoveTarget - l_Enemy.transform.position).normalized;
                float l_Distance = m_QMoveEnemysSpeed * Time.deltaTime;

                if (!l_Enemy.TryGetComponent(out Collider l_Collider))
                    continue;

                Vector3 l_Right = Vector3.Cross(Vector3.up, l_Direction).normalized;
                float l_HalfWidth = l_Collider.bounds.extents.x;

                Vector3[] rayOrigins = new Vector3[]
                {
                l_Enemy.transform.position,
                l_Enemy.transform.position + l_Right * l_HalfWidth,
                l_Enemy.transform.position - l_Right * l_HalfWidth
                };

                bool l_HasHit = false;

                foreach (var origin in rayOrigins)
                {
                    if (Physics.Raycast(origin, l_Direction, out RaycastHit l_Hit, l_Distance + m_Q2ColisionsOffset, m_Q2CanColisioneMask, QueryTriggerInteraction.Ignore))
                    {
                        Debug.DrawRay(origin, l_Direction * (l_Distance + m_Q2ColisionsOffset), Color.red, 0.1f);
                        Debug.Log($"{l_Enemy.name} ha colisionado con {l_Hit.collider.name} desde {origin}");

                        if (l_Enemy.TryGetComponent(out ITakeDamage Enemy))
                        {
                            if (l_Enemy.TryGetComponent(out BuffableEntity Buffs))
                            {
                                Buffs.AddBuff(m_QStunBuff.InitializeBuff(m_Q2StunBuffTime, l_Enemy));
                            }

                            float l_Damage = m_QSkill.GetAttribute("Daño base", GetQSkillLevel()) + (m_PercentageSkillPowerQ2 / 100f) * GetCharacterStats().GetAbilityPower();
                            Debug.Log($"{l_Enemy.name} recibe {l_Damage} de daño.");
                            Enemy.TakeDamage(0, l_Damage, m_CharacterStats.GetPlayerName());
                        }

                        if (l_Hit.collider.TryGetComponent(out ITakeDamage HitEnemy))
                        {
                            if (l_Hit.collider.TryGetComponent(out BuffableEntity Buffs))
                            {
                                Buffs.AddBuff(m_QStunBuff.InitializeBuff(m_Q2StunBuffTime, l_Hit.collider.gameObject));
                            }

                            float l_Damage = m_QSkill.GetAttribute("Daño base", GetQSkillLevel()) + (m_PercentageSkillPowerQ2 / 100f) * GetCharacterStats().GetAbilityPower();
                            Debug.Log($"{l_Hit.collider.gameObject.name} recibe {l_Damage} de daño.");
                            HitEnemy.TakeDamage(0, l_Damage, m_CharacterStats.GetPlayerName());
                        }

                        l_HasHit = true;
                        break;
                    }
                }

                if (!l_HasHit)
                {
                    if (Vector3.Distance(l_Enemy.transform.position, l_MoveTarget) > 0.1f)
                    {
                        l_Enemy.transform.position = Vector3.MoveTowards(l_Enemy.transform.position, l_MoveTarget, l_Distance);
                        l_AllArrived = false;
                    }
                }
            }

            if (l_AllArrived)
            {
                m_Q2Throwing = false;

                foreach (GameObject l_Enemy in m_EnemysTrappedQ)
                {
                    Collider[] l_Colliders = l_Enemy.GetComponentsInChildren<Collider>();
                    foreach (Collider col in l_Colliders)
                    {
                        col.enabled = true;
                    }
                }
            }
        }
    }




    private void QUpdate()
    {

        if (m_QChanneling)
        {
            m_QTimer += Time.deltaTime;
            GetCharacterUI().UpdateCastingUI(m_QTimer, m_QChannelingTime);
        }

        if (m_LookAtMouseQ)
            LookAt(GetPositionWithMouse());

        if (m_MoveTongueQ)
        {

            m_TongueController.GetTongueEndPos().position = Vector3.MoveTowards(m_TongueController.GetTongueEndPos().position, m_TongueTargetPos, Time.deltaTime * (m_TongueSpeedQ / 100));
            if (Vector3.Distance(m_TongueController.GetTongueEndPos().position, m_TongueInitPos) >= m_TongueRangeQ / 100)
            {
                m_MoveTongueQ = false;
                m_MoveTongueReverseQ = true;
            }
        }




        if (m_MoveTongueReverseQ)
        {
            m_MoveTongueReverseQ = false;
            SetAnimatorTrigger("QStop");
            EndQSkill();
        }


        if (m_TongueTarget != null)
        {
            m_TongueController.GetTongueEndPos().position = new Vector3(m_TongueTarget.position.x, m_TongueController.GetTongueEndPos().position.y, m_TongueTarget.position.z);
        }


        if (m_QAbsorbing && m_EnemysTrappedQ.Count != 0)
        {
            Vector3 l_TargetPos = m_TongueController.GetTongueEndPos().position;
            bool l_AllArrived = true;

            for (int i = m_EnemysTrappedQ.Count - 1; i >= 0; i--)
            {
                GameObject l_Enemy = m_EnemysTrappedQ[i];

                Vector3 l_MoveTarget = new Vector3(l_TargetPos.x, l_Enemy.transform.position.y, l_TargetPos.z);

                if (Vector3.Distance(l_Enemy.transform.position, l_MoveTarget) > 0.1f)
                {
                    l_Enemy.transform.position = Vector3.MoveTowards(l_Enemy.transform.position, l_MoveTarget, m_QMoveEnemysSpeed * Time.deltaTime);
                    l_AllArrived = false;
                }
            }

            if (l_AllArrived)
            {
                m_QAbsorbing = false;
                foreach (GameObject l_Enemy in m_EnemysTrappedQ)
                {
                    Collider[] l_Colliders = l_Enemy.GetComponentsInChildren<Collider>();
                    foreach (Collider col in l_Colliders)
                    {
                        col.enabled = true;
                    }
                }

            }
        }

        if (m_CanReactiveQ)
        {
            if (Input.GetMouseButtonDown(0))
            {
                StartCoroutine(QSkillReactivation());


            }
        }

        if (m_TongueController != null)
            LookAt(m_TongueController.GetTongueEndPos().position);


        if (m_TongueController != null)
        {
            m_TongueController.GetTongueLineRenderer().SetPosition(0, m_TongueController.GetTongueLineRenderer().transform.InverseTransformPoint(m_TonguePositionFace.position));
            m_TongueController.GetTongueLineRenderer().SetPosition(1, m_TongueController.GetTongueLineRenderer().transform.InverseTransformPoint(m_TongueController.GetTongueEndPos().position));
        }

    }

    IEnumerator QSkillReactivation()
    {

        SetAnimatorTrigger("QReactive");
        SetDisabled(true);

        m_CanReactiveQ = false;
        m_QReactived = true;

        float l_Distance = Vector3.Distance(m_TongueController.GetTongueEndPos().position, GetPositionWithMouse());

        if (l_Distance <= m_Q2ThrowingRange / 100)
        {
            m_Q2ThrowingTargetPos = GetPositionWithMouse();
        }
        else
        {
            m_Q2ThrowingTargetPos = m_TongueController.GetTongueEndPos().position + (GetPositionWithMouse() - m_TongueController.GetTongueEndPos().position).normalized * (m_Q2ThrowingRange / 100);
        }

        yield return new WaitForSeconds(m_Q2AnimationTime / 2);

        m_QAbsorbing = false;
        m_Q2Throwing = true;


        m_QReactived = false;
        if (m_TongueController != null)
            Destroy(m_TongueController.gameObject);

        m_TongueTarget = null;
        m_TongueController = null;
        yield return new WaitForSeconds(m_Q2AnimationTime / 2);
        SetDisabled(false);
        EndQSkill();
    }
    public void TongueDetectCollider(Collider _Collider)
    {
        if (m_TongueTarget != null)
            return;

        Debug.Log("Tongue Detect: " + _Collider.gameObject.name);


        if (_Collider.gameObject.GetComponent<CharacterMaster>())
        {
            return;
        }
        else if (_Collider.gameObject.GetComponent<CharacterStats>())
        {
            m_MoveTongueQ = false;
            m_TongueDetectEnemy = true;
            m_TongueTarget = _Collider.transform;
            StartCoroutine(DoQDamage());
        }
        else
        {
            m_MoveTongueQ = false;
            m_MoveTongueReverseQ = true;
        }

    }

    IEnumerator DoQDamage()
    {

        GameObject l_Explosion = m_TongueController.GetDamageZone();
        m_TongueController.GetDamageZone().SetActive(true);
        l_Explosion.transform.localScale = new Vector3(m_Q1DamageZoneRange / 100, m_Q1DamageZoneRange / 100, m_Q1DamageZoneRange / 100);
        List<Collider> l_CollidersHit = new List<Collider>();
        Collider[] l_HitColliders = Physics.OverlapSphere(l_Explosion.transform.position, l_Explosion.transform.localScale.x / 2.0f, m_DamageLayerMask);

        if (m_EnemysTrappedQ != null)
            m_EnemysTrappedQ.Clear();
        m_EnemysTrappedQ = new List<GameObject>();

        foreach (Collider Entity in l_HitColliders)
        {
            if (!l_CollidersHit.Contains(Entity) && Entity.TryGetComponent(out ITakeDamage Enemy))
            {
                if (Entity.TryGetComponent(out BuffableEntity Buffs))
                {
                    Buffs.AddBuff(m_QImmobilizeBuff.InitializeBuff(m_Q1ImmobilizeBuffTime, Entity.gameObject));
                }
                float l_Damage = (m_QSkill.GetAttribute("Daño base", GetQSkillLevel())) + (m_PercentageSkillPowerQ1 / 100) * GetCharacterStats().GetAbilityPower();
                Debug.Log("TAKEN " + l_Damage + " DAMAGE");
                Enemy.TakeDamage(0, l_Damage, m_CharacterStats.GetPlayerName());
                l_CollidersHit.Add(Entity);
                m_EnemysTrappedQ.Add(Entity.gameObject);
            }
        }
        m_QAbsorbing = true;
        foreach (GameObject l_Enemy in m_EnemysTrappedQ)
        {
            Collider[] l_Colliders = l_Enemy.GetComponentsInChildren<Collider>();
            foreach (Collider col in l_Colliders)
            {
                col.enabled = false;
            }
        }


        StartCoroutine(CanReactiveQ());
        yield return new WaitForSeconds(0.2f);//Change
        if (m_TongueController != null)
            m_TongueController.GetDamageZone().SetActive(false);
    }
    IEnumerator CanReactiveQ()
    {
        m_QReactived = false;
        m_QReactived = false;
        m_CanReactiveQ = true;
        yield return new WaitForSeconds(m_QReactivationTime);
        m_CanReactiveQ = false;
        if (!m_QReactived)
        {
            SetAnimatorTrigger("QStop");

            EndQSkill();
        }

    }
    private void EndQSkill()
    {
        if (m_TongueController != null)
            Destroy(m_TongueController.gameObject);

        m_TongueTarget = null;
        m_TongueController = null;


        base.QSkill();
        m_QSkill.SetUsingSkill(false);

        if (!GetIsLookingForPosition())
            StopMovement();

        StartCoroutine(RepeatQSaver());
        StartCoroutine(ResetTiggersSaverQ());
    }

    IEnumerator RepeatQSaver()
    {
        m_SaverCanDoQ = false;
        yield return null; // Esperar 1 frame por seguridad
        m_SaverCanDoQ = true;

    }
    IEnumerator ResetTiggersSaverQ()
    {
        yield return new WaitForSeconds(0.5f);
        ResetAnimatorTrigger("QStop");
    }

    #endregion
    #region W Skill
    //W SKILL
    protected override void WSkill()
    {

        if (m_QSkill.GetUsingSkill() || m_WSkill.GetUsingSkill() || m_ESkill.GetUsingSkill() || m_RSkill.GetUsingSkill())
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
            StartCoroutine(StartWSkill());


    }
    IEnumerator StartWSkill()
    {
        SetShowingGizmos(false);
        StopAttacking();
        SetDisabled(true);
        SetAnimatorTrigger("IsUsingW");
        if (!GetIsLookingForPosition())
            StopMovement();

        m_LoadingJumpW = false;
        m_WSkillStarted = true;
        m_WChanneling = true;
        m_TimerW = 0;
        GetCharacterUI().SetCastingUIAbilityText("Preparando");
        GetCharacterUI().HideCastingTime();
        GetCharacterUI().UpdateCastingUI(0, 1);
        GetCharacterUI().ShowCastingUI();

        m_WIndicator = Instantiate(m_WIndicatorPrefab, transform.position, Quaternion.identity);
        m_WIndicator.transform.localScale = new Vector3(m_DamageRangeWFirstJump / 100, m_WIndicator.transform.localScale.y, m_DamageRangeWFirstJump / 100);

        yield return new WaitForSeconds(m_WChannelingTime);

        m_WChanneling = false;
        m_TimerW = 0;
        GetCharacterUI().SetCastingUIAbilityText("Cargando salto");
        GetCharacterUI().HideCastingTime();
        GetCharacterUI().UpdateCastingUI(0, 1);
        GetCharacterUI().ShowCastingUI();
        m_LoadingJumpW = true;

    }
    void WUpdate()
    {
        if (m_WChanneling)
        {
            m_TimerW += Time.deltaTime;
            GetCharacterUI().UpdateCastingUI(m_TimerW, m_WChannelingTime);
        }

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
        if (m_WIndicator != null)
        {
            if (Vector3.Distance(transform.position, GetPositionWithMouse()) <= m_MAXRangeW / 100)
            {
                m_WIndicator.transform.position = GetPositionWithMouse();
            }
            else
            {
                Vector3 l_Direction = (GetPositionWithMouse() - transform.position).normalized;
                m_WIndicator.transform.position = transform.position + l_Direction * m_MAXRangeW / 100;
            }
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

                    Buffs.AddBuff(m_WSlowsDownBuff.InitializeBuff(TimeSlowsDown, -PercentageSlowsDown, Entity.gameObject));
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

        if (m_QSkill.GetUsingSkill() || m_WSkill.GetUsingSkill() || m_ESkill.GetUsingSkill() || m_RSkill.GetUsingSkill())
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

        if (m_QSkill.GetUsingSkill() || m_WSkill.GetUsingSkill() || m_ESkill.GetUsingSkill() || m_RSkill.GetUsingSkill())
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
    void RUpdate()
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
