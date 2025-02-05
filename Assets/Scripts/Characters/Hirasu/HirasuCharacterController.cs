using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HirasuCharacterController : CharacterMaster
{
    [Header("--- HIRASU ---")]
    [Header("PASSIVE SKILL")]
	public float m_MinExtraDamage;
	public float m_MaxExtraDamage;
	float m_PassiveExtraDamage;
	public float m_PassiveRange;
	public GameObject m_Staff;
	public GameObject m_StaffFlames;
	public GameObject m_StaffFlames2;
	public float m_MaxSecondAttackTime;
	bool m_SecondAttack;
	float m_SecondAttackTimer;

    [Header("Q SKILL")]
	public float[] m_QDamagePerLevel;
	public float m_QAdditionalDamage;
	public GameObject m_QProjectile;
	public GameObject m_QSplinter;
	public float m_QMaxHoldTime;
	public float m_QMinHoldTime;
	public float m_QDuration;
	public float m_QTapRange;
	public float m_QTapWidth;
	public float m_QHoldRange;
	public float m_QHoldWidth;
	public float m_QSplintersDuration;
	List<HirasuQSplinter> m_ActiveSplinters=new List<HirasuQSplinter>();
	float m_QCurrentHoldTime;

    [Header("W SKILL")]
	public float[] m_WDamagePerLevel;
	public float m_WAdditionalDamageSplinter;
	public float m_WAdditionalDamageExplosion;
	public float m_WExplosionRadius;
	public float m_WTimeToExpand;
	public SpeedBuff m_WSpeedDebuff;
	public float m_WSpeedDebuffAmountSplinter;
	public float m_WSpeedDebuffAmountExplosion;
	public float m_WSpeedDebuffDuration;
	public MarkBuff m_WMarksDebuff;
	public float m_WMarksDuration;
	public float m_WMarksExtraDamage;

    [Header("E SKILL")]
	public float m_ERange;
	public float m_EDuration;
	public SpeedBuff m_EBuff;
	public float[] m_EBuffDuration;
	public float[] m_EBuffSpeed;

    [Header("R SKILL")]
	public float[] m_RDamagePerLevel;
	public float m_RAdditionalDamage;
	public GameObject m_RExplosion;
	public Material m_RExplosionBlueMaterial;
	public float m_RTimeToExpand;
	public float[] m_RExplosionRadius;
	public float m_RSecondExplosionDelay;
	public AudioClip m_RSound;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

		m_Staff.SetActive(false);
		m_StaffFlames.SetActive(false);
		m_StaffFlames2.SetActive(false);
    }
    protected override void Update()
    {
		if(!IsSpawned||!HasAuthority)
        {
            return;
        }

        base.Update();
		
		UpdateHirasuStaffRpc();

		if(m_SecondAttackTimer>0.0f)
		{
			m_SecondAttackTimer-=Time.deltaTime;
			if(m_SecondAttackTimer<=0.0f)
				m_SecondAttack=false;
		}

		if(m_QSkill.GetUsingSkill())
		{
			QHoldTimeCheck();
		}
		if(m_RSkill.GetUsingSkill())
		{
			if(GetIsAttacking())
			{
				StartCoroutine(RAttack());
				m_QSkill.SetUsingSkill(false);
			}
		}

		if(GetUseSkillGizmos()) 
		{
			if(m_ESkill.GetUsingSkill())
				m_SkillIndicatorUI.ChangeArrowSkillIndicatorSize(100.0f, Mathf.Min(m_ERange, Vector3.Distance(GetPositionWithMouse(), transform.position)*100.0f));

			if(Input.GetMouseButtonDown(0))
			{
				if(m_WSkill.GetUsingSkill())
					WTriggerExplosions();
				if(m_ESkill.GetUsingSkill())
					StartCoroutine(EDash(GetPositionWithMouse()));
				if(m_RSkill.GetUsingSkill())
					GetEnemyWithMouse();
			}
		}
    }
	[Rpc(SendTo.Everyone)]
	void UpdateHirasuStaffRpc() 
	{
		if(GetRSkillLevel()<=0 && m_Staff.activeSelf) 
		{
			m_Staff.SetActive(false);
			m_StaffFlames.SetActive(false);
			m_StaffFlames2.SetActive(false);
		}
		else if(GetRSkillLevel()>=1 && !m_Staff.activeSelf) 
		{
			m_Staff.SetActive(true);
			m_CharacterStats.SetAttackRange(m_PassiveRange);
		}
		else if(GetRSkillLevel()>=2 && !m_StaffFlames.activeSelf)
			m_StaffFlames.SetActive(true);
		else if(GetRSkillLevel()>=3 && !m_StaffFlames2.activeSelf)
			m_StaffFlames2.SetActive(true);
	}

	//Q SKILL
	protected override void QSkill()
	{
		if(GetShowingGizmos())
		{
			m_SkillIndicatorUI.ClearDeletableSkillIndicatorUI();
			m_SkillIndicatorUI.ClearNormalSkillIndicatorUI();
			m_SkillIndicatorUI.ClearTargetSkillIndicatorUI();
		}
		m_SkillIndicatorUI.CreateArrowSkillIndicator(m_QSkill.m_IndicatorUIObject, m_QTapWidth, m_QTapRange, transform.position, false);
		
		StopAttacking();
		GetCharacterUI().SetCastingUIAbilityText("Marcaje");
		GetCharacterUI().HideCastingTime();
		GetCharacterUI().ShowCastingUI();
		if(m_RSkill.GetUsingSkill())
			m_RSkill.SetUsingSkill(false);
		StopSkills();
		m_QSkill.SetUsingSkill(true);
		m_QCurrentHoldTime=0.0f;
	}
	void QHoldTimeCheck()
	{
		if(Input.GetKey(KeyCode.Q) && m_QCurrentHoldTime<=m_QMaxHoldTime)
		{
			m_QCurrentHoldTime+=Time.deltaTime;
			GetCharacterUI().UpdateCastingUI(m_QCurrentHoldTime, m_QMaxHoldTime);
			
			if(m_QCurrentHoldTime>m_QMinHoldTime)
				m_SkillIndicatorUI.ChangeArrowSkillIndicatorSize(m_QTapWidth, m_QHoldRange);
		}
		else if(Input.GetKeyUp(KeyCode.Q) || m_QCurrentHoldTime>m_QMaxHoldTime)
		{
			Vector3 l_Direction=GetPositionWithMouse()-transform.position;
			l_Direction.y=0.0f;
			l_Direction.Normalize();
			transform.forward=l_Direction;
			float l_ExtraPhysDamage=0.0f;
			float l_ExtraMagicDamage=0.0f;
			bool l_ReducedCooldown=false;
			if(GetRSkillLevel()>=2)
			{
				l_ExtraPhysDamage=m_PassiveExtraDamage;
				l_ExtraMagicDamage=m_PassiveExtraDamage;
			}
			else if(GetRSkillLevel()>=1)
				l_ExtraPhysDamage=m_PassiveExtraDamage;

			if(m_QCurrentHoldTime<=m_QMinHoldTime)
			{
				SetAnimatorTrigger("IsUsingQTap");
				Vector3 l_DesiredPos=transform.position+l_Direction*(m_QTapRange/100.0f/2.0f);
				Collider[] l_HitColliders=Physics.OverlapBox(l_DesiredPos, new Vector3(m_QTapWidth/100.0f/2.0f, 2.0f, m_QTapRange/100.0f/2.0f), transform.rotation, m_DamageLayerMask);
				foreach(Collider Entity in l_HitColliders)
				{
					if(Entity.TryGetComponent(out ITakeDamage Enemy))
					{
						Enemy.TakeDamage(m_QDamagePerLevel[GetQSkillLevel()-1]+(m_QAdditionalDamage/100.0f*m_CharacterStats.GetBonusAttackDamage())+l_ExtraPhysDamage, l_ExtraMagicDamage);
						for(int i=0; i<2; i++)
						{
							SpawnSplinter(Entity.transform.position, transform.forward, Entity.transform, Enemy);
							if(GetQSkillLevel()<=1)
								break;
						}
						if(GetWSkillLevel()>0 && GetRSkillLevel()>0)
							AddBuffMarkRpc(Entity.GetComponent<NetworkObject>());
						break;
					}
				}
				l_ReducedCooldown=true;
				StartCoroutine(DisableForDuration(m_QSkill.m_SkillDisabledTime));
			}
			else if(m_QCurrentHoldTime<m_QMaxHoldTime)
			{
				SetAnimatorTrigger("IsUsingQHold");
				float l_Range=m_QHoldRange/100.0f;
				GameObject l_Projectile=Instantiate(m_QProjectile, transform.position, m_QProjectile.transform.rotation);
				NetworkObject l_ProjectileNetwork=l_Projectile.GetComponent<NetworkObject>();
				l_ProjectileNetwork.SpawnWithOwnership(GetComponent<NetworkObject>().OwnerClientId);
				SetProjectileStatsRpc(l_ProjectileNetwork, l_Range, l_Direction, l_ExtraPhysDamage, l_ExtraMagicDamage);
				StartCoroutine(DisableForDuration(m_QSkill.m_SkillDisabledTime));
			}
			base.QSkill();
			if(l_ReducedCooldown)
				m_QSkill.SetTimer(m_QSkill.GetCd()/2.0f);
			GetCharacterUI().HideCastingUI();
			m_SkillIndicatorUI.ClearNormalSkillIndicatorUI();
			SetShowingGizmos(false);
			m_QSkill.SetUsingSkill(false);
		}
	}
    [Rpc(SendTo.Everyone)]
    void SetProjectileStatsRpc(NetworkObjectReference Projectile, float Range, Vector3 Direction, float PhysDamage, float MagDamage) 
    {
        NetworkObject l_Projectile=Projectile;
		HirasuQProjectile l_ProjectileScript=l_Projectile.GetComponent<HirasuQProjectile>();
		l_ProjectileScript.SetStats(this, Range, m_QDuration, Direction, m_QDamagePerLevel[GetQSkillLevel()-1]+(m_QAdditionalDamage/100.0f*m_CharacterStats.GetBonusAttackDamage()), 
			PhysDamage, MagDamage, GetQSkillLevel(), m_QSplinter, m_QSplintersDuration, m_QHoldWidth);
    }
    void SpawnSplinter(Vector3 Position, Vector3 Forward, Transform Parent, ITakeDamage Enemy)
    {   
        GameObject l_Splinter=Instantiate(m_QSplinter, Position, m_QSplinter.transform.rotation, Parent);
        l_Splinter.transform.forward=Forward;
        l_Splinter.GetComponent<HirasuQSplinter>().SetStats(this, Enemy, m_QSplintersDuration);
		l_Splinter.GetComponent<NetworkObject>().SpawnWithOwnership(GetComponent<NetworkObject>().OwnerClientId);
    }
	public void AddSplinterToList(HirasuQSplinter Splinter)
	{
		m_ActiveSplinters.Add(Splinter);
	}
	public void DeleteSplintersFromList(HirasuQSplinter Splinter)
	{
		m_ActiveSplinters.Remove(Splinter);
	}

	//W SKILL
	protected override void WSkill()
	{
		if(!m_QSkill.GetUsingSkill())
		{
			if(m_ActiveSplinters.Count>0)
			{
				StopSkills();
				m_WSkill.SetUsingSkill(true);
				if(GetUseSkillGizmos()) 
				{
					if(GetShowingGizmos())
					{
						m_SkillIndicatorUI.ClearDeletableSkillIndicatorUI();
						m_SkillIndicatorUI.ClearNormalSkillIndicatorUI();
						m_SkillIndicatorUI.ClearTargetSkillIndicatorUI();
					}
					foreach(HirasuQSplinter Splinter in m_ActiveSplinters)
						m_SkillIndicatorUI.CreateCircleSkillIndicator(m_WSkill.m_IndicatorUIObject, m_WExplosionRadius, Splinter.transform, true);
					SetShowingGizmos(true);
				} 
				else
					WTriggerExplosions();
			}
		}
	}
	void WTriggerExplosions()
	{
		if(m_ActiveSplinters.Count>0)
		{
			StartCoroutine(DisableForDuration(m_WSkill.m_SkillDisabledTime));
			SetAnimatorTrigger("IsUsingW");
			
			SetShowingGizmos(false);
			m_WSkill.SetUsingSkill(false);

			foreach(HirasuQSplinter Splinter in m_ActiveSplinters)
			{
				if(!Splinter.GetAlreadyExploded())
				{
					StartCoroutine(Splinter.WSpawnExplosion(m_RExplosion, Splinter.transform.position, m_WExplosionRadius, m_WExplosionRadius*2.0f/100.0f, m_WTimeToExpand,
						m_WDamagePerLevel[GetWSkillLevel()-1]+(m_WAdditionalDamageSplinter/100.0f*m_CharacterStats.GetBonusAttackDamage()), 
						m_WDamagePerLevel[GetWSkillLevel()-1]+(m_WAdditionalDamageExplosion/100.0f*m_CharacterStats.GetBonusAttackDamage()), m_DamageLayerMask));
				}
			}
			base.WSkill();
		}
	}


	//E SKILL
	protected override void ESkill()
	{
		if(!m_QSkill.GetUsingSkill())
		{
			StopSkills();
			m_ESkill.SetUsingSkill(true);
			if(GetUseSkillGizmos()) 
			{
				if(GetShowingGizmos())
				{
					m_SkillIndicatorUI.ClearDeletableSkillIndicatorUI();
					m_SkillIndicatorUI.ClearNormalSkillIndicatorUI();
					m_SkillIndicatorUI.ClearTargetSkillIndicatorUI();
				}
				m_SkillIndicatorUI.CreateArrowSkillIndicator(m_ESkill.m_IndicatorUIObject, 100.0f, m_ERange, transform.position, true);
				SetShowingGizmos(true);
			}
			else
				StartCoroutine(EDash(GetPositionWithMouse()));
		}
	}
	IEnumerator EDash(Vector3 Position)
	{
		base.ESkill();
		SetShowingGizmos(false);
		m_ESkill.SetUsingSkill(false);
		StopRecall();
		StopAttacking();
		if(!GetIsLookingForPosition())
			StopMovement();
		StartCoroutine(DisableForDuration(m_EDuration));
        SetAnimatorTrigger("IsUsingE");
		Vector3 l_Direction=Position-transform.position;
		l_Direction.y=0.0f;
		l_Direction.Normalize();
		transform.forward=l_Direction;
		float l_Distance=Vector3.Distance(Position, transform.position);
		Vector3 l_StartPos=transform.position;
		float l_Range=(l_Distance*100.0f)<m_ERange ? l_Distance : m_ERange/100.0f;
		float l_Speed=l_Range/m_EDuration;
		var l_SqrMaxDistance=l_Range*l_Range;
		for(float timePassed=0.0f; timePassed<m_EDuration && (transform.position-l_StartPos).sqrMagnitude<l_SqrMaxDistance; timePassed+=Time.deltaTime)
		{
			transform.position+=l_Speed*l_Direction*Time.deltaTime;
			yield return null;
		}
		if(GetRSkillLevel()>=3)
		{
			GetComponent<BuffableEntity>().AddBuff(m_EBuff.InitializeBuff(m_EBuffDuration[1], m_EBuffSpeed[1], gameObject));
		}
		else if(GetRSkillLevel()>=2)
		{
			GetComponent<BuffableEntity>().AddBuff(m_EBuff.InitializeBuff(m_EBuffDuration[0], m_EBuffSpeed[1], gameObject));
		}
		else if(GetRSkillLevel()>=1)
		{
			GetComponent<BuffableEntity>().AddBuff(m_EBuff.InitializeBuff(m_EBuffDuration[0], m_EBuffSpeed[0], gameObject));
		}
	}

	//R SKILL
	protected override void RSkill()
	{
		if(!m_QSkill.GetUsingSkill())
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
				m_SkillIndicatorUI.CreateCircleSkillIndicator(m_RSkill.m_IndicatorUIObject, GetCharacterStats().GetAttackRange(), transform, false);
				SetShowingGizmos(true);
			}
			else
				GetEnemyWithMouse();
		}
	}
	IEnumerator RAttack()
	{
		base.RSkill();
		SetShowingGizmos(false);
		StartCoroutine(DisableForDuration(m_RSkill.m_SkillDisabledTime));
		SetAnimatorTrigger("IsUsingR");
		SetAnimatorBool("IsAAttacking", false);
		SetAnimatorBool("IsAAttacking2", false);
		GetAudioSource().clip=m_RSound;
		GetAudioSource().Play();
		float l_Damage=m_RDamagePerLevel[GetRSkillLevel()-1]+(m_RAdditionalDamage/100.0f*m_CharacterStats.GetBonusAttackDamage());
		Vector3 l_EnemyPos=m_DesiredEnemy.position;
		l_EnemyPos.y=0.0f;
		Vector3 l_DesiredPos=l_EnemyPos-transform.position;
		l_DesiredPos.Normalize();
		float l_ExplosionRadius=m_CharacterStats.GetCurrentLevel()>=11 ? m_RExplosionRadius[1]/100.0f : m_RExplosionRadius[0]/100.0f;
		float l_Scale=l_ExplosionRadius*2.0f;
		if(GetRSkillLevel()>=3)
			StartCoroutine(RSpawnExplosion(l_EnemyPos, l_DesiredPos, l_ExplosionRadius, l_Scale, l_Damage, true));
		else
			StartCoroutine(RSpawnExplosion(l_EnemyPos, l_DesiredPos, l_ExplosionRadius, l_Scale, l_Damage, false));
		m_RSkill.SetUsingSkill(false);
		SetIsAttacking(false);
		yield return new WaitForSeconds(0.5f);
	}
	IEnumerator RSpawnExplosion(Vector3 EnemyPos, Vector3 DesiredPos, float Radius, float Scale, float Damage, bool SecondExplosion)
	{
		GameObject l_Explosion=Instantiate(m_RExplosion, EnemyPos, m_RExplosion.transform.rotation);
		l_Explosion.GetComponent<NetworkObject>().SpawnWithOwnership(GetComponent<NetworkObject>().OwnerClientId);
		if(SecondExplosion)
			l_Explosion.GetComponent<MeshRenderer>().material=m_RExplosionBlueMaterial;
		Vector3 l_DesiredPos=DesiredPos*Radius;
		float l_Timer=0.0f;
		List<Collider> l_CollidersHit=new List<Collider>();
		while(l_Explosion.transform.localScale.x<Scale)
		{
			l_Explosion.transform.localScale=Vector3.Lerp(Vector3.zero, Vector3.one*Scale, l_Timer/m_RTimeToExpand);
			l_Explosion.transform.position=Vector3.Lerp(EnemyPos, EnemyPos+l_DesiredPos, l_Timer/m_RTimeToExpand);
			l_Timer+=Time.deltaTime;
			Collider[] l_HitColliders=Physics.OverlapSphere(l_Explosion.transform.position, l_Explosion.transform.localScale.x/2.0f, m_DamageLayerMask);
			foreach(Collider Entity in l_HitColliders)
			{
				if(!l_CollidersHit.Contains(Entity) && Entity.TryGetComponent(out ITakeDamage Enemy))
				{
					if(Entity.TryGetComponent(out BuffableEntity Buffs))
					{
						if(Buffs.IsMarkBuffActive(m_WMarksDebuff))
							Damage*=(1.0f+m_WMarksExtraDamage/100.0f);
					}
					Debug.Log("TAKEN "+Damage+" DAMAGE");
					Enemy.TakeDamage(Damage, 0.0f);
					l_CollidersHit.Add(Entity);
				}
			}
			yield return null;
		}
		Destroy(l_Explosion);
		if(SecondExplosion)
		{
			yield return new WaitForSeconds(m_RSecondExplosionDelay);
			StartCoroutine(RSpawnExplosion(EnemyPos, DesiredPos, Radius/2.0f, Scale/2.0f, Damage/2.0f, false));
		}
	}


	public override void LevelUpRpc()
	{
		base.LevelUpRpc();
		m_PassiveExtraDamage=(m_CharacterStats.GetCurrentLevel()-1)*(m_MaxExtraDamage-m_MinExtraDamage)/(18-1)+m_MinExtraDamage;
	}
	protected override void StartAttacking()
	{
        if(m_DesiredEnemy)
        {
            Vector3 l_Dir=m_DesiredEnemy.position-transform.position;
            l_Dir.y=0.0f;
            l_Dir.Normalize();
            transform.forward=l_Dir;
            SetIsAttacking(true);
			if(!m_RSkill.GetUsingSkill())
			{
				if(m_SecondAttack)
					SetAnimatorBool("IsAAttacking2", true);
				else
					SetAnimatorBool("IsAAttacking", true);
			}
        } 
	}
	protected override void StopAttacking()
	{
        if(GetIsAttacking())
        {
            SetIsAttacking(false);
            SetAnimatorBool("IsAAttacking", false);
            SetAnimatorBool("IsAAttacking2", false);
        }
	}
	IEnumerator DisableForDuration(float Duration)
	{
		SetDisabled(true);
		yield return new WaitForSeconds(Duration);
		SetDisabled(false);
	}
	protected override void PerformAutoAttack()
	{
		if(!IsSpawned||!HasAuthority)
        {
            return;
        }

#if UNITY_EDITOR
        Debug.Log("ATTACKING - Since last auto: "+m_TimeSinceLastAuto);
        m_TimeSinceLastAuto=0.0f;
#endif
		float l_ExtraPhysDamage=0.0f;
		float l_ExtraMagicDamage=0.0f;
		if(GetRSkillLevel()>=2)
		{
			l_ExtraPhysDamage=m_PassiveExtraDamage;
			l_ExtraMagicDamage=m_PassiveExtraDamage;
		}
		else if(GetRSkillLevel()>=1)
			l_ExtraPhysDamage=m_PassiveExtraDamage;

		if(GetRSkillLevel()>=1 && GetWSkillLevel()>0)
		{
			AddBuffMarkRpc(m_DesiredEnemy.GetComponent<NetworkObject>());
		}
		if(GetRSkillLevel()>=3)
		{
			if(!m_SecondAttack)
			{
				SetAnimatorBool("IsAAttacking", false);
				SetAnimatorBool("IsAAttacking2", true);
				m_SecondAttack=true;
				m_SecondAttackTimer=m_MaxSecondAttackTime;
			}
			else
			{
				SetAnimatorBool("IsAAttacking2", false);
				SetAnimatorBool("IsAAttacking", true);
				m_SecondAttack=false;
			}
		}
		m_DesiredEnemy.GetComponent<ITakeDamage>().TakeDamage(m_CharacterStats.GetAttackDamage()+l_ExtraPhysDamage, l_ExtraMagicDamage);
	}
    [Rpc(SendTo.Everyone)]
	void AddBuffMarkRpc(NetworkObjectReference Enemy) 
	{
		NetworkObject l_Enemy=Enemy;
		if(l_Enemy.TryGetComponent(out BuffableEntity Buffs))
			Buffs.AddBuff(m_WMarksDebuff.InitializeBuff(m_WMarksDuration, l_Enemy.gameObject));
	}
}
