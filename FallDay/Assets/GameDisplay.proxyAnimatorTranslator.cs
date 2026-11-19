using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
//using static ZombieAnimatorProxy;


// Este cessão de codigo do GameDysplay Traduz os Comandos do Controle de animação do GameDysplay para as proxies
// que rodam as animaçoes a parte


// Os GameObjects ficam fora de cena ou com posição absurda (ex: y = -9999)
public partial class GameDisplay
{
    [Header("Zombie Animator Proxies")]
    public ZombieAnimatorProxy[] zombieProxies;
    public bool debug = false;
    // ^ Um proxy por ZombieSpot — arraste no Inspector

    // Mapeia cada ZombieDisplay ao seu proxy correspondente
    private ZombieAnimatorProxy GetProxyForDisplay(ZombieDisplay display)
    {
        if (zombieProxies == null || zombieProxies.Length == 0)
        {
            if (debug) Debug.LogError("zombieProxies não foi preenchido no Inspector!");
            return null;
        }
        if (display.displayId >= zombieProxies.Length)
        {
            if (debug) Debug.LogError($"displayId {display.displayId} não tem proxy correspondente. Total de proxies: {zombieProxies.Length}");
            return null;
        }
        int index = Mathf.Clamp(display.displayId, 0, zombieProxies.Length - 1);
        return zombieProxies[index];
    }

    public void RegisterAnimationEvents()
    {
        //handler.ZombieDamaged += OnZombieDamaged;
    }

    public void UnregisterAnimationEvents()
    {
        //handler.ZombieDamaged -= OnZombieDamaged;
    }
}