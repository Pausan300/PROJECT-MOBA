using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour
{
    public Image m_QSkillCdImage;
    public Image m_WSkillCdImage;
    public Image m_ESkillCdImage;
    public Image m_RSkillCdImage;

    [Header("HEALTH MANA BARS")]
    public Slider m_HealthBar;
    public Slider m_ManaBar;
    public TextMeshProUGUI m_HealthText;
    public TextMeshProUGUI m_ManaText;
    
    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    public void UpdateHealthManaBars(float Health, float MaxHealth, float Mana, float MaxMana)
    {
        m_HealthBar.value=Health/MaxHealth;
        m_ManaBar.value=Mana/MaxMana;
        m_HealthText.text=Mathf.Round(Health)+"/"+MaxHealth;
        m_ManaText.text=Mathf.Round(Mana)+"/"+MaxMana;
    }
}
