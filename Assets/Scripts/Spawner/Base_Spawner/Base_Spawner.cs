using UnityEditor;
using UnityEngine;

public abstract class Base_Spawner : ScriptableObject
{
    public const string DefaultFileName = "NewSpawner";
    public string spawnerName;
    public GameObject Spawner;
    public GameObject[] prefab; //enemigos
    public Vector3 [] position;
    public float spawnInterval;
    public string campMonsterName;

    /*
     * Se encarga de instanciar el enemigo en la posici�n deseada. A su vez se encarga de reiniciar el temporizador.
     * Con el nombre realizamos las comparaciones  para saber que tipo de monstruo se va a instanciar y donde debemos posicionar el spawner
     * */

    public abstract void SpawnMonster();//insatanciar el monstruo en la posici�n deseada
    public abstract void SpawnSpawner();//instanciar el spawner en la posici�n deseada
    protected virtual void OnEnable()
    {
        if (string.IsNullOrEmpty(spawnerName))
        {
            spawnerName = DefaultFileName;
        }
    }

}
