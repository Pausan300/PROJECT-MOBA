using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSpaceCanvasBillboard : MonoBehaviour
{
    public Camera m_Camera;
    RectTransform m_RectTransform;
    
    void Start()
    {
        m_RectTransform=transform.GetComponent<RectTransform>();
    }
    void LateUpdate()
    {
        m_RectTransform.LookAt(transform.position+m_Camera.transform.rotation*Vector3.forward, m_Camera.transform.rotation*Vector3.up);
    }
}
