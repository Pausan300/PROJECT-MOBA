using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEditor.ProjectWindowCallback;

public class ScriptsPersonalizados
{

    private static string GetSelectedPathOrFallback()
    {
        string path = "Assets";

        foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
                path = Path.GetDirectoryName(path);
            break;
        }
        return path;
    }
    #region CharacterController
    [MenuItem("Assets/Create/Scripts Personalizados/Character Controller", false, 80)]
    public static void CrearNewCharacterController()
    {
        string path = GetSelectedPathOrFallback();
        string defaultName = "NewCharacterController.cs";

        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
            0,
            ScriptableObject.CreateInstance<NewCharacterControllerTemplate>(),
            Path.Combine(path, defaultName),
            null,
            ""
        );
    }

    #endregion

    #region Buffs

    [MenuItem("Assets/Create/Scripts Personalizados/New Buff", false, 80)]
    public static void CrearScriptNewBuff()
    {
        string path = GetSelectedPathOrFallback();
        string defaultName = "NewBuff.cs";

        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
            0,
            ScriptableObject.CreateInstance<BuffTemplateWithTimed>(),
            Path.Combine(path, defaultName),
            null,
            ""
        );
    }

    #endregion

}
public class BuffTemplateWithTimed : EndNameEditAction
{
    public override void Action(int instanceId, string pathName, string resourceFile)
    {
        string scriptName = Path.GetFileNameWithoutExtension(pathName);
        string directory = Path.GetDirectoryName(pathName);

        // Crear Buff base
        string buffContent = $@"
using UnityEngine;

[CreateAssetMenu(menuName = ""Buffs/{scriptName}"")]
public class {scriptName} : Buff
{{
    public TimedBuff InitializeBuff(float Duration, GameObject obj)
    {{
        m_Duration = Duration;
        return new Timed{scriptName}(Duration, this, obj);
    }}
}}";
        File.WriteAllText(pathName, buffContent);

        // Crear Buff timed
        string timedName = "Timed" + scriptName;
        string timedPath = Path.Combine(directory, timedName + ".cs");

        string timedContent = $@"
using UnityEngine;

public class {timedName} : TimedBuff
{{
    private readonly CharacterStats m_StatsComponent;

    public {timedName}(float Duration, Buff buff, GameObject obj) : base(buff)
    {{
        buff.m_Duration = Duration;
        if (obj.TryGetComponent(out ITakeDamage Entity))
            m_StatsComponent = Entity.GetCharacterStats();
    }}

    //Start {timedName}
    protected override void ApplyEffect()
    {{
        if (m_StatsComponent != null)
        {{
            Debug.LogError(""Falta programar inicio {timedName}"");
        }}
    }}

    //End {timedName}
    public override void End()
    {{
        if (m_StatsComponent != null)
        {{
            Debug.LogError(""Falta programar final {timedName}"");
        }}
    }}

    //Update {timedName}
    protected override void ApplyTick(float delta)
    {{
        if (m_StatsComponent != null)
        {{
        }}
    }}
}}";
        File.WriteAllText(timedPath, timedContent);

        AssetDatabase.Refresh();
        Object asset = AssetDatabase.LoadAssetAtPath<Object>(pathName);
        ProjectWindowUtil.ShowCreatedAsset(asset);
    }
}

public class NewCharacterControllerTemplate : EndNameEditAction
{
    public override void Action(int instanceId, string pathName, string resourceFile)
    {
        string scriptName = Path.GetFileNameWithoutExtension(pathName);

        string contenido =
$@"
using System.Collections;
using UnityEngine;
public class { scriptName } : CharacterMaster
{{
    [Header(""--- {scriptName} ---"")]

    [Header(""PASSIVE SKILL"")]

    [Header(""Q SKILL"")]

    public float m_QChannelingTime = 1f; //Se puede borrar si no hace Canalizando la skill
    bool m_QChanneling = true; //Se puede borrar si no hace Canalizando la skill
    float m_QTimer = 0f; //Se puede borrar si no hace Canalizando la skill
    bool m_SaverCanDoQ = true;
    bool m_QSkillStarted = false;

    [Header(""W SKILL"")]

    public float m_WChannelingTime = 1f; //Se puede borrar si no hace Canalizando la skill
    bool m_WChanneling = true; //Se puede borrar si no hace Canalizando la skill
    float m_WTimer = 0f; //Se puede borrar si no hace Canalizando la skill
    bool m_SaverCanDoW = true;
    bool m_WSkillStarted = false;

    [Header(""E SKILL"")]

    public float m_EChannelingTime = 1f; //Se puede borrar si no hace Canalizando la skill
    bool m_EChanneling = true; //Se puede borrar si no hace Canalizando la skill
    float m_ETimer = 0f; //Se puede borrar si no hace Canalizando la skill
    bool m_SaverCanDoE = true;
    bool m_ESkillStarted = false;

    [Header(""R SKILL"")]

    public float m_RChannelingTime = 1f; //Se puede borrar si no hace Canalizando la skill
    bool m_RChanneling = true; //Se puede borrar si no hace Canalizando la skill
    float m_RTimer = 0f; //Se puede borrar si no hace Canalizando la skill
    bool m_SaverCanDoR = true;
    bool m_RSkillStarted = false;

    public override void OnNetworkSpawn()
    {{
        base.OnNetworkSpawn();
    }}
    protected override void Update()
    {{
        if (!IsSpawned || !HasAuthority)
        {{
            return;
        }}

        base.Update();

        GeneralUpdate();

    }}

    void GeneralUpdate(){{

        if (GetUseSkillGizmos())
        {{
            if (Input.GetMouseButtonDown(0))
            {{
                if (m_ESkill.GetUsingSkill() && !m_ESkillStarted)
                    StartCoroutine(StartESkill());

                if (m_QSkill.GetUsingSkill() && !m_QSkillStarted)
                    StartCoroutine(StartQSkill());

                if (m_WSkill.GetUsingSkill() && !m_WSkillStarted)
                    StartCoroutine(StartWSkill());

                if (m_RSkill.GetUsingSkill() && !m_RSkillStarted)
                    StartCoroutine(StartRSkill());
            }}
            if (Input.GetMouseButtonDown(1))
            {{
                if (m_ESkill.GetUsingSkill() && !m_ESkillStarted)
                {{
                    m_ESkill.SetUsingSkill(false);
                    if (GetShowingGizmos())
                    {{
                        m_SkillIndicatorUI.ClearDeletableSkillIndicatorUI();
                        m_SkillIndicatorUI.ClearNormalSkillIndicatorUI();
                        m_SkillIndicatorUI.ClearTargetSkillIndicatorUI();
                        SetShowingGizmos(false);
                    }}
                }}

                if (m_RSkill.GetUsingSkill() && !m_RSkillStarted)
                {{
                    m_RSkill.SetUsingSkill(false);
                    if (GetShowingGizmos())
                    {{
                        m_SkillIndicatorUI.ClearDeletableSkillIndicatorUI();
                        m_SkillIndicatorUI.ClearNormalSkillIndicatorUI();
                        m_SkillIndicatorUI.ClearTargetSkillIndicatorUI();
                        SetShowingGizmos(false);
                    }}
                }}

                if (m_WSkill.GetUsingSkill() && !m_WSkillStarted)
                {{
                    m_WSkill.SetUsingSkill(false);
                    if (GetShowingGizmos())
                    {{
                        m_SkillIndicatorUI.ClearDeletableSkillIndicatorUI();
                        m_SkillIndicatorUI.ClearNormalSkillIndicatorUI();
                        m_SkillIndicatorUI.ClearTargetSkillIndicatorUI();
                        SetShowingGizmos(false);
                    }}
                }}

                if (m_QSkill.GetUsingSkill() && !m_QSkillStarted)
                {{
                    m_QSkill.SetUsingSkill(false);
                    if (GetShowingGizmos())
                    {{
                        m_SkillIndicatorUI.ClearDeletableSkillIndicatorUI();
                        m_SkillIndicatorUI.ClearNormalSkillIndicatorUI();
                        m_SkillIndicatorUI.ClearTargetSkillIndicatorUI();
                        SetShowingGizmos(false);
                    }}
                }}
            }}
        }}

        UpdateQSkill();
        UpdateWSkill();
        UpdateESkill();
        UpdateRSkill();
    }}

    #region Q Skill
    //Q SKILL
    protected override void QSkill()
    {{
        if (m_QSkill.GetUsingSkill() || m_WSkill.GetUsingSkill() || m_ESkill.GetUsingSkill() || m_RSkill.GetUsingSkill())
        {{
            if (!GetUseSkillGizmos())
                return;

            if (m_WSkill.GetUsingSkill() && !m_WSkillStarted)
            {{
                m_WSkill.SetUsingSkill(false);
            }}
            else if (m_RSkill.GetUsingSkill() && !m_RSkillStarted)
            {{
                m_RSkill.SetUsingSkill(false);
            }}
            else if (m_ESkill.GetUsingSkill() && !m_ESkillStarted)
            {{
                m_ESkill.SetUsingSkill(false);
            }}
            else
            {{
                return;
            }}
        }}

        if (!m_SaverCanDoQ)
            return;

        m_QSkill.SetUsingSkill(true);

        if (GetUseSkillGizmos())
        {{

            m_QSkillStarted = false;
            if (GetShowingGizmos())
            {{
                m_SkillIndicatorUI.ClearDeletableSkillIndicatorUI();
                m_SkillIndicatorUI.ClearNormalSkillIndicatorUI();
                m_SkillIndicatorUI.ClearTargetSkillIndicatorUI();
            }}
            //Aqui va el codigo para mostrar la UI --> EJ: m_SkillIndicatorUI.CreateArrowSkillIndicator(m_QSkill.m_IndicatorUIObject, m_UIIndicatorWidthQ, m_RangeQ, transform.position, true);
            SetShowingGizmos(true);
        }}
        else
            StartCoroutine(StartQSkill());

    }}

    IEnumerator StartQSkill()
    {{
        
        m_QSkillStarted = true;
        StopAttacking();
        if (!GetIsLookingForPosition())
            StopMovement();

        GetCharacterUI().SetCastingUIAbilityText(""Canalizando"");
        GetCharacterUI().HideCastingTime();
        GetCharacterUI().UpdateCastingUI(0, 1);
        GetCharacterUI().ShowCastingUI();
        m_QTimer = 0f;
        m_QChanneling = true;


        SetAnimatorTrigger(""IsUsingQ"");

        SetDisabled(true);
        yield return new WaitForSeconds(m_QChannelingTime);
        SetDisabled(false);
        base.QSkill();
        GetCharacterUI().HideCastingUI();
        m_QChanneling = false;
        

        EndQSkill();
    }}

    void UpdateQSkill()
    {{
        if (!m_QSkill.GetUsingSkill() && !m_QSkillStarted)
            return;

        if (m_QChanneling)
        {{
            m_QTimer += Time.deltaTime;
            GetCharacterUI().UpdateCastingUI(m_QTimer, m_QChannelingTime);
        }}

    }}

    void EndQSkill()
    {{
        m_QSkillStarted = false;
        m_QSkill.SetUsingSkill(false);
        StartCoroutine(RepeatQSaver());
    }}
    IEnumerator RepeatQSaver()
    {{
        m_SaverCanDoQ = false;
        yield return null; // Esperar 1 frame por seguridad
        m_SaverCanDoQ = true;

    }}

    #endregion
    #region W Skill
    //W SKILL
    protected override void WSkill()
    {{
        if (m_QSkill.GetUsingSkill() || m_WSkill.GetUsingSkill() || m_ESkill.GetUsingSkill() || m_RSkill.GetUsingSkill())
        {{
            if (!GetUseSkillGizmos())
                return;

            if (m_QSkill.GetUsingSkill() && !m_QSkillStarted)
            {{
                m_QSkill.SetUsingSkill(false);
            }}
            else if (m_RSkill.GetUsingSkill() && !m_RSkillStarted)
            {{
                m_RSkill.SetUsingSkill(false);
            }}
            else if (m_ESkill.GetUsingSkill() && !m_ESkillStarted)
            {{
                m_ESkill.SetUsingSkill(false);
            }}
            else
            {{
                return;
            }}
        }}

        if (!m_SaverCanDoW)
            return;

        m_WSkill.SetUsingSkill(true);

        if (GetUseSkillGizmos())
        {{

            m_WSkillStarted = false;
            if (GetShowingGizmos())
            {{
                m_SkillIndicatorUI.ClearDeletableSkillIndicatorUI();
                m_SkillIndicatorUI.ClearNormalSkillIndicatorUI();
                m_SkillIndicatorUI.ClearTargetSkillIndicatorUI();
            }}
            //Aqui va el codigo para mostrar la UI --> EJ: m_SkillIndicatorUI.CreateArrowSkillIndicator(m_WSkill.m_IndicatorUIObject, m_UIIndicatorWidthW, m_RangeW, transform.position, true);
            SetShowingGizmos(true);
        }}
        else
            StartCoroutine(StartWSkill());


    }}

    IEnumerator StartWSkill()
    {{
        m_WSkillStarted = true;
        StopAttacking();
        if (!GetIsLookingForPosition())
            StopMovement();

        GetCharacterUI().SetCastingUIAbilityText(""Canalizando"");
        GetCharacterUI().HideCastingTime();
        GetCharacterUI().UpdateCastingUI(0, 1);
        GetCharacterUI().ShowCastingUI();
        m_WTimer = 0f;
        m_WChanneling = true;


        SetAnimatorTrigger(""IsUsingW"");

        SetDisabled(true);
        yield return new WaitForSeconds(m_WChannelingTime);
        SetDisabled(false);
        base.WSkill();
        GetCharacterUI().HideCastingUI();
        m_WChanneling = false;


        EndWSkill();
    }}

    void UpdateWSkill()
    {{
        if (!m_WSkill.GetUsingSkill() && !m_WSkillStarted)
            return;

        if (m_WChanneling)
        {{
            m_WTimer += Time.deltaTime;
            GetCharacterUI().UpdateCastingUI(m_WTimer, m_WChannelingTime);
        }}

    }}

    void EndWSkill()
    {{

        m_WSkillStarted = false;
        m_WSkill.SetUsingSkill(false);
        StartCoroutine(RepeatWSaver());
    }}
    IEnumerator RepeatWSaver()
    {{
        m_SaverCanDoW = false;
        yield return null; // Esperar 1 frame por seguridad
        m_SaverCanDoW = true;

    }}

    #endregion
    #region E Skill

    //E SKILL
    protected override void ESkill()
    {{
        if (m_QSkill.GetUsingSkill() || m_WSkill.GetUsingSkill() || m_ESkill.GetUsingSkill() || m_RSkill.GetUsingSkill())
        {{
            if (!GetUseSkillGizmos())
                return;

            if (m_WSkill.GetUsingSkill() && !m_WSkillStarted)
            {{
                m_WSkill.SetUsingSkill(false);
            }}
            else if (m_RSkill.GetUsingSkill() && !m_RSkillStarted)
            {{
                m_RSkill.SetUsingSkill(false);
            }}
            else if (m_QSkill.GetUsingSkill() && !m_QSkillStarted)
            {{
                m_QSkill.SetUsingSkill(false);
            }}
            else
            {{
                return;
            }}
        }}

        if (!m_SaverCanDoE)
            return;

        m_ESkill.SetUsingSkill(true);

        if (GetUseSkillGizmos())
        {{

            m_ESkillStarted = false;
            if (GetShowingGizmos())
            {{
                m_SkillIndicatorUI.ClearDeletableSkillIndicatorUI();
                m_SkillIndicatorUI.ClearNormalSkillIndicatorUI();
                m_SkillIndicatorUI.ClearTargetSkillIndicatorUI();
            }}
            //Aqui va el codigo para mostrar la UI --> EJ: m_SkillIndicatorUI.CreateArrowSkillIndicator(m_ESkill.m_IndicatorUIObject, m_UIIndicatorWidthE, m_RangeE, transform.position, true);
            SetShowingGizmos(true);
        }}
        else
            StartCoroutine(StartESkill());

    }}
    IEnumerator StartESkill()
    {{
        m_ESkillStarted = true;
        StopAttacking();
        if (!GetIsLookingForPosition())
            StopMovement();

        GetCharacterUI().SetCastingUIAbilityText(""Canalizando"");
        GetCharacterUI().HideCastingTime();
        GetCharacterUI().UpdateCastingUI(0, 1);
        GetCharacterUI().ShowCastingUI();
        m_ETimer = 0f;
        m_EChanneling = true;


        SetAnimatorTrigger(""IsUsingE"");

        SetDisabled(true);
        yield return new WaitForSeconds(m_EChannelingTime);
        SetDisabled(false);
        base.ESkill();
        GetCharacterUI().HideCastingUI();
        m_EChanneling = false;


        EndESkill();
    }}

    void UpdateESkill()
    {{
        if (!m_ESkill.GetUsingSkill() && !m_ESkillStarted)
            return;

        if (m_EChanneling)
        {{
            m_ETimer += Time.deltaTime;
            GetCharacterUI().UpdateCastingUI(m_ETimer, m_EChannelingTime);
        }}

    }}
    void EndESkill()
    {{
        m_ESkillStarted = false;
        m_ESkill.SetUsingSkill(false);
        StartCoroutine(RepeatESaver());
    }}
    IEnumerator RepeatESaver()
    {{
        m_SaverCanDoE = false;
        yield return null; // Esperar 1 frame por seguridad
        m_SaverCanDoE = true;

    }}

    #endregion
    #region R Skill
    //R SKILL
    protected override void RSkill()
    {{
        if (m_QSkill.GetUsingSkill() || m_WSkill.GetUsingSkill() || m_ESkill.GetUsingSkill() || m_RSkill.GetUsingSkill())
        {{
            if (!GetUseSkillGizmos())
                return;

            if (m_WSkill.GetUsingSkill() && !m_WSkillStarted)
            {{
                m_WSkill.SetUsingSkill(false);
            }}
            else if (m_QSkill.GetUsingSkill() && !m_QSkillStarted)
            {{
                m_QSkill.SetUsingSkill(false);
            }}
            else if (m_ESkill.GetUsingSkill() && !m_ESkillStarted)
            {{
                m_ESkill.SetUsingSkill(false);
            }}
            else
            {{
                return;
            }}
        }}

        if (!m_SaverCanDoR)
            return;

        m_RSkill.SetUsingSkill(true);

        if (GetUseSkillGizmos())
        {{

            m_RSkillStarted = false;
            if (GetShowingGizmos())
            {{
                m_SkillIndicatorUI.ClearDeletableSkillIndicatorUI();
                m_SkillIndicatorUI.ClearNormalSkillIndicatorUI();
                m_SkillIndicatorUI.ClearTargetSkillIndicatorUI();
            }}
            //Aqui va el codigo para mostrar la UI --> EJ: m_SkillIndicatorUI.CreateArrowSkillIndicator(m_RSkill.m_IndicatorUIObject, m_UIIndicatorWidthR, m_RangeR, transform.position, true);
            SetShowingGizmos(true);
        }}
        else
            StartCoroutine(StartRSkill());



    }}

    IEnumerator StartRSkill()
    {{
        m_RSkillStarted = true;
        StopAttacking();
        if (!GetIsLookingForPosition())
            StopMovement();

        GetCharacterUI().SetCastingUIAbilityText(""Canalizando"");
        GetCharacterUI().HideCastingTime();
        GetCharacterUI().UpdateCastingUI(0, 1);
        GetCharacterUI().ShowCastingUI();
        m_RTimer = 0f;
        m_RChanneling = true;


        SetAnimatorTrigger(""IsUsingR"");

        SetDisabled(true);
        yield return new WaitForSeconds(m_RChannelingTime);
        SetDisabled(false);
        base.RSkill();
        GetCharacterUI().HideCastingUI();
        m_RChanneling = false;


        EndRSkill();
    }}

    void UpdateRSkill()
    {{
        if (!m_RSkill.GetUsingSkill() && !m_RSkillStarted)
            return;

        if (m_RChanneling)
        {{
            m_RTimer += Time.deltaTime;
            GetCharacterUI().UpdateCastingUI(m_RTimer, m_RChannelingTime);
        }}

    }}
    void EndRSkill()
    {{
        m_RSkillStarted = false;
        m_RSkill.SetUsingSkill(false);
        StartCoroutine(RepeatRSaver());
    }}

    IEnumerator RepeatRSaver()
    {{
        m_SaverCanDoR = false;
        yield return null; // Esperar 1 frame por seguridad
        m_SaverCanDoR = true;

    }}
    #endregion

    public override void LevelUpRpc()
    {{
        base.LevelUpRpc();
    }}

    IEnumerator DisableForDuration(float Duration)
    {{
        SetDisabled(true);
        yield return new WaitForSeconds(Duration);
        SetDisabled(false);
    }}
}}



";

        File.WriteAllText(pathName, contenido);
        AssetDatabase.Refresh();

        Object asset = AssetDatabase.LoadAssetAtPath<Object>(pathName);
        ProjectWindowUtil.ShowCreatedAsset(asset);
    }
}
