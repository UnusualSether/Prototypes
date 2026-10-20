using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAnimProfile", menuName = "Animation/Character Anim Profile")]
public class CharacterAnimationProfile : ScriptableObject
{
    public string EnemyTipe; // Check ZombieAnimatorProxy partial enum data for int refrence 
    [Tooltip("List for the animation clips to be used in the proxyes")]
    public List<AnimationSwap> ClipList;
}

[System.Serializable]
public struct AnimationSwap
{
    [Tooltip("The default clip sitting inside the Animator Controller")]
    public AnimationClip BaseClip;

    [Tooltip("This specific enemy variation's replacement clip")]
    public AnimationClip OverrideClip;
}