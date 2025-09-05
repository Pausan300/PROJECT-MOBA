
using UnityEngine;

public interface ITakeDamage
{
	void TakeDamage(float PhysDamage, float MagicDamage, bool IgnoreResistances, string SourceId, GameObject SourceObject);
	CharacterStats GetCharacterStats();
	TowerController GetNearTower();
	void SetNearTower(TowerController Tower);
}
public interface ITakeDamageStructure 
{
	void TakeDamage(float PhysDamage, float MagicDamage, bool IgnoreResistances, string SourceId);
	StructureStats GetStructureStats();
	bool GetIsDestroyed();
}
