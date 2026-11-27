using UnityEngine;

/// <summary>
/// Passos por distância percorrida (funciona sem animação).
/// Se o seu jogo tem pulo/queda, prefira Animation Events com SfxEmitter.Play("footstep").
/// </summary>
public class FootstepPlayer : MonoBehaviour
{
    [SerializeField] private string sfxId = "footstep";

    [Tooltip("Distância entre um passo e outro (em unidades do mundo).")]
    [SerializeField] private float stepDistance = 0.6f;

    private Vector2 lastPosition;
    private float accumulated;

    private void OnEnable()
    {
        lastPosition = transform.position;
        accumulated = 0f;
    }

    private void Update()
    {
        Vector2 pos = transform.position;
        accumulated += Vector2.Distance(pos, lastPosition);
        lastPosition = pos;

        if (accumulated >= stepDistance)
        {
            accumulated = 0f;
            AudioManager.Play(sfxId);
        }
    }
}
