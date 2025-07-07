using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CharacterStats;
using Random = UnityEngine.Random;

public class WilldurrCharacterController : CharacterMaster
{
    [Header("--- WILLDURR ---")]

    //https://wiki.leagueoflegends.com/en-us/Vamp

    [Header("PASSIVE SKILL")]
    public SoulTheftBuff m_PSoulTheftBuff;
    public PassiveWilldurrBuff m_PWilldurrBuff;
    public float m_PTimeToTheftSoul = 1.5f;
    int m_PSouls;

    float m_PAttackDamageAdded = 0;
    public SoulsWithEnemyType[] m_SoulsWithEnemysTypes;

    [Tooltip("Fase 1 cuando llegas a 25 almas.")]
    public float m_AttackDamageToAddF1 = 10;
    [Tooltip("Fase 2 cuando llegas a 50 almas.")]
    public float m_OmnisuctionToAddF2 = 5;
    [Tooltip("Fase 3 cuando llegas a 75 almas.")]
    public float m_AttackDamageToAddF3 = 30;
    [Tooltip("Fase 4 cuando llegas a 100 almas.")]
    public float m_OmnisuctionToAddF4 = 15;
    [Tooltip("Fase 5 A partir de 100 almas, por cada 5 almas nuevas.")]
    public float m_AttackDamageToAddF5 = 2;
    [Tooltip("Fase 5 A partir de 100 almas, por cada 5 almas nuevas.")]
    public float m_OmnisuctionToAddF5 = 1;

    [Header("Q SKILL")]
    public GameObject m_QHiboxGameObject;
    public float m_JumpRangeQ = 150f;
    public float m_JumpTimeQ = 0.75f;

    public float m_QPercentageBonusAttackDamageFirst = 85;
    public float m_QPercentageBonusAttackDamageSecond = 95;

    [Header("Q1")]
    public float m_InitialRadiusQ1 = 100;
    public float m_FinalRadiusQ1 = 250;
    float m_AngleQ1ToDoDamage = 160;

    [Header("Q2")]
    public float m_InitialRadiusQ2 = 100;
    public float m_FinalRadiusQ2 = 350;



    float m_JumpSpeedQ;
    float m_ActualHitboxRadius = 0;
    float l_QDistanceTraveled = 0f;
    bool m_SaverCanDoQ = true;
    bool m_QSkillStarted;
    Vector3 m_InitialPositionQ;
    bool m_JumpingQ = false;
    List<Collider> m_CollidersHitQZone = new List<Collider>();

    [Header("W SKILL")]
    public GameObject m_WilldurrWScythePrefab;
    GameObject m_WilldurrWScythe;
    public GameObject m_WExplosion;
    public float m_WRange = 500f;
    public float m_WHitboxRatio = 250f;
    public float m_WHitboxRatioReactived = 300f;
    public StunBuff m_WStunBuff;
    public float m_WStunBuffTime = 0.75f;
    [Tooltip("Los enemigos golpeados en la trayectoria de Reactivación, son atraídos hasta quedar a m_WArribalRangeReactivation unidades de Willdurr.")]
    public float m_WArribalRangeReactivation = 150f;


    public float m_WChannelingTime = 0.15f;
    public float m_WaitReactivationTimeW = 0.75f;
    public float m_CanReactiveTimeW = 2.5f;
    [Tooltip("Tiempo para calcular la velocidad de la W en reactivación.")]
    public float m_WReactivedTimeToReturn = 0.7f;


    [UnityEngine.Range(0f, 100f)]
    public float m_WPercentageBonusAttackDamageFirst = 55;
    [UnityEngine.Range(0f, 100f)]
    public float m_WPercentageBonusAttackDamageSecond = 55;

    bool m_WReactived = false;
    float m_WReactivedSpeed = 0;
    bool m_SaverCanDoW = true;
    bool m_WSkillStarted;
    bool m_CanReactiveW = false;
    bool m_WChanneling = false;
    float m_WTimer = 0;
    Coroutine m_WCoroutine;
    Vector3 m_TargetPositionW;
    List<Collider> m_CollidersHitWZone = new List<Collider>();

    [Header("E SKILL")]
    public GameObject m_FearZoneE;
    public Transform m_Head;
    public float m_UIIndicatorWidthE = 50;
    public FearBuff m_FearBuffE;
    public float m_FearBuffTimeE = 1.25f;
    public float m_RangeE = 700;

    [Tooltip("Tiempo para calcular la velocidad de la Cabeza en llegar a su destino.")]
    public float m_HeadArriveTargetTimeE = 0.4f;
    float m_SpeedHeatE = 600;
    public float m_FearRadioE = 250;


    public float m_EChannelingTime = 0.15f;
    public float m_WaitReactivationTimeE = 0.75f;
    public float m_CanReactiveTimeE = 2.5f;



    Vector3 m_TargetHeadPosition;
    bool m_EChanneling = false;
    bool m_HeadCanMove = false;
    float m_ETimer = 0f;
    bool m_CanReactiveE = false;
    bool m_SaverCanDoE = true;
    bool m_ESkillStarted;
    Coroutine m_ECoroutine;

    [Header("R SKILL")]
    public SpeedBuff m_SlowDownRBuff;
    public GameObject m_RZonePrefab;
    public GameObject m_RHeadPrefab;
    public LayerMask m_RHeadLayerMask;
    public float m_MINRangeR = 800.0f;
    public float m_ScaleRZoneSpeed = 1000.0f;
    public float m_RSkillDuration = 15f;
    public float m_RChannelingTime = 0.2f;
    [UnityEngine.Range(50f, 450f)]
    [Tooltip("Distancia minima entre cabezas.")]
    public float m_RMINDistanceHeads = 105f;
    [Tooltip("Hitbox para hacer la reactivacion de la E.")]
    public float m_HeadRRadiusHitbox = 100f;
    [Tooltip("Tiempo enfriamiento para hacer la reactivacion de la E.")]
    public float m_CoolingEInRSkill = 1.5f;


    GameObject m_RZone;
    List<Vector3> m_PosToSpawnRHeads;
    List<GameObject> m_RHeads;

    List<Collider> m_CollidersHitRZone = new List<Collider>();
    float m_ActualRangeR;
    float m_MAXRangeThisR;
    float m_RTimer = 0f;

    bool m_RChanneling = false;
    bool m_ScalingRZone = false;
    bool m_RSkillStarted = false;
    bool m_SaverCanDoR = true;

    [Header("OTHER VALUES")]
    public Transform m_HeadPos;
    float m_HeatReturnSpeed = 600;
    bool m_ReturnHead = false;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        m_FearZoneE.SetActive(false);
        m_PSouls = 0;
        GetComponent<BuffableEntity>().AddBuff(m_PWilldurrBuff.InitializeBuff(m_PSouls, gameObject));
        m_QHiboxGameObject.SetActive(false);
    }
    protected override void Update()
    {
        if (!IsSpawned || !HasAuthority)
        {
            return;
        }

        base.Update();

        if (GetUseSkillGizmos())
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (m_ESkill.GetUsingSkill() && !m_ESkillStarted)
                    m_ECoroutine = StartCoroutine(StartESkill());

                if (m_QSkill.GetUsingSkill() && !m_QSkillStarted)
                    StartCoroutine(StartQSkill());

                if (m_WSkill.GetUsingSkill() && !m_WSkillStarted)
                    m_WCoroutine = StartCoroutine(StartWSkill());

                if (m_RSkill.GetUsingSkill() && !m_RSkillStarted)
                    StartCoroutine(StartRSkill());


            }
            if (Input.GetMouseButtonDown(1))
            {
                if (m_ESkill.GetUsingSkill() && !m_ESkillStarted)
                {
                    m_ESkill.SetUsingSkill(false);
                    if (GetShowingGizmos())
                    {
                        m_SkillIndicatorUI.ClearDeletableSkillIndicatorUI();
                        m_SkillIndicatorUI.ClearNormalSkillIndicatorUI();
                        m_SkillIndicatorUI.ClearTargetSkillIndicatorUI();
                        SetShowingGizmos(false);
                    }
                }

                if (m_RSkill.GetUsingSkill() && !m_RSkillStarted)
                {
                    m_RSkill.SetUsingSkill(false);
                    if (GetShowingGizmos())
                    {
                        m_SkillIndicatorUI.ClearDeletableSkillIndicatorUI();
                        m_SkillIndicatorUI.ClearNormalSkillIndicatorUI();
                        m_SkillIndicatorUI.ClearTargetSkillIndicatorUI();
                        SetShowingGizmos(false);
                    }
                }

                if (m_WSkill.GetUsingSkill() && !m_WSkillStarted)
                {
                    m_WSkill.SetUsingSkill(false);
                    if (GetShowingGizmos())
                    {
                        m_SkillIndicatorUI.ClearDeletableSkillIndicatorUI();
                        m_SkillIndicatorUI.ClearNormalSkillIndicatorUI();
                        m_SkillIndicatorUI.ClearTargetSkillIndicatorUI();
                        SetShowingGizmos(false);
                    }
                }

                if (m_QSkill.GetUsingSkill() && !m_QSkillStarted)
                {
                    m_QSkill.SetUsingSkill(false);
                    if (GetShowingGizmos())
                    {
                        m_SkillIndicatorUI.ClearDeletableSkillIndicatorUI();
                        m_SkillIndicatorUI.ClearNormalSkillIndicatorUI();
                        m_SkillIndicatorUI.ClearTargetSkillIndicatorUI();
                        SetShowingGizmos(false);
                    }
                }
            }
        }

        UpdateQSkill();
        UpdateWSkill();
        UpdateESkill();
        UpdateRSkill();

        if (m_ReturnHead)
        {
            m_Head.position = m_HeadPos.position;
            if (Vector3.Distance(m_Head.position, m_HeadPos.position) <= 0.01f)
            {
                m_ReturnHead = false;
                m_Head.SetParent(m_HeadPos);
                m_Head.localRotation = Quaternion.identity;
                m_Head.localPosition = Vector3.zero;
            }
        }
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F1))
            AddSoul(EnemyType.MINION);
#endif
    }

    #region Q Skill
    //Q SKILL
    protected override void QSkill()
    {
        if (m_QSkill.GetUsingSkill() || (m_WSkill.GetUsingSkill() && m_WSkillStarted && m_WChanneling))
        {
            if (!GetUseSkillGizmos())
                return;

            if (m_ESkill.GetUsingSkill() && !m_ESkillStarted)
            {
                m_ESkill.SetUsingSkill(false);
            }
            else if (m_RSkill.GetUsingSkill() && !m_RSkillStarted)
            {
                m_RSkill.SetUsingSkill(false);
            }
            else if (m_WSkill.GetUsingSkill() && !m_WSkillStarted)
            {
                m_WSkill.SetUsingSkill(false);
            }
            else
            {
                return;
            }
        }

        if (!m_SaverCanDoQ)
            return;

        m_QSkill.SetUsingSkill(true);

        if (GetUseSkillGizmos())
        {

            m_QSkillStarted = false;
            if (GetShowingGizmos())
            {
                m_SkillIndicatorUI.ClearDeletableSkillIndicatorUI();
                m_SkillIndicatorUI.ClearNormalSkillIndicatorUI();
                m_SkillIndicatorUI.ClearTargetSkillIndicatorUI();
            }
            m_SkillIndicatorUI.CreateArrowSkillIndicator(m_QSkill.m_IndicatorUIObject, 100, m_JumpRangeQ, transform.position, true);
            SetShowingGizmos(true);
        }
        else
            StartCoroutine(StartQSkill());


    }
    IEnumerator StartQSkill()
    {
        m_JumpSpeedQ = m_JumpRangeQ / m_JumpTimeQ;
        m_CollidersHitQZone = new List<Collider>();
        LookAt(((GetPositionWithMouse() - transform.position).normalized * (m_JumpRangeQ / 100)) + transform.position);
        m_InitialPositionQ = transform.position;
        m_JumpingQ = true;


        StopAttacking();
        if (!GetIsLookingForPosition())
            StopMovement();

        if (m_QSkill.GetThisIsSpecialLoad())
            m_ActualHitboxRadius = m_InitialRadiusQ2;
        else
            m_ActualHitboxRadius = m_InitialRadiusQ1;
        SetDisabled(true);

        if (m_QSkill.GetThisIsSpecialLoad())
            SetAnimatorTrigger("IsUsingQ2");
        else
            SetAnimatorTrigger("IsUsingQ1");
        l_QDistanceTraveled = 0f;
        m_QHiboxGameObject.SetActive(true);
        m_QSkillStarted = true;
        yield return null;
    }
    void UpdateQSkill()
    {
        if (!m_QSkill.GetUsingSkill() && !m_QSkillStarted)
            return;

        float l_DistancePercent = 1;

        if (m_JumpingQ)
        {
            float l_Desplazamiento = (m_JumpSpeedQ / 100f) * Time.deltaTime;
            transform.Translate(Vector3.forward * l_Desplazamiento);
            l_QDistanceTraveled += l_Desplazamiento;

            float l_DistanciaMaxima = m_JumpRangeQ / 100f;
            l_DistancePercent = l_QDistanceTraveled / l_DistanciaMaxima;

            if (l_QDistanceTraveled >= l_DistanciaMaxima)
            {
                m_JumpingQ = false;
                m_QHiboxGameObject.SetActive(false);
                EndQSkill();
            }
        }


        if (m_QSkill.GetThisIsSpecialLoad())
            m_ActualHitboxRadius = m_InitialRadiusQ2 + ((m_FinalRadiusQ2 - m_InitialRadiusQ2) * l_DistancePercent);
        else
            m_ActualHitboxRadius = m_InitialRadiusQ1 + ((m_FinalRadiusQ1 - m_InitialRadiusQ1) * l_DistancePercent);

        m_QHiboxGameObject.transform.localScale = new Vector3(m_ActualHitboxRadius / 100, m_ActualHitboxRadius / 100, m_ActualHitboxRadius / 100);
    }
    public void OnTriggerEnterQ(Collider Entity)
    {
        if (Entity.GetComponent<CharacterMaster>())
            return;

        if (m_QSkill.GetThisIsSpecialLoad())
        {
            if (!m_CollidersHitQZone.Contains(Entity) && Entity.TryGetComponent(out ITakeDamage Enemy))
            {
                float l_Damage = (m_QSkill.GetAttribute("Daño 3r ataque", GetQSkillLevel())) + (m_QPercentageBonusAttackDamageSecond / 100) * GetCharacterStats().GetBonusAttackDamage();
                Debug.Log("TAKEN " + l_Damage + " DAMAGE");
                Enemy.TakeDamage(l_Damage, 0, false, m_CharacterStats.GetPlayerName());
                m_CollidersHitQZone.Add(Entity);
            }
        }
        else
        {
            Vector3 l_EntityDirection = (Entity.transform.position - transform.position).normalized;
            float l_Angle = Vector3.Angle(transform.forward, l_EntityDirection);

            if (Vector3.Angle(transform.forward, l_EntityDirection) < m_AngleQ1ToDoDamage / 2f)
            {
                Debug.Log("Entidad delante: " + Entity.name);
                if (!m_CollidersHitQZone.Contains(Entity) && Entity.TryGetComponent(out ITakeDamage Enemy))
                {
                    float l_Damage = (m_QSkill.GetAttribute("Daño base", GetQSkillLevel())) + (m_QPercentageBonusAttackDamageFirst / 100) * GetCharacterStats().GetBonusAttackDamage();
                    Debug.Log("TAKEN " + l_Damage + " DAMAGE");
                    Enemy.TakeDamage(l_Damage, 0, false, m_CharacterStats.GetPlayerName());
                    m_CollidersHitQZone.Add(Entity);
                }

            }
        }
    }
    void EndQSkill()
    {
        if (m_CollidersHitQZone != null)
            m_CollidersHitQZone.Clear();
        m_CollidersHitQZone = null;

        SetDisabled(false);
        m_QSkillStarted = false;
        m_QSkill.SetUsingSkill(false);
        base.QSkill();
        StartCoroutine(RepeatQSaver());
        StartCoroutine(RepeatESaver());
    }
    IEnumerator RepeatQSaver()
    {
        m_SaverCanDoQ = false;
        yield return null; // Esperar 1 frame por seguridad
        m_SaverCanDoQ = true;

    }

    #endregion
    #region W Skill
    //W SKILL
    protected override void WSkill()
    {
        if (m_QSkill.GetUsingSkill() || m_WSkill.GetUsingSkill())
        {
            if (!GetUseSkillGizmos())
                return;

            if (m_ESkill.GetUsingSkill() && !m_ESkillStarted)
            {
                m_ESkill.SetUsingSkill(false);
            }
            else if (m_RSkill.GetUsingSkill() && !m_RSkillStarted)
            {
                m_RSkill.SetUsingSkill(false);
            }
            else if (m_QSkill.GetUsingSkill() && !m_QSkillStarted)
            {
                m_QSkill.SetUsingSkill(false);
            }
            else
            {
                return;
            }
        }

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
            m_SkillIndicatorUI.CreateArrowSkillIndicator(m_WSkill.m_IndicatorUIObject, m_WHitboxRatio, m_WRange, transform.position, true);
            SetShowingGizmos(true);
        }
        else
            m_WCoroutine = StartCoroutine(StartWSkill());
    }
    IEnumerator StartWSkill()
    {
        m_WSkillStarted = true;

        if (Vector3.Distance(GetPositionWithMouse(), transform.position) <= (m_WRange / 100))
            m_TargetPositionW = GetPositionWithMouse();
        else
            m_TargetPositionW = ((GetPositionWithMouse() - transform.position).normalized * (m_WRange / 100)) + transform.position;

        m_TargetPositionW.y = transform.position.y;

        LookAt(m_TargetPositionW);

        StopAttacking();
        if (!GetIsLookingForPosition())
            StopMovement();

        GetCharacterUI().SetCastingUIAbilityText("Canalizando");
        GetCharacterUI().HideCastingTime();
        GetCharacterUI().UpdateCastingUI(0, 1);
        GetCharacterUI().ShowCastingUI();
        m_WTimer = 0f;
        m_WChanneling = true;
        m_CanReactiveW = false;
        m_WReactived = false;
        m_CollidersHitWZone = new List<Collider>();




        SetAnimatorTrigger("IsUsingW");

        SetDisabled(true);
        yield return new WaitForSeconds(m_WChannelingTime);
        SetDisabled(false);
        base.WSkill();
        GetCharacterUI().HideCastingUI();
        m_WChanneling = false;

        m_WilldurrWScythe = Instantiate(m_WilldurrWScythePrefab, m_TargetPositionW, transform.rotation);
        m_WilldurrWScythe.GetComponent<WilldurrWScythe>().m_CharacterController = this;
        GameObject l_Explosion = Instantiate(m_WExplosion, m_TargetPositionW, Quaternion.identity);
        l_Explosion.transform.localScale = new Vector3((m_WHitboxRatio / 100) * 2, (m_WHitboxRatio / 100) * 2, (m_WHitboxRatio / 100) * 2);

        List<Collider> l_CollidersHit = new List<Collider>();
        Collider[] l_HitColliders = Physics.OverlapSphere(m_TargetPositionW, m_WHitboxRatio / 100, m_DamageLayerMask);
        float l_Damage = (m_WSkill.GetAttribute("Daño base", GetWSkillLevel())) + (m_WPercentageBonusAttackDamageFirst / 100) * GetCharacterStats().GetBonusAttackDamage();
        foreach (Collider Entity in l_HitColliders)
        {
            if (!l_CollidersHit.Contains(Entity) && Entity.TryGetComponent(out ITakeDamage Enemy))
            {
                if (Entity.TryGetComponent(out BuffableEntity Buffs))
                {

                    Buffs.AddBuff(m_WStunBuff.InitializeBuff(m_WStunBuffTime, Entity.gameObject));
                    Buffs.AddBuff(m_PSoulTheftBuff.InitializeBuff(this, m_PTimeToTheftSoul, Entity.gameObject));
                }
                Debug.Log("TAKEN " + l_Damage + " DAMAGE");
                Enemy.TakeDamage(l_Damage, 0, false, m_CharacterStats.GetPlayerName());
                l_CollidersHit.Add(Entity);
            }
        }

        yield return new WaitForSeconds(m_WaitReactivationTimeW);
        Destroy(l_Explosion);


        m_CanReactiveW = true;

        yield return new WaitForSeconds(m_CanReactiveTimeW);
        m_CanReactiveW = false;


        EndWSkill();
    }
    void UpdateWSkill()
    {
        if (!m_WSkill.GetUsingSkill() && !m_WSkillStarted)
            return;

        if (m_WChanneling)
        {
            m_WTimer += Time.deltaTime;
            GetCharacterUI().UpdateCastingUI(m_WTimer, m_WChannelingTime);
        }

        if (m_CanReactiveW)
        {
            if (Input.GetKeyDown(m_WSkillKey))
            {
                m_WReactived = true;
                m_WReactivedSpeed = ((transform.position - m_WilldurrWScythe.transform.position).magnitude / m_WReactivedTimeToReturn);
                m_CanReactiveW = false;
                if (m_WCoroutine != null)
                    StopCoroutine(m_WCoroutine);
                m_WCoroutine = null;

                m_WilldurrWScythe.GetComponent<WilldurrWScythe>().SetReactive(m_WHitboxRatioReactived);

            }
        }

        if (m_WReactived)
        {
            Vector3 targetPosition = transform.position;

            m_WilldurrWScythe.transform.position = Vector3.MoveTowards(m_WilldurrWScythe.transform.position, new Vector3(targetPosition.x, m_WilldurrWScythe.transform.position.y, targetPosition.z), m_WReactivedSpeed * Time.deltaTime);
            m_WilldurrWScythe.transform.LookAt(new Vector3(targetPosition.x, m_WilldurrWScythe.transform.position.y, targetPosition.z));
            m_WilldurrWScythe.transform.rotation = Quaternion.Euler(m_WilldurrWScythe.transform.eulerAngles.x, m_WilldurrWScythe.transform.eulerAngles.y - 180, m_WilldurrWScythe.transform.eulerAngles.z);


            for (int i = m_CollidersHitWZone.Count - 1; i >= 0; i--)
            {
                Collider entity = m_CollidersHitWZone[i];
                Transform entityTransform = entity.transform;
                Vector3 entityTargetPos = new Vector3(targetPosition.x, entityTransform.position.y, targetPosition.z);

                float distance = Vector3.Distance(entityTransform.position, entityTargetPos);

                if (distance > m_WArribalRangeReactivation / 100f)
                    entityTransform.position = Vector3.MoveTowards(entityTransform.position, entityTargetPos, m_WReactivedSpeed * Time.deltaTime);
                else
                    m_CollidersHitWZone.RemoveAt(i);
            }

            if (Vector3.Distance(m_WilldurrWScythe.transform.position, new Vector3(targetPosition.x, m_WilldurrWScythe.transform.position.y, targetPosition.z)) <= 0.05f)
            {
                m_WReactived = false;
                EndWSkill();
            }

        }

    }
    public void OnTriggerEnterWReactivation(Collider Entity)
    {
        if (!m_WReactived)
            return;
        if (Entity.GetComponent<CharacterMaster>())
            return;

        if (!m_CollidersHitWZone.Contains(Entity) && Entity.TryGetComponent(out ITakeDamage Enemy))
        {
            if (Entity.TryGetComponent(out BuffableEntity Buffs))
            {
                Buffs.AddBuff(m_PSoulTheftBuff.InitializeBuff(this, m_PTimeToTheftSoul, Entity.gameObject));
            }

            float l_Damage = (m_WSkill.GetAttribute("Daño Reactivación", GetWSkillLevel())) + (m_WPercentageBonusAttackDamageSecond / 100) * GetCharacterStats().GetBonusAttackDamage();
            Debug.Log("TAKEN " + l_Damage + " DAMAGE");
            Enemy.TakeDamage(l_Damage, 0, false, m_CharacterStats.GetPlayerName());
            m_CollidersHitWZone.Add(Entity);
        }
    }

    void EndWSkill()
    {
        m_CollidersHitWZone.Clear();
        Destroy(m_WilldurrWScythe);
        m_WilldurrWScythe = null;
        m_WSkillStarted = false;
        m_WSkill.SetUsingSkill(false);
        if (m_WCoroutine != null)
            StopCoroutine(m_WCoroutine);
        m_WCoroutine = null;
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
        if (m_QSkill.GetUsingSkill() || (m_WSkill.GetUsingSkill() && m_WSkillStarted && m_WChanneling) || m_ESkill.GetUsingSkill() || m_RSkill.GetUsingSkill())
        {
            if (!GetUseSkillGizmos())
                return;

            if (m_WSkill.GetUsingSkill() && !m_WSkillStarted)
            {
                m_WSkill.SetUsingSkill(false);
            }
            else if (m_RSkill.GetUsingSkill() && !m_RSkillStarted)
            {
                m_RSkill.SetUsingSkill(false);
            }
            else if (m_QSkill.GetUsingSkill() && !m_QSkillStarted)
            {
                m_QSkill.SetUsingSkill(false);
            }
            else
            {
                return;
            }
        }

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
            m_SkillIndicatorUI.CreateArrowSkillIndicator(m_ESkill.m_IndicatorUIObject, m_UIIndicatorWidthE, m_RangeE, transform.position, true);
            SetShowingGizmos(true);
        }
        else
            m_ECoroutine = StartCoroutine(StartESkill());




    }
    IEnumerator StartESkill()
    {
        m_ESkillStarted = true;
        m_CanReactiveE = false;
        m_HeadCanMove = false;
        m_ReturnHead = false;

        if (Vector3.Distance(GetPositionWithMouse(), transform.position) <= (m_RangeE / 100))
            m_TargetHeadPosition = GetPositionWithMouse();
        else
            m_TargetHeadPosition = ((GetPositionWithMouse() - transform.position).normalized * (m_RangeE / 100)) + transform.position;

        m_TargetHeadPosition.y = m_Head.position.y;
        m_SpeedHeatE = ((m_TargetHeadPosition - m_Head.position).magnitude / m_HeadArriveTargetTimeE) * 100;

        LookAt(m_TargetHeadPosition);

        StopAttacking();
        if (!GetIsLookingForPosition())
            StopMovement();

        GetCharacterUI().SetCastingUIAbilityText("Canalizando");
        GetCharacterUI().HideCastingTime();
        GetCharacterUI().UpdateCastingUI(0, 1);
        GetCharacterUI().ShowCastingUI();
        m_ETimer = 0f;
        m_EChanneling = true;


        SetAnimatorTrigger("IsUsingE");

        SetDisabled(true);
        yield return new WaitForSeconds(m_EChannelingTime);
        SetDisabled(false);
        base.ESkill();
        GetCharacterUI().HideCastingUI();
        m_EChanneling = false;
        m_Head.SetParent(null);
        m_HeadCanMove = true;

        yield return new WaitForSeconds(m_WaitReactivationTimeE);
        m_CanReactiveE = true;

        yield return new WaitForSeconds(m_CanReactiveTimeE);
        m_CanReactiveE = false;
        m_ReturnHead = true;


        EndESkill();
    }
    IEnumerator DoEFear()
    {

        GameObject l_Explosion = m_FearZoneE;
        m_FearZoneE.SetActive(true);
        l_Explosion.transform.localScale = new Vector3(m_FearRadioE / 100, m_FearRadioE / 100, m_FearRadioE / 100);


        List<Collider> l_CollidersHit = new List<Collider>();
        Collider[] l_HitColliders = Physics.OverlapSphere(l_Explosion.transform.position, l_Explosion.transform.localScale.x / 2.0f, m_DamageLayerMask);
        foreach (Collider Entity in l_HitColliders)
        {
            if (!l_CollidersHit.Contains(Entity) && Entity.TryGetComponent(out ITakeDamage Enemy))
            {
                if (Entity.TryGetComponent(out BuffableEntity Buffs))
                {
                    Buffs.AddBuff(m_FearBuffE.InitializeBuff(transform.position, m_FearBuffTimeE, Entity.gameObject));
                    Debug.Log("Buff Fear Added to " + Entity.name);
                }

                l_CollidersHit.Add(Entity);
            }
        }

        yield return new WaitForSeconds(0.2f);//Change
        m_FearZoneE.SetActive(false);
    }
    void UpdateESkill()
    {
        if (!m_ESkill.GetUsingSkill() && !m_ESkillStarted)
            return;

        if (m_EChanneling)
        {
            m_ETimer += Time.deltaTime;
            GetCharacterUI().UpdateCastingUI(m_ETimer, m_EChannelingTime);
        }



        if (m_CanReactiveE)
        {
            if (Input.GetKeyDown(m_ESkillKey))
            {
                transform.position = new Vector3(m_Head.position.x, transform.position.y, m_Head.position.z);
                m_CanReactiveE = false;
                m_ReturnHead = true;
                m_HeadCanMove = false;
                StartCoroutine(DoEFear());

                if (!m_QSkill.GetUsingSkill())
                {
                    StopAttacking();

                    if (!GetIsLookingForPosition())
                        StopMovement();
                }

                EndESkill();
            }
        }

        if (m_HeadCanMove)
        {
            m_Head.position = Vector3.MoveTowards(m_Head.position, m_TargetHeadPosition, (m_SpeedHeatE / 100) * Time.deltaTime);
            if (Vector3.Distance(m_Head.position, m_TargetHeadPosition) <= 0.05f)
            {
                m_HeadCanMove = false;
            }
        }

    }

    void EndESkill()
    {
        if (m_ECoroutine != null)
            StopCoroutine(m_ECoroutine);
        m_ECoroutine = null;

        m_ESkill.SetUsingSkill(false);

        StartCoroutine(RepeatESaver());
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
        if (m_QSkill.GetUsingSkill() || (m_WSkill.GetUsingSkill() && m_WSkillStarted && m_WChanneling) || m_ESkill.GetUsingSkill() || m_RSkill.GetUsingSkill())
        {
            if (!GetUseSkillGizmos())
                return;

            if (m_ESkill.GetUsingSkill() && !m_ESkillStarted)
            {
                m_ESkill.SetUsingSkill(false);
            }
            else if (m_WSkill.GetUsingSkill() && !m_WSkillStarted)
            {
                m_WSkill.SetUsingSkill(false);
            }
            else if (m_QSkill.GetUsingSkill() && !m_QSkillStarted)
            {
                m_QSkill.SetUsingSkill(false);
            }
            else
            {
                return;
            }
        }

        if (!m_SaverCanDoR)
            return;

        m_RSkill.SetUsingSkill(true);

        m_MAXRangeThisR = m_MINRangeR + (((int)(m_PSouls / 5)) * 15);
        Debug.LogError(m_MAXRangeThisR);

        if (GetUseSkillGizmos())
        {
            m_RSkillStarted = false;
            if (GetShowingGizmos())
            {
                m_SkillIndicatorUI.ClearDeletableSkillIndicatorUI();
                m_SkillIndicatorUI.ClearNormalSkillIndicatorUI();
                m_SkillIndicatorUI.ClearTargetSkillIndicatorUI();
            }
            m_SkillIndicatorUI.CreateCircleSkillIndicator(m_RSkill.m_IndicatorUIObject, m_MAXRangeThisR, transform, false);
            SetShowingGizmos(true);
        }
        else
            StartCoroutine(StartRSkill());
    }

    IEnumerator StartRSkill()
    {
        m_RSkillStarted = true;

        GetCharacterUI().SetCastingUIAbilityText("Cargando");
        GetCharacterUI().HideCastingTime();
        GetCharacterUI().UpdateCastingUI(0, 1);
        GetCharacterUI().ShowCastingUI();
        m_RTimer = 0f;
        m_RChanneling = true;
        StopAttacking();
        if (!GetIsLookingForPosition())
            StopMovement();
        SetAnimatorTrigger("IsUsingR");
        SetDisabled(true);
        GetCharacterStats().SetImmuneCC(true);
        yield return new WaitForSeconds(m_EChannelingTime);
        GetCharacterStats().SetImmuneCC(false);
        SetDisabled(false);
        base.RSkill();
        GetCharacterUI().HideCastingUI();
        m_RChanneling = false;



        m_CollidersHitRZone = new List<Collider>();
        m_RZone = Instantiate(m_RZonePrefab, new Vector3(transform.position.x, transform.position.y + 0.05f, transform.position.z), Quaternion.identity);
        m_RZone.GetComponent<WilldurrRZone>().m_CharacterController = this;
        m_RZone.transform.localScale = Vector3.zero;
        m_ActualRangeR = 0;
        m_ScalingRZone = true;

        m_PosToSpawnRHeads = new List<Vector3>();
        m_RHeads = new List<GameObject>();

        for (int i = 0; i < (m_MAXRangeThisR / 100); i++)
        {
            m_PosToSpawnRHeads.Add(GetHeadPosRandom(i));
            Debug.Log("Range to spawn head " + (i + 1) + ": " + m_PosToSpawnRHeads[i]);
        }
        yield return new WaitForSeconds(m_RSkillDuration);
        EndRSkill();

    }

    private Vector3 GetHeadPosRandom(int HeadNum)
    {
        float l_Range = Random.Range(0, m_MAXRangeThisR);

        float l_AnguloAleatorio = Random.Range(0f, 360f);
        float l_Radianes = l_AnguloAleatorio * Mathf.Deg2Rad;

        Vector3 l_HeadPos = m_RZone.transform.position + new Vector3(Mathf.Cos(l_Radianes), 0f, Mathf.Sin(l_Radianes)) * (l_Range / 100);
        l_HeadPos.y = m_RZone.transform.position.y;
        bool l_CanBePos = true;

        if (m_PosToSpawnRHeads.Count > 0)
        {
            foreach (Vector3 HeadPos in m_PosToSpawnRHeads)
            {
                if (Vector3.Distance(l_HeadPos, HeadPos) < m_RMINDistanceHeads / 100)
                {
                    l_CanBePos = false;
                }
            }
            if (!l_CanBePos)
            {
                l_HeadPos = GetHeadPosRandom(HeadNum);
            }

        }
        return l_HeadPos;
    }
    void UpdateRSkill()
    {
        if (!m_RSkill.GetUsingSkill() && !m_RSkillStarted)
            return;

        if (m_RChanneling)
        {
            m_RTimer += Time.deltaTime;
            GetCharacterUI().UpdateCastingUI(m_RTimer, m_RChannelingTime);
        }

        if (m_ScalingRZone)
        {

            m_ActualRangeR = m_ActualRangeR + Time.deltaTime * m_ScaleRZoneSpeed;
            m_RZone.transform.localScale = new Vector3(m_ActualRangeR / 100 * 2, m_ActualRangeR / 100 * 2, m_ActualRangeR / 100 * 2);

            foreach (Vector3 HeadPosition in m_PosToSpawnRHeads)
            {
                bool l_Spawned = false;
                foreach (GameObject Head in m_RHeads)
                {
                    if (Vector3.Distance(Head.transform.position, HeadPosition) <= 0.01f)
                        l_Spawned = true;
                }

                if (!l_Spawned && Vector3.Distance(HeadPosition, m_RZone.transform.position) <= m_ActualRangeR / 100)
                {
                    GameObject l_ThisHead = Instantiate(m_RHeadPrefab, HeadPosition, Quaternion.identity);
                    m_RHeads.Add(l_ThisHead);
                    l_ThisHead.GetComponent<WilldurrHeadR>().SetRadiusHitbox(m_HeadRRadiusHitbox);
                }

            }




            if (m_ActualRangeR >= m_MAXRangeThisR)
            {
                m_ScalingRZone = false;
                m_RZone.transform.localScale = new Vector3(m_MAXRangeThisR / 100 * 2, m_MAXRangeThisR / 100 * 2, m_MAXRangeThisR / 100 * 2);
            }
        }

        if (Input.GetKeyDown(m_ESkillKey))
        {
            if (GetESkillLevel() <= 0)
            {
                Debug.Log("E STILL LOCKED BOBI");
            }
            else if (m_ESkill.GetIsOnCd())
            {
                Debug.Log("E STILL ON COOLDOWN BOBI");
            }
            else if (m_CharacterStats.GetCurrentMana() < m_ESkill.GetMana(GetESkillLevel()))
            {
                Debug.Log("NOT ENOUGH MANA TO USE E");
            }
            else
            {
                Vector3 l_MousePosition = Input.mousePosition;
                l_MousePosition.z = 10.0f;
                Vector3 l_MouseDirection = GetCameraController().GetCamera().ScreenToWorldPoint(l_MousePosition) - GetCameraController().GetCamera().transform.position;
                RaycastHit l_CameraRaycastHit;
                if (Physics.Raycast(GetCameraController().GetCamera().transform.position, l_MouseDirection, out l_CameraRaycastHit, 1000.0f, m_RHeadLayerMask))
                {
                    if (l_CameraRaycastHit.collider.gameObject.GetComponent<WilldurrHeadR>())
                    {
                        transform.position = new Vector3(l_CameraRaycastHit.transform.position.x, transform.position.y, l_CameraRaycastHit.transform.position.z);
                        StopAttacking();

                        if (!GetIsLookingForPosition())
                            StopMovement();

                        m_ESkill.SetCd(m_CoolingEInRSkill);
                        m_ESkill.SetTimer(m_ESkill.GetCd());
                        GetCharacterUI().m_ESkillCdImage.fillAmount = 1.0f;
                        GetCharacterUI().m_ESkillCdText.enabled = true;
                        m_ESkill.SetIsOnCd(true);
                        StartCoroutine(PowersCooldown(m_ESkill));
                        StopRecall();
                    }
                }
            }

        }


    }

    public void TriggerEnterRZone(Collider Entity)
    {
        if (Entity.GetComponent<CharacterMaster>())
            return;

        if (!m_CollidersHitRZone.Contains(Entity) && Entity.TryGetComponent(out ITakeDamage Enemy))
        {
            if (Entity.TryGetComponent(out BuffableEntity Buffs))
            {
                Buffs.AddBuff(m_SlowDownRBuff.InitializeBuff(m_RSkillDuration, m_SlowDownRBuff.m_Value1, Entity.gameObject));
                Debug.Log("Buff SlowDown Added to " + Entity.name);
            }

            m_CollidersHitRZone.Add(Entity);
        }
    }

    public void TriggerExitRZone(Collider Entity)
    {
        if (!m_RSkill.GetUsingSkill() && !m_RSkillStarted)
            return;

        if (m_CollidersHitRZone.Contains(Entity) && Entity.TryGetComponent(out ITakeDamage Enemy))
        {
            if (Entity.TryGetComponent(out BuffableEntity Buffs))
            {
                Buffs.GetBuffWithKey(m_SlowDownRBuff).EndBuffNow();
                Debug.Log("Buff SlowDown Removed to " + Entity.name);
            }

            m_CollidersHitRZone.Remove(Entity);
        }
    }


    void EndRSkill()
    {
        m_RSkill.SetUsingSkill(false);
        m_RSkillStarted = false;

        foreach (Collider CollidersHitRZone in m_CollidersHitRZone)
        {
            if (CollidersHitRZone.TryGetComponent(out BuffableEntity Buffs))
            {
                Buffs.GetBuffWithKey(m_SlowDownRBuff).EndBuffNow();
                Debug.Log("Buff SlowDown Removed to " + CollidersHitRZone.name);
            }
        }

        StartCoroutine(RepeatRSaver());
        Destroy(m_RZone);
        m_RZone = null;
        foreach (GameObject Head in m_RHeads)
        {
            Destroy(Head);
        }
        m_RHeads = null;
        m_PosToSpawnRHeads = null;

        m_CollidersHitRZone = null;

        if (!(GetESkillLevel() <= 0))
            m_ESkill.SetCooldown(GetESkillLevel());
    }

    IEnumerator RepeatRSaver()
    {
        m_SaverCanDoR = false;
        m_SaverCanDoE = false;
        yield return null; // Esperar 1 frame por seguridad
        m_SaverCanDoR = true;
        m_SaverCanDoE = true;

    }
    #endregion


    public void AddSoul(EnemyType _EnemyType)
    {
        int l_AddedSouls = 0;

        foreach (SoulsWithEnemyType Item in m_SoulsWithEnemysTypes)
        {
            if (Item.m_EnemyType == _EnemyType)
            {
                l_AddedSouls = Item.m_Souls;
                GetComponent<BuffableEntity>().AddBuff(m_PWilldurrBuff.InitializeBuff(m_PSouls + l_AddedSouls, gameObject));
            }
        }

        if (m_PSouls < 25 && m_PSouls + l_AddedSouls >= 25)
        {
            GetCharacterStats().AddAttackDamage(m_AttackDamageToAddF1);
            m_PAttackDamageAdded = m_AttackDamageToAddF1;
        }

        if (m_PSouls < 50 && m_PSouls + l_AddedSouls >= 50)
        {
            GetCharacterStats().AddOmnivamp(m_OmnisuctionToAddF2);
        }

        if (m_PSouls < 75 && m_PSouls + l_AddedSouls >= 75)
        {
            GetCharacterStats().AddAttackDamage(m_AttackDamageToAddF3);
            m_PAttackDamageAdded += m_AttackDamageToAddF3;
        }

        if (m_PSouls < 100 && m_PSouls + l_AddedSouls >= 100)
        {
            GetCharacterStats().AddOmnivamp(m_OmnisuctionToAddF4);
        }

        if (m_PSouls + l_AddedSouls > 100)
        {
            float l_ExtraValue = ((int)(m_PSouls + l_AddedSouls - 100) / 5) - ((int)(m_PSouls - 100) / 5);

            GetCharacterStats().AddOmnivamp(m_OmnisuctionToAddF5);
            GetCharacterStats().AddAttackDamage(m_AttackDamageToAddF5 * l_ExtraValue);
            m_PAttackDamageAdded += m_AttackDamageToAddF5 * l_ExtraValue;
        }

        m_PSouls += l_AddedSouls;
    }

    protected override void PerformAutoAttack()
    {
        if (m_DesiredEnemy == null)
        {
            StopAttacking();
            return;
        }

        if (m_DesiredEnemy.TryGetComponent(out BuffableEntity Buffs))
            Buffs.AddBuff(m_PSoulTheftBuff.InitializeBuff(this, m_PTimeToTheftSoul, m_DesiredEnemy.gameObject));

        base.PerformAutoAttack();
    }
    public override void LevelUpRpc()
    {
        base.LevelUpRpc();

        GetCharacterStats().AddAttackDamage(m_PAttackDamageAdded);
    }

    IEnumerator DisableForDuration(float Duration)
    {
        SetDisabled(true);
        yield return new WaitForSeconds(Duration);
        SetDisabled(false);
    }


}

[System.Serializable]
public class SoulsWithEnemyType
{
    public EnemyType m_EnemyType;
    public int m_Souls;
}