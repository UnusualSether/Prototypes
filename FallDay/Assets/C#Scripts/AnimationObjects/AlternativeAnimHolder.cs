using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "OverrideAnimHolder", menuName = "Animation/Override Anim Holder")]
public class AlternativeAnimHolder : ScriptableObject
{
    [Tooltip("All enemy variations _Warning: All the elements need to be in the same order as other lists.")]
    public List<CharacterAnimationProfile> characterAnimationGroups;

    // Dicionário de cache para transformar a busca linear (foreach) em busca instantânea (O(1))
    private List<AnimationSwap> animationSwaps;

    /// <summary>
    /// Retorna o perfil de animação correto para o tipo de inimigo selecionado.
    /// </summary>
    public List<AnimationSwap> GetProfile(string enemyType)
    {
        // Inicializa o dicionário de busca na primeira vez que o método for chamado
        if (animationSwaps == null)
        {
            animationSwaps = new List<AnimationSwap>();
            foreach (var profile in characterAnimationGroups)
            {
                if (profile != null)
                {
                    //!profileLookup.ContainsKey(profile.EnemyTipe) <= may be nscessaryFor modification
                    if (profile.EnemyTipe == enemyType)
                    {
                        foreach (AnimationSwap Swap in profile.ClipList)
                        {
                            animationSwaps.Add(Swap);
                        }
                    }
                }
            }
        }

        // Tenta resgatar o perfil de forma ultra performática
        return animationSwaps;
    }
}
