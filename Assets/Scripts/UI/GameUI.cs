using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public Toggle m_AutoAttackToggle;
    public Toggle m_SkillGizmosToggle;

    void Start()
    {
        SetAutoAttackToggle();
        SetSkillGizmosToggle();
    }

    void Update()
    {
        
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
