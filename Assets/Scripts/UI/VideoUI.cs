using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;
using UnityEngine.Events;

public class VideoUI : MonoBehaviour
{
    CharacterMaster m_Character;

    [Header("RESOLUTION")]
    public TMP_Dropdown m_ResolutionDropdown;
    Resolution[] m_Resolutions;
    int m_ResolutionIndex;
    [Header("SCREEN MODE")]
    public TMP_Dropdown m_ScreenModeDropdown;
    [Header("FPS")]
    public TMP_Dropdown m_FrameRateDropdown;
    public List<string> m_FrameRateList;
    [Header("COLORBLIND MODE")]
    public TMP_Dropdown m_ColorblindModeDropdown;
    public Color m_DefaultOwnHealthColor;
    public Color m_DefaultAlliesHealthColor;
    public Color m_DefaultEnemiesHealthColor;
    public Color m_DeuteranopiaOwnHealthColor;
    public Color m_DeuteranopiaAlliesHealthColor;
    public Color m_DeuteranopiaEnemiesHealthColor;
    public Color m_TritanopiaOwnHealthColor;
    public Color m_TritanopiaAlliesHealthColor;
    public Color m_TritanopiaEnemiesHealthColor;
    public Color m_ProtanopiaOwnHealthColor;
    public Color m_ProtanopiaAlliesHealthColor;
    public Color m_ProtanopiaEnemiesHealthColor;
    public enum ColorblindType 
    {
        NONE,
        DEUTERANOPIA,
        TRITANOPIA,
        PROTANOPIA
    }
    ColorblindType m_ColorblindSelected;


    public void InitSettings()
    {
        SetResolutionsDropdown();
        SetFrameRateDropdown();
        SetColorblindModeDropdown();
    }

    void SetResolutionsDropdown() 
    {
        m_Resolutions=Screen.resolutions;
        List<string> l_Options=new List<string>();
        for(int i=0; i<m_Resolutions.Length; ++i)
            l_Options.Add(m_Resolutions[i].width+"x"+m_Resolutions[i].height);
        m_ResolutionDropdown.ClearOptions();
        m_ResolutionDropdown.AddOptions(l_Options);
        m_ResolutionDropdown.value=PlayerPrefs.GetInt("Resolution");
        ChangeResolution();
    }
    public void ChangeResolution() 
    {
        m_ResolutionIndex=m_ResolutionDropdown.value;
        Screen.SetResolution(m_Resolutions[m_ResolutionIndex].width, m_Resolutions[m_ResolutionIndex].height, Screen.fullScreenMode);
        PlayerPrefs.SetInt("Resolution", m_ResolutionIndex);
    }

    public void ChangeScreenMode() 
    {
        switch(m_ScreenModeDropdown.value) 
        {
            default:
                break;
            case 0:
                Screen.fullScreenMode=FullScreenMode.FullScreenWindow;
                break;
            case 1:
                Screen.fullScreenMode=FullScreenMode.Windowed;
                break;
            case 2:
                Screen.fullScreenMode=FullScreenMode.MaximizedWindow;
                break;
        }
    }

    void SetFrameRateDropdown() 
    {
        m_FrameRateDropdown.ClearOptions();
        m_FrameRateList.Add("No Limit");
        m_FrameRateDropdown.AddOptions(m_FrameRateList);
        m_FrameRateDropdown.value=PlayerPrefs.GetInt("FrameRate");
        ChangeFrameRate();
    }
    public void ChangeFrameRate() 
    {
        if(m_FrameRateDropdown.value>=m_FrameRateList.Count)
            Application.targetFrameRate=-1;
        else 
        {
            int.TryParse(m_FrameRateList[m_FrameRateDropdown.value], out int l_Fps);
            Application.targetFrameRate=l_Fps;
        }
        PlayerPrefs.SetInt("FrameRate", m_FrameRateDropdown.value);
    }

    void SetColorblindModeDropdown() 
    {
        m_ColorblindModeDropdown.value=PlayerPrefs.GetInt("Colorblind");
        ChangeColorblindMode();
    }
    public void ChangeColorblindMode() 
    {
        switch(m_ColorblindModeDropdown.value)
        {
            case 0:
                m_ColorblindSelected=ColorblindType.NONE;
                break;
            case 1:
                m_ColorblindSelected=ColorblindType.DEUTERANOPIA;
                break;
            case 2:
                m_ColorblindSelected=ColorblindType.TRITANOPIA;
                break;
            case 3:
                m_ColorblindSelected=ColorblindType.PROTANOPIA;
                break;
        }
        PlayerPrefs.SetInt("Colorblind", m_ColorblindModeDropdown.value);
        foreach(CharacterMaster Player in m_Character.GetGameManager().GetPlayersList()) 
        {
            if(Player!=m_Character)
                Player.m_IngameCharacterUI.ChangeHealthColor(GetColor(false, false));
            else
                Player.m_IngameCharacterUI.ChangeHealthColor(GetColor(true, false));
        }
        foreach(EnemyDummy Enemy in m_Character.GetGameManager().GetEnemiesList()) 
        {
            Enemy.m_IngameUI.ChangeHealthColor(GetColor(true, true));
        }
    }
    public Color GetColor(bool Own, bool Enemy) 
    {
        switch(m_ColorblindSelected)
        {
            case ColorblindType.NONE:
                if(Enemy)
                    return m_DefaultEnemiesHealthColor;
                else if(Own)
                    return m_DefaultOwnHealthColor;
                else
                    return m_DefaultAlliesHealthColor;
            case ColorblindType.DEUTERANOPIA:
                if(Enemy)
                    return m_DeuteranopiaEnemiesHealthColor;
                else if(Own)
                    return m_DeuteranopiaOwnHealthColor;
                else
                    return m_DeuteranopiaAlliesHealthColor;
            case ColorblindType.TRITANOPIA:
                if(Enemy)
                    return m_TritanopiaEnemiesHealthColor;
                else if(Own)
                    return m_TritanopiaOwnHealthColor;
                else
                    return m_TritanopiaAlliesHealthColor;
            case ColorblindType.PROTANOPIA:
                if(Enemy)
                    return m_ProtanopiaEnemiesHealthColor;
                else if(Own)
                    return m_ProtanopiaOwnHealthColor;
                else
                    return m_ProtanopiaAlliesHealthColor;
            default:
                return m_DefaultOwnHealthColor;
        }
    }

    //GETTERS AND SETTERS
    public CharacterMaster GetPlayer() 
    {
        return m_Character;
    }
    public void SetPlayer(CharacterMaster Player)
    {
        m_Character=Player;
    }
}
