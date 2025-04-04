using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ShunsoCharacterController : CharacterMaster
{
    [Header("--- SHUNSO ---")]
    [Header("PASSIVE SKILL")]
	public float m_MinExtraDamage;

    [Header("Q SKILL")]
	public float m_MinExtraDamage1;

    [Header("W SKILL")]
	public float m_MinExtraDamage2;

    [Header("E SKILL")]
	public float m_MinExtraDamage3;

    [Header("R SKILL")]
	public float m_MinExtraDamage4;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }
    protected override void Update()
    {
		if(!IsSpawned||!HasAuthority)
        {
            return;
        }

        base.Update();
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
		
	}

	//R SKILL
	protected override void RSkill()
	{
		StartCoroutine(DisableForDuration(m_RSkill.m_SkillDisabledTime));
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
