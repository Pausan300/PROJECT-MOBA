using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "NewJungleMonsterSpawner", menuName = "Spawner/JungleMonsterSpawner")]
public class Jungle_Monsters_Spawners : Base_Spawner
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public override void Spawn()
    {
        for (int i = 0; i < prefab.Length; i++)
        {
            Instantiate(prefab[i], position[i], Quaternion.identity);
        }
    }


}

