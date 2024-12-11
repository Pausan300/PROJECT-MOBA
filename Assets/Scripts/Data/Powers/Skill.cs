using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName="Powers/Skill")]
public class Skill : Power
{
    int m_SkillLevel;
    public float[] m_SkillCooldownPerLevel;
    public float[] m_SkillManaPerLevel;
    public float m_SkillDisabledTime;
    float m_SkillMana;
    public bool m_CancelableWithMouseClick;
    bool m_UsingSkill;
    public SkillAttribute[] m_AttributeList;

    public override void SetInitStats()
    {
        base.SetInitStats();
        m_SkillLevel=0;
        m_SkillMana=m_SkillManaPerLevel[0];
        SetCd(m_SkillCooldownPerLevel[0]);
        SetUsingSkill(false);
    }

    public void LevelUp()
    {
        m_SkillLevel++;
        SetCd(m_SkillLevel);
        SetMana(m_SkillLevel);
    }
    public int GetLevel()
    {
        return m_SkillLevel;
    }
    public void SetLevel(int Level)
    {
        m_SkillLevel=Level;
    }
    public float GetMana()
    {
        return m_SkillMana;
    }
    public void SetMana(int Level)
    {
        m_SkillMana=m_SkillManaPerLevel[Level-1];
    }
    public void SetCd(int Level)
    {
        SetCd(m_SkillCooldownPerLevel[Level-1]);
    }
    public bool GetUsingSkill()
    {
        return m_UsingSkill;
    }
    public void SetUsingSkill(bool True)
    {
        m_UsingSkill=True;
    }
}

[Serializable]
public class SkillAttribute 
{
    public string m_AttributeId;
    public List<float> m_LevelScaling;
}

[CustomEditor(typeof(SkillAttribute))]
public class SkillAttributeCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SerializedProperty l_PopupType=serializedObject.FindProperty("m_AttributeId");
        SerializedProperty l_CharacterUI=serializedObject.FindProperty("m_LevelScaling");

        EditorGUILayout.PropertyField(l_PopupType);
        //l_Inspector.m_PopupType=(InspectableElementUI.PopupType)EditorGUILayout.EnumPopup("Group", l_Inspector.m_PopupType);
        EditorGUILayout.PropertyField(l_CharacterUI);
        //l_Inspector.m_CharacterUI=(CharacterUI)EditorGUILayout.ObjectField("CharacterUI", l_Inspector.m_CharacterUI, typeof(CharacterUI), true);

        //if(l_Inspector.m_PopupType==InspectableElementUI.PopupType.STAT)
        //{
        //    SerializedProperty l_StatName=serializedObject.FindProperty("m_StatName");
        //    EditorGUILayout.PropertyField(l_StatName);
        //    //l_Inspector.m_StatName=EditorGUILayout.TextField("Stat Name", l_Inspector.m_StatName);
        //    SerializedProperty l_StatDescription = serializedObject.FindProperty("m_StatDescription");
        //    EditorGUILayout.PropertyField(l_StatDescription, GUILayout.Height(80));
        //    //l_Inspector.m_StatDescription=EditorGUILayout.TextArea(l_Inspector.m_StatDescription, GUILayout.Height(60));
        //}
        serializedObject.ApplyModifiedProperties();
    }
}