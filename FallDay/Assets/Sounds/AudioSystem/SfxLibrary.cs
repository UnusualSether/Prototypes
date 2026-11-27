using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SfxEntry
{
    [Tooltip("Nome usado no código: AudioManager.Play(\"footstep\")")]
    public string id;

    [Tooltip("Se tiver mais de um clip, um é sorteado a cada vez (evita som repetitivo).")]
    public AudioClip[] clips;

    [Range(0f, 1f)] public float volume = 1f;

    [Range(0.5f, 2f)] public float minPitch = 0.95f;
    [Range(0.5f, 2f)] public float maxPitch = 1.05f;

    [Tooltip("0 = som 2D (UI, jogador). 1 = 3D, diminui com a distância (explosões, inimigos).")]
    [Range(0f, 1f)] public float spatialBlend = 0f;
    public float minDistance = 2f;
    public float maxDistance = 30f;

    [Tooltip("Tempo mínimo entre duas execuções do mesmo som. Evita estourar com 20 explosões no mesmo frame.")]
    public float cooldown = 0.04f;
}

/// <summary>
/// Grupo de efeitos sonoros. Sugestão: uma biblioteca para UI, uma para o jogador, uma para inimigos/explosões.
/// Botão direito no Project > Create > Audio > Sfx Library.
/// </summary>
[CreateAssetMenu(menuName = "Audio/Sfx Library", fileName = "NewSfxLibrary")]
public class SfxLibrary : ScriptableObject
{
    public List<SfxEntry> entries = new List<SfxEntry>();
}
