using System.Collections.Generic;
using UnityEngine;
public partial class ZombieAnimatorProxy
{
    /*
    public enum EnemyTipe
    {
        basic = 0,
        fast = 1,
        heavy = 2
    }
    */
    [Header("Scriptable Object References")]
    public ProxyesInfoHolder proxyesInfoHolder;
    private CharacterAnimationProfile activeProfileObject;

    private AnimatorOverrideController overrideController;
    private List<KeyValuePair<AnimationClip, AnimationClip>> clipOverrides;

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

    public void setUpNewEnemy(string selectEnemy)
    {
        overrideController.GetOverrides(clipOverrides);
        CharacterAnimationProfile profileToApply = FindClipList(selectEnemy);
        ApplyProfile(profileToApply);
    }

    public CharacterAnimationProfile FindClipList(string enemyTipe)
    {
        // Uses the ultra-fast lookup we added to ProxyesInfoHolder
        if (proxyesInfoHolder != null)
        {
            return proxyesInfoHolder.GetProfile(enemyTipe);
        }
        return null;
    }

    private void ApplyProfile(CharacterAnimationProfile newProfileObject)
    {
        activeProfileObject = newProfileObject;
        if (newProfileObject == null || newProfileObject.ClipList == null) return;

        // 1. Map [BaseClip -> OverrideClip] from your ScriptableObject
        Dictionary<AnimationClip, AnimationClip> lookup = new Dictionary<AnimationClip, AnimationClip>(newProfileObject.ClipList.Count);

        for (int i = 0; i < newProfileObject.ClipList.Count; i++)
        {
            AnimationSwap swap = newProfileObject.ClipList[i];
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