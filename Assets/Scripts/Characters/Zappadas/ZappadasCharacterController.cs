using System.Collections;
using UnityEngine;
public class ZappadasCharacterController : CharacterMaster
{
    [Header("--- ZAPPADAS ---")]

    [Header("PASSIVE SKILL")]
    public DarkPowerBuff m_PZappadasBuff;
    public float m_DarkPowerEvery = 1f;
    public int m_DarkPowerToAddWithTime = 1;
    public int m_MAXDarkPower = 100;
    int m_PDarkPower = 0;

    [Header("Q SKILL")]

    public float m_QChannelingTime = 1f; //Se puede borrar si no hace Canalizando la skill
    bool m_QChanneling = true; //Se puede borrar si no hace Canalizando la skill
    float m_QTimer = 0f; //Se puede borrar si no hace Canalizando la skill
    bool m_SaverCanDoQ = true;
    bool m_QSkillStarted = false;

    [Header("W SKILL")]

    public float m_WChannelingTime = 1f; //Se puede borrar si no hace Canalizando la skill
    bool m_WChanneling = true; //Se puede borrar si no hace Canalizando la skill
    float m_WTimer = 0f; //Se puede borrar si no hace Canalizando la skill
    bool m_SaverCanDoW = true;
    bool m_WSkillStarted = false;

    [Header("E SKILL")]

    public float m_EChannelingTime = 1f; //Se puede borrar si no hace Canalizando la skill
    bool m_EChanneling = true; //Se puede borrar si no hace Canalizando la skill
    float m_ETimer = 0f; //Se puede borrar si no hace Canalizando la skill
    bool m_SaverCanDoE = true;
    bool m_ESkillStarted = false;

    [Header("R SKILL")]

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
            //Aqui va el codigo para mostrar la UI --> EJ: m_SkillIndicatorUI.CreateArrowSkillIndicator(m_QSkill.m_IndicatorUIObject, m_UIIndicatorWidthQ, m_RangeQ, transform.position, true);
            SetShowingGizmos(true);
        }
        else
            StartCoroutine(StartQSkill());




    }

    IEnumerator StartQSkill()
    {

        StopAttacking();
        if (!GetIsLookingForPosition())
            StopMovement();

        GetCharacterUI().SetCastingUIAbilityText("Canalizando");
        GetCharacterUI().HideCastingTime();
        GetCharacterUI().UpdateCastingUI(0, 1);
        GetCharacterUI().ShowCastingUI();
        m_QTimer = 0f;
        m_QChanneling = true;


        SetAnimatorTrigger("IsUsingQ");

        SetDisabled(true);
        yield return new WaitForSeconds(m_QChannelingTime);
        SetDisabled(false);
        base.QSkill();
        GetCharacterUI().HideCastingUI();
        m_QChanneling = false;


        EndQSkill();
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

    }

    void EndQSkill()
    {
        m_QSkillStarted = false;
        m_QSkill.SetUsingSkill(false);
        base.QSkill();
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

        m_WSkillStarted = false;
        m_WSkill.SetUsingSkill(false);
        base.WSkill();
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
        m_ESkillStarted = false;
        m_ESkill.SetUsingSkill(false);
        base.ESkill();
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

