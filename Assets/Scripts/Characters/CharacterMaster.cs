using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterMaster : MonoBehaviour, ITakeDamage
{
    CameraController m_CharacterCamera;
    Animator m_CharacterAnimator;
    AudioSource m_AudioSource;

    [Header("CHARACTER STATS")]
    public CharacterStats m_CharacterStats;

    [Header("CHARACTER UI")]
    public CharacterUI m_CharacterUI;

    [Header("RECALL")]
    public float m_RecallTime;
    public Transform m_RecallTpPoint;
    float m_CurrentRecallTime;
    bool m_Recalling;

    Vector3 m_DesiredPosition;
    public Transform m_SelectedCharacter;
    public Transform m_DesiredEnemy;
    bool m_GoingToDesiredPosition;
    bool m_LookingForNextPosition;
    bool m_Attacking;
    bool m_Disabled;
    public float m_TimeSinceLastAuto;
    float m_AttackAnimLength;

    [Header("SUMMONERS")]
    public Summoner m_SummSpell1;
    public Summoner m_SummSpell2;
    public KeyCode m_SummSpell1Key;
    public KeyCode m_SummSpell2Key;

    [Header("SKILLS")]
    public Skill m_PassiveSkill;
    public Skill m_QSkill;
    public Skill m_WSkill;
    public Skill m_ESkill;
    public Skill m_RSkill;
    public KeyCode m_QSkillKey;
    public KeyCode m_WSkillKey;
    public KeyCode m_ESkillKey;
    public KeyCode m_RSkillKey;
    public LayerMask m_DamageLayerMask;
    bool m_ShowingGizmos;
    bool m_UseSkillGizmos;

    [Header("INPUT BUFFERING")]
    public InputBufferController m_InputBufferController;
    public delegate void InputDelegate();
    public InputDelegate m_QInputDelegate;
    public InputDelegate m_WInputDelegate;
    public InputDelegate m_EInputDelegate;
    public InputDelegate m_RInputDelegate;
    public InputDelegate m_Summ1InputDelegate;
    public InputDelegate m_Summ2InputDelegate;

	protected virtual void Start()
	{
        m_CharacterCamera=GetComponent<CameraController>();
        m_CharacterAnimator=GetComponent<Animator>();
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
        SetInitStats();
        SetZeroCooldown(false);
        m_GoingToDesiredPosition=false;
        m_DesiredEnemy=null;

        m_QInputDelegate=QSkill;
        m_WInputDelegate=WSkill;
        m_EInputDelegate=ESkill;
        m_RInputDelegate=RSkill;
        m_Summ1InputDelegate=SummonerSpell1;
        m_Summ2InputDelegate=SummonerSpell2;
	}
    protected virtual void Update()
    {
        MouseTargeting();
        if(!m_Disabled)
        {
            CharacterMovement();
        }

        m_CharacterUI.UpdateHealthManaBars(m_CharacterStats.GetCurrentHealth(), m_CharacterStats.GetMaxHealth(), m_CharacterStats.GetCurrentMana(), m_CharacterStats.GetMaxMana());
        m_CharacterUI.UpdatePrimStats(m_CharacterStats.GetAttackDamage(), m_CharacterStats.GetArmor(), m_CharacterStats.GetAttackSpeed(), m_CharacterStats.GetCritChance(), 
            m_CharacterStats.GetAbilityPower(), m_CharacterStats.GetMagicRes(), m_CharacterStats.GetCdr(), m_CharacterStats.GetMovSpeed());
        m_CharacterUI.UpdateSeconStats(m_CharacterStats.GetHealthRegen(), m_CharacterStats.GetArmorPenFixed(), m_CharacterStats.GetArmorPenPct(), m_CharacterStats.GetLifeSteal(), 
            m_CharacterStats.GetAttackRange(), m_CharacterStats.GetManaRegen(), m_CharacterStats.GetMagicPenFixed(), m_CharacterStats.GetMagicPenPct(), m_CharacterStats.GetOmniDrain(), 
            m_CharacterStats.GetTenacity(), m_CharacterStats.GetShieldsHealsPower());
        if(m_Recalling)
        {
            m_CharacterUI.UpdateCastingUI(m_CurrentRecallTime, m_RecallTime);
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

        if(m_CharacterStats.GetCurrentLevel()<18)
            m_CharacterUI.UpdateExpBar(m_CharacterStats.GetCurrentExp(), m_CharacterStats.m_CharacterBaseStats.m_ExpPerLevel[m_CharacterStats.GetCurrentLevel()]);

        if(!m_Disabled)
        {
            if(Input.GetKeyDown(KeyCode.B))
                UseRecall();

            m_InputBufferController.CheckInputBuffer();
        }

        if(Input.GetKeyDown(m_QSkillKey))
            QSkillInput();
        if(Input.GetKeyDown(m_WSkillKey))
            WSkillInput();
        if(Input.GetKeyDown(m_ESkillKey))
            ESkillInput();
        if(Input.GetKeyDown(m_RSkillKey))
            RSkillInput();

        if(Input.GetKeyDown(m_SummSpell1Key))
            UseSummonerSpell1();
        if(Input.GetKeyDown(m_SummSpell2Key))
            UseSummonerSpell2();
        
    }
    public Vector3 GetMouseDir()
    {
        Vector3 l_MousePosition=Input.mousePosition;
        l_MousePosition.z=10.0f;
        return m_CharacterCamera.m_Camera.ScreenToWorldPoint(l_MousePosition)-m_CharacterCamera.m_Camera.transform.position;
    }
    public Transform GetEnemy()
    {
        Vector3 l_MouseDirection=GetMouseDir();
        RaycastHit l_CameraRaycastHit;
        if(Physics.Raycast(m_CharacterCamera.m_Camera.transform.position, l_MouseDirection, out l_CameraRaycastHit, 1000.0f, m_CharacterCamera.m_CameraLayerMask))
        {
            if(l_CameraRaycastHit.transform.CompareTag("Enemy"))
                return l_CameraRaycastHit.transform;
        }
        return null;
    }
    public CharacterStats GetSelectedCharacterStats()
    {
        Vector3 l_MouseDirection=GetMouseDir();
        RaycastHit l_CameraRaycastHit;
        if(Physics.Raycast(m_CharacterCamera.m_Camera.transform.position, l_MouseDirection, out l_CameraRaycastHit, 1000.0f, m_CharacterCamera.m_SelectHitboxLayerMask))
        {
            if(l_CameraRaycastHit.transform.TryGetComponent(out ITakeDamage Stats))
                return Stats.GetCharacterStats();
        }
        return null;
    }
    public Vector3 GetPosition()
    {
        Vector3 l_MouseDirection=GetMouseDir();
        RaycastHit l_CameraRaycastHit;
        if(Physics.Raycast(m_CharacterCamera.m_Camera.transform.position, l_MouseDirection, out l_CameraRaycastHit, 1000.0f, m_CharacterCamera.m_TerrainLayerMask))
        {
            if(l_CameraRaycastHit.transform.CompareTag("Terrain"))
                return l_CameraRaycastHit.point;
        }
        return Vector3.zero;
    }
    void MouseTargeting()
    {
        if(Input.GetMouseButtonDown(1))
        {
            if(m_UseSkillGizmos && IsAnySkillBeingUsed())
            {
                m_CharacterUI.ClearDeletableSkillIndicatorUI();
			    m_CharacterUI.ClearTargetSkillIndicatorUI();
                m_ShowingGizmos=false;
                StopSkillsCancelableWithMouseClick();
            }
            else
            {
                m_DesiredEnemy=GetEnemy();
                if(m_DesiredEnemy)
                {
                    m_DesiredPosition=m_DesiredEnemy.position;
                    m_DesiredPosition.y=0.0f;
                    m_GoingToDesiredPosition=true;
                    StopRecall();
                }
                m_LookingForNextPosition=true;
            }
        }
        else if(Input.GetMouseButtonUp(1))
            m_LookingForNextPosition=false;
                
        if(Input.GetMouseButtonDown(0)) 
        {
            if(!EventSystem.current.IsPointerOverGameObject())
            {
                CharacterStats l_SelectedCharacterStats=GetSelectedCharacterStats();
                if(l_SelectedCharacterStats)
                    m_CharacterUI.ShowTargetInfoUI(l_SelectedCharacterStats);
                else
                    m_CharacterUI.HideTargetInfoUI();
            }
            m_CharacterUI.ClearDeletableSkillIndicatorUI();
        }

        if(m_LookingForNextPosition)
        {
            if(GetPosition()!=Vector3.zero && !m_DesiredEnemy)
            {
                m_DesiredPosition=GetPosition();
                m_DesiredPosition.y=0.0f;
                m_DesiredEnemy=null;
                m_GoingToDesiredPosition=true;
                StopAttacking();
                StopRecall();
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
                l_MinDistance=m_CharacterStats.GetAttackRange()/100.0f;
            else
                l_MinDistance=0.1f;
            transform.forward=l_CharacterDirection;
            if(Vector3.Distance(transform.position, m_DesiredPosition)>l_MinDistance)
            {
                transform.position+=l_CharacterDirection*(m_CharacterStats.GetMovSpeed()/100.0f)*Time.deltaTime;
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
    public Vector3 GetPositionWithMouse()
    {
        Vector3 l_MousePosition=Input.mousePosition;
        l_MousePosition.z=10.0f;
        Vector3 l_MouseDirection=m_CharacterCamera.m_Camera.ScreenToWorldPoint(l_MousePosition)-m_CharacterCamera.m_Camera.transform.position;
        RaycastHit l_CameraRaycastHit;
        if(Physics.Raycast(m_CharacterCamera.m_Camera.transform.position, l_MouseDirection, out l_CameraRaycastHit, 1000.0f, m_CharacterCamera.m_TerrainLayerMask))
        {
            if(l_CameraRaycastHit.transform.CompareTag("Terrain"))
            {
                return l_CameraRaycastHit.point;
            }
        }
        return m_CharacterCamera.m_Camera.ScreenToWorldPoint(l_MousePosition);
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
	void QSkillInput()
    {
        if(m_QSkill.GetLevel()<=0)
        {
            Debug.Log("Q STILL LOCKED BOBI");
        }
        else if(m_QSkill.GetIsOnCd())
        {
            Debug.Log("Q STILL ON COOLDOWN BOBI");
        }
        else if(m_CharacterStats.GetCurrentMana()<m_QSkill.GetMana())
        {
            Debug.Log("NOT ENOUGH MANA TO USE Q");
        }
        else
        {
            m_InputBufferController.AddInput(new InputBufferAction(InputBufferAction.Action.QPRESSED, m_QInputDelegate));
           // QSkill();
        }
    }
    protected virtual void QSkill()
    {
		m_QSkill.SetTimer(m_QSkill.GetCd());
		m_CharacterUI.m_QSkillCdImage.fillAmount=1.0f;
        m_CharacterUI.m_QSkillCdText.enabled=true;
        m_CharacterStats.SetCurrentMana(m_CharacterStats.GetCurrentMana()-m_QSkill.GetMana());
        m_QSkill.SetIsOnCd(true);
        StartCoroutine(PowersCooldown(m_QSkill));
        StopRecall();
    }
    void WSkillInput()
    {
        if(m_WSkill.GetLevel()<=0)
        {
            Debug.Log("W STILL LOCKED BOBI");
        }
        else if(m_WSkill.GetIsOnCd())
        {
            Debug.Log("W STILL ON COOLDOWN BOBI");
        }
        else if(m_CharacterStats.GetCurrentMana()<m_WSkill.GetMana())
        {
            Debug.Log("NOT ENOUGH MANA TO USE W");
        }
        else
        {
            m_InputBufferController.AddInput(new InputBufferAction(InputBufferAction.Action.WPRESSED, m_WInputDelegate));
            //WSkill();
        }
    }
    protected virtual void WSkill()
    {
        m_WSkill.SetTimer(m_WSkill.GetCd());
        m_CharacterUI.m_WSkillCdImage.fillAmount=1.0f;
        m_CharacterUI.m_WSkillCdText.enabled=true;
        m_CharacterStats.SetCurrentMana(m_CharacterStats.GetCurrentMana()-m_WSkill.GetMana());
        m_WSkill.SetIsOnCd(true);
        StartCoroutine(PowersCooldown(m_WSkill));
        StopRecall();
    }
    void ESkillInput()
    {
        if(m_ESkill.GetLevel()<=0)
        {
            Debug.Log("E STILL LOCKED BOBI");
        }
        else if(m_ESkill.GetIsOnCd())
        {
            Debug.Log("E STILL ON COOLDOWN BOBI");
        }
        else if(m_CharacterStats.GetCurrentMana()<m_ESkill.GetMana())
        {
            Debug.Log("NOT ENOUGH MANA TO USE E");
        }
        else
        {
            m_InputBufferController.AddInput(new InputBufferAction(InputBufferAction.Action.EPRESSED, m_EInputDelegate));
            //ESkill();
        }
    }
    protected virtual void ESkill()
    {
        m_ESkill.SetTimer(m_ESkill.GetCd());
        m_CharacterUI.m_ESkillCdImage.fillAmount=1.0f;
        m_CharacterUI.m_ESkillCdText.enabled=true;
        m_CharacterStats.SetCurrentMana(m_CharacterStats.GetCurrentMana()-m_ESkill.GetMana());
        m_ESkill.SetIsOnCd(true);
        StartCoroutine(PowersCooldown(m_ESkill));
        StopRecall();
    }
    void RSkillInput()
    {
        if(m_RSkill.GetLevel()<=0)
        {
            Debug.Log("R STILL LOCKED BOBI");
        }
        else if(m_RSkill.GetIsOnCd())
        {
            Debug.Log("R STILL ON COOLDOWN BOBI");
        }
        else if(m_CharacterStats.GetCurrentMana()<m_RSkill.GetMana())
        {
            Debug.Log("NOT ENOUGH MANA TO USE R");
        }
        else
        {
            m_InputBufferController.AddInput(new InputBufferAction(InputBufferAction.Action.RPRESSED, m_RInputDelegate));
            //RSkill();
        }
    }
    protected virtual void RSkill()
    {
		m_RSkill.SetTimer(m_RSkill.GetCd());
		m_CharacterUI.m_RSkillCdImage.fillAmount=1.0f;
        m_CharacterUI.m_RSkillCdText.enabled=true;
        m_CharacterStats.SetCurrentMana(m_CharacterStats.GetCurrentMana()-m_RSkill.GetMana());
        m_RSkill.SetIsOnCd(true);
        StartCoroutine(PowersCooldown(m_RSkill));
        StopRecall();
    }
    void UseSummonerSpell1()
    {
        if(m_SummSpell1.GetIsOnCd())
        {
            Debug.Log("SS1 STILL ON COOLDOWN BOBI");
        }
        else
        {
            m_InputBufferController.AddInput(new InputBufferAction(InputBufferAction.Action.DPRESSED, m_Summ1InputDelegate));
        }
    }
    void SummonerSpell1() 
    {
        m_SummSpell1.SetTimer(m_SummSpell1.GetCd());
        m_CharacterUI.m_SumSpell1CdImage.fillAmount=1.0f;
        m_CharacterUI.m_SumSpell1CdText.enabled=true;
        m_SummSpell1.SetIsOnCd(true);
        StartCoroutine(PowersCooldown(m_SummSpell1));
        StopRecall();
    }
    void UseSummonerSpell2()
    {
        if(m_SummSpell2.GetIsOnCd())
        {
            Debug.Log("SS2 STILL ON COOLDOWN BOBI");
        }
        else
        {
            m_InputBufferController.AddInput(new InputBufferAction(InputBufferAction.Action.FPRESSED, m_Summ2InputDelegate));
        }
    }
    void SummonerSpell2() 
    {
        m_SummSpell2.SetTimer(m_SummSpell2.GetCd());
        m_CharacterUI.m_SumSpell2CdImage.fillAmount=1.0f;
        m_CharacterUI.m_SumSpell2CdText.enabled=true;
        m_SummSpell2.SetIsOnCd(true);
        StartCoroutine(PowersCooldown(m_SummSpell2));
        StopRecall();
    }

    IEnumerator PowersCooldown(Power PowerOnCd) 
    {
        while(PowerOnCd.GetIsOnCd())
        {
            PowerOnCd.Tick(Time.deltaTime);
            m_CharacterUI.UpdatePowerUI(PowerOnCd.m_PowerType, PowerOnCd.GetTimer(), PowerOnCd.GetCd(), PowerOnCd.GetZeroCooldown());
            yield return null;
        }
    }    
    void UseRecall()
    {
        if(!m_Recalling)
        {
            m_CharacterUI.SetCastingUIAbilityText("Recall");
            m_CharacterUI.ShowCastingTime();
            m_CharacterUI.ShowCastingUI();
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
            m_CharacterUI.HideCastingUI();
            m_Recalling=false;
        }
    }
    void TeleportToSpawn()
    {
        transform.position=m_RecallTpPoint.position;
        m_Recalling=false;
        m_CharacterUI.HideCastingUI();
    }
    public virtual void LevelUp()
    {
        m_CharacterStats.LevelUp();
        if(m_CharacterStats.GetCurrentLevel()>=18)
            m_CharacterUI.UpdateExpBar(1.0f, 1.0f);
        m_CharacterUI.ShowLevelUpSkillButtons();
        m_CharacterUI.UpdateCharacterLevel(m_CharacterStats.GetCurrentLevel());
    }
    public virtual void SetInitStats()
    {
        m_CharacterStats.SetInitStats();

        m_QSkill.SetInitStats();
        m_WSkill.SetInitStats();
        m_ESkill.SetInitStats();
        m_RSkill.SetInitStats();
        m_SummSpell1.SetInitStats();
        m_SummSpell2.SetInitStats();

        m_CharacterUI.SetPlayer(this);
        m_CharacterUI.SetPlayerName(m_CharacterStats.GetPlayerName());
        m_CharacterUI.SetPowersImages(m_QSkill.m_Sprite, m_WSkill.m_Sprite, m_ESkill.m_Sprite, m_RSkill.m_Sprite, m_SummSpell1.m_Sprite, m_SummSpell2.m_Sprite);
        m_CharacterUI.UpdateCharacterLevel(m_CharacterStats.GetCurrentLevel());
        m_CharacterUI.ResetSkillLevelPoints();
        m_CharacterUI.HideLevelUpSkillButtons();
        m_CharacterUI.ShowLevelUpSkillButtons();
    }
	public void TakeDamage(float PhysDamage, float MagicDamage)
    {
        float l_TotalPhysDamage=PhysDamage/(1.0f+m_CharacterStats.GetArmor()/100.0f);
        float l_TotalMagicDamage=MagicDamage/(1.0f+m_CharacterStats.GetMagicRes()/100.0f);
        if(PhysDamage>0.0f)
        {
            m_CharacterStats.SetCurrentHealth(m_CharacterStats.GetCurrentHealth()-l_TotalPhysDamage);
            Debug.Log("Taking "+PhysDamage+" physical damage, reduced to "+(PhysDamage/(1.0f+m_CharacterStats.GetArmor()/100.0f))+" damage");
        }
        if(MagicDamage>0.0f)
        {
            m_CharacterStats.SetCurrentHealth(m_CharacterStats.GetCurrentHealth()-l_TotalMagicDamage);
            Debug.Log("Taking "+MagicDamage+" magical damage, reduced to "+(MagicDamage/(1.0f+m_CharacterStats.GetMagicRes()/100.0f))+" damage");
        }
        if(m_CurrentRecallTime>0.2f)
            StopRecall();
        m_CharacterUI.SpawnDamageNumbers(l_TotalPhysDamage, l_TotalMagicDamage);
	}
    public bool IsAnySkillBeingUsed()
    {
        return (m_QSkill.GetUsingSkill() && m_QSkill.m_CancelableWithMouseClick) || (m_WSkill.GetUsingSkill() && m_WSkill.m_CancelableWithMouseClick) 
            || (m_ESkill.GetUsingSkill() && m_ESkill.m_CancelableWithMouseClick) || (m_RSkill.GetUsingSkill() && m_RSkill.m_CancelableWithMouseClick);
    }
    public void StopSkills()
    {
        m_QSkill.SetUsingSkill(false);
        m_WSkill.SetUsingSkill(false);
        m_ESkill.SetUsingSkill(false);
        m_RSkill.SetUsingSkill(false);
    }
    public void StopSkillsCancelableWithMouseClick()
    {
        if(m_QSkill.m_CancelableWithMouseClick)
            m_QSkill.SetUsingSkill(false);
        if(m_WSkill.m_CancelableWithMouseClick)
            m_WSkill.SetUsingSkill(false);
        if(m_ESkill.m_CancelableWithMouseClick)
            m_ESkill.SetUsingSkill(false);
        if(m_RSkill.m_CancelableWithMouseClick)
            m_RSkill.SetUsingSkill(false);
    }

    //LLAMADA POR EVENTO EN LA ANIMACION DE AUTOATAQUE
    protected virtual void PerformAutoAttack()
    {
#if UNITY_EDITOR
        Debug.Log("ATTACKING - Since last auto: "+m_TimeSinceLastAuto);
        m_TimeSinceLastAuto=0.0f;
#endif
        m_DesiredEnemy.GetComponent<ITakeDamage>().TakeDamage(m_CharacterStats.GetAttackDamage(), m_CharacterStats.GetAbilityPower());
    }

    //GETTERS & SETTERS
    public CharacterStats GetCharacterStats()
    {
        return m_CharacterStats;
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
    public CharacterUI GetCharacterUI()
    {
        return m_CharacterUI;
    }
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
    public void SetZeroCooldown(bool Active)
    {
        m_QSkill.SetZeroCooldown(Active);
        m_WSkill.SetZeroCooldown(Active);
        m_ESkill.SetZeroCooldown(Active);
        m_RSkill.SetZeroCooldown(Active);
        m_SummSpell1.SetZeroCooldown(Active);
        m_SummSpell2.SetZeroCooldown(Active);
    }
    public bool GetShowingGizmos()
    {
        return m_ShowingGizmos;
    }
    public void SetShowingGizmos(bool True)
    {
        m_ShowingGizmos=True;
    }
    public bool GetUseSkillGizmos() 
    {
        return m_UseSkillGizmos;
    }
    public void SetUseSkillGizmos(bool True) 
    {
        m_UseSkillGizmos=True;
    }
}
