using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RapatuCharacterController : CharacterMaster
{
    [Header("--- RAPATU ---")]
    [Header("PASSIVE SKILL")]

    [Header("Q SKILL")]
    public float m_QAdditionalDamage;

    [Header("W SKILL")]
    public float m_WAdditionalDamageSplinter;

    [Header("E SKILL")]
    public float m_ERange;

    [Header("R SKILL")]
    public float m_RAdditionalDamage;

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


    //Q SKILL
    protected override void QSkill()
    {

    }

    //W SKILL
    protected override void WSkill()
    {

    }



    //E SKILL
    protected override void ESkill()
    {

    }

    //R SKILL
    protected override void RSkill()
    {

    }

    public override void LevelUpRpc()
    {
        base.LevelUpRpc();
    }
    protected override void StartAttacking()
    {
        SetIsAttacking(true);


    }
    protected override void StopAttacking()
    {
        if (GetIsAttacking())
        {
            SetIsAttacking(false);
        }
    }
    IEnumerator DisableForDuration(float Duration)
    {
        SetDisabled(true);
        yield return new WaitForSeconds(Duration);
        SetDisabled(false);
    }
    protected override void PerformAutoAttack()
    {
        if (!IsSpawned || !HasAuthority)
        {
            return;
        }
    }
}
