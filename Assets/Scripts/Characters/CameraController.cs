using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera m_Camera;
    public LayerMask m_CameraLayerMask;
    public Vector3 m_MinFollowCharacterOffset;
    public Vector3 m_MaxFollowCharacterOffset;
    Vector3 m_FollowCharacterOffset;
    float m_CameraLerp;
    public float m_CameraSpeed;
    public float m_WidthLimitOffset;
    public float m_HeightLimitOffset;
    bool m_Locked;

	private void Start()
	{
	    m_Locked=true;
        m_CameraLerp=0.0f;
        m_FollowCharacterOffset=m_MaxFollowCharacterOffset;
	}
	private void LateUpdate()
	{
        CameraMovement();
	}
    void CameraMovement()
    {
        Vector3 l_CameraPosition=transform.position;

        m_CameraLerp+=Input.mouseScrollDelta.y*0.1f;
        m_CameraLerp=Mathf.Clamp(m_CameraLerp, 0.0f, 1.0f);
        m_FollowCharacterOffset.y=Mathf.Lerp(m_MaxFollowCharacterOffset.y, m_MinFollowCharacterOffset.y, m_CameraLerp);
        m_FollowCharacterOffset.z=Mathf.Lerp(m_MaxFollowCharacterOffset.z, m_MinFollowCharacterOffset.z, m_CameraLerp);

        if(m_Locked || Input.GetKey(KeyCode.Space))
        {
            l_CameraPosition+=m_FollowCharacterOffset;
        }
        else
        {
            l_CameraPosition=m_Camera.transform.position;
            if(Input.mousePosition.x<=m_WidthLimitOffset)
                l_CameraPosition.x-=m_CameraSpeed*Time.deltaTime;
            else if(Input.mousePosition.x>=Screen.width-m_WidthLimitOffset)
                l_CameraPosition.x+=m_CameraSpeed*Time.deltaTime;
            if(Input.mousePosition.y<=m_HeightLimitOffset)
                l_CameraPosition.z-=m_CameraSpeed*Time.deltaTime;
            else if(Input.mousePosition.y>=Screen.height-m_HeightLimitOffset)
                l_CameraPosition.z+=m_CameraSpeed*Time.deltaTime;
        }

        if(Input.GetKeyDown(KeyCode.Y))
            m_Locked=!m_Locked;
                
        m_Camera.transform.position=l_CameraPosition;
    }
}
