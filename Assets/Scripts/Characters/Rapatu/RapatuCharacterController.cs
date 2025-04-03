using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
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

    }


    //Q SKILL
    protected override void QSkill()
    {
    }

    //W SKILL
    protected override void WSkill()
    {

    }



    //E SKILL
    protected override void ESkill()
    {
        base.ESkill();
        m_ESkill.SetUsingSkill(true);
        SetAnimatorTrigger("IsUsingE");
        StartCoroutine(DisableForDuration(m_ESkill.m_SkillDisabledTime));
        StartCoroutine(ESkillCoroutine());

    }
    IEnumerator ESkillCoroutine()
    {
        RapatuEHealingArea l_HealingAreaE;
        yield return new WaitForSeconds(m_ESkill.m_SkillDisabledTime);
        l_HealingAreaE = Instantiate(m_HealingAreaEPrefab, transform.position, Quaternion.identity).GetComponent<RapatuEHealingArea>();
        l_HealingAreaE.SetHealingAria(transform, m_HealingAreaRadius / 100);
        m_ESkill.SetUsingSkill(false);


        float l_HealLifeValue = (m_ESkill.GetAttribute("Curación", GetESkillLevel())) + (m_PercentageSkillPower / 100) * GetCharacterStats().GetAbilityPower() + (m_PercentageAdditionalLife / 100) * GetCharacterStats().GetBonusHealth();

        l_HealingAreaE.Heal(l_HealLifeValue);
        for (int i = 0; i < m_HealingTimes - 1; i++)
        {
            yield return new WaitForSeconds(m_HealingTimeSpace);
            l_HealingAreaE.Heal(l_HealLifeValue);

        }

        Destroy(l_HealingAreaE.gameObject);
    }
    //R SKILL
    protected override void RSkill()
    {
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
