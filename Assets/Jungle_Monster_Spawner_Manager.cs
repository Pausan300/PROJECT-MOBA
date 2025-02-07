using UnityEngine;

public class Jungle_Monster_Spawner_Manager : MonoBehaviour
{
    public GameObject[] jungleMonsterSpawnerPrefabs;
    
    public Vector3[] desiredSpawnerPositions;
    public float[] campsTimers;
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        getChidsPosition();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SpawnJungleMonsterSpawners()
    {
        for (int i = 0; i < jungleMonsterSpawnerPrefabs.Length; i++)
        {
            Instantiate(jungleMonsterSpawnerPrefabs[i], desiredSpawnerPositions[i], Quaternion.identity);
            //SetSpawnerTimer(jungleMonsterSpawnerPrefabs[i]);
        }
    }

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
        SpawnJungleMonsterSpawners();
       
    }

    private void SetSpawnerTimer(GameObject jungleSpawner)
    {
        switch (jungleSpawner)
        {
            case GameObject spawner when spawner.name == "S":
                campsTimers[0] = 10f;
                break;

        }

    }*/
 }
