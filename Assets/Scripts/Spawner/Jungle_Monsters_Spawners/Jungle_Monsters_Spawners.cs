using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "NewJungleMonsterSpawner", menuName = "Spawner/JungleMonsterSpawner")]
public class Jungle_Monsters_Spawners : Base_Spawner
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public const string FileName = "NewJungleMonsterSpawner";

    protected override void OnEnable()
    {
        base.OnEnable();
        if (string.IsNullOrEmpty(spawnerName))
        {
            spawnerName = FileName;
        }
    }
    public override void SpawnMonster() //Instancia al monstruo
    {
        for (int i = 0; i < prefab.Length; i++)
        {
            Instantiate(prefab[i], position[i], Quaternion.identity);
        }
    }

    public override void SpawnSpawner() //Instancia al spawner
    {
        Instantiate(Spawner, position[0], Quaternion.identity);
    }



}

