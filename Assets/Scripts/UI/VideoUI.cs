using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;

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

    void Start()
    {
        SetResolutionsDropdown();
        SetFrameRateDropdown();
    }

    void Update()
    {
        
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

    public void ChangeColorblindMode() 
    {
        switch(m_ColorblindModeDropdown.value)
        {
            case 0:
                Debug.Log("Colorblind off");
                break;
            case 1:
                //Deuteranopia
                break;
            case 2:
                //Tritanopia
                break;
            case 3:
                //Protanopia
                break;
        }
    }
}
