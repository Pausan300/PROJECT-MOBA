using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITakeDamage
{
	void TakeDamage(float PhysDamage, float MagicDamage);
	CharacterStats GetCharacterStats();
}
