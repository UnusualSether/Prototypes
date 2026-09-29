using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "ProxyesInfoHolder", menuName = "Animation/Proxyes Info Holder")]
public class ProxyesInfoHolder : ScriptableObject
{
    [Tooltip("All enemy variations _Warning: All the elements need to be in the same order as other lists.")]
    public List<CharacterAnimationProfile> characterAnimationGroups;

    // Dicionário de cache para transformar a busca linear (foreach) em busca instantânea (O(1))
    private Dictionary<string, CharacterAnimationProfile> profileLookup;

    /// <summary>
    /// Retorna o perfil de animação correto para o tipo de inimigo selecionado.
    /// </summary>
    public CharacterAnimationProfile GetProfile(string enemyType)
    {
        // Inicializa o dicionário de busca na primeira vez que o método for chamado
        if (profileLookup == null)
        {
            profileLookup = new Dictionary<string, CharacterAnimationProfile>(characterAnimationGroups.Count);
            foreach (var profile in characterAnimationGroups)
            {
                if (profile != null && !profileLookup.ContainsKey(profile.EnemyTipe))
                {
                    profileLookup.Add(profile.EnemyTipe, profile);
                }
            }
        }

        // Tenta resgatar o perfil de forma ultra performática
        profileLookup.TryGetValue(enemyType, out CharacterAnimationProfile result);
        return result;
    }
}
