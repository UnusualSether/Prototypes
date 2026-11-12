using UnityEngine;

[CreateAssetMenu(fileName = "CharacterAnimControl", menuName = "CharacterAnim")]
public class CharacterAnimControl : ScriptableObject
{
    private Transform characterTransform;
    private Animator animator;

    private float CharactRotation; // Variable to store the character's rotation value in a float format from 0-1 (if 1 then 360 degrees, if 0.5 then 180 degrees, etc. & 1 is set to 0)
    private bool isCharacterMoving; // Variable to store the character's movement state (true if moving, false if not)

}
