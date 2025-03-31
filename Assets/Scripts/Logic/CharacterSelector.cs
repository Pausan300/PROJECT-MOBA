using NUnit.Framework;
using System.Collections.Generic;
using System;
using Unity.Netcode;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEditor;

public class CharacterSelector : MonoBehaviour
{
    public NetworkManager m_NetworkManager;
    [HideInInspector] public int selectedCharacterIndex;

    public CharacterToChose[] m_Characters;
    private void Awake()
    {
        if (m_NetworkManager != null && m_Characters.Length > 0)
        {
            m_NetworkManager.NetworkConfig.PlayerPrefab = m_Characters[selectedCharacterIndex].m_Prefab;
        }
    }
}

[Serializable]
public class CharacterToChose
{
    public string m_Name;
    public GameObject m_Prefab;
}


[CustomEditor(typeof(CharacterSelector))]
public class CharacterSelectorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CharacterSelector characterSelector = (CharacterSelector)target;


        if (characterSelector.m_Characters.Length > 0)
        {
            string[] characterNames = new string[characterSelector.m_Characters.Length];
            for (int i = 0; i < characterSelector.m_Characters.Length; i++)
            {
                characterNames[i] = characterSelector.m_Characters[i].m_Name;
            }

            characterSelector.selectedCharacterIndex = EditorGUILayout.Popup("Selected Character", characterSelector.selectedCharacterIndex, characterNames);
        }
        else
        {
            EditorGUILayout.LabelField("No characters available.");
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(characterSelector);
        }
        DrawDefaultInspector();
    }
}
