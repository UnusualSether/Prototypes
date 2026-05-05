using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public partial class GameDisplay
{
    [Header("Zombie Animator Proxies")]
    public ZombieAnimatorProxy[] zombieProxies;
    // ^ Um proxy por ZombieSpot — arraste no Inspector
    // Os GameObjects ficam fora de cena ou com posição absurda (ex: y = -9999)

    // Mapeia cada ZombieDisplay ao seu proxy correspondente
    private ZombieAnimatorProxy GetProxyForDisplay(ZombieDisplay display)
    {
        if (zombieProxies == null || zombieProxies.Length == 0)
        {
            //Debug.LogError("zombieProxies não foi preenchido no Inspector!");
            return null;
        }
        if (display.displayId >= zombieProxies.Length)
        {
            //Debug.LogError($"displayId {display.displayId} não tem proxy correspondente. Total de proxies: {zombieProxies.Length}");
            return null;
        }
        int index = Mathf.Clamp(display.displayId, 0, zombieProxies.Length - 1);
        return zombieProxies[index];
    }

    public void RegisterAnimationEvents()
    {
        handler.ZombieDamaged += OnZombieDamaged;
    }

    public void UnregisterAnimationEvents()
    {
        handler.ZombieDamaged -= OnZombieDamaged;
    }

    // Chamado quando o zumbi entra no display
    public void StartZombieAnimation(ZombieDisplay display)
    {

        ZombieAnimatorProxy proxy = GetProxyForDisplay(display);
        handler.zPhaseChange += OnZombiePhaseChanged;
        handler.zPhaseChange += ActionTest;
        proxy.SetPhase(display.displayedZombie.phase);
        display.activeAnimation = StartCoroutine(
            SyncSpriteToUI(display, proxy)
        );
    }

    // Chamado quando o zumbi sai do display
    public void StopZombieAnimation(ZombieDisplay display)
    {
        handler.zPhaseChange -= OnZombiePhaseChanged;
        StopDisplayAnimation(display);

        ZombieAnimatorProxy proxy = GetProxyForDisplay(display);
        if (proxy == null) return;
        proxy.ResetProxy();
        display.displayElement.style.backgroundImage = StyleKeyword.Null;
    }
    public void ActionTest(Zombie zombie) { Debug.LogWarning("ActionTest foi chamado!"); }

    // Responde à mudança de fase do zumbi
    private void OnZombiePhaseChanged(Zombie zombie)
    {
        Debug.Log($"Fase do zumbi mudou para {zombie.phase}");
        ZombieDisplay display = occupiedZombieDisplay
            .Find(d => d.displayedZombie == zombie);

        if (display == null) return;

        ZombieAnimatorProxy proxy = GetProxyForDisplay(display);
        proxy.SetPhase(zombie.phase);
    }

    // Responde ao evento de dano do GameHandler
    private void OnZombieDamaged(Zombie zombie)
    {
        ZombieDisplay display = occupiedZombieDisplay
            .Find(d => d.displayedZombie == zombie);

        if (display == null) return;

        ZombieAnimatorProxy proxy = GetProxyForDisplay(display);
        proxy.TriggerDamaged();
    }

    // Copia o sprite atual do Animator para o elemento do UI Toolkit a cada frame
    private IEnumerator SyncSpriteToUI(ZombieDisplay display, ZombieAnimatorProxy proxy)
    {
        while (true)
        {
            Sprite current = proxy.CurrentSprite;

            if (current != null)
            {
                display.displayElement.style.backgroundImage =
                    new StyleBackground(current);
            }

            yield return null; // espera o próximo frame
        }
    }

    private void StopDisplayAnimation(ZombieDisplay display)
    {
        if (display.activeAnimation != null)
        {
            StopCoroutine(display.activeAnimation);
            display.activeAnimation = null;
        }
    }
}