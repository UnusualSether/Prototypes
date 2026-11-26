using UnityEngine;
[ExecuteInEditMode]

/// <summary>
/// This class manages the character  views, assigning them eight way sprites which change according to how the camera is looking at them.
/// </summary>
public class EightWayAnimationCTLR : MonoBehaviour
{
    /// <summary>
    /// The character's 8 sprites will be placed in this array in the following order:
    ///0 = front, 1 = front-right, 2 = right, 3 = right-back, 4 = back, 5 = back-left, 6 = left, 7 = left-front.
    /// </summary>
    //public Sprite[] directionalSprites = new Sprite[8]; // array of sprites for the 8 directions

    public Animator animator;
    public PathFolower pathFolower;
    //public SpriteRenderer spriteRenderer;
    private Transform cameraTransform;
    [Header("Transform used to determine the forward direction of the sprite")]
    public Transform directionSource;

    //////////////////////////////////
    [Header("Debugging")]
    [Tooltip("Set this to true to enable debug logs of this component")]
    public bool debug = false; 
    [Header("LockZombieAnimations: Set this to true to lock the zombie animations and prevent them from being changed, with the exception of Rotation, For Testing purposes")]
    public bool LockZombieAnimations = false; // Set this to true to lock the zombie animations and prevent them from being changed For Testing purposes
    private bool IsWalking = false;
    //////////////////////////////////

    public void Start()
    {
        //spriteRenderer = GetComponent<SpriteRenderer>();
        cameraTransform = Camera.main.transform;
        animator = (animator == null) ? gameObject.GetComponent<Animator>() : animator;
    }

    public void LateUpdate()
    {
        FaceCamera();
        UpdateSprite();
    }


    private void FaceCamera()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - cameraTransform.position);
    }

    private void UpdateSprite()
    {
        int frameCount = 8; //directionalSprites.Length;


        Vector3 toCamera = transform.position - cameraTransform.position;
        toCamera.y = 0f;
        toCamera.Normalize();

        Vector3 spriteFront = directionSource.transform.forward;
        spriteFront.y = 0f;
        spriteFront.Normalize();

        float angle = Vector3.SignedAngle(toCamera, spriteFront, directionSource.up);

        if (angle < 0f)
        {
            angle += 360;
        }

        float sizeOfSectors = 360 / frameCount;
        int frameIndex = Mathf.RoundToInt(angle / sizeOfSectors) % frameCount;

        if(debug) Debug.Log($"[EightWaySpriteView] {this.gameObject.name} angle: {angle}, frameIndex: {frameIndex}");

        if(animator != null) animator.SetInteger("Rotation", frameIndex);
        UpdateWalkCondition();
    }
    public void Attack()
    {
        if(!LockZombieAnimations && animator != null) animator.SetTrigger("Attack");
    }
    public void Damaged()
    {
        if(!LockZombieAnimations && animator != null) animator.SetTrigger("Damage");
    }
    private void UpdateWalkCondition()
    {
        if(pathFolower != null)
        {
            bool isWalking = pathFolower.IsWalking;
            if(!LockZombieAnimations && animator != null) animator.SetBool("Walk", isWalking);
        }
    }
    [ContextMenu("Debug Call Animation Attack")]
    private void DebugCallAnimationAttack()
    {
        if(LockZombieAnimations && animator != null) animator.SetTrigger("Attack");
    }
    [ContextMenu("Debug Call Animation Damaged")]
    private void DebugCallAnimationDamaged()
    {
        if(LockZombieAnimations && animator != null) animator.SetTrigger("Damage");
    }
    [ContextMenu("Debug Call Animation Walk")]
    private void DebugCallAnimationWalk()
    {
        IsWalking = !IsWalking;
        if(LockZombieAnimations && animator != null) animator.SetBool("Walk", IsWalking);
    }
}
