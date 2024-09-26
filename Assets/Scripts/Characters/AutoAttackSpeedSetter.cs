using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoAttackSpeedSetter : StateMachineBehaviour
{
	CharacterMaster m_Character;
	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator,AnimatorStateInfo stateInfo,int layerIndex)
	{
		m_Character=animator.transform.GetComponent<CharacterMaster>();
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator,AnimatorStateInfo stateInfo,int layerIndex)
	{
		float l_AttacksPerSecond=1.0f/m_Character.GetAttackAnimationLength();
		float l_AnimSpeedIncrease=m_Character.m_AttackSpeed/l_AttacksPerSecond;
		if(animator.GetFloat("AAttackSpeed")!=l_AnimSpeedIncrease)
			animator.SetFloat("AAttackSpeed", l_AnimSpeedIncrease);
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	//override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	//{
	//    
	//}

	// OnStateMove is called right after Animator.OnAnimatorMove()
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	//{
	//    // Implement code that processes and affects root motion
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK()
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	//{
	//    // Implement code that sets up animation IK (inverse kinematics)
	//}
}
