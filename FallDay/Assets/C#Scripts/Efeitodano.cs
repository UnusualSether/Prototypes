using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EfeitoDanoVisual : MonoBehaviour
{
    [Header("Configuração Visual")]
    public Image telaVermelha;

    [Tooltip("Velocidade mancha")]
    public float velocidadeDeSumico = 2f;

    [Tooltip("intensidade")]
    public float intensidadeDoDano = 0.6f;

    [Header("Configuração de Áudio")]
    public AudioSource fonteAudio;

    [Tooltip("som de dano")]
    public AudioClip somDeDano;

    private void OnEnable()
    {
        GameHandler.PlayerTookDamage += AtivarPiscarDano;
    }

    private void OnDisable()
    {
        GameHandler.PlayerTookDamage -= AtivarPiscarDano;
    }

    private void Start()
    {
        if (telaVermelha != null)
        {
            Color corInvisivel = telaVermelha.color;
            corInvisivel.a = 0f;
            telaVermelha.color = corInvisivel;
        }

        
        if (fonteAudio == null)
        {
            fonteAudio = GetComponent<AudioSource>();
        }
    }

    private void AtivarPiscarDano(float danoRecebido)
    {
        
        if (fonteAudio != null && somDeDano != null)
        {
            fonteAudio.PlayOneShot(somDeDano);
        }

        if (telaVermelha != null)
        {
            StopAllCoroutines();
            StartCoroutine(EfeitoFadeOut());
        }
    }

    private IEnumerator EfeitoFadeOut()
    {
        Color corAtual = telaVermelha.color;
        corAtual.a = intensidadeDoDano;
        telaVermelha.color = corAtual;

        while (telaVermelha.color.a > 0)
        {
            corAtual.a -= Time.deltaTime * velocidadeDeSumico;
            telaVermelha.color = corAtual;
            yield return null;
        }
    }
}