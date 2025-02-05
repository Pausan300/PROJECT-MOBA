using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RebindControlsUI : MonoBehaviour
{
    [SerializeField]
    InputActionReference m_InputActionReference;

    [SerializeField]
    bool m_ExcludeMouse=true;
    [Range(0, 10)]
    [SerializeField]
    int m_SelectedBinding;
    [SerializeField]
    InputBinding.DisplayStringOptions m_DisplayStringOptions;
    [Header("BINDING INFO - DO NOT EDIT")]
    [SerializeField]
    InputBinding m_InputBinding;
    int m_BindingIndex;

    string m_ActionName;

    [Header("UI FIELDS")]
    [SerializeField]
    TextMeshProUGUI m_ActionText;
    [SerializeField]
    Button m_RebindButton;
    [SerializeField]
    TextMeshProUGUI m_RebindText;
    [SerializeField]
    Button m_ResetButton;

    private void OnEnable()
    {
        m_RebindButton.onClick.AddListener(() => DoRebind());
        m_ResetButton.onClick.AddListener(() => ResetBinding());

        if(m_InputActionReference!=null) 
        {
            GetBindingInfo();
            InputManager.LoadBindingOverride(m_ActionName);
            UpdateUI();
        }

        InputManager.m_RebindComplete+=UpdateUI;
        InputManager.m_RebindCanceled+=UpdateUI;
    }
    private void OnDisable()
    {
        InputManager.m_RebindComplete-=UpdateUI;
        InputManager.m_RebindCanceled-=UpdateUI;
    }
    private void OnValidate()
    {
        if(m_InputActionReference!=null)
            return;
        GetBindingInfo();
        UpdateUI();
    }
    void GetBindingInfo() 
    {
        if(m_InputActionReference.action!=null)
            m_ActionName=m_InputActionReference.action.name;

        if(m_InputActionReference.action.bindings.Count>m_SelectedBinding) 
        {
            m_InputBinding=m_InputActionReference.action.bindings[m_SelectedBinding];
            m_BindingIndex=m_SelectedBinding;
        }
    }
    void UpdateUI() 
    {
        if(m_ActionText!=null)
            m_ActionText.text=m_ActionName;

        if(m_RebindText!=null)
        {
            if(Application.isPlaying) 
            {
                m_RebindText.text=InputManager.GetBindingName(m_ActionName, m_BindingIndex);
            }
            else
                m_RebindText.text=m_InputActionReference.action.GetBindingDisplayString(m_BindingIndex);
        }
    }

    void DoRebind() 
    {
        InputManager.StartRebind(m_ActionName, m_BindingIndex, m_RebindText/*, m_ExcludeMouse*/);
    }
    void ResetBinding() 
    {
        InputManager.ResetBinding(m_ActionName, m_BindingIndex);
        UpdateUI();
    }
}
