

using System.Collections;
using UnityEngine;
public class ZappadasCharacterController : CharacterMaster
{
    [Header("--- ZappadasCharacterController ---")]

    [Header("PASSIVE SKILL")]

    [Header("Q SKILL")]

    bool m_SaverCanDoQ = true;

    [Header("W SKILL")]

    bool m_SaverCanDoW = true;

    [Header("E SKILL")]

    bool m_SaverCanDoE = true;

    [Header("R SKILL")]

    bool m_SaverCanDoR = true;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

    }
    protected override void Update()
    {
        if (!IsSpawned || !HasAuthority)
        {
            return;
        }

        base.Update();

    }

    #region Q Skill
    //Q SKILL
    protected override void QSkill()
    {
        if (m_QSkill.GetUsingSkill() || m_WSkill.GetUsingSkill() || m_ESkill.GetUsingSkill() || m_RSkill.GetUsingSkill())
            return;

        if (!m_SaverCanDoQ)
            return;

        EndQSkill();

    }
    void EndQSkill()
    {

        base.QSkill();
        StartCoroutine(RepeatQSaver());
    }
    IEnumerator RepeatQSaver()
    {
        m_SaverCanDoQ = false;
        yield return null; // Esperar 1 frame por seguridad
        m_SaverCanDoQ = true;

    }

    #endregion
    #region W Skill
    //W SKILL
    protected override void WSkill()
    {
        if (m_QSkill.GetUsingSkill() || m_WSkill.GetUsingSkill() || m_ESkill.GetUsingSkill() || m_RSkill.GetUsingSkill())
            return;

        if (!m_SaverCanDoW)
            return;

        EndWSkill();

    }
    void EndWSkill()
    {

        base.WSkill();
        StartCoroutine(RepeatWSaver());
    }
    IEnumerator RepeatWSaver()
    {
        m_SaverCanDoW = false;
        yield return null; // Esperar 1 frame por seguridad
        m_SaverCanDoW = true;

    }

    #endregion
    #region E Skill

    //E SKILL
    protected override void ESkill()
    {
        if (m_QSkill.GetUsingSkill() || m_WSkill.GetUsingSkill() || m_ESkill.GetUsingSkill() || m_RSkill.GetUsingSkill())
            return;

        if (!m_SaverCanDoE)
            return;

        EndESkill();


    }
    void EndESkill()
    {

        base.ESkill();
        StartCoroutine(RepeatESaver());
    }
    IEnumerator RepeatESaver()
    {
        m_SaverCanDoE = false;
        yield return null; // Esperar 1 frame por seguridad
        m_SaverCanDoE = true;

    }

    #endregion
    #region R Skill
    //R SKILL
    protected override void RSkill()
    {
        if (m_QSkill.GetUsingSkill() || m_WSkill.GetUsingSkill() || m_ESkill.GetUsingSkill() || m_RSkill.GetUsingSkill())
            return;

        if (!m_SaverCanDoR)
            return;

        EndRSkill();
    }
    void EndRSkill()
    {
        base.RSkill();
        StartCoroutine(RepeatRSaver());
    }

    IEnumerator RepeatRSaver()
    {
        m_SaverCanDoR = false;
        yield return null; // Esperar 1 frame por seguridad
        m_SaverCanDoR = true;

    }
    #endregion


    public override void LevelUpRpc()
    {
        base.LevelUpRpc();
    }

    IEnumerator DisableForDuration(float Duration)
    {
        SetDisabled(true);
        yield return new WaitForSeconds(Duration);
        SetDisabled(false);
    }
}

