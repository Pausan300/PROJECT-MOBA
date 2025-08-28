using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZappadasRSkill : MonoBehaviour
{
    public GameObject m_Circle;
    ZappadasCharacterController m_CharacterController;

    public float m_RBaseAttraction = 200;
    public float m_REdgeAttraction = 500;
    public float m_RAttractionTime = 0.7f;

    public float m_RRadius_01 = 300;
    public float m_RRadius_02 = 700;
    public float m_RRadius_03 = 1100;
    public float m_RRadius_04 = 1500;

    [UnityEngine.Range(0f, 100f)]
    public float m_PercentageSkillPowerR1 = 65;
    [UnityEngine.Range(0f, 100f)]
    public float m_PercentageSkillPowerR2 = 90;

    public float m_REdgeWidth = 200;

    public float m_RExpansionTime = 0.5f;
    public float m_RContractionTime = 0.5f;
    private float[] m_Radii;

    bool m_CanDestroy = false;

    List<ZappadasEnemysToAbsorb> m_EnemysToAbsorb = new List<ZappadasEnemysToAbsorb>();
    List<ZappadasEnemysToAbsorb> m_EnemysToAbsorbEdge = new List<ZappadasEnemysToAbsorb>();

    public void SetRSkill(ZappadasCharacterController _ZappadasCharacterController)
    {
        m_CharacterController = _ZappadasCharacterController;
        m_EnemysToAbsorb = new List<ZappadasEnemysToAbsorb>();
        m_EnemysToAbsorbEdge = new List<ZappadasEnemysToAbsorb>();
        m_CanDestroy = false;
        m_Radii = new float[]
        {
            m_RRadius_01 / 100f,
            m_RRadius_02 / 100f,
            m_RRadius_03 / 100f,
            m_RRadius_04 / 100f
        };

        StartCoroutine(ScaleRoutine());
    }
    private IEnumerator ScaleRoutine()
    {
        for (int i = 0; i < m_Radii.Length; i++)
        {
            bool l_DamageDone = false;
            float targetRadius = m_Radii[i];
            Vector3 startScale = new Vector3(0f, 1f, 0f);
            Vector3 endScale = new Vector3(targetRadius * 2, 1f, targetRadius * 2);

            float elapsed = 0f;
            while (elapsed < m_RExpansionTime)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / m_RExpansionTime;
                m_Circle.transform.localScale = Vector3.Lerp(startScale, endScale, t);
                yield return null;

                if (elapsed > m_RExpansionTime - 0.1f && !l_DamageDone)
                {
                    l_DamageDone = true;
                    OnReachMaxRadius(i + 1);
                }
            }
            m_Circle.transform.localScale = endScale;

            

            elapsed = 0f;
            while (elapsed < m_RContractionTime)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / m_RContractionTime;
                m_Circle.transform.localScale = Vector3.Lerp(endScale, startScale, t);
                yield return null;
            }
            m_Circle.transform.localScale = startScale;
        }

        m_CanDestroy = true;
    }


    private void OnReachMaxRadius(int rangeIndex)
    {
        float l_ActualRadius = 0;
        int l_ActualThis = 1;
        switch (rangeIndex)
        {
            case 1:
                l_ActualRadius = m_RRadius_01 / 100;
                l_ActualThis = 1;
                break;

            case 2:
                l_ActualRadius = m_RRadius_02 / 100;
                l_ActualThis = 2;
                break;

            case 3:
                l_ActualRadius = m_RRadius_03 / 100;
                l_ActualThis = 3;
                break;

            case 4:
                l_ActualRadius = m_RRadius_04 / 100;
                l_ActualThis = 4;
                break;

            default:
                l_ActualThis = 1;
                Debug.LogWarning("Rango no reconocido: " + rangeIndex);
                break;
        }

        List<Collider> l_CollidersHit = new List<Collider>();
        Collider[] l_HitColliders = Physics.OverlapSphere(transform.position, l_ActualRadius, m_CharacterController.m_DamageLayerMask);


        foreach (Collider Entity in l_HitColliders)
        {
            if (!l_CollidersHit.Contains(Entity) && Entity.TryGetComponent(out ITakeDamage Enemy))
            {
                float l_Damage;
                float l_Distance = Vector3.Distance(transform.position, Entity.transform.position);

                float r1 = m_RRadius_01 / 100f;
                float r2 = m_RRadius_02 / 100f;
                float r3 = m_RRadius_03 / 100f;
                float r4 = m_RRadius_04 / 100f;
                float edgeWidth = m_REdgeWidth / 100f;

                bool l_Borde = false;

                if (l_Distance < r1)
                {
                    l_Borde = true;
                }
                else if ((l_Distance >= r2 - edgeWidth && l_Distance <= r2) ||
                         (l_Distance >= r3 - edgeWidth && l_Distance <= r3) ||
                         (l_Distance >= r4 - edgeWidth && l_Distance <= r4))
                {
                    l_Borde = true;
                }
                Debug.Log("Distance: " + l_Distance + ",   Edge? " + l_Borde);
                if (l_Borde)
                {
                    l_Damage = m_CharacterController.m_RSkill.GetAttribute("Daño en el borde", m_CharacterController.GetRSkillLevel())
                             + (m_PercentageSkillPowerR2 / 100f) * m_CharacterController.GetCharacterStats().GetAbilityPower()
                             + m_CharacterController.m_RSkill.GetAttribute("Daño en el borde", m_CharacterController.GetRSkillLevel()) * l_ActualThis;
                    ZappadasEnemysToAbsorb zappadasEnemysToAbsorb = new ZappadasEnemysToAbsorb();
                    zappadasEnemysToAbsorb.transform = Entity.transform;
                    zappadasEnemysToAbsorb.timeAbsorbed = 0;
                    m_EnemysToAbsorbEdge.Add(zappadasEnemysToAbsorb);
                }
                else
                {
                    l_Damage = m_CharacterController.m_RSkill.GetAttribute("Daño base", m_CharacterController.GetRSkillLevel())
                             + (m_PercentageSkillPowerR1 / 100f) * m_CharacterController.GetCharacterStats().GetAbilityPower();

                    ZappadasEnemysToAbsorb zappadasEnemysToAbsorb = new ZappadasEnemysToAbsorb();
                    zappadasEnemysToAbsorb.transform = Entity.transform;
                    zappadasEnemysToAbsorb.timeAbsorbed = 0;
                    m_EnemysToAbsorb.Add(zappadasEnemysToAbsorb);
                }


                Debug.Log("TAKEN " + l_Damage + " DAMAGE");
                Enemy.TakeDamage(0, l_Damage, false, m_CharacterController.m_CharacterStats.GetPlayerName(), m_CharacterController.gameObject);
                m_CharacterController.AddmDarkPowerDamageLightlessWithSkill(Entity.gameObject);
                l_CollidersHit.Add(Entity);

            }
        }





    }

    void UpdateAbsorptionList(List<ZappadasEnemysToAbsorb> list, float attractionRange)
    {
        for (int i = list.Count - 1; i >= 0; i--)
        {
            ZappadasEnemysToAbsorb enemyData = list[i];
            if (enemyData.transform == null)
            {
                list.RemoveAt(i);
                continue;
            }

            Vector3 currentPos = enemyData.transform.position;
            Vector3 targetPos = new Vector3(transform.position.x, currentPos.y, transform.position.z);

            Vector3 directionXZ = new Vector3(targetPos.x - currentPos.x, 0f, targetPos.z - currentPos.z);
            float distanceXZ = directionXZ.magnitude;

            enemyData.timeAbsorbed += Time.deltaTime;
            float t = Mathf.Clamp01(enemyData.timeAbsorbed / m_RAttractionTime);

            Vector3 previousPosXZ = new Vector3(currentPos.x, 0f, currentPos.z);

            Vector3 newPosXZ = Vector3.Lerp(
                previousPosXZ,
                new Vector3(targetPos.x, 0f, targetPos.z),
                t
            );

            float deltaDistance = Vector3.Distance(previousPosXZ, newPosXZ);
            enemyData.m_TraveledDistance += deltaDistance;

            enemyData.transform.position = new Vector3(newPosXZ.x, currentPos.y, newPosXZ.z);

            if (enemyData.m_TraveledDistance >= attractionRange || enemyData.timeAbsorbed >= m_RAttractionTime)
            {
                list.RemoveAt(i);
            }
        }
    }




    void Update()
    {

        float baseAttractionRange = m_RBaseAttraction / 100f;
        float edgeAttractionRange = m_REdgeAttraction / 100f;

        UpdateAbsorptionList(m_EnemysToAbsorb, baseAttractionRange);
        UpdateAbsorptionList(m_EnemysToAbsorbEdge, edgeAttractionRange);

        if (m_CanDestroy)
        {
            if (m_EnemysToAbsorbEdge.Count == 0 && m_EnemysToAbsorb.Count == 0)
            {
                Destroy(gameObject);
            }
        }
    }



}

public class ZappadasEnemysToAbsorb
{
    public Transform transform;
    public float timeAbsorbed;
    public float m_TraveledDistance;

}