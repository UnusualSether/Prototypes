using System.Collections.Generic;
using UnityEngine;

public partial class ZombieAnimatorProxy : MonoBehaviour
{

    public SpriteRenderer spriteRenderer;
    public bool debugisOn = false;
    
    private Animator currentAnimator;

    private Dictionary<string, CharacterAnimationProfile> AnimationProfile;
    private bool AnimationIsActive = false; // animation Gate, controlles when there is an override (override is a difrent script)

    // Hashes dos parâmetros do Animator — mais performático que strings
    private static readonly int PhaseParam = Animator.StringToHash("Phase");    // Gets parameter Phase from Animator
    private static readonly int DamagedParam = Animator.StringToHash("Damaged");    // Gets parameter Damaged from Animator

    public Sprite CurrentSprite => spriteRenderer.sprite;

    public void SetPhase(Zombie.ZombiePhase phase)
    {
        currentAnimator.SetInteger(PhaseParam, (int)phase);
        if (debugisOn) Debug.Log($"Setting animation phase to {(int)phase} T%HISIS ADDAS");
    }

    public void TriggerDamaged()
    {
        currentAnimator.SetTrigger(DamagedParam);
    }

    public void SetAnimationLock()
    {
        AnimationIsActive = true;
    }
    public void ResetProxy()
    {
        currentAnimator.Rebind();
        currentAnimator.Update(0f);
        AnimationIsActive = false;
    }
}