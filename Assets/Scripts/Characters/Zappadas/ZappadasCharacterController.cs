using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.UI.Image;
public class ZappadasCharacterController : CharacterMaster
{
    [Header("--- ZAPPADAS ---")]

    [Header("PASSIVE SKILL")]
    public DarkPowerBuff m_PZappadasBuff;
    public int m_DarkPowerEvery = 1;
    public int m_DarkPowerDamageLightlessWithSkill = 5;
    public int m_DarkPowerToAddWithTime = 1;
    public int m_MAXDarkPower = 100;
    int m_PDarkPower = 0;

    [Header("Q SKILL")]
    public int m_DarkPowerToUpgradeSkillQ = 100;
    bool m_QSkillUpgrade = false;

    public GameObject m_QProjectilePrefab;
    public GameObject m_QProjectileUpgradePrefab;
    List<GameObject> m_QProjectiles;

    public float m_OffsetYProjectileQ = 1f;
    public float m_DistanceProjectileSpawn = 2;

    public float m_ProjectileHitboxQ = 100f;
    public float m_ExplosionHitboxQ = 80f;
    public float m_RangeQ = 1300f;
    public float m_ProjectileSpeedQ = 900f;
    public float m_ReactivationWaitTimeQ = 0.3f;


    float m_TimeToDestroyProjectilesQ = 5f;
    int m_ProjectilesCount;
    Coroutine m_DestroyProjectilesWithTimeCoroutineQ;

    [UnityEngine.Range(0f, 100f)]
    public float m_PercentageSkillPowerQ1 = 60;

    [Header("Q Skill Upgrade")]
    [UnityEngine.Range(0f, 100f)]
    public float m_PercentageSkillPowerQ2 = 70;
    public float m_Q2TimeToArribeTarget = 0.25f;
    public float m_Q2Range = 300;
    public float m_Q2AditionalDamageMinions = 25;
    private int m_Q2MaxBounces = 3;





    bool m_CanReactiveQ;
    public float m_QChannelingTime = 0.15f; //Se puede borrar si no hace Canalizando la skill
    bool m_QChanneling = true; //Se puede borrar si no hace Canalizando la skill
    float m_QTimer = 0f; //Se puede borrar si no hace Canalizando la skill
    bool m_SaverCanDoQ = true;
    bool m_QSkillStarted = false;

    [Header("W SKILL")]
    public int m_DarkPowerToUpgradeSkillW = 100;
    bool m_WSkillUpgrade = false;


    public float m_WWormholeRange = 900f;
    public float m_WWormholeDuration = 8f;
    public float m_WWormholeStartPosOffset = 100f;
    public float m_WProjectileExitRange = 800f;
    public float m_WProjectileExitSpeed = 900f;
    [UnityEngine.Range(0f, 100f)]
    public float m_WProjectileHitboxIncrease = 10f;
    public float m_WWormholeStartHitboxRadius = 150f;

    public float m_WDarkPowerRange = 450f;


    public GameObject m_WWormholeStartPrefab;
    public GameObject m_SkillsIndicatorUIObject;

    Vector3 m_WWormholeEndPosition;
    bool m_LoadingWSkill = false;



    public float m_WChannelingTime = 0.05f; //Se puede borrar si no hace Canalizando la skill
    bool m_WChanneling = true; //Se puede borrar si no hace Canalizando la skill
    float m_WTimer = 0f; //Se puede borrar si no hace Canalizando la skill
    bool m_SaverCanDoW = true;
    bool m_WSkillStarted = false;

    [Header("E SKILL")]
    public int m_DarkPowerToUpgradeSkillE = 100;
    bool m_ESkillUpgrade = false;
    bool m_LoadingESkill = false;


    float m_RangeE = 1400;
    float m_EHitboxRadius = 200;
    float m_DistanceBetweenBalls = 250;
    float m_TimeToDestroyEBall = 1.5f;

    Vector3 m_EBallsInstancePosition;
    Vector3 m_EDirection;
    public GameObject m_BallsPrefab;
    [UnityEngine.Range(0f, 100f)]
    public float m_PercentageSkillPowerE = 50;
    public StunBuff m_EStunBuff;
    public float m_EStunBuffTime = 0.5f;

    Vector3 m_PosPushBall_01;
    Vector3 m_PosPushBall_02;
    Vector3 m_PosPushBall_03;

    List<Transform> m_EnemysToPushBall_01 = new List<Transform>();
    List<Transform> m_EnemysToPushBall_02 = new List<Transform>();
    List<Transform> m_EnemysToPushBall_03 = new List<Transform>();

    List<GameObject> m_EnemysWithDarkE = new List<GameObject>();
    public float m_EDarkBuffTime = 5f;

    bool m_EPushEnemies = false;
    public float m_PushSpeed = 5f;

    float m_EChannelingTime = 1f; //Se puede borrar si no hace Canalizando la skill
    bool m_EChanneling = true; //Se puede borrar si no hace Canalizando la skill
    float m_ETimer = 0f; //Se puede borrar si no hace Canalizando la skill
    bool m_SaverCanDoE = true;
    bool m_ESkillStarted = false;

    [Header("R SKILL")]
    public GameObject m_RPrefab;
    int m_DarkPowerToUpgradeSkillR = 100;
    bool m_RSkillUpgrade = false;
    public GameObject m_RProjectilPrefab;
    public Transform m_RProjectilSpawnPos;
    GameObject m_RProyectil;
    Vector3 m_RTargetPos;
    bool m_RMove = false;

    public float m_RRange = 2000;
    public float m_RProyectilSpeed = 700;
    float m_RWidth = 200;

    public float m_RChannelingTime = 0.5f; //Se puede borrar si no hace Canalizando la skill
    bool m_RChanneling = true; //Se puede borrar si no hace Canalizando la skill
    float m_RTimer = 0f; //Se puede borrar si no hace Canalizando la skill
    bool m_SaverCanDoR = true;
    bool m_RSkillStarted = false;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        ResetDarkPower();
        StartCoroutine(AddDarkPowerCorrutine());
        m_EPushEnemies = false;
        m_RMove = false;
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
                    StartCoroutine(StartESkill());

                if (m_QSkill.GetUsingSkill() && !m_QSkillStarted)
                    StartCoroutine(StartQSkill());

                if (m_WSkill.GetUsingSkill() && !m_WSkillStarted)
                    StartCoroutine(StartWSkill());

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

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F1))
            AddDarkPower(100);
#endif

        if (m_EPushEnemies)
        {
            PushListAwayFromBall(m_EnemysToPushBall_01, m_PosPushBall_01);
            PushListAwayFromBall(m_EnemysToPushBall_02, m_PosPushBall_02);
            PushListAwayFromBall(m_EnemysToPushBall_03, m_PosPushBall_03);
        }

    }
    void PushListAwayFromBall(List<Transform> enemyList, Vector3 ballPosition)
    {
        foreach (Transform enemy in enemyList)
        {
            if (enemy == null) continue;

            Vector3 enemyPosXZ = new Vector3(enemy.position.x, 0, enemy.position.z);
            Vector3 ballPosXZ = new Vector3(ballPosition.x, 0, ballPosition.z);

            Vector3 directionAway = (enemyPosXZ - ballPosXZ).normalized;

            Vector3 newPos = enemy.position + directionAway * m_PushSpeed * Time.deltaTime;
            enemy.position = new Vector3(newPos.x, enemy.position.y, newPos.z);
        }
    }
    #region Q Skill
    //Q SKILL
    protected override void QSkill()
    {
        if (m_QSkill.GetUsingSkill() || m_WSkill.GetUsingSkill() || m_ESkill.GetUsingSkill() || m_RSkill.GetUsingSkill())
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
            else if (m_ESkill.GetUsingSkill() && !m_ESkillStarted)
            {
                m_ESkill.SetUsingSkill(false);
            }
            else
            {
                return;
            }
        }

        if (!m_SaverCanDoQ)
            return;

        m_CanReactiveQ = false;
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
            m_SkillIndicatorUI.CreateArrowSkillIndicator(m_QSkill.m_IndicatorUIObject, m_ProjectileHitboxQ, m_RangeQ, transform.position, true);
            SetShowingGizmos(true);
        }
        else
            StartCoroutine(StartQSkill());




    }

    IEnumerator StartQSkill()
    {
        m_QSkillUpgrade = false;
        m_QSkillStarted = true;
        StopAttacking();
        if (!GetIsLookingForPosition())
            StopMovement();

        GetCharacterUI().SetCastingUIAbilityText("Canalizando");
        GetCharacterUI().HideCastingTime();
        GetCharacterUI().UpdateCastingUI(0, 1);
        GetCharacterUI().ShowCastingUI();
        m_QTimer = 0f;
        m_QChanneling = true;
        m_TimeToDestroyProjectilesQ = m_QSkill.GetAttribute("Tiempo para que desaparezcan las bolas", GetQSkillLevel());
        m_ProjectilesCount = (int)m_QSkill.GetAttribute("Cantidad de bolas", GetQSkillLevel());

        SetAnimatorTrigger("IsUsingQ");

        if (m_PDarkPower >= m_DarkPowerToUpgradeSkillQ)
        {
            AddDarkPower(-m_DarkPowerToUpgradeSkillQ);
            m_QSkillUpgrade = true;
        }


        SetDisabled(true);
        yield return new WaitForSeconds(m_QChannelingTime);
        SetDisabled(false);
        base.QSkill();
        GetCharacterUI().HideCastingUI();
        m_QChanneling = false;

        m_QProjectiles = new List<GameObject>();


        for (int i = 0; i < m_ProjectilesCount; i++)
        {
            Vector3 l_Position = transform.position + transform.forward * (m_DistanceProjectileSpawn/100);
            l_Position.y += (m_OffsetYProjectileQ/100);

            GameObject l_ActualProjectile = Instantiate(m_QProjectilePrefab, l_Position, Quaternion.identity, transform);
            l_ActualProjectile.GetComponent<ZappadasQProjectile>().SetSizeProjectil(m_ProjectileHitboxQ);
            m_QProjectiles.Add(l_ActualProjectile);
        }

        m_DestroyProjectilesWithTimeCoroutineQ = StartCoroutine(DestroyProjectilesWithTime());
        StartCoroutine(WaitReactiveQ());

    }

    IEnumerator DestroyProjectilesWithTime()
    {
        yield return new WaitForSeconds(m_TimeToDestroyProjectilesQ);
        if (m_QSkill.GetUsingSkill() && m_QSkillStarted)
        {
            if (m_QProjectiles != null)
            {
                if (m_QProjectiles.Count > 0)
                {
                    foreach (GameObject Projectile in m_QProjectiles)
                    {
                        Destroy(Projectile);
                    }
                }
            }
            EndQSkill();
        }
    }

    IEnumerator WaitReactiveQ()
    {
        yield return new WaitForSeconds(m_ReactivationWaitTimeQ);
        m_CanReactiveQ = true;
    }
    void UpdateQSkill()
    {
        if (!m_QSkill.GetUsingSkill() && !m_QSkillStarted)
            return;

        if (m_QChanneling)
        {
            m_QTimer += Time.deltaTime;
            GetCharacterUI().UpdateCastingUI(m_QTimer, m_QChannelingTime);
        }

        if (m_CanReactiveQ)
        {
            if (Input.GetKeyDown(m_QSkillKey))
            {
                m_CanReactiveQ = false;
                GameObject l_ActualProjectile = m_QProjectiles[0];
                m_QProjectiles.RemoveAt(0);
                LookAt(((GetPositionWithMouse() - transform.position).normalized * 2) + transform.position);
                l_ActualProjectile.transform.SetParent(null);

                Vector3 l_Direccion = GetPositionWithMouse() - l_ActualProjectile.transform.position;
                l_Direccion.y = 0;
                l_Direccion = l_Direccion.normalized;
                float l_Damage = (m_QSkill.GetAttribute("Daño base", GetQSkillLevel())) + (m_PercentageSkillPowerQ1 / 100) * GetCharacterStats().GetAbilityPower();
                l_ActualProjectile.GetComponent<ZappadasQProjectile>().SetProjectile(l_Damage, m_ProjectileHitboxQ, m_RangeQ, m_ProjectileSpeedQ, l_Direccion, this);
                
                StopAttacking();
                if (!GetIsLookingForPosition())
                    StopMovement();

                if (m_QProjectiles.Count == 0)
                    EndQSkill();
                else
                    StartCoroutine(WaitReactiveQ());
            }
        }
    }

    public void OnTriggerEnterQ(Collider OtherEntity, GameObject _Projectil)
    {
        if (OtherEntity.GetComponent<CharacterMaster>())
            return;

        if (OtherEntity.TryGetComponent(out ITakeDamage takeDamage))
        {
            List<Collider> l_CollidersHit = new List<Collider>();
            Collider[] l_HitColliders = Physics.OverlapSphere(OtherEntity.transform.position, m_ExplosionHitboxQ / 100, m_DamageLayerMask);


            foreach (Collider Entity in l_HitColliders)
            {
                if (!l_CollidersHit.Contains(Entity) && Entity.TryGetComponent(out ITakeDamage Enemy))
                {
                    float l_Damage = (m_QSkill.GetAttribute("Daño base", GetQSkillLevel())) + (m_PercentageSkillPowerQ1 / 100) * GetCharacterStats().GetAbilityPower();
                    Debug.Log("TAKEN " + l_Damage + " DAMAGE");
                    Enemy.TakeDamage(0, l_Damage, false, m_CharacterStats.GetPlayerName(), gameObject);
                    AddmDarkPowerDamageLightlessWithSkill(Entity.gameObject);
                    l_CollidersHit.Add(Entity);
                }
            }

            Destroy(_Projectil);

            if (m_QSkillUpgrade)
            {
                Vector3 l_SpawnPos = OtherEntity.transform.position;
                l_SpawnPos.y = m_OffsetYProjectileQ;
                GameObject l_QProjectileUpgrade = Instantiate(m_QProjectileUpgradePrefab, l_SpawnPos, Quaternion.identity);
                m_Q2MaxBounces = (int)m_QSkill.GetAttribute("Cantidad de rebotes", GetQSkillLevel());
                float l_Damage = (m_QSkill.GetAttribute("Daño base “Energía oscura”", GetQSkillLevel())) + (m_PercentageSkillPowerQ2 / 100) * GetCharacterStats().GetAbilityPower();
                l_QProjectileUpgrade.GetComponent<ZappadasQProjectileUpgrade>().SetProjectilUpGrade(l_Damage, m_Q2TimeToArribeTarget, m_Q2Range, m_Q2AditionalDamageMinions, m_DamageLayerMask, m_Q2MaxBounces, OtherEntity, GetCharacterStats(), this);

            }


        }
    }

    void EndQSkill()
    {
        StopCoroutine(m_DestroyProjectilesWithTimeCoroutineQ);
        m_CanReactiveQ = false;
        m_QProjectiles.Clear();
        m_QProjectiles = null;
        m_QSkillStarted = false;
        m_QSkill.SetUsingSkill(false);
        StartCoroutine(RepeatQSaver());
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
        if (m_WSkill.GetUsingSkill() || m_ESkill.GetUsingSkill() || m_RSkill.GetUsingSkill())
        {
            if (!GetUseSkillGizmos())
                return;

            if (m_QSkill.GetUsingSkill() && !m_QSkillStarted)
            {
                m_QSkill.SetUsingSkill(false);
            }
            else if (m_RSkill.GetUsingSkill() && !m_RSkillStarted)
            {
                m_RSkill.SetUsingSkill(false);
            }
            else if (m_ESkill.GetUsingSkill() && !m_ESkillStarted)
            {
                m_ESkill.SetUsingSkill(false);
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
            m_SkillIndicatorUI.CreateCircleSkillIndicator(m_WSkill.m_IndicatorUIObject, m_WWormholeRange, transform, false);
            SetShowingGizmos(true);
        }
        else
            StartCoroutine(StartWSkill());


    }

    IEnumerator StartWSkill()
    {

        m_WSkillUpgrade = false;
        m_WSkillStarted = true;
        m_WChanneling = false;

        Vector3 l_TargetPosition;


        Vector3 mousePos = GetPositionWithMouse();

        Vector3 direction = mousePos - transform.position;
        float distance = direction.magnitude;

        if (distance <= m_WWormholeRange / 100)
        {
            l_TargetPosition = mousePos;
        }
        else
        {
            l_TargetPosition = transform.position + direction.normalized * m_WWormholeRange / 100;
        }


        Vector3 l_Direction = l_TargetPosition - transform.position;
        l_Direction.y = 0;

        if (l_Direction.magnitude > m_WWormholeRange)
        {
            l_Direction = l_Direction.normalized * m_WWormholeRange;
        }

        m_WWormholeEndPosition = transform.position + l_Direction;

        m_SkillIndicatorUI.CreateArrowSkillIndicator(m_SkillsIndicatorUIObject, m_WWormholeStartHitboxRadius, m_WProjectileExitRange, m_WWormholeEndPosition, false);
        StopAttacking();
        if (!GetIsLookingForPosition())
            StopMovement();

        SetDisabled(true);
        yield return null;
        m_LoadingWSkill = true;
    }
    IEnumerator StartWWormhole()
    {
        StopAttacking();
        if (!GetIsLookingForPosition())
            StopMovement();

        m_LoadingWSkill = false;

        m_SkillIndicatorUI.ClearNormalSkillIndicatorUI();

        GetCharacterUI().SetCastingUIAbilityText("Canalizando");
        GetCharacterUI().HideCastingTime();
        GetCharacterUI().UpdateCastingUI(0, 1);
        GetCharacterUI().ShowCastingUI();
        m_WTimer = 0f;
        m_WChanneling = true;


        Vector3 l_Direction = GetPositionWithMouse() - m_WWormholeEndPosition;
        l_Direction.y = 0;
        l_Direction.Normalize();

        SetAnimatorTrigger("IsUsingW");

        SetDisabled(true);
        yield return new WaitForSeconds(m_WChannelingTime);
        SetDisabled(false);
        base.WSkill();
        GetCharacterUI().HideCastingUI();
        m_WChanneling = false;



        Vector3 l_Offset = transform.forward * (m_WWormholeStartPosOffset/100);
        Vector3 l_SpawnPosition = transform.position + l_Offset;
        GameObject l_Wormhole = Instantiate(m_WWormholeStartPrefab, l_SpawnPosition, Quaternion.identity);

        if (m_PDarkPower >= m_DarkPowerToUpgradeSkillW)
        {
            AddDarkPower(-m_DarkPowerToUpgradeSkillW);
            m_WSkillUpgrade = true;
        }

        l_Wormhole.GetComponent<ZappadasWWormhole>().SetWWormhole(m_WWormholeEndPosition, l_Direction, m_WWormholeDuration, m_WSkillUpgrade, m_WProjectileExitSpeed, m_WProjectileExitRange, m_WProjectileHitboxIncrease, m_WSkill.GetAttribute("Daño base “Energía oscura”", GetWSkillLevel()), m_WDarkPowerRange, m_WWormholeStartHitboxRadius, this, m_WSkillUpgrade);

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

        if (m_LoadingWSkill)
        {
            if (Input.GetKeyUp(m_WSkillKey) || Input.GetMouseButtonUp(0))
            {
                StartCoroutine(StartWWormhole());
            }
        }


    }

    void EndWSkill()
    {

        m_WSkillUpgrade = false;
        m_WSkillStarted = false;
        m_WSkill.SetUsingSkill(false);
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
        if (m_WSkill.GetUsingSkill() || m_ESkill.GetUsingSkill() || m_RSkill.GetUsingSkill())
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
            m_SkillIndicatorUI.CreateCircleSkillIndicator(m_ESkill.m_IndicatorUIObject, m_RangeE, transform, true);
            SetShowingGizmos(true);
        }
        else
            StartCoroutine(StartESkill());

    }
    IEnumerator StartESkill()
    {

        m_ESkillUpgrade = false;
        m_ESkillStarted = true;
        StopAttacking();
        if (!GetIsLookingForPosition())
            StopMovement();


        Vector3 l_TargetPosition;


        Vector3 mousePos = GetPositionWithMouse();

        Vector3 direction = mousePos - transform.position;
        float distance = direction.magnitude;

        if (distance <= m_RangeE / 100)
        {
            l_TargetPosition = mousePos;
        }
        else
        {
            l_TargetPosition = transform.position + direction.normalized * m_RangeE / 100;
        }


        Vector3 l_Direction = l_TargetPosition - transform.position;
        l_Direction.y = 0;

        if (l_Direction.magnitude > m_RangeE)
        {
            l_Direction = l_Direction.normalized * m_RangeE;
        }

        m_EBallsInstancePosition = transform.position + l_Direction;

        m_SkillIndicatorUI.CreateArrowSkillIndicator(m_SkillsIndicatorUIObject, m_EHitboxRadius, m_DistanceBetweenBalls * 2, m_EBallsInstancePosition, false);
        StopAttacking();
        if (!GetIsLookingForPosition())
            StopMovement();

        SetDisabled(true);

        m_LoadingESkill = true;
        yield return null;
    }
    void SpawnEBalls()
    {

        m_SkillIndicatorUI.ClearNormalSkillIndicatorUI();
        SetDisabled(false);

        m_LoadingESkill = false;
        m_EDirection = (GetPositionWithMouse() - m_EBallsInstancePosition).normalized;
        if (m_EDirection == Vector3.zero)
            m_EDirection = Quaternion.Euler(0, 90, 0) * (transform.position - m_EBallsInstancePosition).normalized;

        base.ESkill();
        if (m_PDarkPower >= m_DarkPowerToUpgradeSkillE)
        {
            AddDarkPower(-m_DarkPowerToUpgradeSkillE);
            m_ESkillUpgrade = true;
        }
        GameObject ball = Instantiate(m_BallsPrefab, m_EBallsInstancePosition, Quaternion.identity);
        ball.GetComponent<ZappadasEBall>().SetBallE(m_TimeToDestroyEBall, this, m_ESkillUpgrade, 1);

        Vector3 forwardPos = m_EBallsInstancePosition + m_EDirection * (m_DistanceBetweenBalls / 100);
        ball = Instantiate(m_BallsPrefab, forwardPos, Quaternion.identity);
        ball.GetComponent<ZappadasEBall>().SetBallE(m_TimeToDestroyEBall, this, m_ESkillUpgrade, 2);

        Vector3 backwardPos = m_EBallsInstancePosition + m_EDirection * (m_DistanceBetweenBalls / 100) * 2;
        ball = Instantiate(m_BallsPrefab, backwardPos, Quaternion.identity);
        ball.GetComponent<ZappadasEBall>().SetBallE(m_TimeToDestroyEBall, this, m_ESkillUpgrade, 3);

        EndESkill();
    }
    public void ExploteEBall(bool Updated, Vector3 pos, int ballNum)
    {

        List<Collider> l_CollidersHit = new List<Collider>();
        Collider[] l_HitColliders = Physics.OverlapSphere(pos, m_EHitboxRadius / 100, m_DamageLayerMask);
        if (m_EnemysToPushBall_01 == null || m_EnemysToPushBall_02 == null || m_EnemysToPushBall_03 == null || m_EnemysWithDarkE == null)
        {
            m_EnemysToPushBall_01 = new List<Transform>();
            m_EnemysToPushBall_02 = new List<Transform>();
            m_EnemysToPushBall_03 = new List<Transform>();
            m_EnemysWithDarkE = new List<GameObject>();
        }

        foreach (Collider Entity in l_HitColliders)
        {
            if (!l_CollidersHit.Contains(Entity) && Entity.TryGetComponent(out ITakeDamage Enemy))
            {
                if (Entity.TryGetComponent(out BuffableEntity Buffs))
                {
                    Buffs.AddBuff(m_EStunBuff.InitializeBuff(m_EStunBuffTime, Entity.gameObject));
                }
                float l_Damage = (m_ESkill.GetAttribute("Daño base", GetESkillLevel())) + (m_PercentageSkillPowerE / 100) * GetCharacterStats().GetAbilityPower();
                Debug.Log("TAKEN " + l_Damage + " DAMAGE");
                Enemy.TakeDamage(0, l_Damage, false, m_CharacterStats.GetPlayerName(), gameObject);
                AddmDarkPowerDamageLightlessWithSkill(Entity.gameObject);
                l_CollidersHit.Add(Entity);
                if (ballNum == 1)
                {
                    m_EnemysToPushBall_01.Add(Entity.transform);
                }

                if (ballNum == 2)
                {
                    m_EnemysToPushBall_02.Add(Entity.transform);
                }

                if (ballNum == 3)
                {
                    m_EnemysToPushBall_03.Add(Entity.transform);
                }

                if (Updated)
                {
                    m_EnemysWithDarkE.Add(Entity.gameObject);
                }
            }
        }
        if (ballNum == 1)
        {
            m_PosPushBall_01 = pos;
        }

        if (ballNum == 2)
        {
            m_PosPushBall_02 = pos;
        }

        if (ballNum == 3)
        {
            m_PosPushBall_03 = pos;
        }
        m_EPushEnemies = true;
        StartCoroutine(PushEnemiesE());
        if (Updated)
        {
            StartCoroutine(DeleteDarkECoroutine());
        }
    }
    IEnumerator PushEnemiesE()
    {
        yield return new WaitForSeconds(m_EStunBuffTime / 3);
        m_EnemysToPushBall_01 = null;
        m_EnemysToPushBall_02 = null;
        m_EnemysToPushBall_03 = null;
        m_EPushEnemies = false;
    }
    IEnumerator DeleteDarkECoroutine()
    {
        yield return new WaitForSeconds(m_EDarkBuffTime);
        DeleteDarkE(true);
    }
    void DeleteDarkE(bool ignoreAdds = false)
    {
        if (!ignoreAdds)
        {
            m_ESkill.SetCd(0);
            m_CharacterStats.SetCurrentManaRpc(m_ESkill.GetAttribute("Maná recuperado por Mancha de la Oscuridad", GetESkillLevel()));
        }

        m_EnemysWithDarkE.Clear();
        m_EnemysWithDarkE = new List<GameObject>();
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

        if (m_LoadingESkill)
        {
            if (Input.GetKeyUp(m_ESkillKey) || Input.GetMouseButtonUp(0))
            {
                SpawnEBalls();
            }
        }

    }
    void EndESkill()
    {

        m_ESkillUpgrade = false;
        m_ESkillStarted = false;
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
        if (m_WSkill.GetUsingSkill() || m_ESkill.GetUsingSkill() || m_RSkill.GetUsingSkill())
        {
            if (!GetUseSkillGizmos())
                return;

            if (m_WSkill.GetUsingSkill() && !m_WSkillStarted)
            {
                m_WSkill.SetUsingSkill(false);
            }
            else if (m_QSkill.GetUsingSkill() && !m_QSkillStarted)
            {
                m_QSkill.SetUsingSkill(false);
            }
            else if (m_ESkill.GetUsingSkill() && !m_ESkillStarted)
            {
                m_ESkill.SetUsingSkill(false);
            }
            else
            {
                return;
            }
        }

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
            m_SkillIndicatorUI.CreateArrowSkillIndicator(m_RSkill.m_IndicatorUIObject, m_RWidth, m_RRange, transform.position, true);
            SetShowingGizmos(true);
        }
        else
            StartCoroutine(StartRSkill());


    }

    IEnumerator StartRSkill()
    {

        m_RSkillUpgrade = false;
        m_RSkillStarted = true;
        StopAttacking();
        if (!GetIsLookingForPosition())
            StopMovement();

        GetCharacterUI().SetCastingUIAbilityText("Canalizando");
        GetCharacterUI().HideCastingTime();
        GetCharacterUI().UpdateCastingUI(0, 1);
        GetCharacterUI().ShowCastingUI();
        m_RTimer = 0f;
        m_RChanneling = true;

        Vector3 l_TargetPosition;


        Vector3 mousePos = GetPositionWithMouse();

        Vector3 direction = mousePos - transform.position;
        float distance = direction.magnitude;

        if (distance <= m_RRange / 100)
        {
            l_TargetPosition = mousePos;
        }
        else
        {
            l_TargetPosition = transform.position + direction.normalized * m_RRange / 100;
        }

        m_RProyectil = Instantiate(m_RProjectilPrefab, m_RProjectilSpawnPos.position, Quaternion.identity);
        m_RTargetPos = l_TargetPosition;
        m_RMove = true;

        SetAnimatorTrigger("IsUsingR");

        SetDisabled(true);
        yield return new WaitForSeconds(m_RChannelingTime);
        SetDisabled(false);
        base.RSkill();
        GetCharacterUI().HideCastingUI();
        m_RChanneling = false;

        m_RMove = true;
    }

    void SpawnRSkill()
    {
        GameObject l_RPrefab = Instantiate(m_RPrefab, new Vector3(m_RProyectil.transform.position.x, transform.position.y, m_RProyectil.transform.position.z), Quaternion.identity);
        l_RPrefab.GetComponent<ZappadasRSkill>().SetRSkill(this);
        Destroy(m_RProyectil);
        EndRSkill();
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

        if (m_RMove)
        {
            m_RProyectil.transform.position = Vector3.MoveTowards(
                m_RProyectil.transform.position,
                m_RTargetPos,
                m_RProyectilSpeed / 100 * Time.deltaTime
            );

            if (Vector3.Distance(m_RProyectil.transform.position, m_RTargetPos) < 0.01f)
            {
                m_RMove = false;
                SpawnRSkill();
            }
        }
    }
    void EndRSkill()
    {
        m_RMove = false;
        m_RSkillUpgrade = false;
        m_RSkillStarted = false;
        m_RSkill.SetUsingSkill(false);
        base.RSkill();
        StartCoroutine(RepeatRSaver());
    }

    IEnumerator RepeatRSaver()
    {
        m_SaverCanDoR = false;
        yield return null; // Esperar 1 frame por seguridad
        m_SaverCanDoR = true;

    }
    #endregion


    int GetDarkPower()
    {
        return m_PDarkPower;
    }
    public void AddmDarkPowerDamageLightlessWithSkill(GameObject Enemy)
    {
        AddDarkPower(m_DarkPowerDamageLightlessWithSkill);


        if (m_EnemysWithDarkE != null)
        {
            foreach (GameObject enemy in m_EnemysWithDarkE)
            {
                if (enemy.name == Enemy.name)
                {
                    DeleteDarkE();
                    return;
                }
            }
        }

    }

    public void AddDarkPower(int DarkPowerToAdd)
    {
        if (m_PDarkPower + DarkPowerToAdd >= m_MAXDarkPower)
            m_PDarkPower = m_MAXDarkPower;
        else
            m_PDarkPower += DarkPowerToAdd;

        GetComponent<BuffableEntity>().AddBuff(m_PZappadasBuff.InitializeBuff(m_PDarkPower, gameObject));
    }
    void ResetDarkPower()
    {
        m_PDarkPower = 0;
    }
    IEnumerator AddDarkPowerCorrutine()
    {
        yield return new WaitForSeconds(m_DarkPowerEvery);
        AddDarkPower(m_DarkPowerToAddWithTime);
        StartCoroutine(AddDarkPowerCorrutine());
    }

    public override void LevelUpRpc()
    {
        base.LevelUpRpc();
    }
}

