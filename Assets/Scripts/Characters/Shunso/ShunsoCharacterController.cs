using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class ShunsoCharacterController : CharacterMaster
{
    [Header("--- SHUNSO ---")]
    [Header("PASSIVE SKILL")]
	public MarkBuff m_PMarkDebuff;
	public GameObject m_PArea;
	public GameObject m_PManaParticle;
	List<GameObject> m_AffectedEnemies=new List<GameObject>(); 
	float m_ReduceCooldownTimer;

    [Header("Q SKILL")]
	public GameObject m_QProjectile;
	bool m_QGoRight;

    [Header("W SKILL")]
	public GameObject m_WProjectile;

    [Header("E SKILL")]
	public SpeedBuff m_ESpeedBuff;
	bool m_MaxEStacksBuffActive;
	[HideInInspector]
	public bool m_GainStacksCooldown;

    [Header("R SKILL")]
	public GameObject m_RProjectile;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
		float l_Size=m_PassiveSkill.GetAttribute("Radio")/100.0f*2.0f;
		m_PArea.GetComponent<RectTransform>().sizeDelta=new Vector2(l_Size, l_Size);
		m_PArea.SetActive(false);
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

		if(m_AffectedEnemies.Count>=3)
			ReduceSkillsCooldown();
		else
			m_ReduceCooldownTimer=0.0f;
    }

	//PASSIVE SKILL
	void AddPassiveDebuffMark(GameObject Enemy) 
	{
		TimedBuff l_Buff=m_PMarkDebuff.InitializeBuff(m_PassiveSkill.GetAttribute("Duracion"), m_PassiveSkill.GetAttribute("Intervalo"), Enemy);
		l_Buff.m_OnTick+=ManaDrain;
		l_Buff.m_OnEnd+=RemoveEnemyFromList;
		Enemy.GetComponent<BuffableEntity>().AddBuff(l_Buff);
		if(!m_AffectedEnemies.Contains(Enemy))
			m_AffectedEnemies.Add(Enemy);
		if(!m_PArea.activeSelf)
			m_PArea.SetActive(true);
	}
	void ManaDrain(GameObject Enemy) 
	{
		CharacterStats l_Enemy=Enemy.GetComponent<CharacterStats>();
		l_Enemy.SetCurrentManaRpc(l_Enemy.GetCurrentMana()-m_PassiveSkill.GetAttribute("Drenaje mana"));
		ShunsoPManaParticle l_ManaParticle=Instantiate(m_PManaParticle, Enemy.transform.position, m_PManaParticle.transform.rotation, null).GetComponent<ShunsoPManaParticle>();
		l_ManaParticle.SetStats(transform);
		l_ManaParticle.m_OnReachTarget+=ManaRegeneration;
	}
	void ManaRegeneration() 
	{
		float l_ManaRegen=m_PassiveSkill.GetAttribute("Drenaje mana");
		Collider[] l_HitColliders=Physics.OverlapSphere(transform.position, m_PassiveSkill.GetAttribute("Radio")/100.0f, m_AlliesLayerMask);
		foreach(Collider Entity in l_HitColliders)
		{
            if(Entity.TryGetComponent(out CharacterStats Ally))
		        Ally.SetCurrentManaRpc(m_CharacterStats.GetCurrentMana()+l_ManaRegen);
        }
		if(m_AffectedEnemies.Count<=0)
			m_PArea.SetActive(false);
	}
	void RemoveEnemyFromList(GameObject Enemy) 
	{
		m_AffectedEnemies.Remove(Enemy);
		if(m_AffectedEnemies.Count<=0)
			m_PArea.SetActive(false);
	}
	void ReduceSkillsCooldown() 
	{
		m_ReduceCooldownTimer+=Time.deltaTime;
		if(m_ReduceCooldownTimer>=m_PassiveSkill.GetAttribute("Intervalo")) 
		{
			float l_Cooldown=m_PassiveSkill.GetAttribute("Recuperacion enfriamiento");
			if(m_QSkill.GetIsOnCd())
				m_QSkill.SetTimer(m_QSkill.GetTimer()-l_Cooldown);
			if(m_WSkill.GetIsOnCd())
				m_WSkill.SetTimer(m_WSkill.GetTimer()-l_Cooldown);
			m_ReduceCooldownTimer=0.0f;
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
		ShunsoQProjectile l_ProjectileScript=l_Projectile.GetComponent<ShunsoQProjectile>();
		l_ProjectileScript.SetStats(this, m_QSkill.GetAttribute("Daño base", GetQSkillLevel()), m_QSkill.GetAttribute("Velocidad proyectil"), 
			m_QSkill.GetAttribute("Ancho proyectil"), m_QSkill.GetAttribute("Rango proyectil"), m_QSkill.GetAttribute("Velocidad arañazo"), m_QSkill.GetAttribute("Ancho arañazo"), 
			m_QSkill.GetAttribute("Rango arañazo"), l_Direction, m_QGoRight);
		l_ProjectileScript.m_EStacksOnHit+=GainEStacks;
		l_ProjectileScript.m_OnDamageEnemy+=AddPassiveDebuffMark;
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
			ShunsoWProjectile l_ProjectileScript=l_Projectile.GetComponent<ShunsoWProjectile>();
			l_ProjectileScript.SetStats(this, m_WSkill.GetAttribute("Puntos de Vida Corrupta", GetWSkillLevel()), m_WSkill.GetAttribute("Daño de Vida Corrupta", GetWSkillLevel()),
				m_WSkill.GetAttribute("Velocidad"), m_WSkill.GetAttribute("Ancho"), m_WSkill.GetAttribute("Rango"), l_Offset, l_Direction);
			l_ProjectileScript.m_EStacksOnHit+=GainEStacks;
			l_ProjectileScript.m_OnDamageEnemy+=AddPassiveDebuffMark;
			l_Offset+=m_WSkill.GetAttribute("Separacion");
		}
		base.WSkill();
	}
	public IEnumerator WAlreadyGainedEStacks() 
	{
		m_GainStacksCooldown=true;
		yield return new WaitForSeconds(0.2f);
		m_GainStacksCooldown=false;
	}

	//E SKILL
	protected override void ESkill()
	{
		BuffableEntity l_CharacterBuffs=GetComponent<BuffableEntity>();
		if(l_CharacterBuffs.GetBuffWithKey(m_ESpeedBuff)!=null) 
		{
			if(l_CharacterBuffs.GetBuffWithKey(m_ESpeedBuff).GetCurrentStacks()>=m_ESpeedBuff.m_MaxStacks) 
			{
				AddHealth(m_ESkill.GetAttribute("Curación", GetESkillLevel()) + GetCharacterStats().GetBonusAttackDamage()*0.8f);
				l_CharacterBuffs.GetBuffWithKey(m_ESpeedBuff).End();
				l_CharacterBuffs.RemoveBuff(l_CharacterBuffs.GetBuffWithKey(m_ESpeedBuff));
				base.ESkill();
			}
		}
	}
	void GainEStacks(int Stacks) 
	{
		if(GetESkillLevel()>0) 
		{
			TimedBuff l_Buff=m_ESpeedBuff.InitializeBuff(m_ESkill.GetAttribute("Duracion"), m_ESkill.GetAttribute("Velocidad"), gameObject);
			l_Buff.m_OnFullStacks+=OnMaxEStacks;
			for(int i=0; i<Stacks; ++i)
				GetComponent<BuffableEntity>().AddBuff(l_Buff);
		}
	}
	void OnMaxEStacks(bool Max) 
	{
		if(Max && !m_MaxEStacksBuffActive) 
		{
			m_MaxEStacksBuffActive=true;
			m_CharacterStats.SetBonusAttackDamage(m_CharacterStats.GetBonusAttackDamage()+m_ESkill.GetAttribute("Daño de ataque adicional"));
		}
		else if(!Max)
		{
			m_MaxEStacksBuffActive=false;
			m_CharacterStats.SetBonusAttackDamage(m_CharacterStats.GetBonusAttackDamage()-m_ESkill.GetAttribute("Daño de ataque adicional"));
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
		ShunsoRProjectile l_ProjectileScript=l_Projectile.GetComponent<ShunsoRProjectile>();
		l_ProjectileScript.SetStats(this, m_RSkill.GetAttribute("Daño base", GetRSkillLevel()), m_RSkill.GetAttribute("Velocidad"), m_RSkill.GetAttribute("Radio mordisco"), 
			l_TargetPos);
		l_ProjectileScript.m_EStacksOnHit+=GainEStacks;
		l_ProjectileScript.m_OnDamageEnemy+=AddPassiveDebuffMark;
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
		ShunsoAA l_ProjectileScript=l_Projectile.GetComponent<ShunsoAA>();
        l_ProjectileScript.SetStats(m_DesiredEnemy, m_CharacterStats.GetAttackDamage(), 0.0f, this);
		l_ProjectileScript.m_EStacksOnHit+=GainEStacks;
		l_ProjectileScript.m_OnHitEffects+=AddPassiveDebuffMark;
		//l_Projectile.GetComponent<RangedAutoAttack>().m_OnHitEffects+=GainEStacks;
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
