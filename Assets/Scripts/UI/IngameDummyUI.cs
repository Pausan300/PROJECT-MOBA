using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngameDummyUI : IngameEnemyUI
{
    [Header("DUMMY")]
    public Slider m_IngameManaBar;
    public TextMeshProUGUI m_IngameLevelText;
    public TextMeshProUGUI m_PlayerNameText;

    public void UpdateHealthManaBars(float HealthRounded, float MaxHealth, float ManaRounded, float MaxMana)
    {
        m_IngameHealthBar.value = HealthRounded / MaxHealth;
        m_IngameManaBar.value = ManaRounded / MaxMana;
    }
    public void UpdateCharacterLevel(int Level)
    {
        m_IngameLevelText.text = Level.ToString();
    }

    //GETTERS AND SETTERS
    public void SetPlayerName(string Name)
    {
        m_PlayerNameText.text = Name;
    }
}