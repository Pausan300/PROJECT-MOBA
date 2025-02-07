using UnityEditor;
using UnityEngine;

public abstract class Base_Spawner : ScriptableObject
{
    public GameObject[] prefab;
    public Vector3 [] position;
    public float spawnInterval;

    public abstract void Spawn();
}
