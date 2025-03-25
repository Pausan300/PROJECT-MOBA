using TMPro;
using UnityEngine;

public class SkillsSpecificationsPrefabUI : MonoBehaviour
{
    public TextMeshProUGUI m_SkillName;
    public TextMeshProUGUI m_SkillStatsLV;

    public void SetSkillsSpecificationsPrefabUI(string _Name, string SkillStats)
    {
        m_SkillName.text = _Name;
        m_SkillStatsLV.text = SkillStats;
    }
}
