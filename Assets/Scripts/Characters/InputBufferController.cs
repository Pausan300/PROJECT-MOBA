using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct InputBufferAction 
{
    public enum Action 
    {
        QPRESSED,
        WPRESSED,
        EPRESSED,
        RPRESSED,
        DPRESSED,
        FPRESSED
    }
    public Action m_PressedAction;
    public CharacterMaster.InputDelegate m_Method;
    public InputBufferAction(Action PressedAction, CharacterMaster.InputDelegate Method) 
    {
        m_PressedAction=PressedAction;
        m_Method=Method;
    }
}

public class InputBufferController : MonoBehaviour
{
    List<InputBufferAction> m_InputBuffer=new List<InputBufferAction>();

    public void CheckInputBuffer() 
    {
        if(m_InputBuffer.Count>0)
        {
            foreach(InputBufferAction Action in m_InputBuffer.ToArray())
            {
                m_InputBuffer.Remove(Action);
                Action.m_Method();
                break;
            }
        }
    }
    public void AddInput(InputBufferAction Action) 
    {
        if(m_InputBuffer.Count>0)
            m_InputBuffer.Remove(m_InputBuffer[0]);
        m_InputBuffer.Add(Action);
    }
    public List<InputBufferAction> GetInputBuffer() 
    {
        return m_InputBuffer;
    }
}
