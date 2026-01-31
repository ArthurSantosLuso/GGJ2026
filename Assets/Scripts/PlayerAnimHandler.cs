using UnityEngine;

public class PlayerAnimHandler : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void SetPlayerMoveValue(float value)
    {
        animator.SetFloat("moveValue", value);
    }

    public void AttackAnim()
    {
        animator.SetTrigger("attack");
    }

    public void SetRunAnim(bool value)
    {
        animator.SetBool("isRunning", value);
    }

    public void SetAimAnim(bool value)
    {
        if (value == true)
        {
            animator.SetLayerWeight(animator.GetLayerIndex("Arms Layer"), 1f);
            animator.SetLayerWeight(animator.GetLayerIndex("Top Layer"), 0.5f);
        }
        else
        {
            animator.SetLayerWeight(animator.GetLayerIndex("Arms Layer"), 0f);
            animator.SetLayerWeight(animator.GetLayerIndex("Top Layer"), 1f);
        }

        animator.SetBool("isAiming", value);
    }
}
