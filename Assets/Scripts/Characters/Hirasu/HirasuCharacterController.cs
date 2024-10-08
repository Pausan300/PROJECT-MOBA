using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HirasuCharacterController : CharacterMaster
{
    [Header("--- HIRASU ---")]
    [Header("PASSIVE SKILL")]
	public float m_MinPhysicDamage;
	public float m_MaxPhysicDamage;
	public float m_MinMagicDamage;
	public float m_MaxMagicDamage;
	public GameObject m_Staff;
	public GameObject m_StaffFlames;
	public float m_MaxSecondAttackTime;
	bool m_SecondAttack;
	float m_SecondAttackTimer;
    [Header("Q SKILL")]
	public GameObject m_QProjectile;
	public float m_QMaxHoldTime;
	public float m_QDuration;
	public float m_QTapRange;
	public float m_QHoldRange;
	public float m_QSplintersDuration;
	float m_QCurrentHoldTime;
    [Header("W SKILL")]
    [Header("E SKILL")]
	public float m_ERange;
	public float m_EDuration;
    [Header("R SKILL")]
	public GameObject m_RExplosion;
	public Material m_RExplosionBlueMaterial;
	public float m_RTimeToExpand;
	public float[] m_RExplosionRadius;
	public float[] m_RDamagePerLevel;
	public float m_RAdditionalDamage;
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
		if(m_UsingR)
		{
			if(GetIsAttacking())
			{
				StartCoroutine(RAttack());
				m_UsingR=false;
			}
		}

    }

	protected override void QSkill()
	{
		Vector3 l_Direction=GetDirectionWithMouse();
		l_Direction.Normalize();
		SetAnimatorTrigger("IsUsingQ");
		base.QSkill();
	}
	protected override void WSkill()
	{
		base.WSkill();
	}
	protected override void ESkill()
	{
		Vector3 l_Direction=GetDirectionWithMouse();
		l_Direction.Normalize();
		StartCoroutine(EDash(l_Direction));
	}
	IEnumerator EDash(Vector3 Direction)
	{
		base.ESkill();
		SetDisabled(true);
		StopRecall();
		StopAttacking();
		if(!GetIsLookingForPosition())
			StopMovement();
        SetAnimatorBool("IsUsingE", true);
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
        SetAnimatorBool("IsUsingE", false);
		SetDisabled(false);
	}
	protected override void RSkill()
	{
		GetEnemyWithMouse();
		m_UsingR=true;
	}
	IEnumerator RAttack()
	{
		base.RSkill();
		SetDisabled(true);
		float l_Damage=m_RDamagePerLevel[m_RSkillLevel-1]+(GetAttackDamage()*(m_RAdditionalDamage/100.0f));
		SetAnimatorBool("IsUsingR", true);
		SetAnimatorBool("IsAAttacking", false);
		SetAnimatorBool("IsAAttacking2", false);
		GetAudioSource().clip=m_RSound;
		GetAudioSource().Play();
		Vector3 l_EnemyPos=m_DesiredEnemy.position;
		l_EnemyPos.y=0.0f;
		Vector3 l_DesiredPos=l_EnemyPos-transform.position;
		l_DesiredPos.Normalize();
		Debug.Log(l_EnemyPos+"  "+l_DesiredPos+"  "+m_DesiredEnemy);
		float l_ExplosionRadius=m_CurrentLevel>=11 ? m_RExplosionRadius[1]/100.0f : m_RExplosionRadius[0]/100.0f;
		float l_Scale=l_ExplosionRadius*2.0f;
		if(m_RSkillLevel>=3)
			StartCoroutine(RSpawnExplosion(l_EnemyPos, l_DesiredPos, l_ExplosionRadius, l_Scale, l_Damage, true));
		else
			StartCoroutine(RSpawnExplosion(l_EnemyPos, l_DesiredPos, l_ExplosionRadius, l_Scale, l_Damage, false));
		yield return new WaitForSeconds(0.5f);
		SetDisabled(false);
		m_UsingR=false;
		SetAnimatorBool("IsUsingR", false);
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
		Debug.Log(l_DesiredPos);
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

	public override void LevelUp()
	{
		base.LevelUp();
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
	protected override void PerformAutoAttack()
	{
#if UNITY_EDITOR
        Debug.Log("ATTACKING - Since last auto: "+m_TimeSinceLastAuto);
        m_TimeSinceLastAuto=0.0f;
#endif
		float l_PassiveExtraDamage=0.0f;
		if(m_RSkillLevel>=3)
		{
			l_PassiveExtraDamage=(m_CurrentLevel-1)*(m_MaxPhysicDamage-m_MinPhysicDamage)/(18-1)+m_MinPhysicDamage;
			m_DesiredEnemy.GetComponent<ITakeDamage>().TakeDamage(GetAttackDamage()+l_PassiveExtraDamage, l_PassiveExtraDamage);
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
		else if(m_RSkillLevel>=2)
		{
			l_PassiveExtraDamage=(m_CurrentLevel-1)*(m_MaxPhysicDamage-m_MinPhysicDamage)/(18-1)+m_MinPhysicDamage;
			m_DesiredEnemy.GetComponent<ITakeDamage>().TakeDamage(GetAttackDamage()+l_PassiveExtraDamage, l_PassiveExtraDamage);
		}
		else if(m_RSkillLevel>=1)
		{
			l_PassiveExtraDamage=(m_CurrentLevel-1)*(m_MaxPhysicDamage-m_MinPhysicDamage)/(18-1)+m_MinPhysicDamage;
			m_DesiredEnemy.GetComponent<ITakeDamage>().TakeDamage(GetAttackDamage()+l_PassiveExtraDamage, 0.0f);
		}
		else
			m_DesiredEnemy.GetComponent<ITakeDamage>().TakeDamage(GetAttackDamage(), 0.0f);
	}
}
