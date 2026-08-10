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

    public int bulletIndexer = 0;

    public List<string> cachedBullets = new List<string>();

    public List<Zombie> cachedZombies = new List<Zombie>();

    public List<Zombie> displayedZombies = new List<Zombie>();

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
        RegisterAnimationEvents();

        //Find the Bullet Displays using a for loop
        var bulletDisplaysFound = ui.Query<VisualElement>().Where(e => e.name.StartsWith("BSpot")).ToList();

        //Debug.Log($"{bulletDisplaysFound.Count} bullet displays found");

        bulletDisplay = bulletDisplaysFound.ToArray();


        //Cache Current Bullets
        cachedBullets = handler.selectableBullets.ToList();


        //Find the Zombie Displays 

        var zombieDisplaysFound = ui.Query<VisualElement>().Where(e => e.name.StartsWith("ZombieSpot")).ToList();

        //Debug.Log(zombieDisplaysFound.Count + "Zombie Displays");


        foreach (var display in zombieDisplaysFound)
        {

            int nextDisplayNumber = zombieDisplayList.Count;

            //Debug.Log(zombieDisplayList.Count);

            display.RegisterCallback<PointerEnterEvent>(SelectZombie);

            zombieDisplayList.Add(new ZombieDisplay

            {

                displayId = zombieDisplayList.Count

                , displayElement = display

            }



          );
        }

        //Debug.Log($"Populated zombie class list with {zombieDisplayList.Count}");

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
        UnregisterAnimationEvents();
        ResetLists();
    }
    //Is here to detect changes in the other scripts.
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

    private void HandleZombieDisplay()
    {
        //Handle new zombie coming in          <=======
        if (cachedZombies.Count < handler.ZombieList.Count)
        {
            Zombie newZombie =
                handler.ZombieList.Except(cachedZombies).First();

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
             // add change animation controler of the 
            assignedDisplay.displayedZombie = newZombie;
            //Debug.Log($"Zombie Display {assignedDisplay.displayId} now contains zombie with ID {assignedDisplay.displayedZombie.id}");

            VisualElement zombieDisplayElement = ui.Query<VisualElement>().Where(e => e.name == $"ZombieSpot{assignedDisplay.displayId + 1}");

            if (zombieDisplayElement == null)
            {
                Debug.Log("Failed to find corresponding zombie display, backing out...");
                return;
            }

            zombieDisplayElement.AddToClassList("zombieSpotOccupied");

            zombieDisplayList.Remove(assignedDisplay);
            StartZombieAnimation(assignedDisplay);
            occupiedZombieDisplay.Add(assignedDisplay);

            cachedZombies = new List<Zombie>(handler.ZombieList);
        }

        //Handle Zombie Leaving
        if (cachedZombies.Count > handler.ZombieList.Count)
        {
            var leavingZombie =
                cachedZombies.Except(handler.ZombieList).First();

            if (leavingZombie == null)
            {
                Debug.Log("Failed to find leaving zombie, backing out...");
                return;
            }

            ZombieDisplay assignedDisplay =
                 occupiedZombieDisplay.FirstOrDefault(e => e.displayedZombie == leavingZombie);

            if (assignedDisplay == null)
            {
                Debug.Log("Failed to find the assigned display. Backing out.");
                return;
            }

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
            StopZombieAnimation(assignedDisplay);
            occupiedZombieDisplay.Remove(assignedDisplay);


            cachedZombies = new List<Zombie>(handler.ZombieList);
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

    #endregion

    #region EndGameReset
    private void ResetLists()
    {
        zombieDisplayList.Clear();
        occupiedZombieDisplay.Clear();
        cachedZombies.Clear();
        cachedBullets.Clear();
        displayedZombies.Clear();
        zombieDisplayLookup.Clear();
    }
    #endregion
}
