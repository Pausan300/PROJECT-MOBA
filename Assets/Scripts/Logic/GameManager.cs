using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager m_GameManagerInstance;

    private NetworkVariable<float> m_GameTimer = new NetworkVariable<float>();
    bool m_GameStarted;

    [Header("SCENE ACTORS LISTS")]
    List<CharacterMaster> m_CharactersList = new List<CharacterMaster>();
    List<EnemyDummy> m_DummiesList = new List<EnemyDummy>();
    List<MinionController> m_MinionsList = new List<MinionController>();
    List<TowerController> m_TowersList=new List<TowerController>();


    void Awake()
    {
        if (m_GameManagerInstance == null)
            m_GameManagerInstance = this;

        foreach(TowerController Tower in FindObjectsByType<TowerController>(FindObjectsSortMode.None))
            m_TowersList.Add(Tower);
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
        }
    }

    public void AddToPlayerList(CharacterMaster Player)
    {
        m_CharactersList.Add(Player);
        StartGame();
    }
    public List<CharacterMaster> GetPlayersList()
    {
        return m_CharactersList;
    }
    void StartGame() 
    {
        if(m_GameStarted)
            return;
        m_GameStarted = true;
        foreach(TowerController Tower in m_TowersList)
            Tower.m_IngameUI.SetCameraController(m_CharactersList[0].GetCameraController());
    }

    public void AddToEnemyList(EnemyDummy Enemy)
    {
        m_DummiesList.Add(Enemy);
        //Enemy.SetIngameUICamera(m_CharactersList[0].GetCameraController());
    }
    public List<EnemyDummy> GetEnemiesList()
    {
        m_DummiesList.RemoveAll(e => e == null || e.gameObject == null);
        return m_DummiesList;
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
