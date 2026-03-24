using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine.Jobs;
using Unity.VisualScripting;
using JetBrains.Annotations;
using UnityEngine.Rendering.Universal;
using UnityEngine.AI;
using System.Linq;
using System;
using System.Collections;
using UnityEngine.UIElements;
using System.Globalization;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices.WindowsRuntime;

public class GameHandler : MonoBehaviour
{


    //Names are extremely important due to the way unity's UI toolkit builder works, for that reason be very
    // careful when changing names of anything in the bullet arrays or in the UI itself.

    

    //Bullet types that can show up in the grid.
    public string[] bulletList;

    //Currently available to select bullets player has not yet selected.
    public string[] selectableBullets;
    
    //Bullets player has selected.
    public List<string> readyBullets = new List<string>();




    public UIDocument uiDoc;

    public VisualElement ui;

    public OnRailsStateMachine railMachine;

    public VisualElement[] bulletButton;

    public BulletTable bulletTable;

    public Weapon currentWeapon = new Shotgun();

    #region Events
    //Player related events
    public event Action SucessfulShot;
    public event Action FailedShot;

    //Zombie related events
    public event Action ZombieSpawned;
    public event Action ZombieKilled;

    #endregion


    [Header("Zombies!")]

    public int numberOfZombiesInLookup;


    public int numberOfZombiesinList;

    bool ZombieSpawnGate;

    public int zombieHP;

    private int stachedZombieID;

    public int preferenceZombie;
    public int nulledPreference = -1;

    public List<Zombie> ZombieList = new List<Zombie>();

    public Dictionary<int, Zombie> zombieLookup;

    public float zombieSpawnTimer;

    

    //Event Subbing and Unsubbing

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


    public Dictionary<string, BulletType> bulletLookup;
    public class BulletType
    {
        public string name;

        public string description;

        public int Damage;
    }

    public class BulletTable
    {
        public List<BulletType> bulletTypes;
    }



    /// <summary>
    /// The Zombie class which contains HP, its id which is used by the gamehandler to find out which zombie to damage & an enum for checking how close the zombie is to the player.
    /// </summary>
    public class Zombie
    {
        public int id = 0;

        public int hp;

       public enum ZombiePhase
        {
            Far,
            Approach,
            Close

        }

        public ZombiePhase phase;

        public int currentDisplay;
    }


    private void Start()
    {
        ui = uiDoc.rootVisualElement;

        //Fetch all bullet buttons

        var bulletDisplaysFound = ui.Query<VisualElement>().Where(e => e.name.StartsWith("BSpot"));

        bulletButton = bulletDisplaysFound.ToList().ToArray();

        foreach(var bullet in bulletButton)
        {
            bullet.RegisterCallback<PointerEnterEvent>(SelectedBullet);
        }
        //Create the current usable bullets

        bulletTable = new BulletTable();

        //Add the bullets
        bulletTable.bulletTypes = new List<BulletType>()
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

        foreach (var bullet in bulletTable.bulletTypes)
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
       
  

        if (ZombieSpawnGate == false)
        {
            if (ZombieList.Count == 4)
            {
                return;
            }
            else
            {
                StartCoroutine(ZombieSpawner());
            }
        }


    }

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

            phase = Zombie.ZombiePhase.Far

        }

        );

        var newZombie = ZombieList.Last();

        zombieLookup.Add(newZombie.id, newZombie);


        //Check to make sure list and dictionary line up
        numberOfZombiesInLookup = zombieLookup.Count;
        numberOfZombiesinList = ZombieList.Count;


        ZombieSpawnGate = false;

        


    }

    public void ApplyDamage(int damage, Zombie zombieToDamage)
    {

        

        

        if (zombieToDamage == null)
        {
            Debug.Log("Tried to damage invalid zombie, try again!");
            ApplyDamage(damage, zombieToAimAt());
            return;
        }

        

        var zombieDeathCheck = zombieToDamage.hp -= damage;
       
        
        if (zombieDeathCheck <= 0)
        {
            Debug.Log($"killed zombie ID {zombieToDamage.id} removing them from selectable zombies.");

           
            ZombieList.Remove(zombieToDamage);

            zombieLookup.Remove(zombieToDamage.id);


            

            //Update the dictionary with the new zombie ids

            zombieLookup.Clear();

            foreach (var zombie in ZombieList)
            {
                zombieLookup.Add(zombie.id, zombie);
            }

            //Check to make sure list and dictionary line up
            numberOfZombiesInLookup = zombieLookup.Count();
            numberOfZombiesinList = ZombieList.Count();

            preferenceZombie = nulledPreference;

            ZombieKilled?.Invoke();

           

        }

        else
        {
            zombieToDamage.hp -= damage;

            Debug.Log($"Zombie with id {zombieToDamage.id} took {damage} damage and now has {zombieToDamage.hp} hp.");
        }
           
        
        

            
        

        
    }

    public Zombie zombieToAimAt()
    {
        if (preferenceZombie != nulledPreference)
        {
            var foundZombie = zombieLookup[preferenceZombie];
            if (foundZombie == null)
            {
                Debug.Log("Preference zombie did not return a valid zombie! Auto aiming fail safe activating...");
                return ZombieList.First();
            }

            return foundZombie;
        }

        return ZombieList.First();
    }

    #endregion
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
        if (!BulletsNeedStock())
        {
            return;
        }

        for (int i = 0; i < selectableBullets.Length; i++)
        {
            if (string.IsNullOrEmpty(selectableBullets[i]))
            {
                int maxAttempts = 100;
                int attempts = 0;
                string chosenBullet;

                do
                {
                    int randomBulletIndex = UnityEngine.Random.Range(0, bulletList.Length);
                    chosenBullet = bulletList[randomBulletIndex];


                    if (selectableBullets.Length <= 0)
                    {
                        selectableBullets[i] = chosenBullet;
                    }

                    string[] futureBoard = (string[])selectableBullets.Clone();
                    futureBoard[i] = chosenBullet;

                    if (!InvalidBoard(futureBoard))
                        break;

                    attempts++;
                    if (attempts >= maxAttempts)
                    {
                        Debug.LogWarning("Could not find a valid board after 100 attempts!");
                        break;
                    }
                }
                while (true);

                selectableBullets[i] = chosenBullet;
            }
        }
    }

    public bool BulletsNeedStock()
    {

         var list = selectableBullets.ToList();
        foreach( var item in list)
        {
            if (string.IsNullOrEmpty(item))
            {
                return true;
            }
        }

        return false;
    }

    #region Softlock prevention

    private bool InvalidBoard(string[] futureBoard)
    {


        


        
        bool[] visited = new bool[futureBoard.Length];

        for (int i = 0; i < futureBoard.Length; i++)
        {
            if (visited[i] || string.IsNullOrEmpty(futureBoard[i]))
                continue;

            
            List<int> group = new List<int>();
            Queue<int> queue = new Queue<int>();
            queue.Enqueue(i);
            visited[i] = true;

            while (queue.Count > 0)
            {
                int current = queue.Dequeue();
                group.Add(current);

                foreach (int neighbor in FetchNeighbours(current))
                {
                    if (!visited[neighbor] && futureBoard[neighbor] == futureBoard[i])
                    {
                        visited[neighbor] = true;
                        queue.Enqueue(neighbor);
                    }
                }
            }

            if (group.Count >= 3)
                return false; 
        }

        return true; 

    }

    public int[] FetchNeighbours(int index)
    {
        

        
        List<int> neighbours = new List<int>();
        int cols = 3;
        int rows = 3;

        int row = index / cols;
        int col = index % cols; 

        int[] dr = { 0, 0, 1, -1 };
        int[] dc = { 1, -1, 0, 0 };

        for (int d = 0; d < 4; d++)
        {
            int newRow = row + dr[d];
            int newCol = col + dc[d];

            if (newRow >= 0 && newRow < rows && newCol >= 0 && newCol < cols)
                neighbours.Add(newRow * cols + newCol);
        }

        return neighbours.ToArray();


    }

    #endregion

    public void HandleDamage(BulletType bulletType, int numberUsed)
    {

         int rawDamage= bulletType.Damage * numberUsed;

        var totalDamage = currentWeapon.WeaponEffect(numberUsed, rawDamage, zombieToAimAt());

        Debug.Log($"Did {totalDamage} damage with {numberUsed} {bulletType.name}s!");

        ApplyDamage(totalDamage, zombieToAimAt());
      
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

}
