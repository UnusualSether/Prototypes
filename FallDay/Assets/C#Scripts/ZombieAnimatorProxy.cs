using UnityEngine;

public class ZombieAnimatorProxy : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    public bool debugisOn = false;

    // Hashes dos parâmetros do Animator — mais performático que strings
    private static readonly int PhaseParam = Animator.StringToHash("Phase");
    private static readonly int DamagedParam = Animator.StringToHash("Damaged");

    public Sprite CurrentSprite => spriteRenderer.sprite;

    public void SetPhase(Zombie.ZombiePhase phase)
    {
        animator.SetInteger(PhaseParam, (int)phase);
        if (debugisOn) Debug.Log($"Setting animation phase to {(int)phase} T%HISIS ADDAS");
    }

    public void TriggerDamaged()
    {
        animator.SetTrigger(DamagedParam);
    }

    public void ResetProxy()
    {
        animator.Rebind();
        animator.Update(0f);
    }
}