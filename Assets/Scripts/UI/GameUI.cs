using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public OptionsUI m_OptionsUI;

    [Header("CAMERA SENSITIVITY")]
    public Slider m_CameraSensitivitySlider;
    [Range(1, 50)]
    public float m_MinCameraSensitivity;
    [Range(1, 50)]
    public float m_MaxCameraSensitivity;

    [Header("AUTO ATTACK TOGGLE")]
    public Toggle m_AutoAttackToggle;
    [Header("GIZMOS TOGGLE")]
    public Toggle m_SkillGizmosToggle;

    public void InitSettings()
    {
        SetAutoAttackToggle();
        SetSkillGizmosToggle();
        SetCameraSensitivitySlider();
    }

    void SetCameraSensitivitySlider() 
    {
        m_CameraSensitivitySlider.minValue=m_MinCameraSensitivity;
        m_CameraSensitivitySlider.maxValue=m_MaxCameraSensitivity;
        m_CameraSensitivitySlider.value=PlayerPrefs.GetFloat("CameraSensitivity");
        OnCameraSensitivityChanged();
    }
    public void OnCameraSensitivityChanged() 
    {
        m_OptionsUI.GetPlayer().GetCameraController().SetCameraSpeed(m_CameraSensitivitySlider.value);
        PlayerPrefs.SetFloat("CameraSensitivity", m_CameraSensitivitySlider.value);
    }

    void SetAutoAttackToggle() 
    {
        m_AutoAttackToggle.isOn=PlayerPrefs.GetInt("AutoAttack")==1 ? true : false;
    }
    public void OnAutoAttackChanged()
    {
        PlayerPrefs.SetInt("AutoAttack", m_AutoAttackToggle.isOn ? 1 : 0);
    }

    void SetSkillGizmosToggle() 
    {
        m_SkillGizmosToggle.isOn=PlayerPrefs.GetInt("SkillGizmos")==1 ? true : false;
    }
    public void OnSkillGizmosChanged() 
    {
        PlayerPrefs.SetInt("SkillGizmos", m_SkillGizmosToggle.isOn ? 1 : 0);
    }

    public bool IsAutoAttackEnabled() 
    {
        return m_AutoAttackToggle.isOn;
    }
    public bool IsSkillGizmosEnabled() 
    {
        return m_SkillGizmosToggle.isOn;
    }
}
