using UnityEngine;

public abstract class State : ScriptableObject
{
    public abstract void EnterState(Basic_Jungle_Monster_Behaviour_Manager manager);
    public abstract void UpdateState(Basic_Jungle_Monster_Behaviour_Manager manager);
}   