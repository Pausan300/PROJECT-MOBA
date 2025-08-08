


public interface ITakeDamage
{
	void TakeDamage(float PhysDamage, float MagicDamage, bool IgnoreResistances, string SourceId);
	CharacterStats GetCharacterStats();
}
public interface ITakeDamageTower 
{
	void TakeDamage(float PhysDamage, float MagicDamage, bool IgnoreResistances, string SourceId);
	TowerStats GetTowerStats();
	bool GetIsDestroyed();
}
