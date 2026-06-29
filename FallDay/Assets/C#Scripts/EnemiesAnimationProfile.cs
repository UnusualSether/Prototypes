using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAnimProfile", menuName = "Animation/Character Anim Profile")]
public class CharacterAnimationProfile : ScriptableObject
{
    [Header("WARNING:")]
    [Header("Each scriptableObject CharacterAnimationProfile")]
    [Header("needs the folowing ID in int format,")]
    [Header("to specify the tipe of Enemy these animations belong to.")]
    [Header("check info in a proxy component")]
    [Header("                   ")]
    
    [Tooltip("Or Check ZombieAnimatorProxy.EnemyAnimationReskinner partial enum data for int refrence")]
    public int EnemyTipe; // Check ZombieAnimatorProxy partial enum data for int refrence 
    [Tooltip("List for the animation clips to be used in the proxyes")]
    public List<AnimationSwap> ClipList;
}

[System.Serializable]
public struct AnimationSwap
{
    public AnimationClip Clip; // This specific character's version of it
}