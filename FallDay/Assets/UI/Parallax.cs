using UnityEngine;
using UnityEngine.UI;

public class Parallax : MonoBehaviour
{
    [SerializeField] private RawImage Background;

    [SerializeField] private float Xsize, Ysize;

    void Update()
    {
        Background.uvRect = new Rect(Background.uvRect.position + new Vector2(Xsize, Ysize) * Time.deltaTime, Background.uvRect.size);
    }
}
