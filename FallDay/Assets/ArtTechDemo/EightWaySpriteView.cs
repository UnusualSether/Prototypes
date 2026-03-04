using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;


/// <summary>
/// This class manages the character  views, assigning them eight way sprites which change according to how the camera is looking at them.
/// </summary>
public class EightWaySpriteView : MonoBehaviour
{

    /// <summary>
    /// The character's 8 sprites will be placed in this array in the following order:
    ///0 = front, 1 = front-right, 2 = right, 3 = right-back, 4 = back, 5 = back-left, 6 = left, 7 = left-front.
    /// </summary>
    public Sprite[] directionalSprites = new Sprite[8];

    public SpriteRenderer spriteRenderer;
    public Transform cameraTransform;

    public void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        cameraTransform = Camera.main.transform;
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
        if (directionalSprites == null || directionalSprites.Length == 0)
        {
            Debug.Log($"{this.gameObject.name} has no sprites assigned to it!");
            return;
        }

        int frameCount = directionalSprites.Length;


        Vector3 toCamera = transform.position - cameraTransform.position;
        toCamera.y = 0f;
        toCamera.Normalize();

        Vector3 spriteFront = Vector3.forward;
        spriteFront.y = 0f;
        spriteFront.Normalize();

        float angle = Vector3.SignedAngle(toCamera, spriteFront, Vector3.up);

        if (angle < 0f)
        {
            angle += 360;
        }

        float sizeOfSectors = 360 / frameCount;
        int frameIndex = Mathf.RoundToInt(angle / sizeOfSectors) % frameCount;

        if (frameIndex > 4)
        {
            spriteRenderer.flipX = true;
        }

        else
        {
            spriteRenderer.flipX = false;
        }
            spriteRenderer.sprite = directionalSprites[frameIndex];

    }
}
