using UnityEngine;

public class OptionsUI : MonoBehaviour
{
    CharacterMaster m_Character;

    //CONTROLS MENU
    public GameObject m_ControlsMenu;

    //VIDEO MENU
    public VideoUI m_VideoMenu;

    //AUDIO MENU
    public AudioUI m_AudioMenu;

    //GAME MENU
    public GameUI m_GameMenu;

    void Start()
    {
        HideOptionsUI();
        HideAllSubmenus();
    }

    public void ExitGame() 
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying=false;
#else
        Application.Quit();
#endif
    }

    //Hide and Show Methods
    public void HideOptionsUI() 
    {
        gameObject.SetActive(false);
    }
    public void ShowOptionsUI() 
    {
        gameObject.SetActive(true);
    }

    public void HideAllSubmenus() 
    {
        HideControlsUI();
        HideVideoUI();
        HideAudioUI();
        HideGameUI();
    }

    public void HideControlsUI() 
    {
        m_ControlsMenu.SetActive(false);
    }
    public void ShowControlsUI() 
    {
        m_ControlsMenu.SetActive(true);
    }

    public void HideVideoUI() 
    {
        m_VideoMenu.gameObject.SetActive(false);
    }
    public void ShowVideoUI() 
    {
        m_VideoMenu.gameObject.SetActive(true);
    }

    public void HideAudioUI() 
    {
        m_AudioMenu.gameObject.SetActive(false);
    }
    public void ShowAudioUI() 
    {
        m_AudioMenu.gameObject.SetActive(true);
    }

    public void HideGameUI() 
    {
        m_GameMenu.gameObject.SetActive(false);
    }
    public void ShowGameUI() 
    {
        m_GameMenu.gameObject.SetActive(true);
    }

    //GETTERS AND SETTERS
    public void SetPlayer(CharacterMaster Player)
    {
        m_Character=Player;
    }
}
