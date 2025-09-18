using UnityEngine;

public class EnergyWall : MonoBehaviour
{
    [Header("WALL TYPE")]
    public DamageInstance.DamageType m_Type;
    public Material m_PhysicalMaterial;
    public Material m_MagicMaterial;
    public MeshRenderer m_PortalMesh;
    public float m_ChangeTypeMaxTime; 
    float m_LastChangeTime;

    [Header("CRYSTALS")]
    public GameObject m_CrystalPrefab;
    public float m_CrystalSpawnMinRadius;
    public float m_CrystalSpawnMaxRadius;
    public float m_CrystalSpawnMaxTime;
    float m_LastSpawnTime;

    
    void Start()
    {
        ChangeMaterial();
    }
    void Update()
    {
        float l_Timer=GameManager.m_GameManagerInstance.GetGameTimer();
        UpdateChangeTypeTimer(l_Timer);
        UpdateCrystalSpawnTimer(l_Timer);
    }

    void UpdateChangeTypeTimer(float Delta) 
    {
        if(Delta-m_LastChangeTime>=m_ChangeTypeMaxTime) 
        {
            ChangePortalType(); 
            m_LastChangeTime=Delta;
        }
    }
    void ChangePortalType() 
    {
        if(m_Type==DamageInstance.DamageType.PHYSICAL)
            m_Type=DamageInstance.DamageType.MAGIC;
        else
            m_Type=DamageInstance.DamageType.PHYSICAL;
        ChangeMaterial();
    }
    void ChangeMaterial() 
    {
        if(m_Type==DamageInstance.DamageType.PHYSICAL)
            m_PortalMesh.material=m_PhysicalMaterial;
        else
            m_PortalMesh.material=m_MagicMaterial;
    }

    void UpdateCrystalSpawnTimer(float Delta) 
    {
        if(Delta-m_LastSpawnTime>=m_CrystalSpawnMaxTime) 
        {
            SpawnCrystal();
            m_LastSpawnTime=Delta;
        }
    }
    void SpawnCrystal() 
    {
        float l_RandomRange=Random.Range(m_CrystalSpawnMinRadius, m_CrystalSpawnMaxRadius)/100.0f;
        float l_RandomAngle=Random.Range(0f, 360f);
        l_RandomAngle*=Mathf.Deg2Rad;
        Vector3 l_Pos=transform.position+new Vector3(Mathf.Cos(l_RandomAngle), 0f, Mathf.Sin(l_RandomAngle))*l_RandomRange;
        GameObject l_Crystal=Instantiate(m_CrystalPrefab, l_Pos, m_CrystalPrefab.transform.rotation, null);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Projectile Projectile)) 
        {
            Projectile.GetDamageInstance().ChangeDamageType(m_Type);
        }
    }
}
