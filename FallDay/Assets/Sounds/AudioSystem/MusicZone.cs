using UnityEngine;

/// <summary>
/// Troca a música quando o jogador entra em uma sala/área (collider com Is Trigger).
/// Ao sair, volta para a música que tocava antes.
/// Versão 2D. Para jogo 3D troque por OnTriggerEnter/Exit(Collider) e RequireComponent(Collider).
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class MusicZone : MonoBehaviour
{
    [SerializeField] private MusicPlaylist playlist;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool restoreOnExit = true;

    private MusicPlaylist previous;

    private void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (AudioManager.CurrentPlaylist != playlist)
            previous = AudioManager.CurrentPlaylist;

        AudioManager.PlayMusic(playlist);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag) || !restoreOnExit) return;

        // Só volta se a música da sala ainda for a atual (evita brigar com a zona vizinha)
        if (previous != null && AudioManager.CurrentPlaylist == playlist)
            AudioManager.PlayMusic(previous);
    }
}
