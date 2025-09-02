using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[System.Serializable]
public class Lane 
{
    public Transform m_SpawnPoint;
    public List<Transform> m_PathPoints;
    public List<WaveEnemies> m_WaveEnemies;
}
[System.Serializable]
public class WaveEnemies 
{
    public GameObject m_EnemyPrefab;
    public int m_Amount;
}

public class EnemySpawner : NetworkBehaviour
{
    [Header("ENEMY SPAWNING")]
    public List<Lane> m_Lanes;
    public float m_TimeForFirstWave;
    bool m_FirstWaveSpawned;
    public float m_TimeBetweenWaves;
    public float m_DelayBetweenSpawns;
    float m_LastSpawnedWaveTime;

    void Update()
    {
        float l_WaitTime=m_TimeBetweenWaves;
        if(!m_FirstWaveSpawned)
            l_WaitTime=m_TimeForFirstWave;

        if(GameManager.m_GameManagerInstance.GetGameTimer()-m_LastSpawnedWaveTime>=l_WaitTime)
        {
            m_LastSpawnedWaveTime=GameManager.m_GameManagerInstance.GetGameTimer();
            for(int lane=0; lane<m_Lanes.Count; ++lane) 
                StartCoroutine(SpawnEnemies(lane));
        }
    }

    IEnumerator SpawnEnemies(int LaneIndex)
    {
        for(int i=0; i<m_Lanes[LaneIndex].m_WaveEnemies.Count; ++i) 
        {
            for(int enemy=0; enemy<m_Lanes[LaneIndex].m_WaveEnemies[i].m_Amount; ++enemy)
            {
                SetupEnemyRpc(i, LaneIndex);
                yield return new WaitForSeconds(m_DelayBetweenSpawns);
            }
        }   
    }

    [Rpc(SendTo.Everyone)]
    void SetupEnemyRpc(int EnemyIndex, int LaneIndex)
    {
        if(!IsSpawned || !HasAuthority)
        {
            return;
        }

        GameObject l_Enemy = Instantiate(m_Lanes[LaneIndex].m_WaveEnemies[EnemyIndex].m_EnemyPrefab, m_Lanes[LaneIndex].m_SpawnPoint.position+new Vector3(0.0f, 1.0f, 0.0f), 
            m_Lanes[LaneIndex].m_WaveEnemies[EnemyIndex].m_EnemyPrefab.transform.rotation);
        EnemyMovement l_EnemyScript=l_Enemy.GetComponent<EnemyMovement>();
        l_EnemyScript.SetMovement(true);
        l_EnemyScript.SetMovementPoints(m_Lanes[LaneIndex].m_PathPoints);
        l_Enemy.GetComponent<NetworkObject>().Spawn();
    }
}
