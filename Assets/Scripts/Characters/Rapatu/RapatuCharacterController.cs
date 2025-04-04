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

    [Header("R SKILL")]
    public float m_RangeR = 1000;
    public float m_AutoJumpSecondsR = 2;

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

        if (m_RSkill.GetUsingSkill())
        {
            RHoldTimeCheck();
        }
        if (m_ESkill.GetUsingSkill() && m_CanStopE)
        {
            if (Input.GetKeyDown(m_ESkillKey))
            {
                StartCoroutine(StopESkill());
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

        base.ESkill();
        m_ESkill.SetUsingSkill(true);
        SetAnimatorTrigger("IsUsingE");
        SetDisabled(true);
        m_CanStopE = false;
        if (!GetIsLookingForPosition())
            StopMovement();
        StartCoroutine(ESkillCoroutine());

    }
    IEnumerator StopESkill()
    {
        SetAnimatorTrigger("EStop");
        if (!GetIsLookingForPosition())
            StopMovement();
        SetDisabled(false);
        StopCoroutine(ESkillCoroutine());
        Destroy(m_HealingAreaE.gameObject);
        yield return new WaitForSeconds(0.1f);
        m_CanStopE = false;
        m_ESkill.SetUsingSkill(false);
    }
    RapatuEHealingArea m_HealingAreaE;
    bool m_CanStopE = false;
    public float m_AmimDelayE = 0.5f;
    IEnumerator ESkillCoroutine()
    {
        yield return new WaitForSeconds(m_AmimDelayE);
        m_HealingAreaE = Instantiate(m_HealingAreaEPrefab, transform.position, Quaternion.identity).GetComponent<RapatuEHealingArea>();
        m_HealingAreaE.SetHealingAria(transform, m_HealingAreaRadius / 100);
        m_CanStopE = true;


        float l_HealLifeValue = (m_ESkill.GetAttribute("Curación", GetESkillLevel())) + (m_PercentageSkillPower / 100) * GetCharacterStats().GetAbilityPower() + (m_PercentageAdditionalLife / 100) * GetCharacterStats().GetBonusHealth();

        m_HealingAreaE.Heal(l_HealLifeValue);
        for (int i = 0; i < m_HealingTimes - 1; i++)
        {
            yield return new WaitForSeconds(m_HealingTimeSpace);
            if (!m_HealingAreaE.IsDestroyed())
                m_HealingAreaE.Heal(l_HealLifeValue);

        }

        if (!m_HealingAreaE.IsDestroyed())
            StartCoroutine(StopESkill());
    }
    //R SKILL
    protected override void RSkill()
    {
        if (m_RSkill.GetUsingSkill())
            return;
        if (m_ESkill.GetUsingSkill())
            return;

        m_RSkill.SetUsingSkill(true);
        SetDisabled(true);
        SetAnimatorTrigger("IsUsingR");
        if (!GetIsLookingForPosition())
            StopMovement();
        m_TimerR = 0;
    }
    float m_TimerR;
    public float m_MinTimeToBeRedy = 1f;
    bool m_Jumping = false;
    void RHoldTimeCheck()
    {
        if (!m_Jumping)
        {
            if (Input.GetKey(KeyCode.R))
            {
                m_TimerR += Time.deltaTime;
                Debug.LogError("Cargando...");
                if (m_TimerR >= m_AutoJumpSecondsR)
                {
                    StartJumpingR();
                }
            }
            else if (Input.GetKeyUp(KeyCode.R) && m_TimerR >= m_MinTimeToBeRedy)
            {
                Debug.LogError("R Up");
                StartJumpingR();
            }
            else
            {
                Debug.LogError("R Jump because you Up R");
                StartJumpingR();
            }
        }
    }
    void StartJumpingR()
    {
        base.RSkill();
        m_Jumping = true;
        SetAnimatorTrigger("RJump");
        JumpR();
    }
    public void EndJump()
    {
        Debug.LogError("Floor");
        EndRSkill();

    }

    void JumpR()
    {
        Debug.LogError("Jumping");
    }
    void EndRSkill()
    {
        SetAnimatorTrigger("RJumpStop");
        m_RSkill.SetUsingSkill(false);
        SetDisabled(false);
        m_Jumping = false;
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
