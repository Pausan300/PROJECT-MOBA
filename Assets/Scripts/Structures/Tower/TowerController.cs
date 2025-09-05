using System.Collections.Generic;
using UnityEngine;

public class TowerController : MonoBehaviour, ITakeDamageStructure
{
    Animator m_TowerAnimator;

    [Header("STATS")]
    public StructureStats m_TowerStats;
    public float m_TimeToRebuild;
    float m_RebuildAnimLength;
    bool m_Destroyed;
    bool m_Untargetable;

    [Header("CONNECTIONS")]
    public TowerController m_PreviousTower;
    public TowerController m_NextTower;
    public LineRenderer m_ConnectionLine;
    public NexusController m_Nexus;
    
    [Header("CHECKPOINTS")]
    public float m_DamageThreshold;
    bool m_Threshold1Reached;
    bool m_Threshold2Reached;
    bool m_Threshold3Reached;
    bool m_Threshold4Reached;

    [Header("ATTACK")]
    public GameObject m_ProjectilePrefab;
    public Transform m_ShootingPoint;
    public SphereCollider m_AttackRadiusCollider;
    public float m_TimeToHit;
    public float m_ProjectileExplosionRadius;
    public LayerMask m_DamageLayerMask;
    public float m_AllyAttackedNearTowerRadius;
    float m_AttackCooldown;
    float m_AttackTimer;

    [Header("TARGETS")]
    List<GameObject> m_MinionTargetList=new List<GameObject>();
    List<GameObject> m_CharacterTargetList=new List<GameObject>();
    GameObject m_CurrentTarget;

    [Header("UI")]
    public IngameStructureUI m_IngameUI;


    void Start() 
    {
        m_AttackCooldown=1.0f/m_TowerStats.GetAttackSpeed();
        m_AttackRadiusCollider.radius=m_TowerStats.GetAttackRange()/100.0f;
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
            SetIsUntargetable(true);
        GameManager.m_GameManagerInstance.m_AssignCameras+=m_IngameUI.SetCameraController;
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
    void TryPickNewTarget() 
    {
        if(m_MinionTargetList.Count>0)
            GetClosestTarget(m_MinionTargetList);
        else if(m_CharacterTargetList.Count>0)
            GetClosestTarget(m_CharacterTargetList);
        else
            m_CurrentTarget=null;
    }
    public void GetClosestTarget(List<GameObject> TargetList) 
    {
        float l_ClosestDist=0.0f;
        GameObject l_ClosestTarget=null;

        for (int i=0; i<TargetList.Count; ++i)
        {
            float l_Dist=(TargetList[i].transform.position-transform.position).magnitude;
            if (l_Dist<l_ClosestDist || i==0) 
            {
                l_ClosestTarget=TargetList[i];
                l_ClosestDist=l_Dist;
            }
        }
        m_CurrentTarget=l_ClosestTarget;
    }
    bool CheckIsEnemy(CharacterStats.TeamType TeamType)
    {
        if((m_TowerStats.m_TeamType==CharacterStats.TeamType.ALLY && TeamType==CharacterStats.TeamType.ENEMY) || 
            (m_TowerStats.m_TeamType==CharacterStats.TeamType.ENEMY && TeamType==CharacterStats.TeamType.ALLY))
            return true;
        else
            return false;
    }
    void AddTargetToList(GameObject Target, CharacterStats.EnemyType EnemyType) 
    {
        if(EnemyType==CharacterStats.EnemyType.MINION)
            m_MinionTargetList.Add(Target);
        else
            m_CharacterTargetList.Add(Target);
    }
    public void RemoveTargetFromList(GameObject Target, CharacterStats.EnemyType EnemyType) 
    {
        if(EnemyType==CharacterStats.EnemyType.MINION) 
        {
            if(m_MinionTargetList.Contains(Target))
                m_MinionTargetList.Remove(Target);
        }
        else 
        {
            if(m_CharacterTargetList.Contains(Target))
                m_CharacterTargetList.Remove(Target);
        }

        if(m_CurrentTarget==Target)
            TryPickNewTarget();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent(out ITakeDamage Enemy) && CheckIsEnemy(Enemy.GetCharacterStats().m_TeamType)) 
        {
            Enemy.SetNearTower(this);

            AddTargetToList(other.gameObject, Enemy.GetCharacterStats().m_EnemyType);

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

            RemoveTargetFromList(other.gameObject, Enemy.GetCharacterStats().m_EnemyType);

            if(m_CurrentTarget==other.gameObject)
                TryPickNewTarget();
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
        if(m_Nexus)
            m_Nexus.SetIsUntargetable(false);
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
    public StructureStats GetStructureStats()
    {
        return m_TowerStats;
    } 
    public List<GameObject> GetTargetList() 
    {
        return m_MinionTargetList;
    }
    public GameObject GetCurrentTarget() 
    {
        return m_CurrentTarget;
    }
    public void SetCurrentTarget(GameObject Target) 
    {
        m_CurrentTarget=Target;
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
