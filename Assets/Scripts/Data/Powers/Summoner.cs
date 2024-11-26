using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Powers/Summoner")]
public class Summoner : Power
{
    public float m_SummonerCooldown; 
    
    public override void SetInitStats()
    {
        base.SetInitStats();
        SetCd(m_SummonerCooldown);
    }
}