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
	public GameObject m_RProjectile;

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
		SetAnimatorTrigger("IsUsingR");
		GetCharacterUI().SetCastingUIAbilityText(m_RSkill.m_PowerName);
		GetCharacterUI().HideCastingTime();
		GetCharacterUI().ShowCastingUI();
		StartCoroutine(RCastProjectile());
	}
	IEnumerator RCastProjectile() 
	{
		Vector3 l_TargetPos=GetPositionWithMouse();
		Vector3 l_Direction=l_TargetPos-transform.position;
		l_Direction.y=0.0f;
		l_Direction.Normalize();
		transform.forward=l_Direction;
		float l_Timer=m_RSkill.m_SkillDisabledTime;
		while(l_Timer>0.0f)
		{
			GetCharacterUI().UpdateCastingUI(l_Timer, m_RSkill.m_SkillDisabledTime);
			l_Timer-=Time.deltaTime;
			yield return null;
		}
		GetCharacterUI().HideCastingUI();
		GameObject l_Projectile=Instantiate(m_RProjectile, transform.position, m_RProjectile.transform.rotation);
		float l_Radius=m_RSkill.GetAttribute("Radio proyectil")/100.0f;
		l_Projectile.transform.localScale=Vector3.one*(l_Radius*2.0f);
		l_Projectile.transform.position+=Vector3.up*l_Radius+l_Direction*(l_Radius*2.0f);
		NetworkObject l_ProjectileNetwork=l_Projectile.GetComponent<NetworkObject>();
		l_ProjectileNetwork.SpawnWithOwnership(GetComponent<NetworkObject>().OwnerClientId);
		l_Projectile.GetComponent<ShunsoRProjectile>().SetStats(this, m_RSkill.GetAttribute("Daño base", GetRSkillLevel()), m_RSkill.GetAttribute("Velocidad"), m_RSkill.GetAttribute("Radio mordisco"), 
			l_TargetPos);
		base.RSkill();
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
