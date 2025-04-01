using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class DamageInstance 
{
    public float m_PhysDamage;
    public float m_MagicDamage;
    public string m_Id;
    public float m_Timer;

    public DamageInstance(float PhysDamage, float MagicDamage, string SourceId) 
    {
        m_PhysDamage=PhysDamage;
        m_MagicDamage=MagicDamage;
        m_Id=SourceId;
        m_Timer=0.0f;
    } 
    public void AddDamage(float PhysDamage, float MagicDamage) 
    {
        m_PhysDamage+=PhysDamage;
        m_MagicDamage+=MagicDamage;
    }
}

public class IngameCharacterUI : MonoBehaviour
{
    CameraController m_CameraController;
    RectTransform m_CanvasRectTransform;
    RectTransform m_RectTransform;
    
    public GameObject m_WorldCanvas;
    public Slider m_IngameHealthBar;
    public Slider m_IngameManaBar;
    public TextMeshProUGUI m_IngameLevelText;
    public TextMeshProUGUI m_PlayerNameText;
    public EmoteObject m_CurrentEmote;
    public Vector2 m_MinSize;
    public Vector2 m_MaxSize;
    float m_InitialPositionY;

    [Header("DAMAGE NUMBERS")]
    public GameObject m_DamageNumbers;
    public Vector3 m_DamageNumbersPosOffset;
    public TMP_FontAsset m_PhysDamageFont;
    public TMP_FontAsset m_MagicDamageFont;
    public List<DamageInstance> m_DamageInstanceList=new List<DamageInstance>();


    void Start()
    {
        m_CanvasRectTransform=m_WorldCanvas.GetComponent<RectTransform>();
        m_RectTransform=GetComponent<RectTransform>();
        m_InitialPositionY=m_RectTransform.anchoredPosition.y;
    }
    private void Update()
    {
        if(m_DamageInstanceList.Count>0) 
        {
            for(int i=m_DamageInstanceList.Count-1; i>=0; --i) 
            {
                m_DamageInstanceList[i].m_Timer+=Time.deltaTime;
                if(m_DamageInstanceList[i].m_Timer>=0.1f) 
                {
                    SpawnDamageNumbers(m_DamageInstanceList[i].m_PhysDamage, m_DamageInstanceList[i].m_MagicDamage);
                    m_DamageInstanceList.Remove(m_DamageInstanceList[i]);
                }
            }
        }
    }
    void LateUpdate()
    {
        if(m_CameraController!=null && m_CanvasRectTransform!=null) 
        {
            m_CanvasRectTransform.LookAt(m_WorldCanvas.transform.position+m_CameraController.GetCamera().transform.rotation*Vector3.forward, m_CameraController.GetCamera().transform.rotation*Vector3.up);
           
            float l_ZoomPct=m_CameraController.GetZoomPct();
            Vector3 l_TargetScale=Vector3.Lerp(m_MaxSize, m_MinSize, l_ZoomPct);
            l_TargetScale.z=1.0f;
            m_RectTransform.localScale=Vector3.Lerp(m_RectTransform.localScale, l_TargetScale, Time.deltaTime*m_CameraController.m_ZoomSpeed);
            Vector2 l_TargetPosition=new Vector2(m_RectTransform.anchoredPosition.x, Mathf.Lerp(m_InitialPositionY, m_InitialPositionY-60.0f, l_ZoomPct));
            m_RectTransform.anchoredPosition=Vector2.Lerp(m_RectTransform.anchoredPosition, l_TargetPosition, Time.deltaTime*m_CameraController.m_ZoomSpeed);
        }
    }
    
    public void AddDamageInstance(float PhysDamage, float MagicDamage, string SourceId) 
    {
        DamageInstance l_DamageInstance=new DamageInstance(PhysDamage, MagicDamage, SourceId);
        if(GetDamageInstance(SourceId)!=null)
            GetDamageInstance(SourceId).AddDamage(PhysDamage, MagicDamage);
        else
            m_DamageInstanceList.Add(l_DamageInstance);
    }
    DamageInstance GetDamageInstance(string SourceId) 
    {
        foreach(DamageInstance Instance in m_DamageInstanceList) 
        {
            if(Instance.m_Id==SourceId)
                return Instance;
        }
        return null;
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

	public void UpdateHealthManaBars(float HealthRounded, float MaxHealth, float ManaRounded, float MaxMana)
    {
        m_IngameHealthBar.value=HealthRounded/MaxHealth;
        m_IngameManaBar.value=ManaRounded/MaxMana;
    }
    public void UpdateCharacterLevel(int Level)
    {
        m_IngameLevelText.text=Level.ToString();
    }
    public void ChangeHealthColor(Color FillColor) 
    {
        m_IngameHealthBar.fillRect.GetComponent<Image>().color=FillColor;
    }
    public void ShowEmote(Image Emote, float Duration) 
    {
        m_CurrentEmote.SetSprite(Emote.sprite);
        m_CurrentEmote.SetMaxDuration(Duration);
        m_CurrentEmote.ShowEmote();
    }

    //GETTERS AND SETTERS
    public void SetCameraController(CameraController _Camera) 
    {
        m_CameraController=_Camera;
    }
    public CameraController GetCameraController() 
    {
        return m_CameraController;
    }
    public void SetPlayerName(string Name)
    {
        m_PlayerNameText.text=Name;
    }
}
