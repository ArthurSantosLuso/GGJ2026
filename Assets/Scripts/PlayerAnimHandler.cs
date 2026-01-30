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
}
