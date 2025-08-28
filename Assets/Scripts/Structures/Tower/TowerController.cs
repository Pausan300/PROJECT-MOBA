using System.Collections.Generic;
using UnityEngine;

public class TowerController : MonoBehaviour, ITakeDamageTower
{
    Animator m_TowerAnimator;

    [Header("STATS")]
    public TowerStats m_TowerStats;
    public float m_TimeToRebuild;
    float m_RebuildAnimLength;
    bool m_Destroyed;
    bool m_Untargetable;

    [Header("TYPE")]
    public CharacterStats.TeamType m_TowerType;

    [Header("CONNECTIONS")]
    public TowerController m_PreviousTower;
    public TowerController m_NextTower;
    public LineRenderer m_ConnectionLine;
    
    [Header("CHECKPOINTS")]
    public float m_DamageThreshold;
    bool m_Threshold1Reached;
    bool m_Threshold2Reached;
    bool m_Threshold3Reached;
    bool m_Threshold4Reached;

    [Header("ATTACK")]
    public GameObject m_ProjectilePrefab;
    public Transform m_ShootingPoint;
    public float m_TimeToHit;
    public float m_ProjectileExplosionRadius;
    public LayerMask m_DamageLayerMask;
    public float m_AllyAttackedNearTowerRadius;
    float m_AttackCooldown;
    float m_AttackTimer;

    [Header("TARGETS")]
    public List<GameObject> m_TargetList=new List<GameObject>();
    public GameObject m_CurrentTarget;

    [Header("UI")]
    public IngameTowerUI m_IngameUI;


    void Start() 
    {
        m_AttackCooldown=1.0f/m_TowerStats.GetAttackSpeed();
        m_TowerAnimator=GetComponent<Animator>();
        AnimationClip[] l_Clips = m_TowerAnimator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in l_Clips)
        {
            switch (clip.name)
            {
                case "TowerRebuildAnim":
                    m_RebuildAnimLength = clip.length;
                    break;
            }
        }
        SetAnimatorFloat("RebuildSpeed", m_RebuildAnimLength/m_TimeToRebuild);

        HideConnection();
        if(m_NextTower) 
        {
            SetIsUntargetable(true);
        }
    }

    void Update()
    {
        if(!m_Destroyed) 
        {
            if(m_CurrentTarget)
                AttackCooldown();
            m_IngameUI.UpdateHealthBar(m_TowerStats.GetCurrentHealth(), m_TowerStats.GetMaxHealth());
        }
    }

    void AttackCooldown() 
    {
        m_AttackTimer+=Time.deltaTime;
        if(m_AttackTimer>=m_AttackCooldown)
            ShootProjectile();
    }
    void ShootProjectile() 
    {
        if(m_Destroyed)
            return;

        m_AttackTimer=0.0f;
        GameObject l_Projectile=Instantiate(m_ProjectilePrefab, m_ShootingPoint.position, m_ProjectilePrefab.transform.rotation);
        TowerProjectile l_ProjectileScript=l_Projectile.GetComponent<TowerProjectile>();
        Vector3 l_TargetPos=new Vector3(m_CurrentTarget.transform.position.x, 0.0f, m_CurrentTarget.transform.position.z);
        l_ProjectileScript.SetStats(this, l_TargetPos, m_TimeToHit, m_TowerStats.GetAttackDamage(), m_ProjectileExplosionRadius);
    }
    public void GetClosestTarget() 
    {
        float l_ClosestDist=0.0f;
        GameObject l_ClosestTarget=null;

        for (int i=0; i<m_TargetList.Count; ++i)
        {
            float l_Dist=(m_TargetList[i].transform.position-transform.position).magnitude;
            if (l_Dist<l_ClosestDist || i==0) 
            {
                l_ClosestTarget=m_TargetList[i];
                l_ClosestDist=l_Dist;
            }
        }
        m_CurrentTarget=l_ClosestTarget;
    }
    bool CheckIsEnemy(CharacterStats.TeamType Type)
    {
        if((m_TowerType==CharacterStats.TeamType.ALLY && Type==CharacterStats.TeamType.ENEMY) || (m_TowerType==CharacterStats.TeamType.ENEMY && Type==CharacterStats.TeamType.ALLY))
            return true;
        else
            return false;
    }
    public void SetTarget(GameObject Target) 
    {
        m_CurrentTarget=Target;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent(out ITakeDamage Enemy) && CheckIsEnemy(Enemy.GetCharacterStats().m_TeamType)) 
        {
            Enemy.SetNearTower(this);
            m_TargetList.Add(other.gameObject);

            if(!m_CurrentTarget) 
            {
                m_CurrentTarget=other.gameObject;
                ShootProjectile();
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.TryGetComponent(out ITakeDamage Enemy) && CheckIsEnemy(Enemy.GetCharacterStats().m_TeamType)) 
        {
            Enemy.SetNearTower(null);

            if(m_TargetList.Contains(other.gameObject))
                m_TargetList.Remove(other.gameObject);

            if(m_CurrentTarget=other.gameObject) 
            {
                if(m_TargetList.Count>0)
                    GetClosestTarget();
                else
                    m_CurrentTarget=null;
            }
        }
    }

    public void TakeDamage(float PhysDamage, float MagicDamage, bool IgnoreResistances, string SourceId)
    {
        if(m_Destroyed || m_Untargetable) 
            return;

        float l_TotalPhysDamage = PhysDamage / (1.0f + m_TowerStats.GetArmor() / 100.0f);
        float l_TotalMagicDamage = MagicDamage / (1.0f + m_TowerStats.GetMagicRes() / 100.0f);
        if (PhysDamage > 0.0f)
            Debug.Log("Taking " + PhysDamage + " physical damage, reduced to " + l_TotalPhysDamage + " damage");
        if (MagicDamage > 0.0f)
            Debug.Log("Taking " + MagicDamage + " magical damage, reduced to " + l_TotalMagicDamage + " damage");
        m_TowerStats.SetCurrentHealthRpc(m_TowerStats.GetCurrentHealth() - (l_TotalPhysDamage + l_TotalMagicDamage));
        m_IngameUI.AddDamageInstance(l_TotalPhysDamage, l_TotalMagicDamage, SourceId);

        if(!m_Threshold1Reached && m_TowerStats.GetCurrentHealth()<=m_TowerStats.GetMaxHealth()-m_DamageThreshold) 
        {
            m_Threshold1Reached=true;
        }
        else if(!m_Threshold2Reached && m_TowerStats.GetCurrentHealth()<=m_TowerStats.GetMaxHealth()-(m_DamageThreshold*2.0f)) 
        {
            m_Threshold2Reached=true;
        }
        else if(!m_Threshold3Reached && m_TowerStats.GetCurrentHealth()<=m_TowerStats.GetMaxHealth()-(m_DamageThreshold*3.0f)) 
        {
            m_Threshold3Reached=true;
        }
        else if(!m_Threshold4Reached && m_TowerStats.GetCurrentHealth()<=m_TowerStats.GetMaxHealth()-(m_DamageThreshold*4.0f)) 
        {
            m_Threshold4Reached=true;
            DestroyTower();
        }
    }
    void DestroyTower() 
    {
        SetAnimatorTrigger("Destroyed");
        if(m_PreviousTower) 
        {
            m_PreviousTower.NextTowerDestroyed();
        }
        if(m_NextTower) 
        {
            m_NextTower.SetAnimatorBool("Rebuild", false);
            HideConnection();
        }
        m_Destroyed=true;
        SetIsUntargetable(true);
    }
    //Llamada en animacion TowerRebuildAnim
    void RebuildTower() 
    {
        m_Destroyed=false;
        m_Threshold1Reached=false;
        m_Threshold2Reached=false;
        m_Threshold3Reached=false;
        m_Threshold4Reached=false;
        m_TowerStats.SetCurrentHealthRpc(m_TowerStats.GetMaxHealth());
        m_IngameUI.UpdateHealthBar(m_TowerStats.GetCurrentHealth(), m_TowerStats.GetMaxHealth());

        if(m_NextTower) 
        {
            NextTowerDestroyed();
        }
        else 
        {
            SetIsUntargetable(false);
        }
        m_PreviousTower.NextTowerRebuild();
    }

    public void NextTowerDestroyed() 
    {
        SetIsUntargetable(false);
        ConnectTower();
        m_NextTower.SetAnimatorBool("Rebuild", true);
    }
    public void NextTowerRebuild() 
    {
        SetIsUntargetable(true);
        HideConnection();
    }
    public void ConnectTower()
    {
        m_ConnectionLine.gameObject.SetActive(true);
        Vector3 l_Point1=m_ConnectionLine.transform.localPosition;
        l_Point1.y=0.0f;
        m_ConnectionLine.SetPosition(0, l_Point1);
        Vector3 l_Point2=transform.InverseTransformPoint(m_NextTower.m_ConnectionLine.transform.position);
        l_Point2.y=0.0f;
        m_ConnectionLine.SetPosition(1, l_Point2);
    }
    public void HideConnection() 
    {
        m_ConnectionLine.gameObject.SetActive(false);
    }

    //GETTERS & SETTERS
    public TowerStats GetTowerStats()
    {
        return m_TowerStats;
    } 
    public void SetAnimatorBool(string Name, bool True)
    {
        m_TowerAnimator.SetBool(Name, True);
    }
    public void SetAnimatorTrigger(string Name)
    {
        m_TowerAnimator.SetTrigger(Name);
    }
    public void SetAnimatorFloat(string Name, float Num) 
    {
        m_TowerAnimator.SetFloat(Name, Num);
    }
    public bool GetIsDestroyed()
    {
        return m_Destroyed;
    }
    public void SetIsDestroyed(bool Destroyed) 
    {
        m_Destroyed=Destroyed;
    }
    public bool GetIsUntargetable()
    {
        return m_Untargetable;
    }
    public void SetIsUntargetable(bool Untargetable) 
    {
        m_Untargetable=Untargetable;
        if(m_Untargetable)
            m_IngameUI.m_IngameHealthBar.gameObject.SetActive(false);
        else
            m_IngameUI.m_IngameHealthBar.gameObject.SetActive(true);
    }
}
