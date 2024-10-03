using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HirasuCharacterController : CharacterMaster
{
    [Header("HIRASU")]
	public float m_ERange;
	public float m_EDuration;

    protected override void Start()
    {
        base.Start();
    }
    protected override void Update()
    {
        base.Update();
    }

	protected override void QSkill()
	{
		base.QSkill();
	}
	protected override void WSkill()
	{
		base.WSkill();
	}
	protected override void ESkill()
	{
		Vector3 l_Direction=GetMouseDirection();
		l_Direction.Normalize();
		StartCoroutine(EDash(l_Direction));
	}
	protected override void RSkill()
	{
		base.RSkill();
	}
	IEnumerator EDash(Vector3 Direction)
	{
		SetDisabled(true);
		StopRecall();
		StopAttacking();
		if(!GetIsMoving())
			StopMovement();
		transform.forward=Direction;
		Vector3 l_StartPos=transform.position;
		float l_Range=m_ERange/100.0f;
		float l_Speed=l_Range/m_EDuration;
		var l_SqrMaxDistance=l_Range*l_Range;
		for(float timePassed=0.0f; timePassed<0.5f && (transform.position-l_StartPos).sqrMagnitude<l_SqrMaxDistance; timePassed+=Time.deltaTime)
		{
			transform.position+=l_Speed*Direction*Time.deltaTime;
			yield return null;
		}
		SetDisabled(false);
		base.ESkill();
	}
}
