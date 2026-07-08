using UnityEngine;

public class BerserkerUpperBodyLayerResetStateAnimationBehaviour : StateMachineBehaviour
{
    //The purpose of this script is to interrupt all upper body layer animations when certain main layer animatinos are called.
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.Play(Animator.StringToHash("Idle"), 2, 0f);
    }
}
