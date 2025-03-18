using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;


public class EmoteObject : MonoBehaviour 
{
    RectTransform m_RectTransform;
    Image m_EmoteImage;
    Animation m_Animation;
    public AnimationClip m_ShowEmoteClip;
    public AnimationClip m_HideEmoteClip;

    float m_Timer;
    float m_MaxDuration;
    bool m_IsShowing;

    private void Awake() 
    {
        m_RectTransform=gameObject.GetComponent<RectTransform>();
        m_EmoteImage=gameObject.GetComponent<Image>();
        m_Animation=gameObject.GetComponent<Animation>();
        HideEmote();
    }
    private void Update()
    {
        if(m_IsShowing) 
        {
            m_Timer+=Time.deltaTime;
            if(m_Timer>=m_MaxDuration)
                HideEmote();
        }
    }

    public void ShowEmote() 
    {
        m_Animation.Play(m_ShowEmoteClip.name);
        m_IsShowing=true;
    }
    public void HideEmote() 
    {
        m_Animation.Play(m_HideEmoteClip.name);
        m_IsShowing=false;
    }

    public void SetMaxDuration(float Duration) 
    {
        m_MaxDuration=Duration;
        m_Timer=0.0f;
    }
    public void SetSprite(Sprite Emote) 
    {
        m_EmoteImage.sprite=Emote;
    }
}