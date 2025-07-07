using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
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
    public int m_Q2MaxBounces = 3;





    bool m_CanReactiveQ;
    public float m_QChannelingTime = 0.15f; //Se puede borrar si no hace Canalizando la skill
    bool m_QChanneling = true; //Se puede borrar si no hace Canalizando la skill
    float m_QTimer = 0f; //Se puede borrar si no hace Canalizando la skill
    bool m_SaverCanDoQ = true;
    bool m_QSkillStarted = false;

    [Header("W SKILL")]
    public int m_DarkPowerToUpgradeSkillW = 100;
    bool m_WSkillUpgrade = false;

    public float m_WChannelingTime = 1f; //Se puede borrar si no hace Canalizando la skill
    bool m_WChanneling = true; //Se puede borrar si no hace Canalizando la skill
    float m_WTimer = 0f; //Se puede borrar si no hace Canalizando la skill
    bool m_SaverCanDoW = true;
    bool m_WSkillStarted = false;

    [Header("E SKILL")]
    public int m_DarkPowerToUpgradeSkillE = 100;
    bool m_ESkillUpgrade = false;

    public float m_EChannelingTime = 1f; //Se puede borrar si no hace Canalizando la skill
    bool m_EChanneling = true; //Se puede borrar si no hace Canalizando la skill
    float m_ETimer = 0f; //Se puede borrar si no hace Canalizando la skill
    bool m_SaverCanDoE = true;
    bool m_ESkillStarted = false;

    [Header("R SKILL")]
    public int m_DarkPowerToUpgradeSkillR = 100;
    bool m_RSkillUpgrade = false;

    public float m_RChannelingTime = 1f; //Se puede borrar si no hace Canalizando la skill
    bool m_RChanneling = true; //Se puede borrar si no hace Canalizando la skill
    float m_RTimer = 0f; //Se puede borrar si no hace Canalizando la skill
    bool m_SaverCanDoR = true;
    bool m_RSkillStarted = false;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        ResetDarkPower();
        StartCoroutine(AddDarkPowerCorrutine());
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


        float l_AngleToAdd = 360 / m_ProjectilesCount;
        float l_ActualAngle = 0;
        for (int i = 0; i < m_ProjectilesCount; i++)
        {
            float l_AngleRadians = l_ActualAngle * Mathf.Deg2Rad;
            Vector3 l_Direction = new Vector3(Mathf.Cos(l_AngleRadians), 0, Mathf.Sin(l_AngleRadians));
            Vector3 l_Position = transform.position + l_Direction * m_DistanceProjectileSpawn;
            l_Position.y += m_OffsetYProjectileQ;

            GameObject l_ActualProjectile = Instantiate(m_QProjectilePrefab, l_Position, Quaternion.identity);
            m_QProjectiles.Add(l_ActualProjectile);
            l_ActualAngle += l_AngleToAdd;
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

                l_ActualProjectile.GetComponent<ZappadasQProjectile>().SetProjectile(m_ProjectileHitboxQ, m_RangeQ, m_ProjectileSpeedQ, l_Direccion, this);

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
                    Enemy.TakeDamage(0, l_Damage, false, m_CharacterStats.GetPlayerName());
                    AddmDarkPowerDamageLightlessWithSkill();
                    l_CollidersHit.Add(Entity);
                }
            }

            Destroy(_Projectil);

            if (m_QSkillUpgrade)
            {
                Vector3 l_SpawnPos = OtherEntity.transform.position;
                l_SpawnPos.y = m_OffsetYProjectileQ;
                GameObject l_QProjectileUpgrade = Instantiate(m_QProjectileUpgradePrefab, l_SpawnPos, Quaternion.identity);

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
        if (m_QSkill.GetUsingSkill() || m_WSkill.GetUsingSkill() || m_ESkill.GetUsingSkill() || m_RSkill.GetUsingSkill())
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
            //Aqui va el codigo para mostrar la UI --> EJ: m_SkillIndicatorUI.CreateArrowSkillIndicator(m_WSkill.m_IndicatorUIObject, m_UIIndicatorWidthW, m_RangeW, transform.position, true);
            SetShowingGizmos(true);
        }
        else
            StartCoroutine(StartWSkill());


    }

    IEnumerator StartWSkill()
    {

        m_WSkillUpgrade = false;
        m_WSkillStarted = true;
        StopAttacking();
        if (!GetIsLookingForPosition())
            StopMovement();

        GetCharacterUI().SetCastingUIAbilityText("Canalizando");
        GetCharacterUI().HideCastingTime();
        GetCharacterUI().UpdateCastingUI(0, 1);
        GetCharacterUI().ShowCastingUI();
        m_WTimer = 0f;
        m_WChanneling = true;


        SetAnimatorTrigger("IsUsingW");

        SetDisabled(true);
        yield return new WaitForSeconds(m_WChannelingTime);
        SetDisabled(false);
        base.WSkill();
        GetCharacterUI().HideCastingUI();
        m_WChanneling = false;


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
            //Aqui va el codigo para mostrar la UI --> EJ: m_SkillIndicatorUI.CreateArrowSkillIndicator(m_ESkill.m_IndicatorUIObject, m_UIIndicatorWidthE, m_RangeE, transform.position, true);
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


        EndESkill();
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
        if (m_QSkill.GetUsingSkill() || m_WSkill.GetUsingSkill() || m_ESkill.GetUsingSkill() || m_RSkill.GetUsingSkill())
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
            //Aqui va el codigo para mostrar la UI --> EJ: m_SkillIndicatorUI.CreateArrowSkillIndicator(m_RSkill.m_IndicatorUIObject, m_UIIndicatorWidthR, m_RangeR, transform.position, true);
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


        SetAnimatorTrigger("IsUsingR");

        SetDisabled(true);
        yield return new WaitForSeconds(m_RChannelingTime);
        SetDisabled(false);
        base.RSkill();
        GetCharacterUI().HideCastingUI();
        m_RChanneling = false;


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

    }
    void EndRSkill()
    {
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
    public void AddmDarkPowerDamageLightlessWithSkill()
    {
        AddDarkPower(m_DarkPowerDamageLightlessWithSkill);
    }

    void AddDarkPower(int DarkPowerToAdd)
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

    IEnumerator DisableForDuration(float Duration)
    {
        SetDisabled(true);
        yield return new WaitForSeconds(Duration);
        SetDisabled(false);
    }
}

