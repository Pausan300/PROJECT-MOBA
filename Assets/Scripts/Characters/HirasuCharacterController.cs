using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HirasuCharacterController : CharacterMaster
{
    
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
		base.ESkill();

	}
	protected override void RSkill()
	{
		//if(GetCurrentHealth()>5000.0f)
			base.RSkill();
	}
}
