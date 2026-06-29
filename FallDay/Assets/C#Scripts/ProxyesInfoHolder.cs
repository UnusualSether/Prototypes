using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProxyesInfoHolder", menuName = "Animation/Proxyes Info Holder")]
public class ProxyesInfoHolder : ScriptableObject
{
    [Header("WARNING:")]
    [Header("Each scriptableObject CharacterAnimationProfile")]
    [Header("needs the folowing ID in int format,")]
    [Header("to specify the tipe of Enemy these animations belong to.")]
    [Header("check info in a proxy component")]
    [Header("                   ")]

    [Tooltip("all Basic Enemys AnimationClip ")]
    public CharacterAnimationProfile BaseAnimationGroup;
    [Tooltip("All Other enemy variations _Warning: All the substitute neet to be in the order of the base List.")]
    public List<CharacterAnimationProfile> characterAnimationGroups;
}
