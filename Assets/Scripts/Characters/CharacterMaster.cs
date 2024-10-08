using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct MovSpeedMultiplicative
{
    public float m_Amount;
    public string m_Name;
    public MovSpeedMultiplicative(float Amount, string Name)
    {
        m_Amount=Amount;
        m_Name=Name;
    }
}

public class CharacterMaster : MonoBehaviour, ITakeDamage
{
    CharacterUI m_CharacterUI;
    CameraController m_CharacterCamera;
    Animator m_CharacterAnimator;
    AudioSource m_AudioSource;

    [Header("PLAYER INFO")]
    public string m_PlayerName;

    [Header("BASE STATS")]
    public float m_BaseHealth;
    public float m_BaseMana;
    public float m_BaseAttackDamage;
    public float m_BaseAttackSpeed;
    public float m_BaseArmor;
    public float m_BaseMagicResist;
    public float m_BaseHealthRegen;
    public float m_BaseManaRegen;
    public float m_AttackRange;
    public float m_BaseMovementSpeed;
    float m_MaxHealth;
    float m_MaxMana;
    float m_CurrentHealth;
    float m_CurrentMana;
    float m_AttackDamage;
    float m_AbilityPower;
    float m_AttackSpeed;
    float m_CooldownReduction;
    float m_CriticalChance;
    float m_CriticalDamage;
    float m_Armor;
    float m_MagicResistance;
    float m_Tenacity;
    float m_ArmorPenetrationFixed;
    float m_ArmorPenetrationPct;
    float m_MagicPenetrationFixed;
    float m_MagicPenetrationPct;
    float m_HealthRegen;
    float m_ManaRegen;
    float m_LifeSteal;
    float m_OmniDrain;
    float m_ShieldsAndHealsPower;
    float m_MovementSpeed;
    
    [Header("GROWTH STATS")]
    public float m_HealthPerLevel;
    public float m_ManaPerLevel;
    public float m_AttackDamagePerLevel;
    public float m_AttackSpeedPerLevel;
    public float m_ArmorPerLevel;
    public float m_MagicResistPerLevel;
    public float m_HealthRegenPerLevel;
    public float m_ManaRegenPerLevel;
    float m_HealthBonus;
    float m_ManaBonus;
    float m_AttackDamageBonus;
    float m_AttackSpeedBonus;
    float m_ArmorBonus;
    float m_MagicResistBonus;
    float m_HealthRegenBonus;
    float m_ManaRegenBonus;
    float m_MoveSpeedBonusFlat;
    float m_MoveSpeedBonusAddi;
    List<MovSpeedMultiplicative> m_MoveSpeedBonusMult=new List<MovSpeedMultiplicative>();

    [Header("EXPERIENCE")]
    public int m_CurrentLevel;
    public int m_SkillPoints;
    public float m_CurrentExp;
    public List<float> m_ExpPerLevel;

    [Header("SKILLS")]
    public float m_QSkillCooldown;
    public float m_QSkillMana;
    public int m_QSkillLevel;
    float m_QSkillTimer;
    bool m_QSkillOnCd;
    public float m_WSkillCooldown;
    public float m_WSkillMana;
    public int m_WSkillLevel;
    float m_WSkillTimer;
    bool m_WSkillOnCd;
    public float m_ESkillCooldown;
    public float m_ESkillMana;
    public int m_ESkillLevel;
    float m_ESkillTimer;
    bool m_ESkillOnCd;
    public float m_RSkillCooldown;
    public float m_RSkillMana;
    public int m_RSkillLevel;
    float m_RSkillTimer;
    bool m_RSkillOnCd;
    public float m_SumSpell1Cooldown;
    float m_SumSpell1Timer;
    bool m_SumSpell1OnCd;
    public float m_SumSpell2Cooldown;
    float m_SumSpell2Timer;
    bool m_SumSpell2OnCd;
    bool m_ZeroCooldown;
    public LayerMask m_DamageLayerMask;

    [Header("RECALL")]
    public float m_RecallTime;
    public Transform m_RecallTpPoint;
    float m_CurrentRecallTime;
    bool m_Recalling;

    Vector3 m_DesiredPosition;
    public Transform m_DesiredEnemy;
    bool m_GoingToDesiredPosition;
    bool m_LookingForNextPosition;
    bool m_Attacking;
    bool m_Disabled;
    public float m_TimeSinceLastAuto;
    float m_AttackAnimLength;

	protected virtual void Start()
	{
        m_CharacterCamera=GetComponent<CameraController>();
        m_CharacterAnimator=GetComponent<Animator>();
        m_CharacterUI=GetComponent<CharacterUI>();
        m_AudioSource=GetComponent<AudioSource>();
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
        m_GoingToDesiredPosition=false;
        m_DesiredEnemy=null;
        m_CharacterUI.SetPlayer(this);
        m_CharacterUI.SetPlayerName(m_PlayerName);
        SetInitStats();
	}
    protected virtual void Update()
    {
        Vector3 l_MousePosition=Input.mousePosition;
        l_MousePosition.z=10.0f;
        Vector3 l_MouseDirection=m_CharacterCamera.m_Camera.ScreenToWorldPoint(l_MousePosition)-m_CharacterCamera.m_Camera.transform.position;
        MouseTargeting(l_MouseDirection);
        if(!m_Disabled)
        {
            CharacterMovement();
        }
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

        if(m_Attacking)
        { 
            if(Input.GetKeyDown(KeyCode.S))
                StopAttacking();
#if UNITY_EDITOR
            m_TimeSinceLastAuto+=Time.deltaTime;
#endif
        }
#if UNITY_EDITOR
        if(Input.GetKeyDown(KeyCode.P))
        {
            m_CurrentExp+=200.0f;
            if(m_CurrentLevel<18)
            {
                if(m_CurrentExp>=m_ExpPerLevel[m_CurrentLevel])
                    LevelUp();
            }
        }
#endif
        if(m_CurrentLevel<18)
            m_CharacterUI.UpdateExpBar(m_CurrentExp, m_ExpPerLevel[m_CurrentLevel]);

        if(Input.GetKey(KeyCode.C))
            m_CharacterUI.ShowSeconStatsPanel();
        else if(Input.GetKeyUp(KeyCode.C))
            m_CharacterUI.HideSeconStatsPanel();

        if(!m_Disabled)
        {
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
            if(Input.GetKeyDown(KeyCode.D))
                UseSummonerSpell1();
            if(Input.GetKeyDown(KeyCode.F))
                UseSummonerSpell2();
        }

        if(m_QSkillOnCd)
            SkillsCooldown(ref m_QSkillOnCd, m_CharacterUI.m_QSkillCdImage, ref m_QSkillTimer, m_QSkillCooldown, m_CharacterUI.m_QSkillCdText);
        if(m_WSkillOnCd)
            SkillsCooldown(ref m_WSkillOnCd, m_CharacterUI.m_WSkillCdImage, ref m_WSkillTimer, m_WSkillCooldown, m_CharacterUI.m_WSkillCdText);
        if(m_ESkillOnCd)
            SkillsCooldown(ref m_ESkillOnCd, m_CharacterUI.m_ESkillCdImage, ref m_ESkillTimer, m_ESkillCooldown, m_CharacterUI.m_ESkillCdText);
        if(m_RSkillOnCd)
            SkillsCooldown(ref m_RSkillOnCd, m_CharacterUI.m_RSkillCdImage, ref m_RSkillTimer, m_RSkillCooldown, m_CharacterUI.m_RSkillCdText);
        if(m_SumSpell1OnCd)
            SkillsCooldown(ref m_SumSpell1OnCd, m_CharacterUI.m_SumSpell1CdImage, ref m_SumSpell1Timer, m_SumSpell1Cooldown, m_CharacterUI.m_SumSpell1CdText);
        if(m_SumSpell2OnCd)
            SkillsCooldown(ref m_SumSpell2OnCd, m_CharacterUI.m_SumSpell2CdImage, ref m_SumSpell2Timer, m_SumSpell2Cooldown, m_CharacterUI.m_SumSpell2CdText);

        m_MovementSpeed=m_BaseMovementSpeed+m_MoveSpeedBonusFlat;
        m_MovementSpeed*=1.0f+(m_MoveSpeedBonusAddi/100.0f);
        for(int i=0; i<m_MoveSpeedBonusMult.Count; i++)
            m_MovementSpeed*=1.0f+(m_MoveSpeedBonusMult[i].m_Amount/100.0f);
    }
    void MouseTargeting(Vector3 MouseDirection)
    {
        RaycastHit l_CameraRaycastHit;
        if(Input.GetMouseButtonDown(1))
        {
            if(Physics.Raycast(m_CharacterCamera.m_Camera.transform.position, MouseDirection, out l_CameraRaycastHit, 1000.0f, m_CharacterCamera.m_CameraLayerMask))
            {
                if(l_CameraRaycastHit.transform.CompareTag("Enemy"))
                {
                    m_DesiredEnemy=l_CameraRaycastHit.transform;
                    m_DesiredPosition=m_DesiredEnemy.position;
                    m_DesiredPosition.y=0.0f;
                    m_GoingToDesiredPosition=true;
                    StopRecall();
                }
            }
            m_LookingForNextPosition=true;
        }
        else if(Input.GetMouseButtonUp(1))
            m_LookingForNextPosition=false;

        if(m_LookingForNextPosition)
        {
            if(Physics.Raycast(m_CharacterCamera.m_Camera.transform.position, MouseDirection, out l_CameraRaycastHit, 1000.0f, m_CharacterCamera.m_CameraLayerMask))
            {
                Debug.DrawLine(m_CharacterCamera.m_Camera.transform.position, l_CameraRaycastHit.point, Color.green);
                if(l_CameraRaycastHit.transform.CompareTag("Terrain"))
                {
                    m_DesiredPosition=l_CameraRaycastHit.point;
                    m_DesiredPosition.y=0.0f;
                    m_DesiredEnemy=null;
                    m_GoingToDesiredPosition=true;
                    StopAttacking();
                    StopRecall();
                }
            }
        }
    }
    void CharacterMovement()
    {
        if(m_GoingToDesiredPosition)
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
                l_MinDistance=0.1f;
            transform.forward=l_CharacterDirection;
            if(Vector3.Distance(transform.position, m_DesiredPosition)>l_MinDistance)
            {
                transform.position+=l_CharacterDirection*(m_MovementSpeed/100.0f)*Time.deltaTime;
                m_CharacterAnimator.SetBool("IsMoving", true);
            } 
            else
            {
                m_GoingToDesiredPosition=false;
                m_CharacterAnimator.SetBool("IsMoving", false);
                StartAttacking();
            }
        }
    }
    public void GetEnemyWithMouse()
    {
        Vector3 l_MousePosition=Input.mousePosition;
        l_MousePosition.z=10.0f;
        Vector3 l_MouseDirection=m_CharacterCamera.m_Camera.ScreenToWorldPoint(l_MousePosition)-m_CharacterCamera.m_Camera.transform.position;
        RaycastHit l_CameraRaycastHit;
        if(Physics.Raycast(m_CharacterCamera.m_Camera.transform.position, l_MouseDirection, out l_CameraRaycastHit, 1000.0f, m_CharacterCamera.m_CameraLayerMask))
        {
            if(l_CameraRaycastHit.transform.CompareTag("Enemy"))
            {
                m_DesiredEnemy=l_CameraRaycastHit.transform;
                m_DesiredPosition=m_DesiredEnemy.position;
                m_DesiredPosition.y=0.0f;
                m_GoingToDesiredPosition=true;
                StopRecall();
            }
        }
    }
    public Vector3 GetDirectionWithMouse()
    {
        Vector3 l_MousePosition=Input.mousePosition;
        l_MousePosition.z=10.0f;
        Vector3 l_MouseDirection=m_CharacterCamera.m_Camera.ScreenToWorldPoint(l_MousePosition)-transform.position;
        l_MouseDirection.y=0.0f;
        return l_MouseDirection;
    }
    public void StopMovement()
    {
        m_GoingToDesiredPosition=false;
        m_LookingForNextPosition=false;
        m_CharacterAnimator.SetBool("IsMoving", false);
    }
    protected virtual void StartAttacking()
    {
        if(m_DesiredEnemy)
        {
            m_Attacking=true;
            m_CharacterAnimator.SetBool("IsAAttacking", true);
        } 
    }
    protected virtual void StopAttacking()
    {
        if(m_Attacking)
        {
            m_Attacking=false;
            m_CharacterAnimator.SetBool("IsAAttacking", false);
        }
    }
	void UseQSkill()
    {
        if(m_QSkillLevel<=0)
        {
            Debug.Log("Q STILL LOCKED BOBI");
        }
        else if(m_QSkillOnCd)
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
            QSkill();
        }
    }
    protected virtual void QSkill()
    {
        m_QSkillTimer=m_QSkillCooldown;
        m_CharacterUI.m_QSkillCdImage.fillAmount=1.0f;
        m_CharacterUI.m_QSkillCdText.enabled=true;
        m_CurrentMana-=m_QSkillMana;
        m_QSkillOnCd=true;
        StopRecall();
    }
    void UseWSkill()
    {
        if(m_WSkillLevel<=0)
        {
            Debug.Log("W STILL LOCKED BOBI");
        }
        else if(m_WSkillOnCd)
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
            WSkill();
        }
    }
    protected virtual void WSkill()
    {
        m_WSkillTimer=m_WSkillCooldown;
        m_CharacterUI.m_WSkillCdImage.fillAmount=1.0f;
        m_CharacterUI.m_WSkillCdText.enabled=true;
        m_CurrentMana-=m_WSkillMana;
        m_WSkillOnCd=true;
        StopRecall();
    }
    void UseESkill()
    {
        if(m_ESkillLevel<=0)
        {
            Debug.Log("E STILL LOCKED BOBI");
        }
        else if(m_ESkillOnCd)
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
            ESkill();
        }
    }
    protected virtual void ESkill()
    {
        m_ESkillTimer=m_ESkillCooldown;
        m_CharacterUI.m_ESkillCdImage.fillAmount=1.0f;
        m_CharacterUI.m_ESkillCdText.enabled=true;
        m_CurrentMana-=m_ESkillMana;
        m_ESkillOnCd=true;
        StopRecall();
    }
    void UseRSkill()
    {
        if(m_RSkillLevel<=0)
        {
            Debug.Log("R STILL LOCKED BOBI");
        }
        else if(m_RSkillOnCd)
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
            RSkill();
        }
    }
    protected virtual void RSkill()
    {
        m_RSkillTimer=m_RSkillCooldown;
        m_CharacterUI.m_RSkillCdImage.fillAmount=1.0f;
        m_CharacterUI.m_RSkillCdText.enabled=true;
        m_CurrentMana-=m_RSkillMana;
        m_RSkillOnCd=true;
        StopRecall();
    }
    void UseSummonerSpell1()
    {
        if(m_SumSpell1OnCd)
        {
            Debug.Log("SS1 STILL ON COOLDOWN BOBI");
        }
        else
        {
            Debug.Log("USE SS1");
            m_SumSpell1Timer=m_SumSpell1Cooldown;
            m_CharacterUI.m_SumSpell1CdImage.fillAmount=1.0f;
            m_CharacterUI.m_SumSpell1CdText.enabled=true;
            m_SumSpell1OnCd=true;
            StopRecall();
        }
    }
    void UseSummonerSpell2()
    {
        if(m_SumSpell2OnCd)
        {
            Debug.Log("SS2 STILL ON COOLDOWN BOBI");
        }
        else
        {
            Debug.Log("USE SS2");
            m_SumSpell2Timer=m_SumSpell2Cooldown;
            m_CharacterUI.m_SumSpell2CdImage.fillAmount=1.0f;
            m_CharacterUI.m_SumSpell2CdText.enabled=true;
            m_SumSpell2OnCd=true;
            StopRecall();
        }
    }
    void SkillsCooldown(ref bool IsOnCd, Image SkillImage, ref float SkillTimer, float SkillCd, TMPro.TextMeshProUGUI CdText)
    {
        if(!m_ZeroCooldown)
        {
            SkillTimer-=Time.deltaTime;
            if(SkillTimer>=1.0f)
                CdText.text=SkillTimer.ToString("f0");
            else
                CdText.text=SkillTimer.ToString("f1");
            SkillImage.fillAmount=SkillTimer/SkillCd;
            if(SkillTimer<=0.0f)
            {
                IsOnCd=false;
                CdText.enabled=false;
            }
        } 
        else
        {
            SkillImage.fillAmount=0.0f;
            SkillTimer=0.0f;
            IsOnCd=false;
            CdText.enabled=false;
        }
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
            StopAttacking();
        }
    }
    public void StopRecall()
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
    public virtual void LevelUp()
    {
        m_CurrentExp-=m_ExpPerLevel[m_CurrentLevel];
        if(m_CurrentExp<0.0f)
            m_CurrentExp=0.0f;
        m_CurrentLevel++;
        if(m_CurrentLevel>=18)
            m_CharacterUI.UpdateExpBar(1.0f, 1.0f);
        m_CharacterUI.ShowLevelUpSkillButtons();
        m_SkillPoints++;
        m_CharacterUI.UpdateCharacterLevel(m_CurrentLevel);
        RecalculateStat(m_MaxHealth, out m_MaxHealth, m_CurrentHealth, out m_CurrentHealth, m_BaseHealth, m_HealthPerLevel, m_HealthBonus);
        RecalculateStat(m_MaxMana, out m_MaxMana, m_CurrentMana, out m_CurrentMana, m_BaseMana, m_ManaPerLevel, m_ManaBonus);
        RecalculateStat(out m_AttackDamage, m_BaseAttackDamage, m_AttackDamagePerLevel, m_AttackDamageBonus);
        RecalculateStat(out m_AttackSpeed, m_BaseAttackSpeed, m_AttackSpeedPerLevel, m_AttackSpeedBonus);
        RecalculateStat(out m_Armor, m_BaseArmor, m_ArmorPerLevel, m_ArmorBonus);
        RecalculateStat(out m_MagicResistance, m_BaseMagicResist, m_MagicResistPerLevel, m_MagicResistBonus);
        RecalculateStat(out m_HealthRegen, m_BaseHealthRegen, m_HealthRegenPerLevel, m_HealthRegenBonus);
        RecalculateStat(out m_ManaRegen, m_BaseManaRegen, m_ManaRegenPerLevel, m_ManaRegenBonus);
    }
    void RecalculateStat(float Stat, out float StatRef, float Current, out float CurrentRef, float BaseStat, float LevelIncr, float Bonus)
    {
        float l_InitMaxStat=Stat;
        StatRef=BaseStat+Bonus+LevelIncr*(m_CurrentLevel-1.0f)*(0.7025f+0.0175f*(m_CurrentLevel-1.0f));
        float l_Difference=StatRef-l_InitMaxStat;
        CurrentRef=Current+l_Difference;
    }
    void RecalculateStat(out float StatRef, float BaseStat, float LevelIncr, float Bonus)
    {
        StatRef=BaseStat+Bonus+LevelIncr*(m_CurrentLevel-1.0f)*(0.7025f+0.0175f*(m_CurrentLevel-1.0f));
    }
    public void SetInitStats()
    {
        m_MaxHealth=m_BaseHealth;
        m_MaxMana=m_BaseMana;
        m_AttackDamage=m_BaseAttackDamage;
        m_AttackSpeed=m_BaseAttackSpeed;
        m_Armor=m_BaseArmor;
        m_MagicResistance=m_BaseMagicResist;
        m_HealthRegen=m_BaseHealthRegen;
        m_ManaRegen=m_BaseManaRegen;
        m_MovementSpeed=m_BaseMovementSpeed;

        m_HealthBonus=0.0f;
        m_ManaBonus=0.0f;
        m_AttackDamageBonus=0.0f;
        m_AttackSpeedBonus=0.0f;
        m_ArmorBonus=0.0f;
        m_MagicResistBonus=0.0f;
        m_ManaRegenBonus=0.0f;
        m_HealthRegenBonus=0.0f;

        m_CurrentHealth=m_MaxHealth;
        m_CurrentMana=m_MaxMana;
        m_CurrentLevel=1;
        m_SkillPoints=1;
        m_QSkillLevel=0;
        m_WSkillLevel=0;
        m_ESkillLevel=0;
        m_RSkillLevel=0;

        m_CharacterUI.UpdateCharacterLevel(m_CurrentLevel);
        m_CharacterUI.ResetSkillLevelPoints();
        m_CharacterUI.HideLevelUpSkillButtons();
        m_CharacterUI.ShowLevelUpSkillButtons();
    }
    public void SetHealthBonus(float Bonus)
    {
        m_HealthBonus+=Bonus;
        RecalculateStat(m_MaxHealth, out m_MaxHealth, m_CurrentHealth, out m_CurrentHealth, m_BaseHealth, m_HealthPerLevel, m_HealthBonus);
    }
    public void SetMovementSpeedBonus(float FlatBonus, float AddiBonus, MovSpeedMultiplicative MultBonus)
    {
        if(FlatBonus!=0.0f)
            m_MoveSpeedBonusFlat+=FlatBonus;
        if(AddiBonus!=0.0f)
            m_MoveSpeedBonusAddi+=AddiBonus;
        if(MultBonus.m_Amount>0.0f)
            m_MoveSpeedBonusMult.Add(MultBonus);
    }
    public IEnumerator RemoveMovementSpeedBonus(float Time, string Name)
    {
        yield return new WaitForSeconds(Time);
        foreach(MovSpeedMultiplicative Bonus in m_MoveSpeedBonusMult)
        {
            if(Bonus.m_Name==Name)
            {
                m_MoveSpeedBonusMult.Remove(Bonus);
                break;
            }
        }
    }
	public void TakeDamage(float PhysDamage, float MagicDamage)
    {
        if(PhysDamage>0.0f)
        {
            m_CurrentHealth-=PhysDamage/(1.0f+m_Armor/100.0f);
            Debug.Log("Taking "+PhysDamage+" physical damage, reduced to "+(PhysDamage/(1.0f+m_Armor/100.0f))+" damage");
        }
        if(MagicDamage>0.0f)
        {
            m_CurrentHealth-=MagicDamage/(1.0f+m_MagicResistance/100.0f);
            Debug.Log("Taking "+MagicDamage+" magical damage, reduced to "+(MagicDamage/(1.0f+m_Armor/100.0f))+" damage");
        }
        if(m_CurrentRecallTime>0.2f)
            StopRecall();
	}

    //LLAMADA POR EVENTO EN LA ANIMACION DE AUTOATAQUE
    protected virtual void PerformAutoAttack()
    {
#if UNITY_EDITOR
        Debug.Log("ATTACKING - Since last auto: "+m_TimeSinceLastAuto);
        m_TimeSinceLastAuto=0.0f;
#endif
        m_DesiredEnemy.GetComponent<ITakeDamage>().TakeDamage(m_AttackDamage, m_AbilityPower);
    }

    //GETTERS & SETTERS
    public float GetAttackAnimationLength()
    {
        return m_AttackAnimLength;
    }
    public void SetAnimatorBool(string Name, bool True)
    {
        m_CharacterAnimator.SetBool(Name, True);
    }
    public void SetAnimatorTrigger(string Name)
    {
        m_CharacterAnimator.SetTrigger(Name);
    }
    public bool GetAnimatorBool(string Name)
    {
        return m_CharacterAnimator.GetBool(Name);
    }
    public bool GetIsLookingForPosition()
    {
        return m_LookingForNextPosition;
    }
    public bool GetIsAttacking()
    {
        return m_Attacking;
    }
    public void SetIsAttacking(bool Attacking)
    {
        m_Attacking=Attacking;
    }
    public void SetDisabled(bool Disabled)
    {
        m_Disabled=Disabled;
    }
    public float GetAttackDamage()
    {
        return m_AttackDamage;
    }
    public float GetAbilityDamage()
    {
        return m_AbilityPower;
    }
    public float GetAttackSpeed()
    {
        return m_AttackSpeed;
    }
    public float GetMaxHealth()
    {
        return m_MaxHealth;
    }
    public void SetMaxHealth(float Health)
    {
        m_MaxHealth=Health;
    }
    public float GetCurrentHealth()
    {
        return m_CurrentHealth;
    }
    public void SetCurrentHealth(float Health)
    {
        m_CurrentHealth=Health;
        Debug.Log("Set health: "+Health);
    }
    public float GetMaxMana()
    {
        return m_MaxMana;
    }
    public void SetCurrentMana(float Mana)
    {
        m_CurrentMana=Mana;
    }
    public void SetZeroCooldown(bool Active)
    {
        m_ZeroCooldown=Active;
    }
    public CameraController GetCameraController()
    {
        return m_CharacterCamera;
    }
    public AudioSource GetAudioSource()
    {
        return m_AudioSource;
    }
    public Animator GetAnimator()
    {
        return m_CharacterAnimator;
    }
}
