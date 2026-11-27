using UnityEngine;

/// <summary>
/// Ponte para tocar sons sem escrever código: Animation Events (passos, ataques),
/// UnityEvents (botões da cena, portas) e eventos de colisão.
/// Em Animation Event: escolha a função Play e escreva o id no campo String (ex.: footstep).
/// </summary>
public class SfxEmitter : MonoBehaviour
{
    [SerializeField] private string defaultId;

    public void Play(string id) => AudioManager.Play(id);                              // 2D
    public void PlayHere(string id) => AudioManager.PlayAt(id, transform.position);    // 3D (explosões, inimigos)
    public void PlayDefault() => AudioManager.Play(defaultId);
}
