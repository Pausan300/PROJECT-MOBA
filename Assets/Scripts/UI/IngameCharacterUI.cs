using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class IngameCharacterUI : MonoBehaviour
{
    Camera m_Camera;
    RectTransform m_RectTransform;
    
    public GameObject m_WorldCanvas;
    public Slider m_IngameHealthBar;
    public Slider m_IngameManaBar;
    public TextMeshProUGUI m_IngameLevelText;
    public TextMeshProUGUI m_PlayerNameText;
    public GameObject m_DamageNumbers;
    public Vector3 m_DamageNumbersPosOffset;
    public TMP_FontAsset m_PhysDamageFont;
    public TMP_FontAsset m_MagicDamageFont;
    
    void Start()
    {
        m_RectTransform=transform.GetComponent<RectTransform>();
    }
    void LateUpdate()
    {
        if(m_Camera!=null && m_RectTransform!=null)
            m_RectTransform.LookAt(transform.position+m_Camera.transform.rotation*Vector3.forward, m_Camera.transform.rotation*Vector3.up);
    }
    
	public void UpdateHealthManaBars(float HealthRounded, float MaxHealth, float ManaRounded, float MaxMana)
    {
        m_IngameHealthBar.value=HealthRounded/MaxHealth;
        m_IngameManaBar.value=ManaRounded/MaxMana;
    }
    public void SpawnDamageNumbers(float PhysDamage, float MagicDamage)
    {
        Vector3 l_PosOffset=m_DamageNumbersPosOffset;
        if(PhysDamage>0.0f)
        {
            GameObject l_PhysDamageText=Instantiate(m_DamageNumbers, m_WorldCanvas.transform);
            l_PhysDamageText.GetComponent<RectTransform>().localPosition=Vector3.zero+l_PosOffset;
            l_PosOffset.y-=0.5f;
            TextMeshProUGUI l_TextMesh=l_PhysDamageText.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            l_TextMesh.font=m_PhysDamageFont;
            l_TextMesh.text=PhysDamage.ToString("f0");
        }
        if(MagicDamage>0.0f)
        {
            GameObject l_MagicDamageText=Instantiate(m_DamageNumbers, m_WorldCanvas.transform);
            l_MagicDamageText.GetComponent<RectTransform>().localPosition=Vector3.zero+l_PosOffset;
            TextMeshProUGUI l_TextMesh=l_MagicDamageText.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            l_TextMesh.font=m_MagicDamageFont;
            l_TextMesh.text=MagicDamage.ToString("f0");
        }
    }
    public void UpdateCharacterLevel(int Level)
    {
        m_IngameLevelText.text=Level.ToString();
    }

    //GETTERS AND SETTERS
    public void SetCamera(Camera _Camera) 
    {
        m_Camera=_Camera;
    }
    public Camera GetCamera() 
    {
        return m_Camera;
    }
    public void SetPlayerName(string Name)
    {
        m_PlayerNameText.text=Name;
    }
}
