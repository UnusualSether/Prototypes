using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Collections;
using System;


public partial class GameDisplay : MonoBehaviour
{

    public VisualElement ui;
    public VisualElement[] bulletDisplay;
    public UIDocument uiDoc;

    public GameHandler handler;



    public List<ZombieDisplay> zombieDisplayList = new List<ZombieDisplay>();

    public List<ZombieDisplay> occupiedZombieDisplay = new List<ZombieDisplay>();

    public int zombieCountWatcher;

    public int bulletIndexer = 0;

    public List<string> cachedBullets = new List<string>();

    public List<Zombie> cachedZombies = new List<Zombie>();

    public List<Zombie> displayedZombies = new List<Zombie>();


    private bool zombieDisplayUpdateGate;

    public Dictionary<int, ZombieDisplay> zombieDisplayLookup;

    [Serializable]
    public class ZombieDisplay
    {
        public int displayId;

        public Zombie displayedZombie;

        public VisualElement displayElement;

        public Coroutine activeAnimation;
    }

    private void Awake()
    {
        ui = uiDoc.rootVisualElement;

        List<VisualElement> numberOfDisplay = new List<VisualElement>();
    }
    private void OnEnable()
    {
        //Events

        handler.ZombieKilled += RemoveCrosshair;

        handler.BulletSelected += ShakeBullet;

        handler.BulletSelected += PlayerClickSound;

        handler.ZombieDamaged += ShakeZombieVisual;

        //Find the Bullet Displays using a for loop
        var bulletDisplaysFound = ui.Query<VisualElement>().Where(e => e.name.StartsWith("BSpot")).ToList();

        Debug.Log($"{bulletDisplaysFound.Count} bullet displays found");

        bulletDisplay = bulletDisplaysFound.ToArray();


        //Cache Current Bullets
        cachedBullets = handler.selectableBullets.ToList();


        //Find the Zombie Displays 

        var zombieDisplaysFound = ui.Query<VisualElement>().Where(e => e.name.StartsWith("ZombieSpot")).ToList();

        Debug.Log(zombieDisplaysFound.Count + "Zombie Displays");

        foreach (var display in zombieDisplaysFound)
        {

            int nextDisplayNumber = zombieDisplayList.Count;

            Debug.Log(zombieDisplayList.Count);

            display.RegisterCallback<PointerEnterEvent>(SelectZombie);

            zombieDisplayList.Add(new ZombieDisplay

            {

                displayId = zombieDisplayList.Count

                , displayElement = display

            }



          );
        }

        Debug.Log($"Populated zombie class list with {zombieDisplayList.Count}");

        zombieDisplayLookup = new Dictionary<int, ZombieDisplay>();

        foreach (var display in zombieDisplayList)
        {
            zombieDisplayLookup.Add(display.displayId, display);
        }


        cachedZombies = handler.ZombieList.ToList();

    }

    private void OnDisable()
    {
        handler.ZombieKilled -= RemoveCrosshair;
        handler.BulletSelected -= ShakeBullet;
        handler.BulletSelected -= PlayerClickSound;
        handler.ZombieDamaged -= ShakeZombieVisual;
    }

    IEnumerator WaitForBullets()
    {
        if (handler.selectableBullets == null)
        {
            yield return new WaitUntil(() => handler.selectableBullets != null);

            Debug.Log("Bullets are available! Caching...");

        }
    }

    private void Update()
    {
        //Handle Bullet Changes
        if (SelectableBulletsHaveChanged() == true)
        {
            //Debug.Log("Handling Bullet Display");
            HandleBulletDisplay();
        }

        if (NumberOfZombiesHasChanged() == true)
        {

            HandleZombieDisplay();

        }

    }


    #region Bullet Spot Handling
    private bool SelectableBulletsHaveChanged()
    {

        var currentBulletsToComp = handler.selectableBullets.ToList();

        bool bulletsAreEqual = cachedBullets.SequenceEqual(currentBulletsToComp);

        if (!bulletsAreEqual)
        {
            //Debug.Log("Bullets Have Changed");
            cachedBullets = handler.selectableBullets.ToList();
            return true;
        }

        return false;


    }

    [ContextMenu("DisplayDebug")]
    private void DisplayBoxDebug()
    {
        Debug.Log($"This is the available number of displaySlots {zombieDisplayList.Count}");
        Debug.Log($"This is the occupied number of displaySlots {occupiedZombieDisplay.Count}");
    }


    private void HandleBulletDisplay()
    {
        //Declare variables for storing the new type of bullet and fetch a reference to 
        //the list of possible bullets
        string storedBullet;

        int bulletToChangeIndex = 0;

        List<string> listOfBullets = handler.bulletList.ToList();

        foreach (var bullet in bulletDisplay)
        {


            //Remove the previous class from the display element.

            //Get the display elements current classes and turn it into a list.
            var classList = bullet.GetClasses().ToList();
            //find the class the visual element currently has using intersect and store it.
            var classToRemove = String.Join(",", listOfBullets.Intersect(classList));

            //Debug.Log(classToRemove);
            //Finally, remove the stored class to remove from the actual visual element.
            bullet.RemoveFromClassList(classToRemove);

            //Debug.Log($"{string.Join(",",classToRemove)} was removed.");




            //Find the bullet the actual game handler has in that spot.
            storedBullet = handler.selectableBullets[bulletToChangeIndex];
            //Add it's corresponding class to the visual element.
            bullet.AddToClassList(storedBullet);



            bulletToChangeIndex++;

            if (bulletToChangeIndex >= bulletDisplay.Length)
            {
                bulletToChangeIndex = 0;
            }
        }
    }

    #endregion

    #region Zombie Spot Handling

    //Find the available zombie spots

    private bool NumberOfZombiesHasChanged()
    {
        /*
        var zombiesToCompare = handler.ZombieList.ToList();

        if (cachedZombies != zombiesToCompare)
        {

            return true;

        }

        return false;
        */
        return cachedZombies.Count != handler.ZombieList.Count;
    }

    private void SelectZombie(PointerEnterEvent ev)
    {
        var selectedElement = (VisualElement)ev.currentTarget;

        if (!selectedElement.ClassListContains("zombieSpotOccupied"))
        {
            Debug.Log("This spot in not occupied. Backing out...");
            return;
        }


        //var elementIdChar = selectedElement.name[name.Length - 1];

        //Debug.Log($"Fetched the element with the number {elementIdChar}");

        //var elementId = (int)Char.GetNumericValue(elementIdChar);

        //ZombieDisplay zombieDisplay = zombieDisplayLookup[elementId - 1];

        //handler.preferenceZombie = zombieDisplay.displayedZombie.id;

        int preferenceZombieID = occupiedZombieDisplay.Find(e => e.displayElement == selectedElement).displayedZombie.id;

        handler.preferenceZombie = preferenceZombieID;

        ClickedCrosshair(selectedElement);
    }



    private void ClickedCrosshair(VisualElement clickedElement)
    {
        Debug.Log("Applying crosshair...");

        foreach (var display in occupiedZombieDisplay)
        {
            display.displayElement.RemoveFromClassList("aimed");

        }

        clickedElement.AddToClassList("aimed");
    }

    private void RemoveCrosshair()
    {
        foreach (var display in occupiedZombieDisplay)
        {
            if (display.displayElement.ClassListContains("aimed"))
            {
                display.displayElement.RemoveFromClassList("aimed");
            }
        }
    }

    private void FindNewCrossHairPosition()
    {


        foreach (var display in occupiedZombieDisplay)
        {
            display.displayElement.RemoveFromClassList("aimed");
        }

        foreach (var display in zombieDisplayList)
        {
            display.displayElement.RemoveFromClassList("aimed");
        }

        var newSelectedDisplay = occupiedZombieDisplay.Find(e => e.displayedZombie.id == handler.preferenceZombie).displayElement;

        newSelectedDisplay.AddToClassList("aimed");


    }



    private void HandleZombieDisplay()
    {
        //Handle new zombie coming in




        if (cachedZombies.Count < handler.ZombieList.Count)
        {




            Zombie newZombie =
                handler.ZombieList.Except(cachedZombies).First();

            handler.ZombieIsClose += UpdateZombieVisual;

            if (newZombie == null)
            {
                Debug.Log("No new zombie found! Backing out.");
                return;
            }

            ZombieDisplay assignedDisplay =
                zombieDisplayList.First();

            if (assignedDisplay == null)
            {
                Debug.Log("Display not found! Backing out.");
                return;
            }


            assignedDisplay.displayedZombie = newZombie;

            Debug.Log($"Zombie Display {assignedDisplay.displayId} now contains zombie with ID {assignedDisplay.displayedZombie.id}");

            VisualElement zombieDisplayElement = ui.Query<VisualElement>().Where(e => e.name == $"ZombieSpot{assignedDisplay.displayId + 1}");

            if (zombieDisplayElement == null)
            {
                Debug.Log("Failed to find corresponding zombie display, backing out...");
                return;
            }

            zombieDisplayElement.AddToClassList("zombieSpotOccupied");


            zombieDisplayList.Remove(assignedDisplay);

            occupiedZombieDisplay.Add(assignedDisplay);
            cachedZombies = new List<Zombie>(handler.ZombieList);

            //if (!occupiedZombieDisplay.Any(x => x.displayElement.ClassListContains("aimed")))
            //{
            // FindNewCrossHairPosition();
            //}

        }

        //Handle Zombie Leaving

        if (cachedZombies.Count > handler.ZombieList.Count)
        {
            var leavingZombie =
                cachedZombies.Except(handler.ZombieList).First();

            handler.ZombieIsClose -= UpdateZombieVisual;

            if (leavingZombie == null)
            {
                Debug.Log("Failed to find leaving zombie, backing out...");
                return;
            }

            ZombieDisplay assignedDisplay =
                 occupiedZombieDisplay.First(e => e.displayedZombie == leavingZombie);

            if (assignedDisplay == null)
            {
                Debug.Log("Failed to find the assigned display. Backing out.");
                return;
            }

           ;

            Debug.Log($"Assigned display ID {assignedDisplay.displayId} which contained {assignedDisplay.displayedZombie.id} will now be nulled.");

            assignedDisplay.displayedZombie = null;

            VisualElement zombieDisplayElement = ui.Query<VisualElement>().Where(e => e.name == $"ZombieSpot{assignedDisplay.displayId + 1}");

            if (zombieDisplayElement == null)
            {
                Debug.Log("Failed to find corresponding zombie display, backing out...");
                return;
            }

            zombieDisplayElement.RemoveFromClassList("zombieSpotOccupied");
            zombieDisplayElement.RemoveFromClassList("warning");
            zombieDisplayList.Add(assignedDisplay);

            occupiedZombieDisplay.Remove(assignedDisplay);


            cachedZombies = new List<Zombie>(handler.ZombieList);

        }



    }

    void UpdateZombieVisual(Zombie zombie)
    {
        ZombieDisplay zombieDisplayToUpdate;

       foreach (var display in occupiedZombieDisplay)
        {
            if (display.displayedZombie == zombie)
            {
                zombieDisplayToUpdate = display;
                zombieDisplayToUpdate.displayElement.AddToClassList("warning");

            }
        }

        
    }

    void ShakeZombieVisual(Zombie zombie)
    {
        ZombieDisplay zombieDisplayToShake;

        foreach (var display in occupiedZombieDisplay)
        {
            if (display.displayedZombie == zombie)
            {
                zombieDisplayToShake = display;
                ShakeZombie(zombieDisplayToShake.displayElement);

            }
        }
    }

    #region Animation Handling
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
    #endregion

    #endregion
}
