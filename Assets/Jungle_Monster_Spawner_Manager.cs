using System.Collections.Generic;
using UnityEngine;

public class Jungle_Monster_Spawner_Manager : MonoBehaviour
{
    /*
     * Tiene hijos que tienen un tag específico.
     * Según el tag y el nombre del spawner instanciaremos un spawner en esa posición.
     * Además le asignaremos un temporizador especifico.
     * 
     * Para instanciar el spawner en la posición deseada, necesitamos la posición de los hijos del manager.
     * Serán guardadas en un diccionario clave valor, la clave será el tag y el valor la posición.
     * Al asignar la posición a cada spawner compararemos el tag con el nombre del spawner.
     * 
     * 
     * */

    
    public Base_Spawner[] jungleMonsterSpawnerPrefabs;
    
    public Dictionary<string, Vector3> desiredSpawnerPositions = new Dictionary<string, Vector3>();
    public Dictionary<string, float> spawnerTimers = new Dictionary<string, float>();

    private void Start()
    {
        GetChildsPositions();
        SetSpawnerTimer();
        AssignSpawnerProperties();
        SpawnSpawners();
    }

    private void SpawnSpawners()
    {
        foreach (Base_Spawner spawner in jungleMonsterSpawnerPrefabs)
        {
            spawner.SpawnSpawner();
        }
    }

    private void GetChildsPositions()
    {
        int childCount = gameObject.transform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            Transform childTransform = gameObject.transform.GetChild(i);
            desiredSpawnerPositions[childTransform.name] = childTransform.position;
            Debug.Log($"Child {i} position: {desiredSpawnerPositions[childTransform.name]}");
        }
    }

    private void SetSpawnerTimer()
    {
        // escalera if else de comparaciones para asignar tiempos y posiciones. 
        foreach (Base_Spawner spawner in jungleMonsterSpawnerPrefabs)
        {
            switch (spawner.spawnerName)
            {
                case "WingedCamp":
                    spawnerTimers[spawner.spawnerName] = 10f;
                   // desiredSpawnerPositions[spawner.spawnerName] = desiredSpawnerPositions["WingedCamp"];
                    //spawner.position[0] = desiredSpawnerPositions["WingedCamp"];
                    break;
                // Añade más casos según sea necesario
                case "WolfCamp":
                    spawnerTimers[spawner.spawnerName] = 15f;
                   // desiredSpawnerPositions[spawner.spawnerName] = desiredSpawnerPositions["WolfCamp"];
                    break;
                case "LizardCamp":
                    spawnerTimers[spawner.spawnerName] = 20f;
                   // desiredSpawnerPositions[spawner.spawnerName] = desiredSpawnerPositions["LizardCamp"];
                    break;
                case "FungusCamp":
                    spawnerTimers[spawner.spawnerName] = 25f;
                   // desiredSpawnerPositions[spawner.spawnerName] = desiredSpawnerPositions["FungusCamp"];
                    break;
                case "WildBoarsCamp":
                    spawnerTimers[spawner.spawnerName] = 30f;
                    //desiredSpawnerPositions[spawner.spawnerName] = desiredSpawnerPositions["WildBoarsCamp"];
                    break;
                default: // el Hornitorrinco
                    spawnerTimers[spawner.spawnerName] = 5f;
                    break;
            }
        }
    }
    private void AssignSpawnerProperties()
    {
        foreach (Base_Spawner spawner in jungleMonsterSpawnerPrefabs)
        {
            if (desiredSpawnerPositions.ContainsKey(spawner.spawnerName))
            {
                spawner.position = new Vector3[] { desiredSpawnerPositions[spawner.spawnerName] };
            }

            if (spawnerTimers.ContainsKey(spawner.spawnerName))
            {
                spawner.spawnInterval = spawnerTimers[spawner.spawnerName];
            }
        }
    }
}

    /* public Vector3[] desiredSpawnerPositions;
     public float[] campsTimers;

     // Start is called once before the first execution of Update after the MonoBehaviour is created
     void Start()
     {
         getChildsPositions();
     }

     // Update is called once per frame
     void Update()
     {

     }

     /*private void SpawnJungleMonsterSpawners()
     {
         for (int i = 0; i < jungleMonsterSpawnerPrefabs.Length; i++)
         {
             Instantiate(jungleMonsterSpawnerPrefabs[i], desiredSpawnerPositions[i], Quaternion.identity);
             //SetSpawnerTimer(jungleMonsterSpawnerPrefabs[i]);
         }
     }*/

    /* private void SpawnSpawners()
     {
         foreach (Base_Spawner spawner in jungleMonsterSpawnerPrefabs)
         {
             spawner.SpawnMonster();
         }
     }

     private void setSpawnerPos() {

        /* for (int i = 0; i < jungleMonsterSpawnerPrefabs.Length; i++)
         {

         }*/
//}
    
    /*private void getChildsPositions()
    {
        int childCount = gameObject.transform.childCount;
        desiredSpawnerPositions = new Vector3[childCount];

        for (int i = 0; i < childCount; i++)
        {
            Transform childTransform = gameObject.transform.GetChild(i);
            desiredSpawnerPositions[i] = childTransform.position;
        }
    }
    /*
    private void getChidsPosition()
    {

        int childCount = gameObject.transform.childCount;
        desiredSpawnerPositions = new Vector3[childCount];

        for (int i = 0; i < childCount; i++)
        {
            Transform childTransform = gameObject.transform.GetChild(i);
            desiredSpawnerPositions[i] = childTransform.position;
            Debug.Log($"Child {i} position: {desiredSpawnerPositions[i]}");
        }
        //SpawnJungleMonsterSpawners();
       
    }*/

   /* private void SetSpawnerTimer(GameObject jungleSpawner)
    {
        switch (jungleSpawner)
        {
            case GameObject spawner when spawner.name == "S":
                campsTimers[0] = 10f;
                break;

        }

    }*/
// }
