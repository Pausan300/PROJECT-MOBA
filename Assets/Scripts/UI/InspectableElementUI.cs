using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

public class InspectableElementUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public enum PopupType
    {
        PASSIVESKILL,
        QSKILL,
        WSKILL,
        ESKILL,
        RSKILL,
        SUMMONER1,
        SUMMONER2,
        STAT
    }
    public PopupType m_PopupType;
	public CharacterUI m_CharacterUI;
    public string m_StatName;
    public string m_StatDescription;

	public void OnPointerEnter(PointerEventData eventData)
	{
        if(m_PopupType==PopupType.STAT)
		    m_CharacterUI.SetPopupType(m_PopupType, m_StatDescription, m_StatName);
        else
		    m_CharacterUI.SetPopupType(m_PopupType, null, null);
	}
	public void OnPointerExit(PointerEventData eventData)
	{
		m_CharacterUI.HidePopup();
	}
}

[CustomEditor(typeof(InspectableElementUI))]
public class InspectableElementCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        InspectableElementUI l_Inspector=(InspectableElementUI)target;

        SerializedProperty l_PopupType=serializedObject.FindProperty("m_PopupType");
        SerializedProperty l_CharacterUI=serializedObject.FindProperty("m_CharacterUI");

        EditorGUILayout.PropertyField(l_PopupType);
        //l_Inspector.m_PopupType=(InspectableElementUI.PopupType)EditorGUILayout.EnumPopup("Group", l_Inspector.m_PopupType);
        EditorGUILayout.PropertyField(l_CharacterUI);
        //l_Inspector.m_CharacterUI=(CharacterUI)EditorGUILayout.ObjectField("CharacterUI", l_Inspector.m_CharacterUI, typeof(CharacterUI), true);

        if (l_Inspector.m_PopupType==InspectableElementUI.PopupType.STAT)
        {
            SerializedProperty l_StatName=serializedObject.FindProperty("m_StatName");
            EditorGUILayout.PropertyField(l_StatName);
            //l_Inspector.m_StatName=EditorGUILayout.TextField("Stat Name", l_Inspector.m_StatName);
            SerializedProperty l_StatDescription = serializedObject.FindProperty("m_StatDescription");
            EditorGUILayout.PropertyField(l_StatDescription, GUILayout.Height(80));
            //l_Inspector.m_StatDescription=EditorGUILayout.TextArea(l_Inspector.m_StatDescription, GUILayout.Height(60));
        }
        serializedObject.ApplyModifiedProperties();
    }
}
