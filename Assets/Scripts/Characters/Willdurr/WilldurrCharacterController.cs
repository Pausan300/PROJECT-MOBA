using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WilldurrCharacterController : CharacterMaster
{
    [Header("--- WILLDURR ---")]



    [Header("PASSIVE SKILL")]

    [Header("Q SKILL")]

    [Header("W SKILL")]

    [Header("E SKILL")]

    [Header("R SKILL")]

    [Header("OTHER VALUES")]
    public bool Varable_Que_Se_Puede_Borrar = true;

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


    }

    #endregion
    #region W Skill
    //W SKILL
    protected override void WSkill()
    {

    }

    #endregion
    #region E Skill

    //E SKILL
    protected override void ESkill()
    {


    }

    #endregion
    #region R Skill
    //R SKILL
    protected override void RSkill()
    {

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
