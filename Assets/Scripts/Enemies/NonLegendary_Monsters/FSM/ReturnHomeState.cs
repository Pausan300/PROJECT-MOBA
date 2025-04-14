using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "FSM/States/ReturnHomeState")]
public class ReturnHomeState : State
{
    public override void EnterState(Basic_Jungle_Monster_Behaviour_Manager manager)
    {
        // Lógica para entrar en el estado ReturnHome
    }

    public override void UpdateState(Basic_Jungle_Monster_Behaviour_Manager manager)
    {
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
        else
        {
            manager.TransitionToState(manager.idleState);
        }
    }
}