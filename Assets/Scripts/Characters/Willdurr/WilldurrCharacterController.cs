using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WilldurrCharacterController : CharacterMaster
{
    [Header("--- WILLDURR ---")]



    [Header("PASSIVE SKILL")]

    [Header("Q SKILL")]

    bool m_SaverCanDoQ = true;

    [Header("W SKILL")]

    bool m_SaverCanDoW = true;

    [Header("E SKILL")]
    public FearBuff m_FearBuffE;
    public float m_FearBuffTimeE = 1.25f;
    public Transform m_Head;
    public GameObject m_FearZoneE;
    public float m_UIIndicatorWidthE = 50;
    public float m_RangeE = 700;
    public float m_SpeedHeatE = 600;
    public float m_FearRadioE = 250;


    public float m_EChannelingTime = 0.15f;
    public float m_WaitReactivationTimeE = 0.75f;
    public float m_CanReactiveTimeE = 2.5f;
    public float m_ReturnHeadTimeE = 2f;



    Vector3 m_TargetHeadPosition;
    bool m_EChanneling = false;
    bool m_HeadCanMove = false;
    float m_ETimer = 0f;
    bool m_CanReactiveE = false;
    bool m_SaverCanDoE = true;
    bool m_ESkillStarted;
    Coroutine m_ECoroutine;

    [Header("R SKILL")]

    bool m_SaverCanDoR = true;


    [Header("OTHER VALUES")]
    public Transform m_HeadPos;
    public float m_HeatReturnSpeed = 600;
    bool m_ReturnHead = false;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        m_FearZoneE.SetActive(false);
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
            }
        }

        UpdateESkill();

        if (m_ReturnHead)
        {
            m_Head.position = Vector3.MoveTowards(m_Head.position, m_HeadPos.position, (m_HeatReturnSpeed / 100) * Time.deltaTime);
            if (Vector3.Distance(m_Head.position, m_HeadPos.position) <= 0.01f)
            {
                m_ReturnHead = false;
                m_Head.SetParent(m_HeadPos);
                m_Head.localRotation = Quaternion.identity;
                m_Head.localPosition = Vector3.zero;
            }
        }
    }

    #region Q Skill
    //Q SKILL
    protected override void QSkill()
    {
        if (m_QSkill.GetUsingSkill() || m_WSkill.GetUsingSkill() || m_ESkill.GetUsingSkill() || m_RSkill.GetUsingSkill())
            return;

        if (!m_SaverCanDoQ)
            return;

        EndQSkill();


    }
    void EndQSkill()
    {

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
            return;

        if (!m_SaverCanDoW)
            return;

        EndWSkill();

    }
    void EndWSkill()
    {

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
        LookAt(m_TargetHeadPosition);

        StopAttacking();
        if (!GetIsLookingForPosition())
            StopMovement();

        GetCharacterUI().SetCastingUIAbilityText("Tirando cabeza");
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
        StartCoroutine(ReturnHeadE());

        yield return new WaitForSeconds(m_WaitReactivationTimeE);
        m_CanReactiveE = true;

        yield return new WaitForSeconds(m_CanReactiveTimeE);
        m_CanReactiveE = false;
        m_ReturnHead = true;


        EndESkill();
    }
    IEnumerator ReturnHeadE()
    {
        yield return new WaitForSeconds(m_ReturnHeadTimeE);
        m_ReturnHead = true;
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
                    Buffs.AddBuff(m_FearBuffE.InitializeBuff(m_FearBuffTimeE, Entity.gameObject));
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
                StopAttacking();

                if (!GetIsLookingForPosition())
                    StopMovement();

                EndESkill();
            }
        }

        if (m_HeadCanMove)
        {
            m_Head.position = Vector3.MoveTowards(m_Head.position, m_TargetHeadPosition, (m_SpeedHeatE / 100) * Time.deltaTime);
            if (Vector3.Distance(m_Head.position, m_TargetHeadPosition) <= 0.1f)
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
        if (m_QSkill.GetUsingSkill() || m_WSkill.GetUsingSkill() || m_ESkill.GetUsingSkill() || m_RSkill.GetUsingSkill())
            return;

        if (!m_SaverCanDoR)
            return;

        EndRSkill();
    }
    void EndRSkill()
    {
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

