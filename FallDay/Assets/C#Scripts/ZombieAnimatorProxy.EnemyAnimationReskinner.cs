using System.Collections.Generic;
using UnityEngine;

public partial class ZombieAnimatorProxy
{
    //private Animator currentAnimator;       (Already In base Script) Enemy animator Component
    public enum EnemyTipe
    {
        basic = 0,
        fast = 1,
        heavy = 2
    }

    [Header("WARNING:")]
    [Header("Each scriptableObject CharacterAnimationProfile")]
    [Header("needs the folowing ID in int format,")]
    [Header("to specify the tipe of Enemy these animations belong to.")]
    [Header("basic = 0, fast = 1, heavy = 2.")]
    [Header("anithing else is deffault = 0")]
    [Header("                   ")]
    public ProxyesInfoHolder proxyesInfoHolder; // The ScriptableObject Storing BaseAnimationGroup (default animations) and CharacterAnimationProfile List<> (Override animations)
    public CharacterAnimationProfile activeProfileObject; // The ScriptableObject Storing the AnimationClips variations (Override animations)

    private AnimatorOverrideController overrideController; // The runtime "overlay sheet controller" sitting over the base controller
    private List<KeyValuePair<AnimationClip, AnimationClip>> clipOverrides; // Our pre-allocated memory table of [BaseClip, CurrentlyPlayingClip]


    private void Awake() // Runs the exact millisecond the character spawns into the scene
    {
        // Create a runtime-editable wrapper around the character's base Animator Controller (works like a clone of the controller, maintaining it's structure)
        overrideController = new AnimatorOverrideController(currentAnimator.runtimeAnimatorController);
        // Plugs that new wrapper back into the Animator so it actually drives the 3D model (or sprites)
        currentAnimator.runtimeAnimatorController = overrideController;

        // Reserve the exact amount of RAM needed for the controller's total clips (prevents mid-game Garbage Collection lag)
        clipOverrides = new List<KeyValuePair<AnimationClip, AnimationClip>>(overrideController.overridesCount);
        // Ask Unity to look at the controller and write its starting default state into our reserved memory list; elements are stored like [KeyValuePair<"SlowWalkClip", "SlowWalkClip">])
        overrideController.GetOverrides(clipOverrides);

        // If the designer dragged a profile into the Inspector box in the Unity Editor...      (For Test Remove after)
        if (activeProfileObject != null)
        {
            // ...immediately swap all the animations over to match that profile
            SelectProfile(0);
        }
    }

    // Call this public function from any other script to instantly transform the character's entire moveset
    public void SelectProfile(EnemyTipe selectEnemy)
    {
        if (selectEnemy == 0)
        {
            ApplyProfile(proxyesInfoHolder.BaseAnimationGroup);
        }
        else if (selectEnemy > 0)
        {
            ApplyProfile(FindClipList(selectEnemy));
        }
    }
    // shitty way to veryfy information ID and get correct ProfileObject from list
    public CharacterAnimationProfile FindClipList(EnemyTipe enemyTipe)
    {
        foreach (CharacterAnimationProfile AnimationListObject in proxyesInfoHolder.characterAnimationGroups) // sershes the list for correct clipList
        {
            if (AnimationListObject != null) // safty
            {
                if (AnimationListObject.EnemyTipe == (int)enemyTipe) // reeds int enemyTipe ID each character has a diffrent ID (finds first ID)
                {
                    return AnimationListObject;
                }
            }
        }
        return null;
    }

    // transforms Enemy Animations
    private void ApplyProfile(CharacterAnimationProfile newProfileObject)
    {
        if (newProfileObject != null)
        {

            // Create a temporary, hyper-fast lookup table in memory (Key = Base Clip, Value = New Clip)
            Dictionary<AnimationClip, AnimationClip> lookup = new Dictionary<AnimationClip, AnimationClip>();

            // Loop through the AnimationSwap List in the CharacterAnimationProfile profile item by item
            for (int i = 0; i < newProfileObject.ClipList.Count; i++)
            {
                if(activeProfileObject != null)
                {
                    AnimationSwap defaultClipHolder = proxyesInfoHolder.BaseAnimationGroup.ClipList[i];
                    AnimationSwap overrideClipHolder = newProfileObject.ClipList[i];
                }
                else if (activeProfileObject == null)
                {

                }
            }
            // Remember which profile we are currently wearing
            activeProfileObject = newProfileObject;
        }
    }
}