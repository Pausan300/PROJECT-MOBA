using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    List<CharacterMaster> m_CharactersList=new List<CharacterMaster>();
    List<EnemyDummy> m_EnemiesList=new List<EnemyDummy>();

    public static GameManager m_GameManagerInstance;

    void Start()
    {
        if(m_GameManagerInstance==null)
            m_GameManagerInstance=this;
    }
    void Update()
    {
        
    }

    public void AddToPlayerList(CharacterMaster Player) 
    {
        m_CharactersList.Add(Player);
    }
    public List<CharacterMaster> GetPlayersList() 
    {
        return m_CharactersList;
    }

    public void AddToEnemyList(EnemyDummy Enemy) 
    {
        m_EnemiesList.Add(Enemy);
    }
    public List<EnemyDummy> GetEnemiesList() 
    {
        return m_EnemiesList;
    }
}
