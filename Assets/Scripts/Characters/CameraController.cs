using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera m_Camera;
    public LayerMask m_CameraLayerMask;
    public Vector3 m_FollowCharacterOffset;
    public float m_CameraSpeed;
    public float m_WidthLimitOffset;
    public float m_HeightLimitOffset;
    bool m_Locked;

	private void Start()
	{
	    m_Locked=true;
	}
	private void LateUpdate()
	{
        CameraMovement();
	}
    void CameraMovement()
    {
        Vector3 l_CameraPosition=transform.position;
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
