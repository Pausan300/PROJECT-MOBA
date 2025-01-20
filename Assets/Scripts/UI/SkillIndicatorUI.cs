using System.Collections.Generic;
using UnityEngine;

public class SkillIndicatorUI : MonoBehaviour
{
    CharacterMaster m_Character;

    RectTransform m_ArrowSkillIndicatorRect;
    List<RectTransform> m_DeletableSkillIndicatorList=new List<RectTransform>();
    List<RectTransform> m_NormalSkillIndicatorList=new List<RectTransform>();
    public struct TargetSkillIndicator 
    {
        public Transform m_TargetObject;
        public RectTransform m_SkillIndicator;
        public TargetSkillIndicator(Transform TargetObject, RectTransform SkillIndicator) 
        {
            m_TargetObject=TargetObject;
            m_SkillIndicator=SkillIndicator;
        }
    }
    List<TargetSkillIndicator> m_TargetSkillIndicatorList=new List<TargetSkillIndicator>();


    void Start()
    {
        ClearDeletableSkillIndicatorUI();
        ClearNormalSkillIndicatorUI();
        ClearTargetSkillIndicatorUI();
    }
    void Update()
    {
         if(m_ArrowSkillIndicatorRect!=null)
            MoveArrowSkillIndicator();
        if(m_TargetSkillIndicatorList.Count>0)
            UpdateTargetSkillIndicatorUI();
    }

    //SKILL INDICATOR METHODS
    public void CreateArrowSkillIndicator(GameObject Arrow, float Width, float Range, Vector3 Position, bool DeleteOnMouseClick)
    {
        GameObject l_Arrow=Instantiate(Arrow, transform);
        RectTransform l_ArrowRect=l_Arrow.GetComponent<RectTransform>();
        l_ArrowRect.sizeDelta=new Vector2(Width, Range);
        l_ArrowRect.position=Position;
        m_ArrowSkillIndicatorRect=l_ArrowRect;
        m_ArrowSkillIndicatorRect.position=new Vector3(m_ArrowSkillIndicatorRect.position.x, 0.05f, m_ArrowSkillIndicatorRect.position.z);
        if(DeleteOnMouseClick)
            m_DeletableSkillIndicatorList.Add(m_ArrowSkillIndicatorRect);
        else
            m_NormalSkillIndicatorList.Add(m_ArrowSkillIndicatorRect);
    }
    public void ChangeArrowSkillIndicatorSize(float Width, float Range)
    {
        m_ArrowSkillIndicatorRect.sizeDelta=new Vector2(Width, Range);
    }
    public void MoveArrowSkillIndicator()
    {
        Vector3 l_MouseDirection=m_Character.GetMouseDir();
        RaycastHit l_CameraRaycastHit;
        if(Physics.Raycast(m_Character.GetCameraController().GetCamera().transform.position, l_MouseDirection, out l_CameraRaycastHit, 1000.0f, m_Character.GetCameraController().m_TerrainLayerMask))
		{
			Quaternion a=Quaternion.LookRotation(l_CameraRaycastHit.point-m_Character.transform.position);
			a.eulerAngles=new Vector3(90.0f, a.eulerAngles.y, a.eulerAngles.z);
			m_ArrowSkillIndicatorRect.transform.rotation=Quaternion.Lerp(a, m_ArrowSkillIndicatorRect.transform.rotation, 0.0f);
		}
    }
    public void CreateCircleSkillIndicator(GameObject Circle, float Radius, Transform Target, bool FollowTargetPos)
    {
        GameObject l_Circle=Instantiate(Circle, transform);
        RectTransform l_CircleRect=l_Circle.GetComponent<RectTransform>();
        l_CircleRect.sizeDelta=new Vector2(Radius/100.0f*2.0f, Radius/100.0f*2.0f);
        l_CircleRect.position=Target.position;
        l_CircleRect.position=new Vector3(l_CircleRect.position.x, 0.05f, l_CircleRect.position.z);
        if(FollowTargetPos)
            m_TargetSkillIndicatorList.Add(new TargetSkillIndicator(Target, l_CircleRect));
        else
            m_DeletableSkillIndicatorList.Add(l_CircleRect);

    }
    public void UpdateTargetSkillIndicatorUI()
    {
        for(int i=m_TargetSkillIndicatorList.Count-1; i>=0; --i) 
        {
            if(m_TargetSkillIndicatorList[i].m_TargetObject==null) 
            {
                Destroy(m_TargetSkillIndicatorList[i].m_SkillIndicator.gameObject);
                m_TargetSkillIndicatorList.Remove(m_TargetSkillIndicatorList[i]);
            }
            else
                m_TargetSkillIndicatorList[i].m_SkillIndicator.position=new Vector3(m_TargetSkillIndicatorList[i].m_TargetObject.position.x, 0.05f, 
                    m_TargetSkillIndicatorList[i].m_TargetObject.position.z);
        }
    }
    public void ClearDeletableSkillIndicatorUI()
    {
        if(m_DeletableSkillIndicatorList.Count>0)
        {
            for(int i=0; i<m_DeletableSkillIndicatorList.Count; ++i)
                Destroy(m_DeletableSkillIndicatorList[i].gameObject);
        }
        m_DeletableSkillIndicatorList.Clear();
    }
    public void ClearNormalSkillIndicatorUI()
    {
        if(m_NormalSkillIndicatorList.Count>0)
        {
            for(int i=0; i<m_NormalSkillIndicatorList.Count; ++i)
                Destroy(m_NormalSkillIndicatorList[i].gameObject);
        }
        m_NormalSkillIndicatorList.Clear();
    }
    public void ClearTargetSkillIndicatorUI() 
    {
        if(m_TargetSkillIndicatorList.Count>0)
        {
            for(int i=0; i<m_TargetSkillIndicatorList.Count; ++i)
                Destroy(m_TargetSkillIndicatorList[i].m_SkillIndicator.gameObject);
        }
        m_TargetSkillIndicatorList.Clear();
    }

    //GETTERS AND SETTERS
    public void SetPlayer(CharacterMaster Player)
    {
        m_Character=Player;
    }
}
