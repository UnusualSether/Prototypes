using UnityEngine;

public class CameraRPGCustomizada : MonoBehaviour
{
    [Header("Alvos")]
    public Transform alvoAtual; // Pode ser o herói, o inimigo, ou um ponto central
    public Vector3 offset = new Vector3(0, 2, -5); // Distância da câmera para o alvo

    [Header("Configurações de Movimento")]
    public float tempoSuavizacao = 0.3f;
    private Vector3 velocidadeAtual; // Usado internamente pelo SmoothDamp

    [Header("Efeito Flutuante (Breathing)")]
    public float amplitudeFlutuacao = 0.5f; // O quão longe ela vai
    public float velocidadeFlutuacao = 1.0f; // A rapidez do vai e vem

    void LateUpdate()
    {
        if (alvoAtual == null) return;

        // 1. Calcula a posição desejada (Alvo + Offset)
        Vector3 posicaoDesejada = alvoAtual.position + offset;

        // 2. Adiciona o efeito flutuante (Seno baseado no tempo)
        float flutuacaoY = Mathf.Sin(Time.time * velocidadeFlutuacao) * amplitudeFlutuacao;
        float flutuacaoX = Mathf.Cos(Time.time * velocidadeFlutuacao * 0.8f) * (amplitudeFlutuacao / 2f);

        posicaoDesejada += new Vector3(flutuacaoX, flutuacaoY, 0);

        // 3. Move a câmera suavemente até a posição desejada
        transform.position = Vector3.SmoothDamp(
            transform.position,
            posicaoDesejada,
            ref velocidadeAtual,
            tempoSuavizacao
        );

        // 4. Faz a câmera olhar para o alvo (Centro da ação)
        // Usamos Lerp na rotação para não "bater" secamente
        Quaternion rotacaoDesejada = Quaternion.LookRotation(alvoAtual.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotacaoDesejada, Time.deltaTime * 5f);
    }

    // Função para chamar no seu BattleManager quando o turno mudar
    public void MudarFoco(Transform novoAlvo)
    {
        alvoAtual = novoAlvo;
    }
}