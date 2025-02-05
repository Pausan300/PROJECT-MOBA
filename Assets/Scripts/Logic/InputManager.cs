using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;
using TMPro;

public class InputManager : MonoBehaviour
{
    public static PlayerInputs m_InputActions;
    public static event Action m_RebindComplete;
    public static event Action m_RebindCanceled;
    public static event Action<InputAction, int> m_RebindStarted;

    private void Awake()
    {
        if(m_InputActions==null)
            m_InputActions=new PlayerInputs();
    }

    public static void StartRebind(string ActionName, int BindingIndex, TextMeshProUGUI StatusText/*, bool ExcludeMouse*/) 
    {
        InputAction l_Action=m_InputActions.asset.FindAction(ActionName);
        if(l_Action==null || l_Action.bindings.Count<=BindingIndex)
        {
            Debug.Log("Couldn't find action or binding");
            return;
        }

        if(l_Action.bindings[BindingIndex].isComposite) 
        {
            var l_FirstPartIndex=BindingIndex+1;
            if(l_FirstPartIndex<l_Action.bindings.Count &&l_Action.bindings[l_FirstPartIndex].isComposite)
                DoRebind(l_Action, BindingIndex, StatusText, true);
        }
        else
            DoRebind(l_Action, BindingIndex, StatusText, false);
    }
    static void DoRebind(InputAction ActionToRebind,  int BindingIndex, TextMeshProUGUI StatusText, bool AllCompositeParts/*, bool ExcludeMouse*/) 
    {
        if(ActionToRebind==null || BindingIndex<0)
            return;

        StatusText.text=$"Press a {ActionToRebind.expectedControlType}";

        ActionToRebind.Disable();

        var l_Rebind=ActionToRebind.PerformInteractiveRebinding(BindingIndex);

        l_Rebind.OnComplete(operation =>
        {
            ActionToRebind.Enable();
            operation.Dispose();

            if(AllCompositeParts) 
            {
                var l_NextBindingIndex=BindingIndex+1;
                if(l_NextBindingIndex<ActionToRebind.bindings.Count &&ActionToRebind.bindings[l_NextBindingIndex].isComposite)
                    DoRebind(ActionToRebind, l_NextBindingIndex, StatusText, AllCompositeParts);
            }

            SaveBindingOverride(ActionToRebind);
            m_RebindComplete?.Invoke();
        });

        l_Rebind.OnCancel(operation =>
        {
            ActionToRebind.Enable();
            operation.Dispose();

            m_RebindCanceled?.Invoke();
        });

        l_Rebind.WithCancelingThrough("<Keyboard>/escape");

        //if(ExcludeMouse)
        //    l_Rebind.WithControlsExcluding("Mouse");

        m_RebindStarted?.Invoke(ActionToRebind, BindingIndex);
        l_Rebind.Start(); 
    }
    public static string GetBindingName(string ActionName, int BindingIndex) 
    {
        if(m_InputActions==null)
            m_InputActions=new PlayerInputs();

        InputAction l_Action=m_InputActions.asset.FindAction(ActionName);
        return l_Action.GetBindingDisplayString(BindingIndex);
    }
    public static void ResetBinding(string ActioName, int BindingIndex) 
    {
        InputAction l_Action=m_InputActions.asset.FindAction(ActioName);

        if(l_Action==null ||l_Action.bindings.Count<=BindingIndex) 
        {
            Debug.Log("COULD NOT FIND ACTION OR BINDING");
            return;
        }

        if(l_Action.bindings[BindingIndex].isComposite) 
        {
            for(int i=BindingIndex; i<l_Action.bindings.Count && l_Action.bindings[i].isComposite; ++i) 
                l_Action.RemoveBindingOverride(i);
        }
        else 
            l_Action.RemoveBindingOverride(BindingIndex);

        SaveBindingOverride(l_Action);
    }

    private static void SaveBindingOverride(InputAction Action) 
    {
        for(int i=0; i<Action.bindings.Count; ++i) 
        {
            PlayerPrefs.SetString(Action.actionMap+Action.name+i, Action.bindings[i].overridePath);
        }
    }
    public static void LoadBindingOverride(string ActionName) 
    {
        if(m_InputActions==null)
            m_InputActions=new PlayerInputs();

        InputAction l_Action=m_InputActions.asset.FindAction(ActionName);

        for(int i=0; i<l_Action.bindings.Count; ++i) 
        {
            if(!string.IsNullOrEmpty(PlayerPrefs.GetString(l_Action.actionMap+l_Action.name+i)))
                l_Action.ApplyBindingOverride(i, PlayerPrefs.GetString(l_Action.actionMap+l_Action.name+i));
        }
    }
}
