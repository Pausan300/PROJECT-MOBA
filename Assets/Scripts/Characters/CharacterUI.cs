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
        float l_HealthRounded=Mathf.Round(Health);
        float l_ManaRounded=Mathf.Round(Mana);
        m_HealthBar.value=l_HealthRounded/MaxHealth;
        m_ManaBar.value=l_ManaRounded/MaxMana;
        m_HealthText.text=l_HealthRounded+"/"+MaxHealth;
        m_ManaText.text=l_ManaRounded+"/"+MaxMana;
    }
}
