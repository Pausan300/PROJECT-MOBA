using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterMaster : MonoBehaviour
{
    CharacterUI m_CharacterUI;
    CameraController m_CharacterCamera;
    Animator m_CharacterAnimator;

    [Header("STATS")]
    public float m_MaxHealth;
    public float m_MaxMana;
    float m_CurrentHealth;
    float m_CurrentMana;
    public float m_AttackDamage;
    public float m_AbilityPower;
    public float m_AttackSpeed;
    public float m_CooldownReduction;
    public float m_CriticalChance;
    public float m_CriticalDamage;
    public float m_Armor;
    public float m_MagicResistance;
    public float m_MovementSpeed;

    public float m_AttackRange;
    public float m_Tenacity;
    public float m_ArmorPenetrationFixed;
    public float m_ArmorPenetrationPct;
    public float m_MagicPenetrationFixed;
    public float m_MagicPenetrationPct;
    public float m_HealthRegen;
    public float m_ManaRegen;
    public float m_LifeSteal;
    public float m_OmniDrain;
    public float m_ShieldsAndHealsPower;

    [Header("SKILLS")]
    public float m_QSkillCooldown;
    public float m_QSkillMana;
    float m_QSkillTimer;
    bool m_QSkillOnCd;
    public float m_WSkillCooldown;
    public float m_WSkillMana;
    float m_WSkillTimer;
    bool m_WSkillOnCd;
    public float m_ESkillCooldown;
    public float m_ESkillMana;
    float m_ESkillTimer;
    bool m_ESkillOnCd;
    public float m_RSkillCooldown;
    public float m_RSkillMana;
    float m_RSkillTimer;
    bool m_RSkillOnCd;

    [Header("RECALL")]
    public float m_RecallTime;
    public Transform m_RecallTpPoint;
    float m_CurrentRecallTime;
    bool m_Recalling;

    Vector3 m_DesiredPosition;
    Transform m_DesiredEnemy;
    bool m_ReachedDesiredPosition;
    bool m_Moving;
    bool m_Attacking;
    float m_TimeSinceLastAuto;
    float m_AttackAnimLength;

	protected virtual void Start()
	{
        m_CharacterCamera=GetComponent<CameraController>();
        m_CharacterAnimator=GetComponent<Animator>();
        m_CharacterUI=GetComponent<CharacterUI>();
        m_CurrentHealth=m_MaxHealth;
        m_CurrentMana=m_MaxMana;
        m_ReachedDesiredPosition=true;
        m_DesiredEnemy=null;
        AnimationClip[] l_Clips=m_CharacterAnimator.runtimeAnimatorController.animationClips;
        foreach(AnimationClip clip in l_Clips)
        {
            switch(clip.name)
            {
                case "AutoAttack":
                    m_AttackAnimLength=clip.length;
                    break;
            }
        }
	}
    protected virtual void Update()
    {
        Vector3 l_MousePosition=Input.mousePosition;
        l_MousePosition.z=10.0f;
        Vector3 l_MouseDirection=m_CharacterCamera.m_Camera.ScreenToWorldPoint(l_MousePosition)-m_CharacterCamera.m_Camera.transform.position;
        MouseTargeting(l_MouseDirection);
        CharacterMovement();
        ResourceRestoring();
        m_CharacterUI.UpdatePrimStats(m_AttackDamage, m_Armor, m_AttackSpeed, m_CriticalChance, m_AbilityPower, m_MagicResistance, m_CooldownReduction, m_MovementSpeed);
        m_CharacterUI.UpdateSeconStats(m_HealthRegen, m_ArmorPenetrationFixed, m_ArmorPenetrationPct, m_LifeSteal, m_AttackRange, m_ManaRegen, m_MagicPenetrationFixed, 
            m_MagicPenetrationPct, m_OmniDrain, m_Tenacity, m_ShieldsAndHealsPower);
        if(m_Recalling)
        {
            m_CharacterUI.UpdateRecallUI(m_CurrentRecallTime, m_RecallTime);
            m_CurrentRecallTime-=Time.deltaTime;
            if(m_CurrentRecallTime<=0.0f)
                TeleportToSpawn();
        }

#if UNITY_EDITOR
        if(m_Attacking)
            m_TimeSinceLastAuto+=Time.deltaTime;
#endif

        if(Input.GetKey(KeyCode.C))
            m_CharacterUI.ShowSeconStatsPanel();
        else if(Input.GetKeyUp(KeyCode.C))
            m_CharacterUI.HideSeconStatsPanel();

        if(Input.GetKeyDown(KeyCode.B))
            UseRecall();

        if(Input.GetKeyDown(KeyCode.Q))
            UseQSkill();
        if(Input.GetKeyDown(KeyCode.W))
            UseWSkill();
        if(Input.GetKeyDown(KeyCode.E))
            UseESkill();
        if(Input.GetKeyDown(KeyCode.R))
            UseRSkill();

        if(m_QSkillOnCd)
            SkillsCooldown(ref m_QSkillOnCd, m_CharacterUI.m_QSkillCdImage, ref m_QSkillTimer, m_QSkillCooldown);
        if(m_WSkillOnCd)
            SkillsCooldown(ref m_WSkillOnCd, m_CharacterUI.m_WSkillCdImage, ref m_WSkillTimer, m_WSkillCooldown);
        if(m_ESkillOnCd)
            SkillsCooldown(ref m_ESkillOnCd, m_CharacterUI.m_ESkillCdImage, ref m_ESkillTimer, m_ESkillCooldown);
        if(m_RSkillOnCd)
            SkillsCooldown(ref m_RSkillOnCd, m_CharacterUI.m_RSkillCdImage, ref m_RSkillTimer, m_RSkillCooldown);
    }
    void MouseTargeting(Vector3 MouseDirection)
    {
        RaycastHit l_CameraRaycastHit;
        if(Input.GetMouseButtonDown(1))
        {
            if(Physics.Raycast(m_CharacterCamera.m_Camera.transform.position, MouseDirection, out l_CameraRaycastHit, 1000.0f, m_CharacterCamera.m_CameraLayerMask))
            {
                Debug.DrawLine(m_CharacterCamera.m_Camera.transform.position, l_CameraRaycastHit.point, Color.red);
                if(l_CameraRaycastHit.transform.CompareTag("Enemy"))
                {
                    m_DesiredEnemy=l_CameraRaycastHit.transform;
                    m_DesiredPosition=m_DesiredEnemy.position;
                    m_DesiredPosition.y=0.0f;
                    m_ReachedDesiredPosition=false;
                }
            }
            m_Moving=true;
        }
        else if(Input.GetMouseButtonUp(1))
            m_Moving=false;

        if(m_Moving)
        {
            if(Physics.Raycast(m_CharacterCamera.m_Camera.transform.position, MouseDirection, out l_CameraRaycastHit, 1000.0f, m_CharacterCamera.m_CameraLayerMask))
            {
                Debug.DrawLine(m_CharacterCamera.m_Camera.transform.position, l_CameraRaycastHit.point, Color.green);
                if(l_CameraRaycastHit.transform.CompareTag("Terrain"))
                {
                    m_DesiredPosition=l_CameraRaycastHit.point;
                    m_DesiredEnemy=null;
                    m_Attacking=false;
                    m_CharacterAnimator.SetBool("IsAAttacking", false);
                    m_ReachedDesiredPosition=false;
                    StopRecall();
                }
            }
        }
    }
    void CharacterMovement()
    {
        if(!m_ReachedDesiredPosition)
        {
            Vector3 l_CharacterDirection=m_DesiredPosition-transform.position;
            l_CharacterDirection.Normalize();
            if(Input.GetKeyDown(KeyCode.S))
            {
                StopMovement();
                return;
            }   
            float l_MinDistance;
            if(m_DesiredEnemy!=null)
                l_MinDistance=m_AttackRange/100.0f;
            else
                l_MinDistance=0.25f;
            transform.forward=l_CharacterDirection;
            if(Vector3.Distance(transform.position, m_DesiredPosition)>l_MinDistance)
            {
                transform.position+=l_CharacterDirection*(m_MovementSpeed/100.0f)*Time.deltaTime;
                m_CharacterAnimator.SetBool("IsMoving", true);
            } 
            else
            {
                m_ReachedDesiredPosition=true;
                m_CharacterAnimator.SetBool("IsMoving", false);
                if(m_DesiredEnemy)
                {
                    m_Attacking=true;
                    m_CharacterAnimator.SetBool("IsAAttacking", true);
                } 
            }
        }
    }
    void StopMovement()
    {
        m_ReachedDesiredPosition=true;
        m_Moving=false;
        m_CharacterAnimator.SetBool("IsMoving", false);
    }
	void UseQSkill()
    {
        if(m_QSkillOnCd)
        {
            Debug.Log("Q STILL ON COOLDOWN BOBI");
        }
        else if(m_CurrentMana<m_QSkillMana)
        {
            Debug.Log("NOT ENOUGH MANA TO USE Q");
        }
        else
        {
            Debug.Log("USE Q SKILL");
            m_QSkillTimer=m_QSkillCooldown;
            m_CharacterUI.m_QSkillCdImage.fillAmount=1.0f;
            m_CurrentMana-=m_QSkillMana;
            m_QSkillOnCd=true;
            StopRecall();
        }
    } 
    void UseWSkill()
    {
        if(m_WSkillOnCd)
        {
            Debug.Log("W STILL ON COOLDOWN BOBI");
        }
        else if(m_CurrentMana<m_WSkillMana)
        {
            Debug.Log("NOT ENOUGH MANA TO USE W");
        }
        else
        {
            Debug.Log("USE W SKILL");
            m_WSkillTimer=m_WSkillCooldown;
            m_CharacterUI.m_WSkillCdImage.fillAmount=1.0f;
            m_CurrentMana-=m_WSkillMana;
            m_WSkillOnCd=true;
            StopRecall();
        }
    }
    void UseESkill()
    {
        if(m_ESkillOnCd)
        {
            Debug.Log("E STILL ON COOLDOWN BOBI");
        }
        else if(m_CurrentMana<m_ESkillMana)
        {
            Debug.Log("NOT ENOUGH MANA TO USE E");
        }
        else
        {
            Debug.Log("USE E SKILL");
            m_ESkillTimer=m_ESkillCooldown;
            m_CharacterUI.m_ESkillCdImage.fillAmount=1.0f;
            m_CurrentMana-=m_ESkillMana;
            m_ESkillOnCd=true;
            StopRecall();
        }
    }
    void UseRSkill()
    {
        if(m_RSkillOnCd)
        {
            Debug.Log("R STILL ON COOLDOWN BOBI");
        }
        else if(m_CurrentMana<m_RSkillMana)
        {
            Debug.Log("NOT ENOUGH MANA TO USE R");
        }
        else
        {
            Debug.Log("USE R SKILL");
            m_RSkillTimer=m_RSkillCooldown;
            m_CharacterUI.m_RSkillCdImage.fillAmount=1.0f;
            m_CurrentMana-=m_RSkillMana;
            m_RSkillOnCd=true;
            StopRecall();
        }
    }
    void SkillsCooldown(ref bool IsOnCd, Image SkillImage, ref float SkillTimer, float SkillCd)
    {
        Debug.Log(SkillTimer);
        SkillTimer-=Time.deltaTime;
        SkillImage.fillAmount=SkillTimer/SkillCd;
        if(SkillTimer<=0.0f)
            IsOnCd=false;
    }
    void ResourceRestoring()
    {
        if(m_CurrentMana<m_MaxMana)
        {
            m_CurrentMana+=m_ManaRegen/5.0f*Time.deltaTime;
            if(m_CurrentMana>m_MaxMana)
                m_CurrentMana=m_MaxMana;
        }
        if(m_CurrentHealth<m_MaxHealth)
        {
            m_CurrentHealth+=m_HealthRegen/5.0f*Time.deltaTime;
            if(m_CurrentHealth>m_MaxHealth)
                m_CurrentHealth=m_MaxHealth;
        }
        m_CharacterUI.UpdateHealthManaBars(m_CurrentHealth, m_MaxHealth, m_CurrentMana, m_MaxMana);
    }
    void UseRecall()
    {
        if(!m_Recalling)
        {
            m_CharacterUI.ShowRecallUI();
            m_CurrentRecallTime=m_RecallTime;
            m_Recalling=true;
            StopMovement();
        }
    }
    void StopRecall()
    {
        if(m_Recalling)
        {
            m_CharacterUI.HideRecallUI();
            m_Recalling=false;
        }
    }
    void TeleportToSpawn()
    {
        transform.position=m_RecallTpPoint.position;
        m_Recalling=false;
        m_CharacterUI.HideRecallUI();
    }

    //LLAMADA POR EVENTO EN LA ANIMACION DE AUTOATAQUE
    void PerformAutoAttack()
    {
        Debug.Log("ATTACKING - Since last auto: "+m_TimeSinceLastAuto);
        m_TimeSinceLastAuto=0.0f;
    }

    public float GetAttackAnimationLength()
    {
        return m_AttackAnimLength;
    }
}
