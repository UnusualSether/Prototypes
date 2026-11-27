using UnityEngine;

/// <summary>
/// Coloque em qualquer objeto da cena (menu, level 1, level 2...) e escolha a playlist.
/// Ao entrar na cena, a música troca com crossfade.
/// Também pode ser chamado por código: GetComponent&lt;LevelMusic&gt;().Play();
/// </summary>
public class LevelMusic : MonoBehaviour
{
    [SerializeField] private MusicPlaylist playlist;
    [SerializeField] private bool playOnStart = true;

    private void Start()
    {
        if (playOnStart) Play();
    }

    public void Play() => AudioManager.PlayMusic(playlist);
}
