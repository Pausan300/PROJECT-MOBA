using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using Random = UnityEngine.Random;

public class WilldurrCharacterController : CharacterMaster
{
    [Header("--- WILLDURR ---")]

    //https://wiki.leagueoflegends.com/en-us/Vamp

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
    public GameObject m_RZonePrefab;
    public GameObject m_RHeadPrefab;
    public float m_MINRangeR = 800.0f;
    public float m_ScaleRZoneSpeed = 1000.0f;
    public float m_RSkillDuration = 15f;
    public float m_RChannelingTime = 0.2f;
    [UnityEngine.Range(50f, 300f)]
    public float m_RMINDistanceHeads = 100f;
    public float m_HeadRRadiusHitbox = 100f;
    public float m_CoolingEInRSkill = 1.5f;
    public LayerMask m_RHeadLayerMask;


    GameObject m_RZone;
    List<float> m_RangeToSpawnRHeads;
    List<GameObject> m_RHeads;

    float m_ActualRangeR;
    float m_MAXRangeThisR;
    float m_RTimer = 0f;

    int m_ActualHeadToSpawn;
    bool m_RChanneling = false;
    bool m_ScalingRZone = false;
    bool m_RSkillStarted = false;
    bool m_SaverCanDoR = true;

    [Header("Fórmula del rango de valores")]
    [Tooltip("ValueA + (X * ValueB) TO ValueC + (X * ValueD) --> X es el número de cabeza (de 0 a depende del rango)")]
    public float m_FormulaValueA = 30f;
    [Tooltip("ValueA + (X * ValueB) TO ValueC + (X * ValueD) --> X es el número de cabeza (de 0 a depende del rango)")]
    public float m_FormulaValueB = 100f;
    [Tooltip("ValueA + (X * ValueB) TO ValueC + (X * ValueD) --> X es el número de cabeza (de 0 a depende del rango)")]
    public float m_FormulaValueC = 130f;
    [Tooltip("ValueA + (X * ValueB) TO ValueC + (X * ValueD) --> X es el número de cabeza (de 0 a depende del rango)")]
    public float m_FormulaValueD = 100f;

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
                if (m_RSkill.GetUsingSkill() && !m_RSkillStarted)
                    StartCoroutine(StartRSkill());
            }
        }

        UpdateESkill();
        UpdateRSkill();

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
        if (m_QSkill.GetUsingSkill() || m_WSkill.GetUsingSkill() || m_RChanneling || m_EChanneling)
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
        if (m_QSkill.GetUsingSkill() || m_WSkill.GetUsingSkill() || m_RChanneling || m_EChanneling)
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
        if (m_QSkill.GetUsingSkill() || m_WSkill.GetUsingSkill() || m_ESkill.GetUsingSkill() || m_RSkill.GetUsingSkill())
            return;

        if (!m_SaverCanDoR)
            return;

        m_RSkill.SetUsingSkill(true);

        m_MAXRangeThisR = m_MINRangeR;


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
        yield return new WaitForSeconds(m_EChannelingTime);
        SetDisabled(false);
        base.RSkill();
        GetCharacterUI().HideCastingUI();
        m_RChanneling = false;



        m_RZone = Instantiate(m_RZonePrefab, new Vector3(transform.position.x, transform.position.y + 0.05f, transform.position.z), Quaternion.identity);
        m_RZone.transform.localScale = Vector3.zero;
        m_ActualRangeR = 0;
        m_ScalingRZone = true;

        m_RangeToSpawnRHeads = new List<float>();
        m_RHeads = new List<GameObject>();

        for (int i = 0; i < (m_MAXRangeThisR / 100); i++)
        {
            m_RangeToSpawnRHeads.Add(GetRandomRangeToSpawn(i));
            Debug.Log("Range to spawn head " + i + 1 + ": " + m_RangeToSpawnRHeads[i]);
        }
        m_ActualHeadToSpawn = 0;

        yield return new WaitForSeconds(m_RSkillDuration);
        EndRSkill();

    }

    private float GetRandomRangeToSpawn(int i)
    {
        float l_Range = Random.Range(m_FormulaValueA + (i * m_FormulaValueB), m_FormulaValueC + (i * m_FormulaValueD));

        /*if (m_FormulaValueA + (1 * m_FormulaValueD) + m_FormulaValueA <= m_RMINDistanceHeads)
        {
            Debug.LogError("m_RMINDistanceHeads da StackOverflow porque le supone imposible cumplir con esta distancia. m_RMINDistanceHeads tiene que ser inferior de " + (m_FormulaValueC + (1 * m_FormulaValueD) + m_FormulaValueA) + " para que no de Error");
        }*/

        if (l_Range > m_MAXRangeThisR)
        {
            l_Range = GetRandomRangeToSpawn(i);
        }
        return l_Range;
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

            if (m_RangeToSpawnRHeads.Count > m_ActualHeadToSpawn && m_ActualRangeR >= m_RangeToSpawnRHeads[m_ActualHeadToSpawn])
            {
                float l_AnguloAleatorio = Random.Range(0f, 360f);
                float l_Radianes = l_AnguloAleatorio * Mathf.Deg2Rad;

                Vector3 l_HeadPos = m_RZone.transform.position + new Vector3(Mathf.Cos(l_Radianes), 0f, Mathf.Sin(l_Radianes)) * (m_RangeToSpawnRHeads[m_ActualHeadToSpawn] / 100);
                l_HeadPos.y = m_RZone.transform.position.y;
                m_RHeads.Add(Instantiate(m_RHeadPrefab, GetHeadPosRandom(m_RangeToSpawnRHeads[m_ActualHeadToSpawn]), Quaternion.identity));
                m_RHeads[m_ActualHeadToSpawn].GetComponent<WilldurrHeadR>().SetRadiusHitbox(m_HeadRRadiusHitbox);

                m_ActualHeadToSpawn++;
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
                        base.ESkill();
                    }
                }
            }

        }


    }

    private Vector3 GetHeadPosRandom(float _Range)
    {

        float l_Result = (m_FormulaValueA + ((m_ActualHeadToSpawn) * m_FormulaValueB) + m_FormulaValueA + ((m_ActualHeadToSpawn - 1) * m_FormulaValueB));
        l_Result -= 30;

        float l_AnguloAleatorio = Random.Range(0f, 360f);
        float l_Radianes = l_AnguloAleatorio * Mathf.Deg2Rad;

        Vector3 l_HeadPos = m_RZone.transform.position + new Vector3(Mathf.Cos(l_Radianes), 0f, Mathf.Sin(l_Radianes)) * (_Range / 100);
        l_HeadPos.y = m_RZone.transform.position.y;
        bool l_CanBePos = true;

        if (m_RHeads.Count > 0)
        {
            foreach (GameObject HeadPos in m_RHeads)
            {
                if (m_FormulaValueA + (m_ActualHeadToSpawn * m_FormulaValueB) + m_FormulaValueA + ((m_ActualHeadToSpawn - 1) * m_FormulaValueB) > m_RMINDistanceHeads)
                {
                    if (Vector3.Distance(l_HeadPos, HeadPos.transform.position) < m_RMINDistanceHeads / 100)
                    {
                        l_CanBePos = false;

                    }
                }
                else
                {
                    if (Vector3.Distance(l_HeadPos, HeadPos.transform.position) < l_Result / 100)
                    {
                        l_CanBePos = false;
                    }
                }
            }
            if (!l_CanBePos)
            {

                l_HeadPos = GetHeadPosRandom(_Range);
            }

        }
        return l_HeadPos;
    }

    void EndRSkill()
    {
        m_RSkill.SetUsingSkill(false);
        m_RSkillStarted = false;

        StartCoroutine(RepeatRSaver());
        Destroy(m_RZone);
        m_RZone = null;
        foreach (GameObject Head in m_RHeads)
        {
            Destroy(Head);
        }
        m_RHeads = null;
        m_RangeToSpawnRHeads = null;

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
