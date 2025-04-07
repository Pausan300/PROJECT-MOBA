using NUnit.Framework;
using System.Threading;
using TMPro;
using UnityEngine;

public class Enemies_Spawner : MonoBehaviour
{
    /*
     * Necesita el prefab del objeto
     * El temporizador global del juego para compartirlo con todos los jugadores. 
     * 
     *Actualmente únicamente trabaja con el deltatime. 
     * 
     * Temporizador inicial.
     * Si el enemigo muere, se reinicia el temporizador.
     * Si el enemigo está  en juego no se aplica el countdowwn.
     * HUD
     * */
    //VARIABLES: 

    private float spawnCountDown;
    public GameObject[] enemyPrefab;
    public bool isEnemyDeath = true;
    private Vector3 initPos;
    public float initCountDown = 5f;
    private bool applyOnce = false;
    private TextMeshProUGUI timerUi;

    void Start()
    {
        GameObject canvas = GameObject.Find("Canvas");

        // Crea un nuevo objeto TextMeshPro
        GameObject textObject = new GameObject("Timer");
        textObject.transform.SetParent(canvas.transform);

        // Añade el componente TextMeshProUGUI al objeto
        timerUi = textObject.AddComponent<TextMeshProUGUI>();

        // Configura las propiedades del TextMeshPro
        timerUi.text = "5";
        timerUi.fontSize = 15;
        timerUi.alignment = TextAlignmentOptions.Center;
        timerUi.rectTransform.sizeDelta = new Vector2(transform.position.x, transform.position.y);
        timerUi.rectTransform.anchoredPosition = new Vector2(0,0);
        initPos = this.transform.position;
        SetCountDown(initCountDown);

    }

    // Update is called once per frame
    void Update()
    {
        if (CheckEnemyDeath())
        {
            if (CountDown() <= 0)
            {
                SpawnEnemy(initPos);
                SetCountDown(initCountDown);
            }
        }

    }


    public void SpawnEnemy(Vector3 initSpawnerPos)
    {
        for (int i = 0; i < enemyPrefab.Length; i++)
        {
            Instantiate(enemyPrefab[i], initSpawnerPos, Quaternion.identity);
            isEnemyDeath = false;
            //el hud se instancia con el enemigo
        }

    }

    private void SetCountDown(float setTimer)
    {
        spawnCountDown = setTimer;
        timerUi.text = spawnCountDown.ToString();
    }

    public float CountDown()
    {
        timerUi.text = spawnCountDown.ToString();
        return spawnCountDown -= Time.deltaTime;
    }

    private bool CheckEnemyDeath()
    {
        return isEnemyDeath;
    }
}

   




        //Funciones cuando exista el manager: 

        /*
         * Public void StartSpawning()
         * public void StopSpawning()
         * 
         * */

    
