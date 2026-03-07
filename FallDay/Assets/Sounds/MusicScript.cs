using UnityEngine;

public class MusicScript : MonoBehaviour
{
    public static MusicScript bgmusic;

    private void Awake()
    {
        if (bgmusic != null)
        {
            Destroy(gameObject);
        }
        else
        {
            bgmusic = this;
        }

        DontDestroyOnLoad(this.gameObject);
    }

}
