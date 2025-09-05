using System.Collections;
using UnityEngine;

public class TowerProjectile : MonoBehaviour
{
    public GameObject m_Projectile;
    public SpriteRenderer m_Area;

    TowerController m_Tower;

    float m_Speed;
    float m_Damage;
    float m_ExplosionRadius;
    Vector3 m_TargetPos;
    Vector3 m_Dir;
    bool m_ReachedTarget;

    void Update()
    {
        if(!m_ReachedTarget) 
        {
            transform.position+=m_Dir*m_Speed*Time.deltaTime;
            if(Vector3.Distance(transform.position, m_TargetPos)<=0.1f)
                StartCoroutine(Explode());
        }
    }

    IEnumerator Explode() 
    {
        m_ReachedTarget=true;
        m_Projectile.SetActive(false);
        m_Area.gameObject.SetActive(true);
        Vector3 l_IndicatorPos=m_TargetPos;
        l_IndicatorPos.y+=0.1f;
        m_Area.transform.localPosition=transform.InverseTransformPoint(l_IndicatorPos);
        m_Area.transform.eulerAngles=new Vector3(90.0f, 0.0f, 0.0f);
        Collider[] l_HitColliders=Physics.OverlapSphere(m_TargetPos, m_ExplosionRadius/100.0f, m_Tower.m_DamageLayerMask);
        foreach(Collider Entity in l_HitColliders)
		{
            if(Entity.TryGetComponent(out ITakeDamage Target) && CheckIsEnemy(Target.GetCharacterStats().m_TeamType)) 
            {
                if(Target.GetCharacterStats().GetEnemyType()==CharacterStats.EnemyType.MINION) 
                {
                    MinionController l_Minion=Entity.GetComponent<MinionController>();
		            Target.TakeDamage(Target.GetCharacterStats().GetMaxHealth()*l_Minion.m_TowerDamagePct, 0.0f, false, "Tower", gameObject);
                }
                else
		            Target.TakeDamage(m_Damage, 0.0f, false, "Tower", gameObject);

                if(Target.GetCharacterStats().GetCurrentHealth()<=0.0f)
                    m_Tower.RemoveTargetFromList(Entity.gameObject, Target.GetCharacterStats().m_EnemyType);
            }
        }
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }

    public void SetStats(TowerController Tower, Vector3 TargetPos, float TimeToHit, float Damage, float Radius) 
    {
        m_Tower=Tower;
        m_TargetPos=TargetPos;
        m_Dir=(m_TargetPos-transform.position).normalized;
        float l_Dist=(m_TargetPos-transform.position).magnitude;
        m_Speed=l_Dist/TimeToHit;
        transform.forward=m_Dir;
        m_Damage=Damage;
        m_ExplosionRadius=Radius;

        float l_AreaWidth=m_ExplosionRadius*2.0f/m_Area.sprite.rect.width;
        m_Area.transform.localScale=new Vector2(l_AreaWidth, l_AreaWidth);
        m_Area.gameObject.SetActive(false);
    }

    bool CheckIsEnemy(CharacterStats.TeamType Type)
    {
        if((m_Tower.m_TowerStats.m_TeamType==CharacterStats.TeamType.ALLY && Type==CharacterStats.TeamType.ENEMY) || 
            (m_Tower.m_TowerStats.m_TeamType==CharacterStats.TeamType.ENEMY && Type==CharacterStats.TeamType.ALLY))
            return true;
        else
            return false;
    }
}
