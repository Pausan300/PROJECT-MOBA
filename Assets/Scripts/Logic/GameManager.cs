using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager m_GameManagerInstance;

    private NetworkVariable<float> m_GameTimer = new NetworkVariable<float>();
    bool m_GameStarted;
    
    public event Action<CameraController> m_AssignCameras;

    [Header("SCENE ACTORS LISTS")]
    List<CharacterMaster> m_CharactersList = new List<CharacterMaster>();
    List<EnemyDummy> m_DummiesList = new List<EnemyDummy>();
    List<MinionController> m_MinionsList = new List<MinionController>();
    List<EnergyWall> m_EnergyWallsList=new List<EnergyWall>();


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
        }
    }

    void StartGame() 
    {
        if(m_GameStarted)
            return;
        m_GameStarted = true;
        m_AssignCameras?.Invoke(m_CharactersList[0].GetCameraController());
    }
    public void EndGame(bool Victory) 
    {
        if(Victory) 
        {
        }
        else 
        {
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
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
    public bool GetGameStarted() 
    {
        return m_GameStarted;
    }
}
