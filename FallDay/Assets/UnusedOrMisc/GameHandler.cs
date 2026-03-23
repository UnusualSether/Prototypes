using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static GameHandler;


public class GameHandler : MonoBehaviour
{
    //Names are extremely important due to the way unity's UI toolkit builder works, for that reason be very
    // careful when changing names of anything in the bullet arrays or in the UI itself.
    
    #region Lists and Arrays
    //Bullet types that can show up in the grid.
    public string[] bulletList;
    //Currently available to select bullets player has not yet selected.
    public string[] selectableBullets;
    //Bullets player has selected.
    public List<string> readyBullets = new List<string>();
    public Dictionary<string, BulletType> bulletLookup;
    #endregion

    #region OtherClassesInUse
    public UIDocument uiDoc;

    public VisualElement ui;

    public OnRailsStateMachine railMachine;

    public VisualElement[] bulletButton;

    public Weapon currentWeapon = new Shotgun();
    #endregion

    #region Events
    //Player related events
    public event Action SucessfulShot;
    public event Action FailedShot;

    //Zombie related events
    public event Action ZombieSpawned;
    public event Action ZombieKilled;

    #endregion

    #region Variables
    [Header("Zombies!")]
    public int numberOfZombiesInLookup;
    public int numberOfZombiesinList;
    bool ZombieSpawnGate;
    public int zombieHP;
    private int stachedZombieID;
    public int SelectedZombie;
    public List<Zombie> ZombieList = new List<Zombie>();
    public List<BulletType> bulletTypes = new List<BulletType>();
    public Dictionary<int, Zombie> zombieLookup;
    public float zombieSpawnTimer;
    #endregion

    #region Delegates
    public delegate void ZombieUpdateHandler(float DeltaTime);
    public static ZombieUpdateHandler OnZombieUpdate;
    
    public delegate void PlayerIsDemeged(float damage);
    public static PlayerIsDemeged PlayerTookDamage;

    public delegate void DestroyZombie(int id);
    public static DestroyZombie destroyZombie;

    #endregion

    #region EnableDisable
    private void OnEnable()
    {
        railMachine.EncounterStarted += ActivateMinigame;
        railMachine.EncounterEnded += DeactivateMinigame;
    }

    private void OnDisable()
    {
        railMachine.EncounterStarted -= ActivateMinigame;
        railMachine.EncounterEnded -= DeactivateMinigame;
    }
    #endregion

    #region Internal Classes
    public class BulletType
    {
        public string name;
        public string description;
        public int Damage;
    }

    public class Zombie
    {
        public int id = 0;
        public int hp;
        public float PhaseTimer = 5;
        private float PhT1 = 0;
        private bool IsFirstUpdate = true;

        public enum ZombiePhase
        {
            Far,
            Approach,
            Close
        }

        public ZombiePhase phase;

        public int currentDisplay;

        public void UpdatePhase(float deltaTime)
        {
            if(phase == ZombiePhase.Close && PhT1 <= 0) 
            {
                PlayerTookDamage.Invoke(1f);
                // <= Place a Destroy Zombie Call
                
                destroyZombie?.Invoke(id);
            }
            else if (IsFirstUpdate) 
            { 
                IsFirstUpdate = false; 
                PhT1 = PhaseTimer; 
            }
            else if (PhT1 <= 0)
            {
                PhT1 = PhaseTimer;
                ChangePhase();
            }
            else
            {
                PhT1 -= deltaTime;
            }
        }
        private void ChangePhase()
        {
            if (phase == ZombiePhase.Far)
            {
                phase = ZombiePhase.Approach;
                Debug.Log($"Zombie with id {id} has changed phase to {phase}");
            }
            else if (phase == ZombiePhase.Approach)
            {
                phase = ZombiePhase.Close;
                Debug.Log($"Zombie with id {id} has changed phase to {phase}");
            }
        }
    }
    #endregion

    #region Player Deamge Handling
    [ContextMenu("InspectorDemageCall")]
    public void InspectorDemageCall()
    {
        Debug.Log("Calling Delegate from Inspector");
        CallPlayerDamage(0);
    }
    public void CallPlayerDamage(float damage)
    {
        if (damage != 0)
        {
            damage = 1f;
        }
        Debug.Log("Calling Delegate");
        PlayerTookDamage?.Invoke(damage);
    }
    #endregion

    #region Unity Functions
    private void Start()
    {

        ui = uiDoc.rootVisualElement;
        destroyZombie += _KillZombie;
        //Fetch all bullet buttons
        var bulletDisplaysFound = ui.Query<VisualElement>().Where(e => e.name.StartsWith("BSpot"));

        bulletButton = bulletDisplaysFound.ToList().ToArray();

        foreach(var bullet in bulletButton)
        {
            bullet.RegisterCallback<PointerEnterEvent>(SelectedBullet);
        }

        //Add the bullets
        bulletTypes = new List<BulletType>()
        {

            new BulletType()
            {
                name = "goodBullet",
                description = "A good bullet.",
                Damage = 10
            },
        
            new BulletType()
            {
               name = "shitBullet",
               description = "A bad bullet.",
               Damage = 5
            },

            new BulletType()
            {
                name = "epicBullet",
                description = "An epic bullet!",
                Damage = 20
            }
        };

        bulletLookup = new Dictionary<string, BulletType>();

        foreach (var bullet in bulletTypes)
        {
            bulletLookup.Add(bullet.name, bullet);
            //Debug.Log($"Added{bullet.name} to bulletLookup.");
        }

        zombieLookup = new Dictionary<int, Zombie>();

        //Sync display and game handler bulletArrays

        //Print Used weapon

        Debug.Log($"You're currently using the {currentWeapon.name}");
    }

    public void Update()
    {

        HandleBulletSelect();

        RestockBullet();

        Reload();
        
        OnZombieUpdate?.Invoke(Time.deltaTime);

        if (ZombieSpawnGate == false)
        {
            if (ZombieList.Count == 4)
            {
                return;
            }
            else
            {
                StartCoroutine(ZombieSpawner());    //<== Why Spawn the Zombie in update? If max is 4 spawn 4 and reset the code when dead. And hide when not in action. _RLH107
            }
        }
    }
    #endregion

    #region Zombie Handling
    IEnumerator ZombieSpawner()
    {
        ZombieSpawnGate = true;
        yield return new WaitForSeconds(zombieSpawnTimer);

        Debug.Log("Grahh....");

        ZombieSpawned?.Invoke();

        int nextZombieID;

        //Give them a brand new Id which is just the last zombies ID plus 1;
        if (ZombieList.Count == 0)
        {
            nextZombieID = 0;
            stachedZombieID = nextZombieID;
        }

        if (ZombieList.Count > 0)
        {
            nextZombieID = ZombieList.Last().id + 1;
            stachedZombieID = nextZombieID;
        }

        ZombieList.Add(new Zombie()
        {
            id = stachedZombieID,

            hp = zombieHP,

            phase = Zombie.ZombiePhase.Far,

            PhaseTimer = 6f
        }
        );

        var newZombie = ZombieList.Last();
        OnZombieUpdate += newZombie.UpdatePhase;
        zombieLookup.Add(newZombie.id, newZombie);

        //Check to make sure list and dictionary line up
        numberOfZombiesInLookup = zombieLookup.Count;
        numberOfZombiesinList = ZombieList.Count;

        Debug.Log($"Spawned zombie with id {newZombie.id} and {newZombie.hp} hp! There are now {ZombieList.Count} zombies in the list and {zombieLookup.Count} zombies in the lookup!");
        ZombieSpawnGate = false;
    }

    public void ApplyDamage(int damage)
    {

        var zombieToDamage = zombieLookup[SelectedZombie];

        if (zombieToDamage == null)
        {
            Debug.Log("Tried to damage invalid zombie, try again!");
            return;
        }

        var zombieDeathCheck = zombieToDamage.hp -= damage;
        if (zombieDeathCheck <= 0)
        {
            _KillZombie(zombieToDamage);
        }
        else
        {
            zombieToDamage.hp -= damage;
            Debug.Log($"Zombie with id {zombieToDamage.id} took {damage} damage and now has {zombieToDamage.hp} hp.");
        }
    }
    public void _KillZombie(int id)
    {
        _KillZombie(zombieLookup[id]);
    }
    public void _KillZombie(Zombie zombieToKill) //<= same as zombie damege ,just skipping a step
    {
        Debug.Log($"killed zombie ID {zombieToKill.id} removing them from selectable zombies.");
        if (zombieLookup[zombieToKill.id] != null)
        {
            ZombieList.Remove(zombieToKill);
            zombieLookup.Remove(zombieToKill.id);
            OnZombieUpdate -= zombieToKill.UpdatePhase;

            //Update the dictionary with the new zombie ids
            int a = 0;
            zombieLookup.Clear();
            foreach (var zombie in ZombieList)
            {
                zombie.id = a;
                zombieLookup.Add(zombie.id, zombie);
                a++;
            }

            //Debug:  Check to make sure list and dictionary line up
            numberOfZombiesInLookup = zombieLookup.Count();
            numberOfZombiesinList = ZombieList.Count();
            ZombieKilled?.Invoke();
            SelectedZombie = ZombieList.First().id;
        }
    }



    #endregion

    #region Bullet Handling
    public void HandleBulletSelect()
    {
        if (Input.touchCount > 0)
        {
             var currentTouch = Input.GetTouch(0);
            //Debug.Log($"There's a finger on the screen! ID : {currentTouch.fingerId}");

              if (currentTouch.phase == TouchPhase.Ended)
              {
                HandleShot();
              }
        }
    }

    public void HandleShot()
    {
        Debug.Log("Shoot!");

        if (readyBullets.Distinct().Count() <= 1 && readyBullets.Count > 2)
        {
            //Debug.Log("Shot Went through!");

            string bulletTypeUsed;
            int numberUsed;

            //Find the number of bullets used
            numberUsed = readyBullets.Count();

            //Debug.Log($"Used {numberUsed} number of bullets");

            //Grab the name of bullet used in the successful shot
            bulletTypeUsed = readyBullets.First();

            //Declare it's actual type as stored in BulletTypes.
            BulletType realBulletType = bulletLookup[bulletTypeUsed];

            //Debug.Log($"I shot{realBulletType.name}");

            SucessfulShot?.Invoke();
            HandleDamage(realBulletType,numberUsed);
            ClearUsedBullets();

            readyBullets.Clear();
        }

        //If selected bullets don't match, no damage goes through
        if (readyBullets.Distinct().Count() != 0 && readyBullets.Count > 2)
        {
            Debug.Log("Shot Failed!");

            //Find all bullets previously tagged with used and remove them from the class list.
            foreach (var bullet in bulletButton)
            {
                if (bullet.ClassListContains("Used"))
                { 
                    bullet.RemoveFromClassList("Used");
                }

            }

            //Clear the ready bullets list
            readyBullets.Clear();
        }

        //Cancel the shot altogether if there arent enough bullets.
        if (readyBullets.Count() < 3)
        {
            Debug.Log("Not Enough bullets selected!");

            //Find all bullets previously tagged with used and remove them from the class list.

            foreach (var bullet in bulletButton)
            {
                if (bullet.ClassListContains("Used"))
                {
                    bullet.RemoveFromClassList("Used");
                }
            }

            //Clear the ready bullets list
            readyBullets.Clear();

            FailedShot?.Invoke();
        }
    }

    public void ClearUsedBullets()
    {
        //Debug.Log("Clearing Used Bullets...");
        int currentBulletIndex = 0;

        foreach (var bullet in bulletButton)
        {
            

            if (bullet.ClassListContains("Used"))
            {
                selectableBullets[currentBulletIndex] = null;
                //Debug.Log($"Nulled {currentBulletIndex} ");                
                bullet.RemoveFromClassList("Used");
            }

            currentBulletIndex++;

            if (currentBulletIndex > selectableBullets.Length)
            {
                currentBulletIndex = 0;
                Debug.Log("Checked all bullets, backing out.");
            }
        }
    }

    public void RestockBullet()
    {
        for (int i = 0; i < selectableBullets.Length; i++)
        {
            if (string.IsNullOrEmpty(selectableBullets[i]))
            {
                int randomBulletIndex = UnityEngine.Random.Range(0,bulletList.Length);
                selectableBullets[i] = bulletList[randomBulletIndex];
            }
        }
    }

    public void HandleDamage(BulletType bulletType, int numberUsed)
    {
        int rawDamage= bulletType.Damage * numberUsed;

        var totalDamage = currentWeapon.WeaponEffect(numberUsed, rawDamage, zombieLookup[SelectedZombie]);

        Debug.Log($"Did {totalDamage} damage with {numberUsed} {bulletType.name}s!");

        ApplyDamage(totalDamage);
    }

    public void SelectedBullet(PointerEnterEvent ev)
    {
        var selectedElement = (VisualElement)ev.currentTarget;

        if (selectedElement.ClassListContains("Used") == false)
        {
            selectedElement.AddToClassList("Used");
            int targButtonIdenity;

            for (int i = 0; i < bulletButton.Length; i++)
            {
                if (selectedElement.name == bulletButton[i].name)
                {
                    targButtonIdenity = i;
                    readyBullets.Add(selectableBullets[targButtonIdenity]);
                }
            }
        }
        else
        {
            Debug.Log("This bullet has already been selected!");
        }
    }

    public void Reload()
    {
        bool reloadGate = false;

        if (Input.GetKey(KeyCode.R) && reloadGate == false)
        {
            reloadGate = true;

            for (int i = 0; i < selectableBullets.Length; i++)
            {
                selectableBullets[i] = null;
            }

            reloadGate = false;
        }
    }
    #endregion

    #region Debug & Active Inactive Minigame
    [ContextMenu("Debug Buttons")]
    public void DebugFunction()
    {
        Debug.Log(ZombieList.Count()+ " This is the number of zombies in zombie list");
        
        foreach (var zombie in ZombieList)
        {
            Debug.Log($"This is the indivual zombie id {zombie.id}");
        }
    }
    public void ActivateMinigame()
    {
        ui.visible = true;
        Debug.Log("I should activate now!");
    }
    public void DeactivateMinigame()
    {
        ui.visible = false;
        Debug.Log("I should deactivate now!");
    }
    #endregion
}
