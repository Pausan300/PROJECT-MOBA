using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "FSM/States/ChaseState")]
public class ChaseState : State
{
    public float chaseRange = 10f;

    public override void EnterState(Basic_Jungle_Monster_Behaviour_Manager manager)
    {
        // Lógica para entrar en el estado Chase
    }

    public override void UpdateState(Basic_Jungle_Monster_Behaviour_Manager manager)
    {
        float distanceToPlayer = Vector3.Distance(manager.transform.position, manager.player.position);

        if (distanceToPlayer > chaseRange)
        {
            manager.TransitionToState(manager.returnHomeState);
        }
        else
        {
            // Lógica para perseguir al jugador
            Debug.Log("ChaseState: Moving along path to player");
            List<AstarNode> path = manager.GetCurrentPath();
            if (path != null && path.Count > 0)
            {
                Vector3 nextPosition = path[0].position;
                manager.transform.position = Vector3.MoveTowards(manager.transform.position, nextPosition, manager.speed * Time.deltaTime);
                if (Vector3.Distance(manager.transform.position, nextPosition) < 0.1f)
                {
                    path.RemoveAt(0);
                }
            }
        }
    }
}