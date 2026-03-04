using UnityEngine;


[ExecuteInEditMode]
public class TileToSize : MonoBehaviour
{
    // Adjust this value in the Inspector to control the physical size of one tile in Unity units
    public float textureToMeshUnits = 1f;

    void Update()
    {
        UpdateTiling();
    }

    void UpdateTiling()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null || renderer.sharedMaterial == null || renderer.sharedMaterial.mainTexture == null)
        {
            return;
        }

        Vector3 scale = transform.localScale;
        // For a standard plane (which is 10x10 units by default), 
        // you would use x and z and potentially adjust for the default size.
        // For a cube or quad, the Y-axis might be relevant.
        Vector2 tiling = new Vector2(scale.x / textureToMeshUnits, scale.y / textureToMeshUnits);

        // Apply the new tiling
        renderer.sharedMaterial.mainTextureScale = tiling;
    }
}
