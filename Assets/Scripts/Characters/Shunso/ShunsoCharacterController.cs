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
	public GameObject m_QProjectile;
	bool m_QGoRight;

    [Header("W SKILL")]
	public GameObject m_WProjectile;

    [Header("E SKILL")]
	public float m_MinExtraDamage3;
	public SpeedBuff m_ESpeedBuff;

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

		if(GetUseSkillGizmos()) 
		{
			if(Input.GetMouseButtonDown(0))
			{
				if(m_QSkill.GetUsingSkill())
					StartCoroutine(QCastProjectile());
				if(m_RSkill.GetUsingSkill())
					StartCoroutine(RCastProjectile());
			}
		}
    }

	//Q SKILL
	protected override void QSkill()
	{
		StopSkills();
		m_QSkill.SetUsingSkill(true);
		if(GetUseSkillGizmos())
		{
			if(GetShowingGizmos())
			{
				m_SkillIndicatorUI.ClearDeletableSkillIndicatorUI();
				m_SkillIndicatorUI.ClearNormalSkillIndicatorUI();
				m_SkillIndicatorUI.ClearTargetSkillIndicatorUI();
			}
			m_SkillIndicatorUI.CreateArrowSkillIndicator(m_QSkill.m_IndicatorUIObject, m_QSkill.GetAttribute("Ancho proyectil"), m_QSkill.GetAttribute("Rango proyectil"), transform.position, true);
			SetShowingGizmos(true);
		}
		else
			StartCoroutine(QCastProjectile());
	}
	IEnumerator QCastProjectile() 
	{
		StartCoroutine(DisableForDuration(m_QSkill.m_SkillDisabledTime));
		SetAnimatorTrigger("IsUsingQ");
		GetCharacterUI().SetCastingUIAbilityText(m_QSkill.m_PowerName);
		GetCharacterUI().HideCastingTime();
		GetCharacterUI().ShowCastingUI();
		SetShowingGizmos(false);
		m_QSkill.SetUsingSkill(false);

		Vector3 l_TargetPos=GetPositionWithMouse();
		Vector3 l_Direction=l_TargetPos-transform.position;
		l_Direction.y=0.0f;
		l_Direction.Normalize();
		transform.forward=l_Direction;
		float l_Timer=m_QSkill.m_SkillDisabledTime;
		while(l_Timer>0.0f)
		{
			GetCharacterUI().UpdateCastingUI(l_Timer, m_QSkill.m_SkillDisabledTime);
			l_Timer-=Time.deltaTime;
			yield return null;
		}
		GetCharacterUI().HideCastingUI();
		GameObject l_Projectile=Instantiate(m_QProjectile, transform.position, m_QProjectile.transform.rotation);
		float l_Width=m_QSkill.GetAttribute("Ancho proyectil")/100.0f;
		l_Projectile.transform.localScale=Vector3.one*l_Width;
		l_Projectile.transform.position+=Vector3.up*l_Width+l_Direction*l_Width;
		NetworkObject l_ProjectileNetwork=l_Projectile.GetComponent<NetworkObject>();
		l_ProjectileNetwork.SpawnWithOwnership(GetComponent<NetworkObject>().OwnerClientId);
		l_Projectile.GetComponent<ShunsoQProjectile>().SetStats(this, m_QSkill.GetAttribute("Daño base", GetQSkillLevel()), m_QSkill.GetAttribute("Velocidad proyectil"), 
			m_QSkill.GetAttribute("Ancho proyectil"), m_QSkill.GetAttribute("Rango proyectil"), m_QSkill.GetAttribute("Velocidad arañazo"), m_QSkill.GetAttribute("Ancho arañazo"), 
			m_QSkill.GetAttribute("Rango arañazo"), l_Direction, m_QGoRight);
		m_QGoRight=!m_QGoRight;
		base.QSkill();
	}

	//W SKILL
	protected override void WSkill()
	{
		StartCoroutine(DisableForDuration(m_WSkill.m_SkillDisabledTime));
		SetAnimatorTrigger("IsUsingW");
		GetCharacterUI().SetCastingUIAbilityText(m_WSkill.m_PowerName);
		GetCharacterUI().HideCastingTime();
		GetCharacterUI().ShowCastingUI();
		StartCoroutine(WCastProjectile());
	}
	IEnumerator WCastProjectile() 
	{
		Vector3 l_TargetPos=GetPositionWithMouse();
		Vector3 l_Direction=l_TargetPos-transform.position;
		l_Direction.y=0.0f;
		l_Direction.Normalize();
		transform.forward=l_Direction;
		float l_Timer=m_WSkill.m_SkillDisabledTime;
		while(l_Timer>0.0f)
		{
			GetCharacterUI().UpdateCastingUI(l_Timer, m_WSkill.m_SkillDisabledTime);
			l_Timer-=Time.deltaTime;
			yield return null;
		}
		GetCharacterUI().HideCastingUI();
		float l_Offset=-m_WSkill.GetAttribute("Separacion")*2.0f;
		for(int i=0; i<5; ++i) 
		{
			GameObject l_Projectile=Instantiate(m_WProjectile, transform.position, m_WProjectile.transform.rotation);
			NetworkObject l_ProjectileNetwork=l_Projectile.GetComponent<NetworkObject>();
			l_ProjectileNetwork.SpawnWithOwnership(GetComponent<NetworkObject>().OwnerClientId);
			l_Projectile.GetComponent<ShunsoWProjectile>().SetStats(this, m_WSkill.GetAttribute("Puntos de Vida Corrupta", GetQSkillLevel()), m_WSkill.GetAttribute("Velocidad"), 
				m_WSkill.GetAttribute("Ancho"), m_WSkill.GetAttribute("Rango"), l_Offset, l_Direction);
			l_Offset+=m_WSkill.GetAttribute("Separacion");
		}
		base.WSkill();
	}

	//E SKILL
	protected override void ESkill()
	{
		BuffableEntity l_CharacterBuffs=GetComponent<BuffableEntity>();
		if(l_CharacterBuffs.GetBuffWithName(m_ESpeedBuff.m_BuffName)!=null) 
		{
			if(l_CharacterBuffs.GetBuffWithName(m_ESpeedBuff.m_BuffName).GetCurrentStacks()>=10) 
			{
				AddHealth(m_ESkill.GetAttribute("Curación", GetESkillLevel()) + GetCharacterStats().GetBonusAttackDamage()*0.8f);
				l_CharacterBuffs.RemoveBuff(l_CharacterBuffs.GetBuffWithName(m_ESpeedBuff.m_BuffName));
				base.ESkill();
			}
		}
	}
	void GainEStacks() 
	{
		if(GetESkillLevel()>0) 
		{
			GetComponent<BuffableEntity>().AddBuff(m_ESpeedBuff.InitializeBuff(m_ESkill.GetAttribute("Duracion"), m_ESkill.GetAttribute("Velocidad"), gameObject));
		}
	}

	//R SKILL
	protected override void RSkill()
	{
		StopSkills();
		m_RSkill.SetUsingSkill(true);
		if(GetUseSkillGizmos())
		{
			if(GetShowingGizmos())
			{
				m_SkillIndicatorUI.ClearDeletableSkillIndicatorUI();
				m_SkillIndicatorUI.ClearNormalSkillIndicatorUI();
				m_SkillIndicatorUI.ClearTargetSkillIndicatorUI();
			}
			m_SkillIndicatorUI.CreateCircleSkillIndicator(m_RSkill.m_IndicatorUIObject, m_RSkill.GetAttribute("Rango")/2.0f, transform, true);
			m_SkillIndicatorUI.CreateCircleSkillIndicator(m_RSkill.m_IndicatorUIObject, m_RSkill.GetAttribute("Radio mordisco"), true);
			SetShowingGizmos(true);
		}
		else
			StartCoroutine(RCastProjectile());
	}
	IEnumerator RCastProjectile() 
	{
		StartCoroutine(DisableForDuration(m_RSkill.m_SkillDisabledTime));
		SetAnimatorTrigger("IsUsingR");
		GetCharacterUI().SetCastingUIAbilityText(m_RSkill.m_PowerName);
		GetCharacterUI().HideCastingTime();
		GetCharacterUI().ShowCastingUI();
		SetShowingGizmos(false);
		m_RSkill.SetUsingSkill(false);

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

    protected override void PerformRangedAutoAttack()
    {
        if (m_DesiredEnemy == null)
            return;
#if UNITY_EDITOR
        Debug.Log("ATTACKING - Since last auto: " + m_TimeSinceLastAuto);
        m_TimeSinceLastAuto = 0.0f;
#endif
        GameObject l_Projectile = Instantiate(m_RangedAutoAttack, m_RangedAutoSpawnPoint.position, transform.rotation);
        NetworkObject l_ProjectileNetwork = l_Projectile.GetComponent<NetworkObject>();
        l_ProjectileNetwork.SpawnWithOwnership(GetComponent<NetworkObject>().OwnerClientId);
        l_Projectile.GetComponent<RangedAutoAttack>().SetStats(m_DesiredEnemy, m_CharacterStats.GetAttackDamage(), 0.0f, this);
		l_Projectile.GetComponent<RangedAutoAttack>().m_OnHitEffects+=GainEStacks;
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
