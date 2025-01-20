using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Camera m_Camera;
    Transform m_FollowTarget;
    public LayerMask m_CameraLayerMask;
    public LayerMask m_TerrainLayerMask;
    public LayerMask m_SelectHitboxLayerMask;
    public Vector3 m_CharacterOffset;
    public float m_MaxZoom;
    public float m_MinZoom;
    public float m_ZoomSpeed;
    Vector3 m_ZoomOffset;
    Vector3 m_FreeCameraOffset;
    float m_OffsetMultiplier;
    float m_CameraLerpPct;
    public float m_CameraSpeed;
    public float m_WidthLimitOffset;
    public float m_HeightLimitOffset;
    bool m_Locked;

	private void Awake()
	{
        m_Camera=GetComponent<Camera>();
        Cursor.lockState=CursorLockMode.Confined;
	    m_Locked=true;
        m_CameraLerpPct=1.0f;
        m_ZoomOffset=-m_Camera.transform.forward*m_MaxZoom;
        m_OffsetMultiplier=m_MaxZoom;
	}
	private void LateUpdate()
	{
        CameraMovement();
	}
    void CameraMovement()
    {
        Vector3 l_DesiredPosition=m_FollowTarget.position+m_CharacterOffset;

        if(Input.mouseScrollDelta.y!=0)
        {
            m_CameraLerpPct+=Input.mouseScrollDelta.y*0.2f;
            m_CameraLerpPct=Mathf.Clamp(m_CameraLerpPct, 0.0f, 1.0f);
            m_OffsetMultiplier=Mathf.Lerp(m_MaxZoom, m_MinZoom, m_CameraLerpPct);
        }
        m_ZoomOffset=Vector3.Lerp(m_ZoomOffset, -m_Camera.transform.forward*m_OffsetMultiplier, Time.deltaTime*m_ZoomSpeed);

        if(Input.GetKeyDown(KeyCode.Y))
        {
            m_Locked=!m_Locked;
            if(!m_Locked)
                m_FreeCameraOffset=l_DesiredPosition;
        }
        if(Input.GetKeyUp(KeyCode.Space))
             m_FreeCameraOffset=l_DesiredPosition;

        if(m_Locked || Input.GetKey(KeyCode.Space))
            m_Camera.transform.position=l_DesiredPosition+m_ZoomOffset;
        else
        {
            if(Input.mousePosition.x<=m_WidthLimitOffset)
                m_FreeCameraOffset.x-=m_CameraSpeed*Time.deltaTime;
            else if(Input.mousePosition.x>=Screen.width-m_WidthLimitOffset)
                m_FreeCameraOffset.x+=m_CameraSpeed*Time.deltaTime;
            if(Input.mousePosition.y<=m_HeightLimitOffset)
                m_FreeCameraOffset.z-=m_CameraSpeed*Time.deltaTime;
            else if(Input.mousePosition.y>=Screen.height-m_HeightLimitOffset)
                m_FreeCameraOffset.z+=m_CameraSpeed*Time.deltaTime;
            m_Camera.transform.position=m_FreeCameraOffset+m_ZoomOffset;
        }   
    }
    public Camera GetCamera() 
    {
        return m_Camera;
    }
    public void SetFollowTarget(Transform Target) 
    {
        m_FollowTarget=Target;
    }
}
