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

    public {timedName}(float Duration, Buff buff, GameObject obj) : base(buff, obj)
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
public class {scriptName} : CharacterMaster
{{
    [Header(""--- {scriptName} ---"")]

    [Header(""PASSIVE SKILL"")]

    [Header(""Q SKILL"")]

    bool m_SaverCanDoQ = true;

    [Header(""W SKILL"")]

    bool m_SaverCanDoW = true;

    [Header(""E SKILL"")]

    bool m_SaverCanDoE = true;

    [Header(""R SKILL"")]

    bool m_SaverCanDoR = true;

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

    }}

    #region Q Skill
    //Q SKILL
    protected override void QSkill()
    {{
        if (m_QSkill.GetUsingSkill() || m_WSkill.GetUsingSkill() || m_ESkill.GetUsingSkill() || m_RSkill.GetUsingSkill())
            return;

        if (!m_SaverCanDoQ)
            return;

        EndQSkill();

    }}
    void EndQSkill()
    {{

        base.QSkill();
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
            return;

        if (!m_SaverCanDoW)
            return;

        EndWSkill();

    }}
    void EndWSkill()
    {{

        base.WSkill();
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
            return;

        if (!m_SaverCanDoE)
            return;

        EndESkill();


    }}
    void EndESkill()
    {{

        base.ESkill();
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
            return;

        if (!m_SaverCanDoR)
            return;

        EndRSkill();
    }}
    void EndRSkill()
    {{
        base.RSkill();
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
