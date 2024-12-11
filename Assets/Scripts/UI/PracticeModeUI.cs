using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PracticeModeUI : MonoBehaviour
{
    public CharacterMaster m_Character;
    Animation m_Animation;
    public AnimationClip m_OpenUIAnim;
    public AnimationClip m_CloseUIAnim;
    public GameObject m_EnemyDummyPrefab;
    public List<EnemyDummy> m_EnemiesList;

    [Header("BUTTONS")]
    public Animation m_CooldownsButtonAnim;
    public Animation m_HealthButtonAnim;
    public Animation m_ManaButtonAnim;
    public Animation m_EnemySpawnButtonAnim;
    public Animation m_EnemyAttacksButtonAnim;
    public Animation m_EnemyResistsButtonAnim;
    public Animation m_EnemyHealthButtonAnim;
    public Animation m_ShowGizmosButtonAnim;

    bool m_Opened;
    bool m_CooldownsButtonActive;
    bool m_HealthButtonActive;
    bool m_ManaButtonActive;
    bool m_EnemySpawnButtonActive;
    bool m_EnemyAttacksButtonActive;
    bool m_EnemyResistsButtonActive;
    bool m_EnemyHealthButtonActive;
    bool m_ShowGizmosButtonActive;
    float m_ButtonAnimTime;
    float m_TimerSinceLastDummyAttack;
    float m_Tick;

    void Start()
    {
        m_Animation=gameObject.GetComponent<Animation>();
    }
    void Update()
    {
        if(m_HealthButtonActive || m_ManaButtonActive)
        {
            m_Tick+=Time.deltaTime;
            if(m_Tick>1.0f)
                ButtonUpdateActions();
        }

        if(m_CooldownsButtonActive)
            m_ButtonAnimTime=m_CooldownsButtonAnim[m_CooldownsButtonAnim.clip.name].time;
        if(m_HealthButtonActive)
            m_ButtonAnimTime=m_HealthButtonAnim[m_HealthButtonAnim.clip.name].time;
        if(m_ManaButtonActive)
            m_ButtonAnimTime=m_ManaButtonAnim[m_ManaButtonAnim.clip.name].time;

        if(Input.GetMouseButtonDown(0))
        {
            if(m_EnemySpawnButtonActive && !EventSystem.current.IsPointerOverGameObject())
                SpawnEnemy();
            if(m_EnemyHealthButtonActive)
                AddHealthToEnemy();
            if(m_EnemyResistsButtonActive)
                AddResistsToEnemy();
        }

        if(m_EnemySpawnButtonActive)
            m_ButtonAnimTime=m_EnemySpawnButtonAnim[m_EnemySpawnButtonAnim.clip.name].time;
        if(m_EnemyResistsButtonActive)
            m_ButtonAnimTime=m_EnemyResistsButtonAnim[m_EnemyResistsButtonAnim.clip.name].time;
        if(m_EnemyHealthButtonActive)
            m_ButtonAnimTime=m_EnemyHealthButtonAnim[m_EnemyHealthButtonAnim.clip.name].time;
        if(m_ShowGizmosButtonActive)
            m_ButtonAnimTime=m_ShowGizmosButtonAnim[m_ShowGizmosButtonAnim.clip.name].time;

        if(m_EnemyAttacksButtonActive)
        {
            m_TimerSinceLastDummyAttack+=Time.deltaTime;
            if(m_TimerSinceLastDummyAttack>=5.0f)
            {
                foreach(EnemyDummy Enemy in m_EnemiesList)
                    Enemy.StartCoroutine(Enemy.PerformAttack());
                m_TimerSinceLastDummyAttack=0.0f;
            }
            m_ButtonAnimTime=m_EnemyAttacksButtonAnim[m_EnemyAttacksButtonAnim.clip.name].time;
        }
    }
    void ButtonUpdateActions()
    {
        if(m_HealthButtonActive)
            m_Character.m_CharacterStats.SetCurrentHealth(m_Character.m_CharacterStats.GetMaxHealth());
        if(m_ManaButtonActive)
            m_Character.m_CharacterStats.SetCurrentMana(m_Character.m_CharacterStats.GetMaxMana());
        m_Tick=0.0f;
    }
    public void OpenCloseUI()
    {
        if(!m_Animation.isPlaying)
        {
            if(!m_Opened)
            {
                m_Opened=true;
                m_Animation.Play(m_OpenUIAnim.name);
            }
            else
            {
                m_Opened=false;
                m_Animation.Play(m_CloseUIAnim.name);
            }
        }
    }
    public void ZeroCooldownButton()
    {
        m_CooldownsButtonActive=!m_CooldownsButtonActive;
        if(m_CooldownsButtonAnim.isPlaying)
        {
            m_CooldownsButtonAnim[m_CooldownsButtonAnim.clip.name].time=0.0f;
            m_CooldownsButtonAnim.Sample();
            m_CooldownsButtonAnim.Stop();
            m_Character.SetZeroCooldown(false);
        }
        else
        { 
            m_CooldownsButtonAnim[m_CooldownsButtonAnim.clip.name].time=m_ButtonAnimTime;
            m_CooldownsButtonAnim.Sample();
            m_CooldownsButtonAnim.Play();
            m_Character.SetZeroCooldown(true);
        }
    }
    public void HealthButton()
    {
        m_HealthButtonActive=!m_HealthButtonActive;
        if(m_HealthButtonAnim.isPlaying)
        {
            m_HealthButtonAnim[m_HealthButtonAnim.clip.name].time=0.0f;
            m_HealthButtonAnim.Sample();
            m_HealthButtonAnim.Stop();
        }
        else
        { 
            m_HealthButtonAnim[m_HealthButtonAnim.clip.name].time=m_ButtonAnimTime;
            m_HealthButtonAnim.Sample();
            m_HealthButtonAnim.Play();
        }
    }
    public void ManaButton()
    {
        m_ManaButtonActive=!m_ManaButtonActive;
        if(m_ManaButtonAnim.isPlaying)
        {
            m_ManaButtonAnim[m_ManaButtonAnim.clip.name].time=0.0f;
            m_ManaButtonAnim.Sample();
            m_ManaButtonAnim.Stop();
        }
        else
        { 
            m_ManaButtonAnim[m_ManaButtonAnim.clip.name].time=m_ButtonAnimTime;
            m_ManaButtonAnim.Sample();
            m_ManaButtonAnim.Play();
        }
    }
    public void LevelUpButton()
    {
        m_Character.LevelUp();
    }
    public void ResetLevelButton()
    {
        m_Character.SetInitStats();
    }
    public void EnemySpawnButton()
    {
        m_EnemySpawnButtonActive=!m_EnemySpawnButtonActive;
        if(m_EnemySpawnButtonAnim.isPlaying)
        {
            m_EnemySpawnButtonAnim[m_EnemySpawnButtonAnim.clip.name].time=0.0f;
            m_EnemySpawnButtonAnim.Sample();
            m_EnemySpawnButtonAnim.Stop();
        }
        else
        { 
            m_EnemySpawnButtonAnim[m_EnemySpawnButtonAnim.clip.name].time=m_ButtonAnimTime;
            m_EnemySpawnButtonAnim.Sample();
            m_EnemySpawnButtonAnim.Play();
        }
    }
    public void EnemyResistsButton()
    {
        m_EnemyResistsButtonActive=!m_EnemyResistsButtonActive;
        if(m_EnemyResistsButtonAnim.isPlaying)
        {
            m_EnemyResistsButtonAnim[m_EnemyResistsButtonAnim.clip.name].time=0.0f;
            m_EnemyResistsButtonAnim.Sample();
            m_EnemyResistsButtonAnim.Stop();
        }
        else
        { 
            m_EnemyResistsButtonAnim[m_EnemyResistsButtonAnim.clip.name].time=m_ButtonAnimTime;
            m_EnemyResistsButtonAnim.Sample();
            m_EnemyResistsButtonAnim.Play();
        }
    }
    public void EnemyHealthUpButton()
    {
        m_EnemyHealthButtonActive=!m_EnemyHealthButtonActive;
        if(m_EnemyHealthButtonAnim.isPlaying)
        {
            m_EnemyHealthButtonAnim[m_EnemyHealthButtonAnim.clip.name].time=0.0f;
            m_EnemyHealthButtonAnim.Sample();
            m_EnemyHealthButtonAnim.Stop();
        }
        else
        { 
            m_EnemyHealthButtonAnim[m_EnemyHealthButtonAnim.clip.name].time=m_ButtonAnimTime;
            m_EnemyHealthButtonAnim.Sample();
            m_EnemyHealthButtonAnim.Play();
        }
    }
    void SpawnEnemy()
    {
        CameraController l_CameraController=m_Character.GetCameraController();
        Vector3 l_MousePosition=Input.mousePosition;
        l_MousePosition.z=10.0f;
        Vector3 l_MouseDirection=l_CameraController.m_Camera.ScreenToWorldPoint(l_MousePosition)-l_CameraController.m_Camera.transform.position;
        RaycastHit l_CameraRaycastHit;
        if(Physics.Raycast(l_CameraController.m_Camera.transform.position, l_MouseDirection, out l_CameraRaycastHit, 1000.0f, l_CameraController.m_CameraLayerMask))
        {
            Debug.DrawLine(l_CameraController.m_Camera.transform.position, l_CameraRaycastHit.point, Color.red);
            if(l_CameraRaycastHit.transform.CompareTag("Terrain"))
            {
                GameObject l_Enemy=Instantiate(m_EnemyDummyPrefab, l_CameraRaycastHit.point+new Vector3(0.0f, 1.0f, 0.0f), m_EnemyDummyPrefab.transform.rotation);
                EnemyDummy l_EnemyScript=l_Enemy.GetComponent<EnemyDummy>();
                l_EnemyScript.SetCanvasCamera(m_Character.GetCameraController().m_Camera);
                m_EnemiesList.Add(l_EnemyScript);
                EnemySpawnButton();
            }
        }
    }
    void AddHealthToEnemy()
    {
        CameraController l_CameraController=m_Character.GetCameraController();
        Vector3 l_MousePosition=Input.mousePosition;
        l_MousePosition.z=10.0f;
        Vector3 l_MouseDirection=l_CameraController.m_Camera.ScreenToWorldPoint(l_MousePosition)-l_CameraController.m_Camera.transform.position;
        RaycastHit l_CameraRaycastHit;
        if(Physics.Raycast(l_CameraController.m_Camera.transform.position, l_MouseDirection, out l_CameraRaycastHit, 1000.0f, l_CameraController.m_CameraLayerMask))
        {
            Debug.DrawLine(l_CameraController.m_Camera.transform.position, l_CameraRaycastHit.point, Color.red);
            if(l_CameraRaycastHit.transform.CompareTag("Enemy"))
                l_CameraRaycastHit.transform.GetComponent<EnemyDummy>().AddHealth(100.0f);
        }
    }
    void AddResistsToEnemy()
    {
        CameraController l_CameraController=m_Character.GetCameraController();
        Vector3 l_MousePosition=Input.mousePosition;
        l_MousePosition.z=10.0f;
        Vector3 l_MouseDirection=l_CameraController.m_Camera.ScreenToWorldPoint(l_MousePosition)-l_CameraController.m_Camera.transform.position;
        RaycastHit l_CameraRaycastHit;
        if(Physics.Raycast(l_CameraController.m_Camera.transform.position, l_MouseDirection, out l_CameraRaycastHit, 1000.0f, l_CameraController.m_CameraLayerMask))
        {
            Debug.DrawLine(l_CameraController.m_Camera.transform.position, l_CameraRaycastHit.point, Color.red);
            if(l_CameraRaycastHit.transform.CompareTag("Enemy"))
            {
                EnemyDummy l_Enemy=l_CameraRaycastHit.transform.GetComponent<EnemyDummy>();
                l_Enemy.m_CharacterStats.SetArmor(l_Enemy.m_CharacterStats.GetArmor()+10.0f);
                l_Enemy.m_CharacterStats.SetMagicRes(l_Enemy.m_CharacterStats.GetMagicRes()+10.0f);
            }
        }
    }
    public void EnemyAttackButton()
    {
        m_EnemyAttacksButtonActive=!m_EnemyAttacksButtonActive;
        if(m_EnemyAttacksButtonAnim.isPlaying)
        {
            m_EnemyAttacksButtonAnim[m_EnemyAttacksButtonAnim.clip.name].time=0.0f;
            m_EnemyAttacksButtonAnim.Sample();
            m_EnemyAttacksButtonAnim.Stop();
        }
        else
        { 
            m_EnemyAttacksButtonAnim[m_EnemyAttacksButtonAnim.clip.name].time=m_ButtonAnimTime;
            m_EnemyAttacksButtonAnim.Sample();
            m_EnemyAttacksButtonAnim.Play();
            m_TimerSinceLastDummyAttack=0.0f;
        }
    }
    public void EraseEnemies()
    {
        foreach(EnemyDummy Enemy in m_EnemiesList)
            Destroy(Enemy.gameObject);
        m_EnemiesList.Clear();
    }
    public void ShowGizmosButton()
    {
        m_ShowGizmosButtonActive=!m_ShowGizmosButtonActive;
        if(m_ShowGizmosButtonAnim.isPlaying)
        {
            m_ShowGizmosButtonAnim[m_ShowGizmosButtonAnim.clip.name].time=0.0f;
            m_ShowGizmosButtonAnim.Sample();
            m_ShowGizmosButtonAnim.Stop();
            m_Character.SetUseSkillGizmos(false);
        }
        else
        { 
            m_ShowGizmosButtonAnim[m_ShowGizmosButtonAnim.clip.name].time=m_ButtonAnimTime;
            m_ShowGizmosButtonAnim.Sample();
            m_ShowGizmosButtonAnim.Play();
            m_Character.SetUseSkillGizmos(true);
        }
    }
}
