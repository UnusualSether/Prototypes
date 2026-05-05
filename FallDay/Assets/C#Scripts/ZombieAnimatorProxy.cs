using UnityEngine;

public class ZombieAnimatorProxy : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Animator animator;

    // Hashes dos parâmetros do Animator — mais performático que strings
    private static readonly int PhaseParam = Animator.StringToHash("Phase");
    private static readonly int DamagedParam = Animator.StringToHash("Damaged");

    public Sprite CurrentSprite => spriteRenderer.sprite;

    public void SetPhase(Zombie.ZombiePhase phase)
    {
        Debug.Log($"Setting animation phase to {(int)phase}");
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