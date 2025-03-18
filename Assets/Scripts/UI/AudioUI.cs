using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioUI : MonoBehaviour
{
    public AudioMixer m_AudioMixer;

    [Header("MASTER AUDIO")]
    public AudioMixerGroup m_MasterMixerGroup;
    public Slider m_MasterVolumeSlider;
    [Range(-10.0f, 0.0f)]
    public float m_MinMasterVolume;
    [Range(0.0f, 20.0f)]
    public float m_MaxMasterVolume;

    [Header("MUSIC AUDIO")]
    public AudioMixerGroup m_MusicMixerGroup;
    public Slider m_MusicVolumeSlider;
    [Range(-10.0f, 0.0f)]
    public float m_MinMusicVolume;
    [Range(0.0f, 20.0f)]
    public float m_MaxMusicVolume;

    [Header("SFX AUDIO")]
    public AudioMixerGroup m_SFXMixerGroup;
    public Slider m_SFXVolumeSlider;
    [Range(-10.0f, 0.0f)]
    public float m_MinSFXVolume;
    [Range(0.0f, 20.0f)]
    public float m_MaxSFXVolume;


    public void InitSettings()
    {
        SetMasterVolumeSlider();
        SetMusicVolumeSlider();
        SetSFXVolumeSlider();
    }

    void SetMasterVolumeSlider() 
    {
        m_MasterVolumeSlider.minValue=m_MinMasterVolume;
        m_MasterVolumeSlider.maxValue=m_MaxMasterVolume;
        m_MasterVolumeSlider.value=PlayerPrefs.GetFloat("MasterVolume");
        m_AudioMixer.SetFloat("MasterVolume", m_MasterVolumeSlider.value);
    }
    public void OnMasterVolumeChanged() 
    {
        m_AudioMixer.SetFloat("MasterVolume", m_MasterVolumeSlider.value);
        PlayerPrefs.SetFloat("MasterVolume", m_MasterVolumeSlider.value);
    }

    void SetMusicVolumeSlider() 
    {
        m_MusicVolumeSlider.minValue=m_MinMusicVolume;
        m_MusicVolumeSlider.maxValue=m_MaxMusicVolume;
        m_MusicVolumeSlider.value=PlayerPrefs.GetFloat("MusicVolume");
        m_AudioMixer.SetFloat("MusicVolume", m_MusicVolumeSlider.value);
    }
    public void OnMusicVolumeChanged() 
    {
        m_AudioMixer.SetFloat("MusicVolume", m_MusicVolumeSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", m_MusicVolumeSlider.value);
    }
    
    void SetSFXVolumeSlider() 
    {
        m_SFXVolumeSlider.minValue=m_MinSFXVolume;
        m_SFXVolumeSlider.maxValue=m_MaxSFXVolume;
        m_SFXVolumeSlider.value=PlayerPrefs.GetFloat("SFXVolume");
        m_AudioMixer.SetFloat("SFXVolume", m_SFXVolumeSlider.value);
    }
    public void OnSFXVolumeChanged() 
    {
        m_AudioMixer.SetFloat("SFXVolume", m_SFXVolumeSlider.value);
        PlayerPrefs.SetFloat("SFXVolume", m_SFXVolumeSlider.value);
    }
}
