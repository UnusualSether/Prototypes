using System.Collections.Generic;
using UnityEngine;

public class CharacterReskinner : MonoBehaviour
{
    public string EnemyType; // The type of enemy to determine which animation profile to use
    public Animator currentAnimator;
    [Header("Scriptable Object References")]
    public AlternativeAnimHolder ZombiesAnimationHolder;
    //private CharacterAnimationProfile activeProfileObject;

    private AnimatorOverrideController overrideController;
    private List<KeyValuePair<AnimationClip, AnimationClip>> clipOverrides;

    bool debugisOn = true; // Set this to true to enable debug logs
    void Awake()
    {
        if (debugisOn) Debug.Log("Awake");
        // Get reference from partial MonoBehaviour class
        if (currentAnimator == null)
        {
            if (debugisOn) Debug.Log("GettingCurrentAnimator");
            currentAnimator = GetComponent<Animator>();
        }
        if (debugisOn) Debug.Log("Generate OverrideController(currentAnimator.runtimeAnimatorController)");
        overrideController = new AnimatorOverrideController(currentAnimator.runtimeAnimatorController);
        currentAnimator.runtimeAnimatorController = overrideController;

        clipOverrides = new List<KeyValuePair<AnimationClip, AnimationClip>>(overrideController.overridesCount);
        overrideController.GetOverrides(clipOverrides);
    }
    public void Start()
    {
        if (debugisOn) Debug.Log("Start");
        setUpNewEnemy(EnemyType);
    }

    public void setUpNewEnemy(string selectEnemy)
    {
        overrideController.GetOverrides(clipOverrides);
        ApplyProfile(FindClipList(selectEnemy));
    }

    public List<AnimationSwap> FindClipList(string enemyTipe)
    {
        // Uses the ultra-fast lookup we added to ProxyesInfoHolder
        if (ZombiesAnimationHolder != null)
        {
            return ZombiesAnimationHolder.GetProfile(enemyTipe);
        }
        return null;
    }

    private void ApplyProfile(List<AnimationSwap> AnimSwapList)
    {
        if (AnimSwapList == null) return;

        // 1. Map [BaseClip -> OverrideClip] from your ScriptableObject
        Dictionary<AnimationClip, AnimationClip> lookup = new Dictionary<AnimationClip, AnimationClip>();

        for (int i = 0; i < AnimSwapList.Count; i++)
        {
            AnimationSwap swap = AnimSwapList[i];
            if (swap.BaseClip != null && swap.OverrideClip != null)
            {
                lookup[swap.BaseClip] = swap.OverrideClip;
            }
        }

        // 2. Safely swap Unity's internal override pairs by matching BaseClip keys
        for (int i = 0; i < clipOverrides.Count; i++)
        {
            KeyValuePair<AnimationClip, AnimationClip> currentPair = clipOverrides[i];

            if (lookup.TryGetValue(currentPair.Key, out AnimationClip replacementClip))
            {
                clipOverrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(currentPair.Key, replacementClip);
            }
        }

        // 3. Apply all swaps to the native C++ engine in one frame
        overrideController.ApplyOverrides(clipOverrides);
    }
}
