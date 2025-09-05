using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class IngameEnemyUI : NetworkBehaviour
{
    CameraController m_CameraController;
    RectTransform m_CanvasRectTransform;

    public GameObject m_BuffMarksCanvasPrefab;
    public float m_BuffMarksCanvasPosY;
    GameObject m_BuffMarksCanvas;
    public GameObject m_WorldCanvas;
    public RectTransform m_HUDRectTransform;
    public Slider m_IngameHealthBar;
    public Slider m_IngameCorruptedHealthBar;
    public Vector2 m_MinSize;
    public Vector2 m_MaxSize;
    float m_InitialPositionY;

    [Header("DAMAGE NUMBERS")]
    public GameObject m_DamageNumbers;
    public Vector3 m_DamageNumbersPosOffset;
    public TMP_FontAsset m_PhysDamageFont;
    public TMP_FontAsset m_MagicDamageFont;
    public List<DamageInstance> m_DamageInstanceList = new List<DamageInstance>();

    [Header("HEALTH NUMBERS")]
    public GameObject m_HealthNumbers;
    public Vector3 m_HealthNumbersPosOffset;
    public List<HealthInstance> m_HealthInstanceList = new List<HealthInstance>();


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        m_CanvasRectTransform = m_WorldCanvas.GetComponent<RectTransform>();
        m_InitialPositionY = m_HUDRectTransform.anchoredPosition.y;
        if (!IsSpawned || !HasAuthority)
            return;
        GameObject l_BuffMarksObject = Instantiate(m_BuffMarksCanvasPrefab, null);
        l_BuffMarksObject.GetComponent<NetworkObject>().Spawn();
        l_BuffMarksObject.GetComponent<NetworkObject>().TrySetParent(transform, false);
        m_BuffMarksCanvas=l_BuffMarksObject;
        m_BuffMarksCanvas.GetComponent<RectTransform>().anchoredPosition=new Vector2(0.0f, m_BuffMarksCanvasPosY);
    }
    private void Update()
    {
        if (m_DamageInstanceList.Count > 0)
        {
            for (int i = m_DamageInstanceList.Count - 1; i >= 0; --i)
            {
                m_DamageInstanceList[i].m_Timer += Time.deltaTime;
                if (m_DamageInstanceList[i].m_Timer >= 0.1f)
                {
                    SpawnDamageNumbers(m_DamageInstanceList[i].m_PhysDamage, m_DamageInstanceList[i].m_MagicDamage);
                    m_DamageInstanceList.Remove(m_DamageInstanceList[i]);
                }
            }
        }
        if (m_HealthInstanceList.Count > 0)
        {
            for (int i = m_HealthInstanceList.Count - 1; i >= 0; --i)
            {
                m_HealthInstanceList[i].m_Timer += Time.deltaTime;
                if (m_HealthInstanceList[i].m_Timer >= 0.1f)
                {
                    SpawnHealthNumbers(m_HealthInstanceList[i].m_Health);
                    m_HealthInstanceList.Remove(m_HealthInstanceList[i]);
                }
            }
        }
    }
    void LateUpdate()
    {
        if (m_CameraController != null && m_CanvasRectTransform != null)
        {
            m_CanvasRectTransform.LookAt(m_WorldCanvas.transform.position + m_CameraController.GetCamera().transform.rotation * Vector3.forward, m_CameraController.GetCamera().transform.rotation * Vector3.up);

            float l_ZoomPct = m_CameraController.GetZoomPct();
            Vector3 l_TargetScale = Vector3.Lerp(m_MaxSize, m_MinSize, l_ZoomPct);
            l_TargetScale.z = 1.0f;
            m_HUDRectTransform.localScale = Vector3.Lerp(m_HUDRectTransform.localScale, l_TargetScale, Time.deltaTime * m_CameraController.m_ZoomSpeed);
            Vector2 l_TargetPosition = new Vector2(m_HUDRectTransform.anchoredPosition.x, Mathf.Lerp(m_InitialPositionY, m_InitialPositionY - 60.0f, l_ZoomPct));
            m_HUDRectTransform.anchoredPosition = Vector2.Lerp(m_HUDRectTransform.anchoredPosition, l_TargetPosition, Time.deltaTime * m_CameraController.m_ZoomSpeed);
        }
    }

    public void AddDamageInstance(float PhysDamage, float MagicDamage, string SourceId)
    {
        DamageInstance l_DamageInstance = new DamageInstance(PhysDamage, MagicDamage, SourceId);
        if (GetDamageInstance(SourceId) != null)
            GetDamageInstance(SourceId).AddDamage(PhysDamage, MagicDamage);
        else
            m_DamageInstanceList.Add(l_DamageInstance);
    }
    public void AddHealthInstance(float HelthToAdd)
    {
        HealthInstance l_HealthInstance = new HealthInstance(HelthToAdd);
        m_HealthInstanceList.Add(l_HealthInstance);
    }
    DamageInstance GetDamageInstance(string SourceId)
    {
        foreach (DamageInstance Instance in m_DamageInstanceList)
        {
            if (Instance.m_Id == SourceId)
                return Instance;
        }
        return null;
    }
    public void SpawnDamageNumbers(float PhysDamage, float MagicDamage)
    {
        Vector3 l_PosOffset = m_DamageNumbersPosOffset;
        if (PhysDamage > 0.0f)
        {
            GameObject l_PhysDamageText = Instantiate(m_DamageNumbers, m_WorldCanvas.transform);
            l_PhysDamageText.GetComponent<RectTransform>().localPosition = Vector3.zero + l_PosOffset;
            l_PosOffset.y -= 0.5f;
            TextMeshProUGUI l_TextMesh = l_PhysDamageText.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            l_TextMesh.font = m_PhysDamageFont;
            l_TextMesh.text = PhysDamage.ToString("f0");
        }
        if (MagicDamage > 0.0f)
        {
            GameObject l_MagicDamageText = Instantiate(m_DamageNumbers, m_WorldCanvas.transform);
            l_MagicDamageText.GetComponent<RectTransform>().localPosition = Vector3.zero + l_PosOffset;
            TextMeshProUGUI l_TextMesh = l_MagicDamageText.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            l_TextMesh.font = m_MagicDamageFont;
            l_TextMesh.text = MagicDamage.ToString("f0");
        }
    }
    public void SpawnHealthNumbers(float Health)
    {
        Vector3 l_PosOffset = m_HealthNumbersPosOffset;
        if (Health > 0.0f)
        {
            GameObject l_HealthText = Instantiate(m_HealthNumbers, m_WorldCanvas.transform);
            l_HealthText.GetComponent<RectTransform>().localPosition = Vector3.zero + l_PosOffset;
            l_PosOffset.y -= 0.5f;
            TextMeshProUGUI l_TextMesh = l_HealthText.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            l_TextMesh.text = Health.ToString("f0");
        }
    }

    public void UpdateHealthBars(float HealthRounded, float MaxHealth, float CorruptedHealth)
    {
        m_IngameHealthBar.value=HealthRounded/MaxHealth;
        m_IngameCorruptedHealthBar.value=CorruptedHealth/MaxHealth;
    }
    public void ChangeHealthColor(Color FillColor)
    {
        m_IngameHealthBar.fillRect.GetComponent<Image>().color = FillColor;
    }
    public void ChangeCorruptedHealthColor(Color FillColor) 
    {
        m_IngameCorruptedHealthBar.fillRect.GetComponent<Image>().color = FillColor;
    }

    //GETTERS AND SETTERS
    public void SetCameraController(CameraController _Camera)
    {
        m_CameraController = _Camera;
    }
    public CameraController GetCameraController()
    {
        return m_CameraController;
    }
    public GameObject GetBuffMarksCanvas() 
    {
        return m_BuffMarksCanvas;
    }
}