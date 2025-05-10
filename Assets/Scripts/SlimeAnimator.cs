using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class SlimeAnimator : MonoBehaviour
{
    private Animator animator;

    // Hashes para eficiencia
    private readonly int isAttackingHash = Animator.StringToHash("IsAttacking");
    private readonly int isHurtHash = Animator.StringToHash("IsHurt");
    private readonly int isDeadHash = Animator.StringToHash("IsDead");

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayIdle()
    {
        animator.SetBool(isAttackingHash, false);
        animator.SetBool(isHurtHash, false);
        animator.SetBool(isDeadHash, false);
    }

    public void PlayAttack(float duration = 0.5f)
    {
        animator.SetBool(isAttackingHash, true);
        StartCoroutine(ResetBoolAfterDelay(isAttackingHash, duration));
    }

    public void PlayHurt(float duration = 0.4f)
    {
        animator.SetBool(isHurtHash, true);
        StartCoroutine(ResetBoolAfterDelay(isHurtHash, duration));
    }

    public void PlayDeath(float duration = 0.7f)
    {
        animator.SetBool(isDeadHash, true);
        StartCoroutine(DestroyAfterAnimation(duration));
    }

    private IEnumerator ResetBoolAfterDelay(int hash, float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.SetBool(hash, false);
    }

    private IEnumerator DestroyAfterAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}