using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using System.Runtime.InteropServices;
using System;
using static Unity.VisualScripting.Member;

public class CharacterMaster : NetworkBehaviour, ITakeDamage
{
    Animator m_CharacterAnimator;
    AudioSource m_AudioSource;
    GameManager m_GameManager;

    [Header("CAMERA")]
    public GameObject m_CameraPrefab;
    CameraController m_CharacterCamera;

    [Header("UI")]
    public GameObject m_CharacterUIPrefab;
    CharacterUI m_CharacterUI;
    public GameObject m_PracticeModeUIPrefab;
    PracticeModeUI m_PracticeModeUI;
    public GameObject m_OptionsUIPrefab;
    OptionsUI m_OptionsUI;
    public IngameCharacterUI m_IngameCharacterUI;
    public SkillIndicatorUI m_SkillIndicatorUI;

    [Header("CHARACTER STATS")]
    public CharacterStats m_CharacterStats;

    [Header("RECALL")]
    public float m_RecallTime;
    public Transform m_RecallTpPoint;
    float m_CurrentRecallTime;
    bool m_Recalling;

    Vector3 m_DesiredPosition;
    public Transform m_DesiredEnemy;
    bool m_GoingToDesiredPosition;
    bool m_LookingForNextPosition;
    bool m_Attacking;
    bool m_Disabled;
    public float m_TimeSinceLastAuto;
    float m_AttackAnimLength;

    [Header("SUMMONERS")]
    public Summoner m_SummSpell1;
    public Summoner m_SummSpell2;
    public KeyCode m_SummSpell1Key;
    public KeyCode m_SummSpell2Key;

    [Header("SKILLS")]
    public Skill m_PassiveSkill;
    public Skill m_QSkill;
    public Skill m_WSkill;
    public Skill m_ESkill;
    public Skill m_RSkill;
    public KeyCode m_QSkillKey;
    public KeyCode m_WSkillKey;
    public KeyCode m_ESkillKey;
    public KeyCode m_RSkillKey;
    public LayerMask m_DamageLayerMask;
    bool m_ShowingGizmos;
    bool m_UseSkillGizmos;
    int m_QSkillLevel;
    int m_WSkillLevel;
    int m_ESkillLevel;
    int m_RSkillLevel;

    [Header("INPUT BUFFERING")]
    public InputBufferController m_InputBufferController;
    public delegate void InputDelegate();
    public InputDelegate m_QInputDelegate;
    public InputDelegate m_WInputDelegate;
    public InputDelegate m_EInputDelegate;
    public InputDelegate m_RInputDelegate;
    public InputDelegate m_Summ1InputDelegate;
    public InputDelegate m_Summ2InputDelegate;

    float m_TimeSinceLastMovement;

    PlayerInputs m_PlayerInputActions;
    InputAction m_MovementAction;

    //SETTINGS
    public bool m_UseKeyboardMovement;

    //public int m_MouseSpeed=10;
    //[DllImport("user32.dll")]
    //public static extern int SystemParametersInfo( int uAction, int uParam, IntPtr lpvParam, int fuWinIni);
    //public const int SPI_SETMOUSESPEED = 113;

    [Header("AUTOATTACK")]
    public GameObject m_RangedAutoAttack;
    public Transform m_RangedAutoSpawnPoint;

    private void Awake()
    {
        m_PlayerInputActions = InputManager.m_InputActions;
    }
    private void OnEnable()
    {
        //m_MovementAction=m_PlayerInputActions.Player.Movement;
        //m_MovementAction.Enable();

        m_PlayerInputActions.Player.QSkill.performed += QSkillInput;
        m_PlayerInputActions.Player.QSkill.Enable();
        m_PlayerInputActions.Player.WSkill.performed += WSkillInput;
        m_PlayerInputActions.Player.WSkill.Enable();
        m_PlayerInputActions.Player.ESkill.performed += ESkillInput;
        m_PlayerInputActions.Player.ESkill.Enable();
        m_PlayerInputActions.Player.RSkill.performed += RSkillInput;
        m_PlayerInputActions.Player.RSkill.Enable();
        m_PlayerInputActions.Player.DSummoner.performed += SummonerSpell1Input;
        m_PlayerInputActions.Player.DSummoner.Enable();
        m_PlayerInputActions.Player.FSummoner.performed += SummonerSpell2Input;
        m_PlayerInputActions.Player.FSummoner.Enable();
        m_PlayerInputActions.Player.Back.performed += UseRecall;
        m_PlayerInputActions.Player.Back.Enable();
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (m_GameManager == null)
            m_GameManager = GameManager.m_GameManagerInstance;
        m_GameManager.AddToPlayerList(this);

        if (m_CharacterCamera == null)
            m_CharacterCamera = Instantiate(m_CameraPrefab, null).GetComponent<CameraController>();
        m_CharacterCamera.SetFollowTarget(transform);

        if (m_CharacterUI == null)
            m_CharacterUI = Instantiate(m_CharacterUIPrefab, GameObject.Find("UI").transform).GetComponent<CharacterUI>();
        m_CharacterUI.SetPlayer(this);

        if (m_OptionsUI == null)
            m_OptionsUI = Instantiate(m_OptionsUIPrefab, GameObject.Find("UI").transform).GetComponent<OptionsUI>();
        m_OptionsUI.SetPlayer(this);

        m_IngameCharacterUI.SetCameraController(m_CharacterCamera);

        m_RecallTpPoint = GameObject.Find("AllySpawnPoint").transform;

        if (!IsSpawned || !HasAuthority)
        {
        }
        else
        {
            m_OptionsUI.InitSettings();
        }

        m_SkillIndicatorUI.SetPlayer(this);

        m_CharacterAnimator = GetComponent<Animator>();
        m_AudioSource = GetComponent<AudioSource>();
        AnimationClip[] l_Clips = m_CharacterAnimator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in l_Clips)
        {
            switch (clip.name)
            {
                case "AutoAttack":
                    m_AttackAnimLength = clip.length;
                    break;
            }
        }
        SetInitStats();
        SetZeroCooldown(false);
        m_GoingToDesiredPosition = false;
        m_DesiredEnemy = null;

        m_QInputDelegate = QSkill;
        m_WInputDelegate = WSkill;
        m_EInputDelegate = ESkill;
        m_RInputDelegate = RSkill;
        m_Summ1InputDelegate = SummonerSpell1;
        m_Summ2InputDelegate = SummonerSpell2;


        if (!IsSpawned || !HasAuthority)
        {
            m_CharacterCamera.GetCamera().gameObject.SetActive(false);
            m_CharacterUI.gameObject.SetActive(false);
            m_OptionsUI.gameObject.SetActive(false);
            return;
        }

        if (m_PracticeModeUI == null)
            m_PracticeModeUI = Instantiate(m_PracticeModeUIPrefab, null).GetComponent<PracticeModeUI>();
        m_PracticeModeUI.SetPlayer(this);
        m_PracticeModeUI.GetComponent<NetworkObject>().SpawnWithOwnership(GetComponent<NetworkObject>().OwnerClientId);
        SpawnCanvasRpc(m_PracticeModeUI.GetComponent<NetworkObject>());
    }
    [Rpc(SendTo.Everyone)]
    void SpawnCanvasRpc(NetworkObjectReference PracticeUI)
    {
        NetworkObject l_PracticeUI = PracticeUI;
        l_PracticeUI.transform.SetParent(GameObject.Find("UI").transform, false);
        if (!IsSpawned || !HasAuthority)
        {
            l_PracticeUI.gameObject.SetActive(false);
        }
    }

    //public static void SetMouseSpeed(int intSpeed )
    //{
    //    IntPtr ptr = new IntPtr(intSpeed);

    //    int b = SystemParametersInfo(SPI_SETMOUSESPEED, 0, ptr, 0);

    //    if (b == 0)
    //    {
    //        Console.WriteLine("Not able to set speed");
    //    }
    //    else if ( b == 1 )
    //    {
    //        Console.WriteLine("Successfully done");
    //    }

    //}

    protected virtual void Update()
    {
        if (!IsSpawned || !HasAuthority)
        {
            return;
        }

        //if(Input.GetKeyDown(KeyCode.N)) 
        //{
        //    SetMouseSpeed(m_MouseSpeed);
        //}

        MouseTargeting();
        if (!m_Disabled)
        {
            if (m_UseKeyboardMovement)
                KeyboardMovement();
            CharacterMovement();

            m_InputBufferController.CheckInputBuffer();
        }

        m_CharacterUI.UpdatePrimStats(m_CharacterStats.GetAttackDamage(), m_CharacterStats.GetArmor(), m_CharacterStats.GetAttackSpeed(), m_CharacterStats.GetCritChance(),
            m_CharacterStats.GetAbilityPower(), m_CharacterStats.GetMagicRes(), m_CharacterStats.GetCdr(), m_CharacterStats.GetMovSpeed());
        m_CharacterUI.UpdateSeconStats(m_CharacterStats.GetHealthRegen(), m_CharacterStats.GetArmorPenFixed(), m_CharacterStats.GetArmorPenPct(), m_CharacterStats.GetLifeSteal(),
            m_CharacterStats.GetAttackRange(), m_CharacterStats.GetManaRegen(), m_CharacterStats.GetMagicPenFixed(), m_CharacterStats.GetMagicPenPct(), m_CharacterStats.GetOmniDrain(),
            m_CharacterStats.GetTenacity(), m_CharacterStats.GetShieldsHealsPower());
        m_CharacterUI.UpdateHealthManaBars(m_CharacterStats.GetCurrentHealth(), m_CharacterStats.GetMaxHealth(), m_CharacterStats.GetCurrentMana(), m_CharacterStats.GetMaxMana());
        UpdateIngameBarsRpc();

        if (m_Recalling)
        {
            m_CharacterUI.UpdateCastingUI(m_CurrentRecallTime, m_RecallTime);
            m_CurrentRecallTime -= Time.deltaTime;
            if (m_CurrentRecallTime <= 0.0f)
                TeleportToSpawn();
        }

        if (m_Attacking)
        {
            if (Input.GetKeyDown(KeyCode.S))
                StopAttacking();
#if UNITY_EDITOR
            m_TimeSinceLastAuto += Time.deltaTime;
#endif
        }

        if (m_CharacterStats.GetCurrentLevel() < 18)
            m_CharacterUI.UpdateExpBar(m_CharacterStats.GetCurrentExp(), m_CharacterStats.m_CharacterBaseStats.m_ExpPerLevel[m_CharacterStats.GetCurrentLevel()]);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!m_OptionsUI.gameObject.activeSelf)
                m_OptionsUI.ShowOptionsUI();
            else
                m_OptionsUI.HideOptionsUI();
        }
    }

    [Rpc(SendTo.Everyone)]
    public void UpdateIngameBarsRpc()
    {
        m_IngameCharacterUI.UpdateHealthManaBars(m_CharacterStats.GetCurrentHealth(), m_CharacterStats.GetMaxHealth(), m_CharacterStats.GetCurrentMana(), m_CharacterStats.GetMaxMana());
    }

    public Vector3 GetMouseDir()
    {
        Vector3 l_MousePosition = Input.mousePosition;
        l_MousePosition.z = 10.0f;
        return m_CharacterCamera.GetCamera().ScreenToWorldPoint(l_MousePosition) - m_CharacterCamera.GetCamera().transform.position;
    }
    public Transform GetEnemy()
    {
        Vector3 l_MouseDirection = GetMouseDir();
        RaycastHit l_CameraRaycastHit;
        if (Physics.Raycast(m_CharacterCamera.GetCamera().transform.position, l_MouseDirection, out l_CameraRaycastHit, 1000.0f, m_CharacterCamera.m_CameraLayerMask))
        {
            if (l_CameraRaycastHit.transform.CompareTag("Enemy"))
                return l_CameraRaycastHit.transform;
        }
        return null;
    }
    public CharacterStats GetSelectedCharacterStats()
    {
        Vector3 l_MouseDirection = GetMouseDir();
        RaycastHit l_CameraRaycastHit;
        if (Physics.Raycast(m_CharacterCamera.GetCamera().transform.position, l_MouseDirection, out l_CameraRaycastHit, 1000.0f, m_CharacterCamera.m_SelectHitboxLayerMask))
        {
            if (l_CameraRaycastHit.transform.TryGetComponent(out ITakeDamage Stats))
                return Stats.GetCharacterStats();
        }
        return null;
    }
    public Vector3 GetPosition()
    {
        Vector3 l_MouseDirection = GetMouseDir();
        RaycastHit l_CameraRaycastHit;
        if (Physics.Raycast(m_CharacterCamera.GetCamera().transform.position, l_MouseDirection, out l_CameraRaycastHit, 1000.0f, m_CharacterCamera.m_TerrainLayerMask))
        {
            if (l_CameraRaycastHit.transform.CompareTag("Terrain"))
                return l_CameraRaycastHit.point;
        }
        return Vector3.zero;
    }
    void MouseTargeting()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (m_UseSkillGizmos && IsAnySkillBeingUsed())
            {
                m_SkillIndicatorUI.ClearDeletableSkillIndicatorUI();
                m_SkillIndicatorUI.ClearTargetSkillIndicatorUI();
                m_ShowingGizmos = false;
                StopSkillsCancelableWithMouseClick();
            }
            else
            {
                m_DesiredEnemy = GetEnemy();
                if (m_DesiredEnemy)
                {
                    NetworkObject l_Enemy = m_DesiredEnemy.GetComponent<NetworkObject>();
                    SetDesiredEnemyRpc(l_Enemy);
                    m_DesiredPosition = m_DesiredEnemy.position;
                    m_DesiredPosition.y = 0.0f;
                    m_GoingToDesiredPosition = true;
                    StopRecall();
                }
                if (!m_UseKeyboardMovement)
                    m_LookingForNextPosition = true;
            }
        }
        else if (Input.GetMouseButtonUp(1))
            m_LookingForNextPosition = false;

        if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                CharacterStats l_SelectedCharacterStats = GetSelectedCharacterStats();
                if (l_SelectedCharacterStats)
                    m_CharacterUI.ShowTargetInfoUI(l_SelectedCharacterStats);
                else
                    m_CharacterUI.HideTargetInfoUI();
            }
            m_SkillIndicatorUI.ClearDeletableSkillIndicatorUI();
        }

        if (m_LookingForNextPosition)
        {
            if (GetPosition() != Vector3.zero && !m_DesiredEnemy)
            {
                m_DesiredPosition = GetPosition();
                m_DesiredPosition.y = 0.0f;
                m_DesiredEnemy = null;
                m_GoingToDesiredPosition = true;
                StopAttacking();
                StopRecall();
            }
        }
    }
    [Rpc(SendTo.Everyone)]
    void SetDesiredEnemyRpc(NetworkObjectReference Enemy)
    {
        NetworkObject l_Enemy = Enemy;
        m_DesiredEnemy = l_Enemy.transform;
    }
    void CharacterMovement()
    {
        float l_MinDistance;
        if (m_DesiredEnemy != null)
            l_MinDistance = m_CharacterStats.GetAttackRange() / 100.0f;
        else
            l_MinDistance = 0.1f;

        if (m_GoingToDesiredPosition)
        {
            Vector3 l_CharacterDirection = m_DesiredPosition - transform.position;
            l_CharacterDirection.Normalize();

            if (Input.GetKeyDown(KeyCode.S))
            {
                StopMovement();
                return;
            }

            if (Vector3.Dot(transform.forward, l_CharacterDirection) < 0.0f)
                transform.forward = Vector3.RotateTowards(transform.forward, l_CharacterDirection, Time.deltaTime * 16.0f, 0.0f);
            else
                transform.forward = l_CharacterDirection;

            if (Vector3.Distance(transform.position, m_DesiredPosition) > l_MinDistance)
            {
                transform.position += l_CharacterDirection * (m_CharacterStats.GetMovSpeed() / 100.0f) * Time.deltaTime;
                m_CharacterAnimator.SetBool("IsMoving", true);
            }
            else
            {
                m_GoingToDesiredPosition = false;
                m_CharacterAnimator.SetBool("IsMoving", false);
                StartAttacking();
            }
        }
        else if (m_OptionsUI.m_GameMenu.IsAutoAttackEnabled() && !m_Attacking)
        {
            GameObject l_ClosestEnemy = GetClosestEnemyInRange(m_CharacterStats.GetAttackRange() / 100.0f);
            if (l_ClosestEnemy)
            {
                m_DesiredEnemy = l_ClosestEnemy.transform;
                StartAttacking();
            }
        }
    }
    void KeyboardMovement()
    {
        m_TimeSinceLastMovement += Time.deltaTime;
        Vector2 l_MovementInput = m_MovementAction.ReadValue<Vector2>();
        Vector3 l_CharacterDirection = Vector3.zero;
        bool l_IsPressingKey = false;
        if (l_MovementInput.x > 0.0f)
        {
            l_CharacterDirection += Vector3.right;
            l_IsPressingKey = true;
        }
        else if (l_MovementInput.x < 0.0f)
        {
            l_CharacterDirection -= Vector3.right;
            l_IsPressingKey = true;
        }
        if (l_MovementInput.y > 0.0f)
        {
            l_CharacterDirection += Vector3.forward;
            l_IsPressingKey = true;
        }
        else if (l_MovementInput.y < 0.0f)
        {
            l_CharacterDirection -= Vector3.forward;
            l_IsPressingKey = true;
        }

        if (l_IsPressingKey)
            m_TimeSinceLastMovement = 0.0f;

        l_CharacterDirection.Normalize();
        if (Vector3.Dot(transform.forward, l_CharacterDirection) < 0.0f)
            transform.forward = Vector3.RotateTowards(transform.forward, l_CharacterDirection, Time.deltaTime * 16.0f, 0.0f);
        else
            transform.forward = Vector3.Lerp(transform.forward, l_CharacterDirection, Time.deltaTime * 8.0f);
        transform.position += l_CharacterDirection * (m_CharacterStats.GetMovSpeed() / 100.0f) * Time.deltaTime;
        if (l_CharacterDirection != Vector3.zero)
            m_CharacterAnimator.SetBool("IsMoving", true);
        else if (!l_IsPressingKey && m_TimeSinceLastMovement > 0.05f)
            m_CharacterAnimator.SetBool("IsMoving", false);
    }
    public GameObject GetClosestEnemyInRange(float Range)
    {
        GameObject[] l_Targets = GameObject.FindGameObjectsWithTag("Enemy");
        if (l_Targets.Length == 0)
            return null;

        float l_Dist = 0.0f;
        GameObject l_ClosestTarget = null;

        for (int i = 0; i < l_Targets.Length; ++i)
        {
            l_Dist = (l_Targets[i].transform.position - transform.position).magnitude;
            if (l_Dist <= Range)
            {
                l_ClosestTarget = l_Targets[i];
            }
        }
        return l_ClosestTarget;
    }
    public void GetEnemyWithMouse()
    {
        Vector3 l_MousePosition = Input.mousePosition;
        l_MousePosition.z = 10.0f;
        Vector3 l_MouseDirection = m_CharacterCamera.GetCamera().ScreenToWorldPoint(l_MousePosition) - m_CharacterCamera.GetCamera().transform.position;
        RaycastHit l_CameraRaycastHit;
        if (Physics.Raycast(m_CharacterCamera.GetCamera().transform.position, l_MouseDirection, out l_CameraRaycastHit, 1000.0f, m_CharacterCamera.m_CameraLayerMask))
        {
            if (l_CameraRaycastHit.transform.CompareTag("Enemy"))
            {
                m_DesiredEnemy = l_CameraRaycastHit.transform;
                m_DesiredPosition = m_DesiredEnemy.position;
                m_DesiredPosition.y = 0.0f;
                m_GoingToDesiredPosition = true;
                StopRecall();
            }
        }
    }
    public Vector3 GetPositionWithMouse()
    {
        Vector3 l_MousePosition = Input.mousePosition;
        l_MousePosition.z = 10.0f;
        Vector3 l_MouseDirection = m_CharacterCamera.GetCamera().ScreenToWorldPoint(l_MousePosition) - m_CharacterCamera.GetCamera().transform.position;
        RaycastHit l_CameraRaycastHit;
        if (Physics.Raycast(m_CharacterCamera.GetCamera().transform.position, l_MouseDirection, out l_CameraRaycastHit, 1000.0f, m_CharacterCamera.m_TerrainLayerMask))
        {
            if (l_CameraRaycastHit.transform.CompareTag("Terrain"))
            {
                return l_CameraRaycastHit.point;
            }
        }
        return m_CharacterCamera.GetCamera().ScreenToWorldPoint(l_MousePosition);
    }
    public void LookAt(Vector3 TargetPosition)
    {
        Vector3 l_Direction = TargetPosition - transform.position;
        l_Direction.y = 0f;

        if (l_Direction != Vector3.zero)
        {
            Quaternion l_Rotation = Quaternion.LookRotation(l_Direction);
            transform.rotation = l_Rotation;
        }
    }
    public void StopMovement()
    {
        m_GoingToDesiredPosition = false;
        m_LookingForNextPosition = false;
        m_CharacterAnimator.SetBool("IsMoving", false);
    }
    protected virtual void StartAttacking()
    {
        if (m_DesiredEnemy)
        {
            Vector3 l_Dir = m_DesiredEnemy.position - transform.position;
            l_Dir.y = 0.0f;
            l_Dir.Normalize();
            transform.forward = l_Dir;
            m_Attacking = true;
            m_CharacterAnimator.SetBool("IsAAttacking", true);
        }
    }
    protected virtual void StopAttacking()
    {
        if (m_Attacking)
        {
            m_Attacking = false;
            m_CharacterAnimator.SetBool("IsAAttacking", false);
        }
    }

    void QSkillInput(InputAction.CallbackContext obj)
    {
        if (m_QSkill.GetHaveLoads())
        {
            if (m_QSkillLevel <= 0)
            {
                Debug.Log("Q STILL LOCKED BOBI");
            }
            else if (m_QSkill.GetActualLoads() <= 0)
            {
                Debug.Log("Q DON'T HAVE LOADS, STILL ON COOLDOWN BOBI");
            }
            else if (m_CharacterStats.GetCurrentMana() < m_QSkill.GetMana(m_QSkillLevel))
            {
                Debug.Log("NOT ENOUGH MANA TO USE Q");
            }
            else
            {
                m_InputBufferController.AddInput(new InputBufferAction(InputBufferAction.Action.QPRESSED, m_QInputDelegate));
            }
        }
        else
        {
            if (m_QSkillLevel <= 0)
            {
                Debug.Log("Q STILL LOCKED BOBI");
            }
            else if (m_QSkill.GetIsOnCd())
            {
                Debug.Log("Q STILL ON COOLDOWN BOBI");
            }
            else if (m_CharacterStats.GetCurrentMana() < m_QSkill.GetMana(m_QSkillLevel))
            {
                Debug.Log("NOT ENOUGH MANA TO USE Q");
            }
            else
            {
                m_InputBufferController.AddInput(new InputBufferAction(InputBufferAction.Action.QPRESSED, m_QInputDelegate));
            }
        }
    }
    protected virtual void QSkill()
    {
        if (!m_QSkill.GetHaveLoads())
        {
            m_QSkill.SetTimer(m_QSkill.GetCd());
            m_CharacterUI.m_QSkillCdImage.fillAmount = 1.0f;
            m_CharacterUI.m_QSkillCdText.enabled = true;
            m_QSkill.SetIsOnCd(true);
            StartCoroutine(PowersCooldown(m_QSkill));
        }
        else
        {
            m_QSkill.LoadUsed();
            m_QSkill.AddActualLoads(-1);
            m_CharacterUI.m_QSkillLoadsText.text = m_QSkill.GetActualLoads().ToString();

            if (m_WSkill.GetHaveSpecialLoad())
            {
                if (m_QSkill.GetSpecialNextLoad())
                    m_CharacterUI.m_QSkillImage.sprite = m_QSkill.m_SpecialLoadSprite;
                else
                    m_CharacterUI.m_QSkillImage.sprite = m_QSkill.m_Sprite;
            }

            m_IngameCharacterUI.SetLoadsInfo(m_QSkill);

            if (m_QSkill.GetActualLoads() + 1 == m_QSkill.GetMAXLoads())
            {
                m_QSkill.SetTimer(m_QSkill.GetCd());
                m_CharacterUI.m_QSkillCdImage.fillAmount = 1.0f;
                m_CharacterUI.m_QSkillCdText.enabled = true;
                m_QSkill.SetIsOnCd(true);
                StartCoroutine(PowersCooldown(m_QSkill));
            }
        }
        m_CharacterStats.SetCurrentManaRpc(m_CharacterStats.GetCurrentMana() - m_QSkill.GetMana(m_QSkillLevel));
        StopRecall();
    }

    void WSkillInput(InputAction.CallbackContext obj)
    {
        if (m_WSkill.GetHaveLoads())
        {
            if (m_WSkillLevel <= 0)
            {
                Debug.Log("W STILL LOCKED BOBI");
            }
            else if (m_WSkill.GetActualLoads() <= 0)
            {
                Debug.Log("W DON'T HAVE LOADS, STILL ON COOLDOWN BOBI");
            }
            else if (m_CharacterStats.GetCurrentMana() < m_WSkill.GetMana(m_WSkillLevel))
            {
                Debug.Log("NOT ENOUGH MANA TO USE W");
            }
            else
            {
                m_InputBufferController.AddInput(new InputBufferAction(InputBufferAction.Action.WPRESSED, m_WInputDelegate));
            }
        }
        else
        {
            if (m_WSkillLevel <= 0)
            {
                Debug.Log("W STILL LOCKED BOBI");
            }
            else if (m_WSkill.GetIsOnCd())
            {
                Debug.Log("W STILL ON COOLDOWN BOBI");
            }
            else if (m_CharacterStats.GetCurrentMana() < m_WSkill.GetMana(m_WSkillLevel))
            {
                Debug.Log("NOT ENOUGH MANA TO USE W");
            }
            else
            {
                m_InputBufferController.AddInput(new InputBufferAction(InputBufferAction.Action.WPRESSED, m_WInputDelegate));
            }
        }
    }
    protected virtual void WSkill()
    {
        if (!m_WSkill.GetHaveLoads())
        {
            m_WSkill.SetTimer(m_WSkill.GetCd());
            m_CharacterUI.m_WSkillCdImage.fillAmount = 1.0f;
            m_CharacterUI.m_WSkillCdText.enabled = true;
            m_WSkill.SetIsOnCd(true);
            StartCoroutine(PowersCooldown(m_WSkill));
        }
        else
        {
            m_WSkill.LoadUsed();
            m_WSkill.AddActualLoads(-1);
            m_CharacterUI.m_WSkillLoadsText.text = m_WSkill.GetActualLoads().ToString();

            if (m_WSkill.GetHaveSpecialLoad())
            {
                if (m_WSkill.GetSpecialNextLoad())
                    m_CharacterUI.m_WSkillImage.sprite = m_WSkill.m_SpecialLoadSprite;
                else
                    m_CharacterUI.m_WSkillImage.sprite = m_WSkill.m_Sprite;
            }

            m_IngameCharacterUI.SetLoadsInfo(m_WSkill);

            if (m_WSkill.GetActualLoads() + 1 == m_WSkill.GetMAXLoads())
            {
                m_WSkill.SetTimer(m_WSkill.GetCd());
                m_CharacterUI.m_WSkillCdImage.fillAmount = 1.0f;
                m_CharacterUI.m_WSkillCdText.enabled = true;
                m_WSkill.SetIsOnCd(true);
                StartCoroutine(PowersCooldown(m_WSkill));
            }
        }
        m_CharacterStats.SetCurrentManaRpc(m_CharacterStats.GetCurrentMana() - m_WSkill.GetMana(m_WSkillLevel));
        StopRecall();
    }

    void ESkillInput(InputAction.CallbackContext obj)
    {
        if (m_ESkill.GetHaveLoads())
        {
            if (m_ESkillLevel <= 0)
            {
                Debug.Log("E STILL LOCKED BOBI");
            }
            else if (m_ESkill.GetActualLoads() <= 0)
            {
                Debug.Log("E DON'T HAVE LOADS, STILL ON COOLDOWN BOBI");
            }
            else if (m_CharacterStats.GetCurrentMana() < m_ESkill.GetMana(m_ESkillLevel))
            {
                Debug.Log("NOT ENOUGH MANA TO USE E");
            }
            else
            {
                m_InputBufferController.AddInput(new InputBufferAction(InputBufferAction.Action.EPRESSED, m_EInputDelegate));
            }
        }
        else
        {
            if (m_ESkillLevel <= 0)
            {
                Debug.Log("E STILL LOCKED BOBI");
            }
            else if (m_ESkill.GetIsOnCd())
            {
                Debug.Log("E STILL ON COOLDOWN BOBI");
            }
            else if (m_CharacterStats.GetCurrentMana() < m_ESkill.GetMana(m_ESkillLevel))
            {
                Debug.Log("NOT ENOUGH MANA TO USE E");
            }
            else
            {
                m_InputBufferController.AddInput(new InputBufferAction(InputBufferAction.Action.EPRESSED, m_EInputDelegate));
            }
        }
    }
    protected virtual void ESkill()
    {
        if (!m_ESkill.GetHaveLoads())
        {
            m_ESkill.SetTimer(m_ESkill.GetCd());
            m_CharacterUI.m_ESkillCdImage.fillAmount = 1.0f;
            m_CharacterUI.m_ESkillCdText.enabled = true;
            m_ESkill.SetIsOnCd(true);
            StartCoroutine(PowersCooldown(m_ESkill));
        }
        else
        {

            m_ESkill.LoadUsed();
            m_ESkill.AddActualLoads(-1);
            m_CharacterUI.m_ESkillLoadsText.text = m_ESkill.GetActualLoads().ToString();

            if (m_ESkill.GetHaveSpecialLoad())
            {
                if (m_ESkill.GetSpecialNextLoad())
                    m_CharacterUI.m_ESkillImage.sprite = m_ESkill.m_SpecialLoadSprite;
                else
                    m_CharacterUI.m_ESkillImage.sprite = m_ESkill.m_Sprite;
            }
            m_IngameCharacterUI.SetLoadsInfo(m_ESkill);

            if (m_ESkill.GetActualLoads() + 1 == m_ESkill.GetMAXLoads())
            {
                m_ESkill.SetTimer(m_ESkill.GetCd());
                m_CharacterUI.m_ESkillCdImage.fillAmount = 1.0f;
                m_CharacterUI.m_ESkillCdText.enabled = true;
                m_ESkill.SetIsOnCd(true);
                StartCoroutine(PowersCooldown(m_ESkill));
            }
        }
        m_CharacterStats.SetCurrentManaRpc(m_CharacterStats.GetCurrentMana() - m_ESkill.GetMana(m_ESkillLevel));
        StopRecall();
    }

    void RSkillInput(InputAction.CallbackContext obj)
    {
        if (m_RSkill.GetHaveLoads())
        {
            if (m_RSkillLevel <= 0)
            {
                Debug.Log("R STILL LOCKED BOBI");
            }
            else if (m_RSkill.GetActualLoads() <= 0)
            {
                Debug.Log("R DON'T HAVE LOADS, STILL ON COOLDOWN BOBI");
            }
            else if (m_CharacterStats.GetCurrentMana() < m_RSkill.GetMana(m_RSkillLevel))
            {
                Debug.Log("NOT ENOUGH MANA TO USE R");
            }
            else
            {
                m_InputBufferController.AddInput(new InputBufferAction(InputBufferAction.Action.RPRESSED, m_RInputDelegate));
            }
        }
        else
        {
            if (m_RSkillLevel <= 0)
            {
                Debug.Log("R STILL LOCKED BOBI");
            }
            else if (m_RSkill.GetIsOnCd())
            {
                Debug.Log("R STILL ON COOLDOWN BOBI");
            }
            else if (m_CharacterStats.GetCurrentMana() < m_RSkill.GetMana(m_RSkillLevel))
            {
                Debug.Log("NOT ENOUGH MANA TO USE R");
            }
            else
            {
                m_InputBufferController.AddInput(new InputBufferAction(InputBufferAction.Action.RPRESSED, m_RInputDelegate));
            }
        }
    }
    protected virtual void RSkill()
    {
        if (m_QSkill.GetHaveLoads())
        {
            m_RSkill.SetTimer(m_RSkill.GetCd());
            m_CharacterUI.m_RSkillCdImage.fillAmount = 1.0f;
            m_CharacterUI.m_RSkillCdText.enabled = true;
            m_RSkill.SetIsOnCd(true);
            StartCoroutine(PowersCooldown(m_RSkill));
        }
        else
        {
            m_RSkill.LoadUsed();
            m_RSkill.AddActualLoads(-1);
            m_CharacterUI.m_RSkillLoadsText.text = m_RSkill.GetActualLoads().ToString();

            if (m_RSkill.GetHaveSpecialLoad())
            {
                if (m_RSkill.GetSpecialNextLoad())
                    m_CharacterUI.m_RSkillImage.sprite = m_RSkill.m_SpecialLoadSprite;
                else
                    m_CharacterUI.m_RSkillImage.sprite = m_RSkill.m_Sprite;
            }

            m_IngameCharacterUI.SetLoadsInfo(m_RSkill);

            if (m_RSkill.GetActualLoads() + 1 == m_RSkill.GetMAXLoads())
            {
                m_RSkill.SetTimer(m_RSkill.GetCd());
                m_CharacterUI.m_RSkillCdImage.fillAmount = 1.0f;
                m_CharacterUI.m_RSkillCdText.enabled = true;
                m_RSkill.SetIsOnCd(true);
                StartCoroutine(PowersCooldown(m_RSkill));
            }
        }
        m_CharacterStats.SetCurrentManaRpc(m_CharacterStats.GetCurrentMana() - m_RSkill.GetMana(m_RSkillLevel));
        StopRecall();
    }

    void SummonerSpell1Input(InputAction.CallbackContext obj)
    {
        if (m_SummSpell1.GetIsOnCd())
        {
            Debug.Log("SS1 STILL ON COOLDOWN BOBI");
        }
        else
        {
            m_InputBufferController.AddInput(new InputBufferAction(InputBufferAction.Action.DPRESSED, m_Summ1InputDelegate));
        }
    }
    void SummonerSpell1()
    {
        m_SummSpell1.SetTimer(m_SummSpell1.GetCd());
        m_CharacterUI.m_SumSpell1CdImage.fillAmount = 1.0f;
        m_CharacterUI.m_SumSpell1CdText.enabled = true;
        m_SummSpell1.SetIsOnCd(true);
        StartCoroutine(PowersCooldown(m_SummSpell1));
        StopRecall();
    }

    void SummonerSpell2Input(InputAction.CallbackContext obj)
    {
        if (m_SummSpell2.GetIsOnCd())
        {
            Debug.Log("SS2 STILL ON COOLDOWN BOBI");
        }
        else
        {
            m_InputBufferController.AddInput(new InputBufferAction(InputBufferAction.Action.FPRESSED, m_Summ2InputDelegate));
        }
    }
    void SummonerSpell2()
    {
        m_SummSpell2.SetTimer(m_SummSpell2.GetCd());
        m_CharacterUI.m_SumSpell2CdImage.fillAmount = 1.0f;
        m_CharacterUI.m_SumSpell2CdText.enabled = true;
        m_SummSpell2.SetIsOnCd(true);
        StartCoroutine(PowersCooldown(m_SummSpell2));
        StopRecall();
    }

    public IEnumerator PowersCooldown(Power PowerOnCd)
    {
        while (PowerOnCd.GetIsOnCd())
        {
            PowerOnCd.Tick(Time.deltaTime);
            m_CharacterUI.UpdatePowerUI(PowerOnCd.m_PowerType, PowerOnCd.GetTimer(), PowerOnCd.GetCd(), PowerOnCd.GetZeroCooldown());
            yield return null;
        }
        PowersCooldownLoads(PowerOnCd);
    }

    void PowersCooldownLoads(Power PowerOnCd)
    {
        if (!PowerOnCd.GetHaveLoads())
            return;

        PowerOnCd.AddActualLoads(1);

        m_IngameCharacterUI.SetLoadsInfo(PowerOnCd);

        switch (PowerOnCd.m_PowerType)
        {
            case Power.PowerType.QSKILL:
                m_CharacterUI.m_QSkillLoadsText.enabled = true;
                m_CharacterUI.m_QSkillLoadsText.text = PowerOnCd.GetActualLoads().ToString();
                break;
            case Power.PowerType.WSKILL:
                m_CharacterUI.m_WSkillLoadsText.enabled = true;
                m_CharacterUI.m_WSkillLoadsText.text = PowerOnCd.GetActualLoads().ToString();
                break;
            case Power.PowerType.ESKILL:
                m_CharacterUI.m_ESkillLoadsText.enabled = true;
                m_CharacterUI.m_ESkillLoadsText.text = PowerOnCd.GetActualLoads().ToString();
                break;
            case Power.PowerType.RSKILL:
                m_CharacterUI.m_RSkillLoadsText.enabled = true;
                m_CharacterUI.m_RSkillLoadsText.text = PowerOnCd.GetActualLoads().ToString();
                break;
        }

        if (PowerOnCd.GetActualLoads() != PowerOnCd.GetMAXLoads())
        {
            PowerOnCd.SetTimer(PowerOnCd.GetCd());

            switch (PowerOnCd.m_PowerType)
            {
                case Power.PowerType.QSKILL:
                    m_CharacterUI.m_QSkillCdImage.fillAmount = 1.0f;
                    m_CharacterUI.m_QSkillCdText.enabled = true;
                    break;
                case Power.PowerType.WSKILL:
                    m_CharacterUI.m_WSkillCdImage.fillAmount = 1.0f;
                    m_CharacterUI.m_WSkillCdText.enabled = true;
                    break;
                case Power.PowerType.ESKILL:
                    m_CharacterUI.m_ESkillCdImage.fillAmount = 1.0f;
                    m_CharacterUI.m_ESkillCdText.enabled = true;
                    break;
                case Power.PowerType.RSKILL:
                    m_CharacterUI.m_RSkillCdImage.fillAmount = 1.0f;
                    m_CharacterUI.m_RSkillCdText.enabled = true;
                    break;
            }

            PowerOnCd.SetIsOnCd(true);
            StartCoroutine(PowersCooldown(PowerOnCd));
        }


    }
    void UseRecall(InputAction.CallbackContext obj)
    {
        if (!m_Recalling && !m_Disabled)
        {
            m_CharacterUI.SetCastingUIAbilityText("Recall");
            m_CharacterUI.ShowCastingTime();
            m_CharacterUI.ShowCastingUI();
            m_CurrentRecallTime = m_RecallTime;
            m_Recalling = true;
            StopMovement();
            StopAttacking();
        }
    }
    public void StopRecall()
    {
        if (m_Recalling)
        {
            m_CharacterUI.HideCastingUI();
            m_Recalling = false;
        }
    }
    void TeleportToSpawn()
    {
        transform.position = m_RecallTpPoint.position;
        m_Recalling = false;
        m_CharacterUI.HideCastingUI();
    }

    [Rpc(SendTo.Everyone)]
    public virtual void LevelUpRpc()
    {
        m_CharacterStats.LevelUp();
        if (m_CharacterStats.GetCurrentLevel() >= 18)
            m_CharacterUI.UpdateExpBar(1.0f, 1.0f);
        m_IngameCharacterUI.UpdateCharacterLevel(m_CharacterStats.GetCurrentLevel());
        m_CharacterUI.ShowLevelUpSkillButtons();
        m_CharacterUI.UpdateCharacterLevel(m_CharacterStats.GetCurrentLevel());
    }
    public virtual void SetInitStats()
    {
        m_CharacterStats.SetInitStats();

        m_QSkill.SetInitStats();
        m_WSkill.SetInitStats();
        m_ESkill.SetInitStats();
        m_RSkill.SetInitStats();
        m_SummSpell1.SetInitStats();
        m_SummSpell2.SetInitStats();
        m_QSkillLevel = 0;
        m_WSkillLevel = 0;
        m_ESkillLevel = 0;
        m_RSkillLevel = 0;

        m_CharacterUI.SetCharacterSprite(m_CharacterStats.GetCharacterIcon());
        m_CharacterUI.SetPowersSprites(m_PassiveSkill.m_Sprite, m_QSkill.m_Sprite, m_WSkill.m_Sprite, m_ESkill.m_Sprite, m_RSkill.m_Sprite, m_SummSpell1.m_Sprite, m_SummSpell2.m_Sprite);
        m_CharacterUI.UpdateCharacterLevel(m_CharacterStats.GetCurrentLevel());
        m_CharacterUI.ResetSkillLevelPoints();
        m_CharacterUI.HideLevelUpSkillButtons();
        m_CharacterUI.ShowLevelUpSkillButtons();
        m_IngameCharacterUI.UpdateCharacterLevel(m_CharacterStats.GetCurrentLevel());
        m_IngameCharacterUI.SetPlayerName(m_CharacterStats.GetPlayerName());
    }
    public void TakeDamage(float PhysDamage, float MagicDamage, string SourceId)
    {
        if (GetCharacterStats().GetImmuneCC())
            return;

        float l_TotalPhysDamage = PhysDamage / (1.0f + m_CharacterStats.GetArmor() / 100.0f);
        float l_TotalMagicDamage = MagicDamage / (1.0f + m_CharacterStats.GetMagicRes() / 100.0f);
        if (PhysDamage > 0.0f)
            Debug.Log("Taking " + PhysDamage + " physical damage, reduced to " + l_TotalPhysDamage + " damage");
        if (MagicDamage > 0.0f)
            Debug.Log("Taking " + MagicDamage + " magical damage, reduced to " + l_TotalMagicDamage + " damage");
        m_CharacterStats.SetCurrentHealthRpc(m_CharacterStats.GetCurrentHealth() - (l_TotalPhysDamage + l_TotalMagicDamage));
        if (m_CurrentRecallTime > 0.2f)
            StopRecall();
        m_IngameCharacterUI.AddDamageInstance(l_TotalPhysDamage, l_TotalMagicDamage, SourceId);
    }
    public void AddHealth(float HealthToAdd)
    {
        if (m_CharacterStats.GetMaxHealth() == m_CharacterStats.GetCurrentHealth())
        {
            return;
        }
        if (m_CharacterStats.GetMaxHealth() <= m_CharacterStats.GetCurrentHealth() + HealthToAdd)
        {
            m_IngameCharacterUI.AddHealthInstance(m_CharacterStats.GetMaxHealth() - m_CharacterStats.GetCurrentHealth());
            m_CharacterStats.SetCurrentHealthRpc(m_CharacterStats.GetMaxHealth());
            return;
        }

        m_CharacterStats.SetCurrentHealthRpc(m_CharacterStats.GetCurrentHealth() + HealthToAdd);

        m_IngameCharacterUI.AddHealthInstance(HealthToAdd);

    }
    public bool IsAnySkillBeingUsed()
    {
        return (m_QSkill.GetUsingSkill() && m_QSkill.m_CancelableWithMouseClick) || (m_WSkill.GetUsingSkill() && m_WSkill.m_CancelableWithMouseClick)
            || (m_ESkill.GetUsingSkill() && m_ESkill.m_CancelableWithMouseClick) || (m_RSkill.GetUsingSkill() && m_RSkill.m_CancelableWithMouseClick);
    }
    public void StopSkills()
    {
        m_QSkill.SetUsingSkill(false);
        m_WSkill.SetUsingSkill(false);
        m_ESkill.SetUsingSkill(false);
        m_RSkill.SetUsingSkill(false);
    }
    public void StopSkillsCancelableWithMouseClick()
    {
        if (m_QSkill.m_CancelableWithMouseClick)
            m_QSkill.SetUsingSkill(false);
        if (m_WSkill.m_CancelableWithMouseClick)
            m_WSkill.SetUsingSkill(false);
        if (m_ESkill.m_CancelableWithMouseClick)
            m_ESkill.SetUsingSkill(false);
        if (m_RSkill.m_CancelableWithMouseClick)
            m_RSkill.SetUsingSkill(false);
    }

    //LLAMADA POR EVENTO EN LA ANIMACION DE AUTOATAQUE
    protected virtual void PerformAutoAttack()
    {
        if (m_DesiredEnemy == null)
        {
            StopAttacking();
            return;
        }
#if UNITY_EDITOR
        Debug.Log("ATTACKING - Since last auto: " + m_TimeSinceLastAuto);
        m_TimeSinceLastAuto = 0.0f;
#endif
        m_DesiredEnemy.GetComponent<ITakeDamage>().TakeDamage(m_CharacterStats.GetAttackDamage(), m_CharacterStats.GetAbilityPower(), m_CharacterStats.GetPlayerName());
    }
    //LLAMADA POR EVENTO EN LA ANIMACION DE AUTOATAQUE
    protected virtual void PerformRangedAutoAttack()
    {
        if (m_DesiredEnemy == null)
            return;
#if UNITY_EDITOR
        Debug.Log("ATTACKING - Since last auto: " + m_TimeSinceLastAuto);
        m_TimeSinceLastAuto = 0.0f;
#endif
        GameObject l_Projectile = Instantiate(m_RangedAutoAttack, m_RangedAutoSpawnPoint.position, transform.rotation);
        NetworkObject l_ProjectileNetwork = l_Projectile.GetComponent<NetworkObject>();
        l_ProjectileNetwork.SpawnWithOwnership(GetComponent<NetworkObject>().OwnerClientId);
        l_Projectile.GetComponent<RangedAutoAttack>().SetStats(m_DesiredEnemy, m_CharacterStats.GetAttackDamage(), 0.0f);
    }

    //GETTERS & SETTERS
    public GameManager GetGameManager()
    {
        return m_GameManager;
    }
    public OptionsUI GetOptionsUI()
    {
        return m_OptionsUI;
    }
    public CharacterStats GetCharacterStats()
    {
        return m_CharacterStats;
    }
    public CameraController GetCameraController()
    {
        return m_CharacterCamera;
    }
    public AudioSource GetAudioSource()
    {
        return m_AudioSource;
    }
    public Animator GetAnimator()
    {
        return m_CharacterAnimator;
    }
    public CharacterUI GetCharacterUI()
    {
        return m_CharacterUI;
    }
    public float GetAttackAnimationLength()
    {
        return m_AttackAnimLength;
    }
    public void SetAnimatorBool(string Name, bool True)
    {
        m_CharacterAnimator.SetBool(Name, True);
    }
    public void SetAnimatorTrigger(string Name)
    {
        m_CharacterAnimator.SetTrigger(Name);
    }
    public void ResetAnimatorTrigger(string Name)
    {
        m_CharacterAnimator.ResetTrigger(Name);
    }
    public bool GetAnimatorBool(string Name)
    {
        return m_CharacterAnimator.GetBool(Name);
    }
    public bool GetIsLookingForPosition()
    {
        return m_LookingForNextPosition;
    }
    public bool GetIsAttacking()
    {
        return m_Attacking;
    }
    public void SetIsAttacking(bool Attacking)
    {
        m_Attacking = Attacking;
    }
    public void SetDisabled(bool Disabled)
    {
        m_Disabled = Disabled;
    }
    public void SetZeroCooldown(bool Active)
    {
        m_QSkill.SetZeroCooldown(Active);
        m_WSkill.SetZeroCooldown(Active);
        m_ESkill.SetZeroCooldown(Active);
        m_RSkill.SetZeroCooldown(Active);
        m_SummSpell1.SetZeroCooldown(Active);
        m_SummSpell2.SetZeroCooldown(Active);
    }
    public bool GetShowingGizmos()
    {
        return m_ShowingGizmos;
    }
    public void SetShowingGizmos(bool True)
    {
        m_ShowingGizmos = True;
    }
    public bool GetUseSkillGizmos()
    {
        return m_OptionsUI.m_GameMenu.IsSkillGizmosEnabled();
    }
    public bool GetUseKeyboardMovement()
    {
        return m_UseKeyboardMovement;
    }
    public void SetUseKeyboardMovement(bool True)
    {
        m_UseKeyboardMovement = True;
    }
    public int GetQSkillLevel()
    {
        return m_QSkillLevel;
    }
    [Rpc(SendTo.Everyone)]
    public void SetQSkillLevelRpc()
    {
        m_QSkillLevel++;
        if (m_QSkill.GetHaveLoads())
        {
            m_QSkill.SetLoadCooldown(m_QSkillLevel);

            if (m_QSkillLevel - 1 == 0)
                PowersCooldownLoads(m_QSkill);

            m_IngameCharacterUI.SetLoadsInfo(m_QSkill);

        }
        else
            m_QSkill.SetCooldown(m_QSkillLevel);

    }
    public int GetWSkillLevel()
    {
        return m_WSkillLevel;
    }
    [Rpc(SendTo.Everyone)]
    public void SetWSkillLevelRpc()
    {
        m_WSkillLevel++;
        if (m_WSkill.GetHaveLoads())
        {
            m_WSkill.SetLoadCooldown(m_WSkillLevel);

            if (m_WSkillLevel - 1 == 0)
                PowersCooldownLoads(m_WSkill);

            m_IngameCharacterUI.SetLoadsInfo(m_WSkill);
        }
        else
            m_WSkill.SetCooldown(m_WSkillLevel);
    }
    public int GetESkillLevel()
    {
        return m_ESkillLevel;
    }
    [Rpc(SendTo.Everyone)]
    public void SetESkillLevelRpc()
    {
        m_ESkillLevel++;
        if (m_ESkill.GetHaveLoads())
        {
            m_ESkill.SetLoadCooldown(m_ESkillLevel);

            if (m_ESkillLevel - 1 == 0)
                PowersCooldownLoads(m_ESkill);

            m_IngameCharacterUI.SetLoadsInfo(m_ESkill);
        }
        else
            m_ESkill.SetCooldown(m_ESkillLevel);

    }
    public int GetRSkillLevel()
    {
        return m_RSkillLevel;
    }
    [Rpc(SendTo.Everyone)]
    public void SetRSkillLevelRpc()
    {
        m_RSkillLevel++;
        if (m_RSkill.GetHaveLoads())
        {
            m_RSkill.SetLoadCooldown(m_RSkillLevel);

            if (m_RSkillLevel - 1 == 0)
                PowersCooldownLoads(m_RSkill);

            m_IngameCharacterUI.SetLoadsInfo(m_RSkill);
        }
        else
            m_RSkill.SetCooldown(m_RSkillLevel);

    }
}
