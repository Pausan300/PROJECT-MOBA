using UnityEngine;
using System.Collections.Generic;

public class Basic_Jungle_Monster_Behaviour_Manager : MonoBehaviour
{
    public State currentState;
    public IdleState idleState;
    public ChaseState chaseState;
    public ReturnHomeState returnHomeState;

    public Transform player;
    public Vector3 homePosition;
    public float speed = 2f;
    public bool isDamaged = false; // Booleana para simular el daño recibido
    public LayerMask obstacleLayer; // Capa de obstáculos

    private AstarPathfinding pathfinding;
    private List<AstarNode> currentPath;
    private float pathUpdateInterval = 1f; // Intervalo de tiempo para actualizar el pathfinding
    private float pathUpdateTimer;

    void Start()
    {
        homePosition = transform.position;
        pathfinding = new AstarPathfinding { obstacleLayer = obstacleLayer };
        TransitionToState(idleState);
        pathUpdateTimer = pathUpdateInterval;
    }

    void Update()
    {
        currentState.UpdateState(this);

        // Simulación de recibir daño
        if (isDamaged)
        {
            TransitionToState(chaseState);
            isDamaged = false; // Resetear la booleana después de cambiar de estado
        }

        // Actualizar el pathfinding en intervalos de tiempo
        pathUpdateTimer -= Time.deltaTime;
        if (pathUpdateTimer <= 0)
        {
            if (currentState == chaseState)
            {
                currentPath = FindPath(transform.position, player.position);
            }
            else if (currentState == returnHomeState)
            {
                currentPath = FindPath(transform.position, homePosition);
            }
            pathUpdateTimer = pathUpdateInterval;
        }
    }

    public void TransitionToState(State newState)
    {
        currentState = newState;
        currentState.EnterState(this);
    }

    public List<AstarNode> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Debug.Log("Calling FindPath from Basic_Jungle_Monster_Behaviour_Manager");
        return pathfinding.FindPath(startPos, targetPos);
    }

    public List<AstarNode> GetCurrentPath()
    {
        return currentPath;
    }
}