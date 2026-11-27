using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Uma lista de músicas. Crie quantas quiser: Menu, Gameplay, Chefe, Caverna, Sala 3...
/// Botão direito no Project > Create > Audio > Music Playlist.
/// </summary>
[CreateAssetMenu(menuName = "Audio/Music Playlist", fileName = "NewPlaylist")]
public class MusicPlaylist : ScriptableObject
{
    public List<AudioClip> tracks = new List<AudioClip>();

    [Range(0f, 1f)] public float volume = 1f;

    [Tooltip("Sorteia a próxima música (sem repetir a anterior). Desmarcado = ordem da lista.")]
    public bool shuffle = true;

    [Tooltip("Duração do crossfade entre músicas, em segundos.")]
    [Min(0f)] public float fadeTime = 1.5f;
}
