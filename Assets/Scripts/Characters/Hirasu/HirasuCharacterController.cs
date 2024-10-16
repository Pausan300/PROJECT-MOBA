using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HirasuCharacterController : CharacterMaster
{
    [Header("--- HIRASU ---")]
    [Header("PASSIVE SKILL")]
	public float m_MinExtraDamage;
	public float m_MaxExtraDamage;
	float m_PassiveExtraDamage;
	public GameObject m_Staff;
	public GameObject m_StaffFlames;
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
	public float m_QSplintersDuration;
	List<HirasuQSplinter> m_ActiveSplinters=new List<HirasuQSplinter>();
	float m_QCurrentHoldTime;
	bool m_UsingQ;
    [Header("W SKILL")]
	public float[] m_WDamagePerLevel;
	public float m_WExplosionRadius;
	public float m_WTimeToExpand;
	public GameObject m_WMarks;
	public float m_WMarksExtraDamage;
	public float m_WMarksDuration;
    [Header("E SKILL")]
	public float m_ERange;
	public float m_EDuration;
    [Header("R SKILL")]
	public float[] m_RDamagePerLevel;
	public float m_RAdditionalDamage;
	public GameObject m_RExplosion;
	public Material m_RExplosionBlueMaterial;
	public float m_RTimeToExpand;
	public float[] m_RExplosionRadius;
	public float m_RSecondExplosionDelay;
	public AudioClip m_RSound;
	bool m_UsingR;

    protected override void Start()
    {
        base.Start();
		m_Staff.SetActive(false);
		m_StaffFlames.SetActive(false);
    }
    protected override void Update()
    {
        base.Update();
		
		if(m_RSkillLevel>=1 && !m_Staff.activeSelf)
			m_Staff.SetActive(true);
		if(m_RSkillLevel>=2 && !m_StaffFlames.activeSelf)
			m_StaffFlames.SetActive(true);

		if(m_SecondAttackTimer>0.0f)
		{
			m_SecondAttackTimer-=Time.deltaTime;
			if(m_SecondAttackTimer<=0.0f)
			{
				m_SecondAttack=false;
			}
		}
		if(m_UsingQ)
		{
			QHoldTimeCheck();
		}
		if(m_UsingR)
		{
			if(GetIsAttacking())
			{
				StartCoroutine(RAttack());
				m_UsingR=false;
			}
		}
    }

#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		if(m_UsingQ)
		{
			Gizmos.color=Color.red;
			Vector3 l_Direction=GetDirectionWithMouse();
			l_Direction.Normalize();
			Vector3 l_DesiredPos=transform.position+l_Direction*(m_QTapRange/100.0f/2.0f);
			Gizmos.matrix=Matrix4x4.TRS(l_DesiredPos, Quaternion.LookRotation(l_Direction, transform.up), new Vector3(m_QTapWidth/100.0f, 2.0f, m_QTapRange/100.0f));
			Gizmos.DrawCube(Vector3.zero, Vector3.one);
		}
	}
#endif

	protected override void QSkill()
	{
		m_QCurrentHoldTime=0.0f;
		StopAttacking();
		if(m_UsingR)
			m_UsingR=false;
		m_UsingQ=true;
		GetCharacterUI().SetCastingUIAbilityText("Marcaje");
		GetCharacterUI().HideCastingTime();
		GetCharacterUI().ShowCastingUI();
	}
	void QHoldTimeCheck()
	{
		if(Input.GetKey(KeyCode.Q) && m_QCurrentHoldTime<=m_QMaxHoldTime)
		{
			m_QCurrentHoldTime+=Time.deltaTime;
			GetCharacterUI().UpdateCastingUI(m_QCurrentHoldTime, m_QMaxHoldTime);
		}
		else if(Input.GetKeyUp(KeyCode.Q) || m_QCurrentHoldTime>m_QMaxHoldTime)
		{
			Vector3 l_Direction=GetDirectionWithMouse();
			l_Direction.Normalize();
			transform.forward=l_Direction;
			float l_ExtraPhysDamage=0.0f;
			float l_ExtraMagicDamage=0.0f;
			if(m_RSkillLevel>=2)
			{
				l_ExtraPhysDamage=m_PassiveExtraDamage;
				l_ExtraMagicDamage=m_PassiveExtraDamage;
			}
			else if(m_RSkillLevel>=1)
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
						Enemy.TakeDamage(m_QDamagePerLevel[m_QSkillLevel-1]+l_ExtraPhysDamage, l_ExtraMagicDamage);
						for(int i=0; i<2; i++)
						{
							SpawnSplinter(Entity.transform.position, transform.forward, Entity.transform);
							if(m_QSkillLevel<=1)
								break;
						}
						break;
					}
				}
			}
			else if(m_QCurrentHoldTime<m_QMaxHoldTime)
			{
				SetAnimatorTrigger("IsUsingQHold");
				float l_Range=m_QHoldRange/100.0f;
				GameObject l_Projectile=Instantiate(m_QProjectile, transform.position, m_QProjectile.transform.rotation);
				HirasuQProjectile l_ProjectileScript=l_Projectile.GetComponent<HirasuQProjectile>();
				l_ProjectileScript.SetStats(this, l_Range, m_QDuration, l_Direction, m_QDamagePerLevel[m_QSkillLevel-1]+(m_QAdditionalDamage/100.0f*GetBonusAttackDamage()), 
					l_ExtraPhysDamage, l_ExtraMagicDamage, m_QSkillLevel, m_QSplinter, m_QSplintersDuration);
			}
			StartCoroutine(DisableForDuration(0.5f));
			m_UsingQ=false;
			base.QSkill();
			GetCharacterUI().HideCastingUI();
		}
	}
    void SpawnSplinter(Vector3 Position, Vector3 Forward, Transform Parent)
    {   
        GameObject l_Splinter=Instantiate(m_QSplinter, Position, m_QSplinter.transform.rotation, Parent);
        l_Splinter.transform.forward=Forward;
        l_Splinter.GetComponent<HirasuQSplinter>().SetStats(this, m_QSplintersDuration);
    }
	public void AddSplinterToList(HirasuQSplinter Splinter)
	{
		m_ActiveSplinters.Add(Splinter);
	}
	public void DeleteSplintersFromList(HirasuQSplinter Splinter)
	{
		m_ActiveSplinters.Remove(Splinter);
	}

	protected override void WSkill()
	{
		if(!m_UsingQ)
		{
			if(m_ActiveSplinters.Count>0)
			{
				StartCoroutine(DisableForDuration(0.5f));
				SetAnimatorTrigger("IsUsingW");
				foreach(HirasuQSplinter Splinter in m_ActiveSplinters)
				{
					if(!Splinter.GetAlreadyExploded())
					{
						StartCoroutine(Splinter.WSpawnExplosion(m_RExplosion, Splinter.transform.position, m_WExplosionRadius, m_WExplosionRadius*2.0f/100.0f, m_WTimeToExpand, 
							m_WDamagePerLevel[m_WSkillLevel-1], m_DamageLayerMask));
					}
				}
				base.WSkill();
			}
		}
	}

	protected override void ESkill()
	{
		if(!m_UsingQ)
		{
			Vector3 l_Direction=GetDirectionWithMouse();
			l_Direction.Normalize();
			StartCoroutine(EDash(l_Direction));
		}
	}
	IEnumerator EDash(Vector3 Direction)
	{
		base.ESkill();
		SetDisabled(true);
		StopRecall();
		StopAttacking();
		if(!GetIsLookingForPosition())
			StopMovement();
        SetAnimatorTrigger("IsUsingE");
		transform.forward=Direction;
		Vector3 l_StartPos=transform.position;
		float l_Range=m_ERange/100.0f;
		float l_Speed=l_Range/m_EDuration;
		var l_SqrMaxDistance=l_Range*l_Range;
		for(float timePassed=0.0f; timePassed<m_EDuration && (transform.position-l_StartPos).sqrMagnitude<l_SqrMaxDistance; timePassed+=Time.deltaTime)
		{
			transform.position+=l_Speed*Direction*Time.deltaTime;
			yield return null;
		}
		if(m_CurrentLevel>=16)
		{
			SetMovementSpeedBonus(0.0f, 0.0f, new MovSpeedMultiplicative(30.0f, "HirasuESkill"));
			StartCoroutine(RemoveMovementSpeedBonus(3.0f, "HirasuESkill"));
		}
		else if(m_CurrentLevel>=11)
		{
			SetMovementSpeedBonus(0.0f, 0.0f, new MovSpeedMultiplicative(30.0f, "HirasuESkill"));
			StartCoroutine(RemoveMovementSpeedBonus(1.0f, "HirasuESkill"));
		}
		else if(m_CurrentLevel>=6)
		{
			SetMovementSpeedBonus(0.0f, 0.0f, new MovSpeedMultiplicative(20.0f, "HirasuESkill"));
			StartCoroutine(RemoveMovementSpeedBonus(1.0f, "HirasuESkill"));
		}
		SetDisabled(false);
	}

	protected override void RSkill()
	{
		if(!m_UsingQ)
		{
			GetEnemyWithMouse();
			m_UsingR=true;
		}
	}
	IEnumerator RAttack()
	{
		base.RSkill();
		StartCoroutine(DisableForDuration(0.5f));
		float l_Damage=m_RDamagePerLevel[m_RSkillLevel-1]+(GetAttackDamage()*(m_RAdditionalDamage/100.0f));
		SetAnimatorTrigger("IsUsingR");
		SetAnimatorBool("IsAAttacking", false);
		SetAnimatorBool("IsAAttacking2", false);
		GetAudioSource().clip=m_RSound;
		GetAudioSource().Play();
		Vector3 l_EnemyPos=m_DesiredEnemy.position;
		l_EnemyPos.y=0.0f;
		Vector3 l_DesiredPos=l_EnemyPos-transform.position;
		l_DesiredPos.Normalize();
		float l_ExplosionRadius=m_CurrentLevel>=11 ? m_RExplosionRadius[1]/100.0f : m_RExplosionRadius[0]/100.0f;
		float l_Scale=l_ExplosionRadius*2.0f;
		if(m_RSkillLevel>=3)
			StartCoroutine(RSpawnExplosion(l_EnemyPos, l_DesiredPos, l_ExplosionRadius, l_Scale, l_Damage, true));
		else
			StartCoroutine(RSpawnExplosion(l_EnemyPos, l_DesiredPos, l_ExplosionRadius, l_Scale, l_Damage, false));
		yield return new WaitForSeconds(0.5f);
		m_UsingR=false;
		SetIsAttacking(false);
	}
	IEnumerator RSpawnExplosion(Vector3 EnemyPos, Vector3 DesiredPos, float Radius, float Scale, float Damage, bool SecondExplosion)
	{
		GameObject l_Explosion=Instantiate(m_RExplosion, EnemyPos, m_RExplosion.transform.rotation);
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
					float l_ExtraDamage=0.0f;
					Enemy.TakeDamage(Damage+l_ExtraDamage, 0.0f);
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

	public override void LevelUp()
	{
		base.LevelUp();
		m_PassiveExtraDamage=(m_CurrentLevel-1)*(m_MaxExtraDamage-m_MinExtraDamage)/(18-1)+m_MinExtraDamage;
	}
	protected override void StartAttacking()
	{
        if(m_DesiredEnemy)
        {
            SetIsAttacking(true);
			if(!m_UsingR)
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
#if UNITY_EDITOR
        Debug.Log("ATTACKING - Since last auto: "+m_TimeSinceLastAuto);
        m_TimeSinceLastAuto=0.0f;
#endif
		float l_ExtraPhysDamage=0.0f;
		float l_ExtraMagicDamage=0.0f;
		if(m_RSkillLevel>=2)
		{
			l_ExtraPhysDamage=m_PassiveExtraDamage;
			l_ExtraMagicDamage=m_PassiveExtraDamage;
		}
		else if(m_RSkillLevel>=1)
			l_ExtraPhysDamage=m_PassiveExtraDamage;

		if(m_RSkillLevel>=1)
		{
			//APLICAR MARCAS
		}
		if(m_RSkillLevel>=3)
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
		m_DesiredEnemy.GetComponent<ITakeDamage>().TakeDamage(GetAttackDamage()+l_ExtraPhysDamage, l_ExtraMagicDamage);
	}
	public override void SetInitStats()
	{
		base.SetInitStats();
		m_Staff.SetActive(false);
		m_StaffFlames.SetActive(false);
		m_PassiveExtraDamage=(m_CurrentLevel-1)*(m_MaxExtraDamage-m_MinExtraDamage)/(18-1)+m_MinExtraDamage;
	}
}
