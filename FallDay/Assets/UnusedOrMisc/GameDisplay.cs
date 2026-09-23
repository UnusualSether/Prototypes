using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Collections;
using System;

public partial class GameDisplay : MonoBehaviour
{

    public VisualElement gameplay_doc_root;
    public VisualElement[] bulletDisplay;
    public UIDocument gameplay_ui_doc;


    public UIDocument direction_ui_doc;

    public DirectionDisplay[] direction_and_reward_buttons = new DirectionDisplay[3];

    public GameHandler handler;

    public Label damage_number_label;

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

        public Label zombie_damage_display_label;

        public VisualElement displayElement;

        public Coroutine activeAnimation;
    }

    [Serializable]
    public class DirectionDisplay
    {
        public Button displayElement;

        public Label reward_name;

        public Label reward_description;

        public Image reward_icon;

        public Reward displayed_reward;

        public void SetParams()
        {

            reward_name = displayElement.Q<Label>("name_of_reward");

            reward_description = displayElement.Q<Label>("reward_description");

            reward_icon = displayElement.Q<Image>("reward_image");

            

        }

        public void UpdateRewardDisplay()
        {

            if (displayed_reward == null)
            {
                return;
            }

            if (reward_icon == null)
            {
                Debug.Log("Didn't find a display for the icon!");
            }

            else
            {
                reward_icon.sprite = displayed_reward.reward_sprite;
            }

            if (reward_name == null)
            {
                Debug.Log("Didn't find a display for the name!");
            }
            else
            {
                reward_name.text = displayed_reward.reward_name;
            }
           


        }





    }

    private void Awake()
    {
        

        
    }
    private void OnEnable()
    {


        gameplay_doc_root = gameplay_ui_doc.rootVisualElement;

        List<VisualElement> numberOfDisplay = new List<VisualElement>();

        //Events

        handler.ZombieKilled += RemoveCrosshair;

        handler.ZombieHurt += ApplyZombieDamageNumber;

        handler.BulletSelected += ShakeBullet;

        handler.BulletSelected += PlayerClickSound;

        handler.ZombieDamaged += ShakeZombieVisual;
        RegisterAnimationEvents();

        ThreeDGameHandler.PlayerChoiceStarted += SetDirectionDisplayOn;
        ThreeDGameHandler.PlayerChoiceEnded += SetDirectionDisplayOff;

        //Find the Bullet Displays using a for loop
        var bulletDisplaysFound = gameplay_doc_root.Query<VisualElement>().Where(e => e.name.StartsWith("BSpot")).ToList();

        //Debug.Log($"{bulletDisplaysFound.Count} bullet displays found");

        //Find the damage number display

        damage_number_label = gameplay_ui_doc.rootVisualElement.Query<Label>("DamageNumberDisplay");

       

        

        bulletDisplay = bulletDisplaysFound.ToArray();


        //Cache Current Bullets
        cachedBullets = handler.selectableBullets.ToList();


        //Find the Zombie Displays 

        var zombieDisplaysFound = gameplay_doc_root.Query<VisualElement>().Where(e => e.name.StartsWith("ZombieSpot")).ToList();

        //Debug.Log(zombieDisplaysFound.Count + "Zombie Displays");


        foreach (var display in zombieDisplaysFound)
        {

            int nextDisplayNumber = zombieDisplayList.Count;

           

            //Debug.Log(zombieDisplayList.Count);

            display.RegisterCallback<PointerEnterEvent>(SelectZombie);

            zombieDisplayList.Add(new ZombieDisplay

            {

                displayId = zombieDisplayList.Count

                , displayElement = display,

                zombie_damage_display_label = display.Q<Label>("ZombieDamageDisplay")

               

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


        handler.NewRewardsGenerated += UpdateButtonRewards;

        handler.PlayerKilledAllZombies += SetDirectionDisplayOn;

        SetEachClassesElementToElement();

    }
    private void OnDisable()
    {
        handler.ZombieKilled -= RemoveCrosshair;
        
        handler.BulletSelected -= ShakeBullet;

        handler.BulletSelected -= PlayerClickSound;

        handler.ZombieDamaged -= ShakeZombieVisual;
        UnregisterAnimationEvents();
        ResetLists();

        ThreeDGameHandler.PlayerChoiceStarted -= SetDirectionDisplayOn;
        ThreeDGameHandler.PlayerChoiceEnded -= SetDirectionDisplayOff;
    }
    //Is here to detect changes in the other scripts.
    private void Update()
    {

        HandleDamageNumberDisplay();

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

    public void ApplyZombieDamageNumber(int total_damage, Zombie damaged_zombie)
    {
        var zombie_display = occupiedZombieDisplay.FirstOrDefault(x => x.displayedZombie == damaged_zombie);

        if (zombie_display == null)
        {
            Debug.Log("Couldn't find zombie display!");
        }

        zombie_display.zombie_damage_display_label.text = total_damage.ToString();

        if (zombie_display.zombie_damage_display_label == null)
        {
            Debug.Log("Couldn't find label!");
        }

        ShakeLabel(zombie_display.zombie_damage_display_label);

        Debug.Log($"Applied {total_damage} to zombie with id nmb {damaged_zombie.id} ");

        HandleNumberDisappear(zombie_display.zombie_damage_display_label);

    }

    public void ResetDamageNumberValue(Label label)
    {
        label.text = "";
    }

    public void HandleNumberDisappear(Label label_to_disappear)
    {
        StartCoroutine(ZombieDamageNumberDuration(2.5f,label_to_disappear));

        
    }

    public IEnumerator ZombieDamageNumberDuration(float duration, Label label_to_disappear)
    {

        yield return new WaitForSeconds(duration);

        ResetDamageNumberValue(label_to_disappear);


    }
    public void HandleDamageNumberDisplay()
    {
        if (handler.current_bullet_damage == 0)
        {
            damage_number_label.text = "";
        }

        else
        {
            damage_number_label.text = handler.current_bullet_damage.ToString();
        }
        

        
    }


    #region Direction Button Handling


    public static event Action<Reward> RewardChosen;

    public static event Action<ThreeDGameHandler.SwipeDirection> DirectionChosen;
    public void UpdatePointRewards(DirectionDisplay display, Reward displayed_r)
    {

        display.displayed_reward = displayed_r;


    }

    public void UpdateButtonRewards()
    {
        for (int i = 0; i < direction_and_reward_buttons.Length; i++)
        {
            direction_and_reward_buttons[i].displayed_reward = handler.reward_trio[i];
        }
   
    }

    public void SetEachClassesElementToElement()
    {

        direction_and_reward_buttons[0].displayElement = direction_ui_doc.rootVisualElement.Query<Button>("up_button");
        direction_and_reward_buttons[1].displayElement = direction_ui_doc.rootVisualElement.Query<Button>("right_button");
        direction_and_reward_buttons[2].displayElement = direction_ui_doc.rootVisualElement.Query<Button>("left_button");

        direction_and_reward_buttons[0].displayElement.clicked += () => SoundOffChosenReward(direction_and_reward_buttons[0].displayed_reward);
        direction_and_reward_buttons[1].displayElement.clicked += () => SoundOffChosenReward(direction_and_reward_buttons[1].displayed_reward);
        direction_and_reward_buttons[2].displayElement.clicked += () => SoundOffChosenReward(direction_and_reward_buttons[2].displayed_reward);

        direction_and_reward_buttons[0].displayElement.clicked += () => SoundOffChosenDirection(ThreeDGameHandler.SwipeDirection.Up);
        direction_and_reward_buttons[1].displayElement.clicked += () => SoundOffChosenDirection(ThreeDGameHandler.SwipeDirection.Right);
        direction_and_reward_buttons[2].displayElement.clicked += () => SoundOffChosenDirection(ThreeDGameHandler.SwipeDirection.Left);


        direction_and_reward_buttons[0].displayElement.clicked += SetDirectionDisplayOff;
        direction_and_reward_buttons[1].displayElement.clicked += SetDirectionDisplayOff;
        direction_and_reward_buttons[2].displayElement.clicked += SetDirectionDisplayOff;


        foreach (var button in direction_and_reward_buttons)
        {
            button.SetParams();
        }
    }

    public void SetButtonDisplayToCurrentRewards()
    {
        var current_rewards = handler.reward_trio;

        for(int i  = 0; i < current_rewards.Length; i++)
        {
            direction_and_reward_buttons[i].displayed_reward = current_rewards[i];
        }

    }

    public void SetDirectionDisplayOff()
    {
        foreach (var dir_button in direction_and_reward_buttons)
        {
            dir_button.displayElement.visible = false;
        }
    }

    public void SetDirectionDisplayOn()
    {
        foreach (var dir_button in direction_and_reward_buttons)
        {
            
            if (ThreeDGameHandler.leftandrightnulling == ThreeDGameHandler.SwipeDirection.Left)
            {
                if (dir_button == direction_and_reward_buttons[1])
                {
                    continue;
                }
            }

            if (ThreeDGameHandler.leftandrightnulling == ThreeDGameHandler.SwipeDirection.Right)
            {
                if (dir_button == direction_and_reward_buttons[2])
                {
                    continue;
                }
            }

            dir_button.displayElement.visible = true;

            dir_button.UpdateRewardDisplay();

            
        }
    }

    public void SoundOffChosenReward(Reward chosen)
    {
        RewardChosen?.Invoke(chosen);
    }

    public void SoundOffChosenDirection(ThreeDGameHandler.SwipeDirection direction)
    {
        DirectionChosen?.Invoke(direction);

        Debug.Log($"This is the button! I'm sounding off the direction as {direction.ToString()}.");
    }


    #endregion

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

            VisualElement zombieDisplayElement = gameplay_doc_root.Query<VisualElement>().Where(e => e.name == $"ZombieSpot{assignedDisplay.displayId + 1}");

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

            VisualElement zombieDisplayElement = gameplay_doc_root.Query<VisualElement>().Where(e => e.name == $"ZombieSpot{assignedDisplay.displayId + 1}");

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
