using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager m_GameManagerInstance;

    private NetworkVariable<float> m_GameTimer = new NetworkVariable<float>();
    bool m_GameStarted;

    //PASAR ESTO A SU PROPIO SCRIPT DE SPAWNEO DE ENEMIGOS
    [Header("ENEMY SPAWNING")]
    public GameObject m_EnemyPrefab;
    public List<Transform> m_EnemySpawnPoints;
    public List<Transform> m_EnemyTargetPoints;
    public int m_EnemyQuantity;
    public float m_TimeBetweenWaves;
    float m_LastSpawnedWaveTime;
    //

    [Header("SCENE ACTORS LISTS")]
    List<CharacterMaster> m_CharactersList = new List<CharacterMaster>();
    List<EnemyDummy> m_EnemiesList = new List<EnemyDummy>();


    void Awake()
    {
        if (m_GameManagerInstance == null)
            m_GameManagerInstance = this;
    }
    void Update()
    {
        if (!IsSpawned || !HasAuthority)
        {
            return;
        }

        if (m_GameStarted)
        {
            m_GameTimer.Value += Time.deltaTime;
            if (m_GameTimer.Value - m_LastSpawnedWaveTime >= m_TimeBetweenWaves)
            {
                m_LastSpawnedWaveTime = m_GameTimer.Value;
                StartCoroutine(SpawnEnemies());
            }
        }
    }

    IEnumerator SpawnEnemies()
    {
        int l_Point = 0;
        for (int i = 0; i < m_EnemyQuantity; ++i)
        {
            SetupEnemyRpc(l_Point);
            l_Point++;
            if (l_Point >= m_EnemySpawnPoints.Count)
                l_Point = 0;
            yield return new WaitForSeconds(0.5f);
        }
    }
    [Rpc(SendTo.Everyone)]
    void SetupEnemyRpc(int Index)
    {
        if (!IsSpawned || !HasAuthority)
        {
            return;
        }

        GameObject l_Enemy = Instantiate(m_EnemyPrefab, m_EnemySpawnPoints[Index].position + new Vector3(0.0f, 1.0f, 0.0f), m_EnemyPrefab.transform.rotation);
        EnemyDummy l_EnemyScript = l_Enemy.GetComponent<EnemyDummy>();
        l_EnemyScript.SetMovement(true);
        l_EnemyScript.SetMovementTarget(m_EnemyTargetPoints[Index]);
        l_Enemy.GetComponent<NetworkObject>().Spawn();
    }

    public void AddToPlayerList(CharacterMaster Player)
    {
        m_CharactersList.Add(Player);
        m_GameStarted = true;
    }
    public List<CharacterMaster> GetPlayersList()
    {
        return m_CharactersList;
    }

    public void AddToEnemyList(EnemyDummy Enemy)
    {
        m_EnemiesList.Add(Enemy);
        //Enemy.SetIngameUICamera(m_CharactersList[0].GetCameraController());
    }
    public List<EnemyDummy> GetEnemiesList()
    {
        m_EnemiesList.RemoveAll(e => e == null || e.gameObject == null);
        return m_EnemiesList;
    }


    public float GetGameTimer()
    {
        return m_GameTimer.Value;
    }
    public string GetGameTimerFormated()
    {
        return string.Format("{0:00}:{1:00}", Mathf.Floor(m_GameTimer.Value / 60.0f), Mathf.Floor(m_GameTimer.Value % 60.0f));
    }
}
