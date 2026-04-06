using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static GameHandler;


public partial class GameHandler : MonoBehaviour
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

    //Objective related events
    public event Action playerKilledAllZombies;

    #endregion

    #region Variables
    [Header("Zombies!")]
    public int numberOfZombiesInLookup;
    public int numberOfZombiesinList;
    bool ZombieSpawnGate;
    public int zombieHP;
    private int stachedZombieID;

    public int preferenceZombie;
    public int nulledPreference = -1;

    public int SelectedZombie;
    public List<Zombie> ZombieList = new List<Zombie>();
    public List<BulletType> bulletTypes = new List<BulletType>();
    public Dictionary<int, Zombie> zombieLookup;
    public float zombieSpawnTimer;

    int enemyDefeatTarget = 1;
    int zombiesSpawned = 0;

    public VisualElement lineCanvas; // Onde Iremos desenhar //
    public List<VisualElement> selectedUIButtons = new List<VisualElement>(); // Lista de botões pressionados para demarcar bottoes precionados pelo player //
    public Color lineColor = Color.white; // A cor da sua linha
    public float lineWidth = 10f;
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
    void OnEnable()
    {
        ThreeDGameHandler.EncounterStarted += ActivateMinigame;
        ThreeDGameHandler.EncounterEnded += DeactivateMinigame;
    }

    void OnDisable()
    {
        ThreeDGameHandler.EncounterStarted -= ActivateMinigame;
        ThreeDGameHandler.EncounterEnded -= DeactivateMinigame;
    }   
    #endregion

    #region Internal Classes
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
                PlayerTookDamage?.Invoke(1f);
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

    #region Player Damage Handling
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
        lineCanvas = ui.Query<VisualElement>("HudDisplayArea").First();
        if (lineCanvas != null)
        {
            lineCanvas.generateVisualContent += OnDrawLines;
        }
    }

    public void Update()
    {
        HandleBulletSelect();

        RestockBullet();

        Reload();
        
        OnZombieUpdate?.Invoke(Time.deltaTime);

        if (ZombieSpawnGate == false)
        {
            if (!CanISpawnEnemies())
            {
                return;
            }

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
        zombiesSpawned++;
    }
    

    private bool CanISpawnEnemies()
    {
        if (!uiDoc.gameObject.activeSelf)
        {
            return false;
        }

        if (zombiesSpawned >= enemyDefeatTarget)
        {
            return false;
        }


        return true;
    }

    private bool HasPlayerCompletedTheEncounter()
    {
        if (zombiesSpawned >= enemyDefeatTarget && zombieLookup.Count == 0)
        {
            return true;
        }

        return false;
    }
    public void ApplyDamage(int damage)
    {
        ApplyDamage(damage, zombieToAimAt());
    }

    public void ApplyDamage(int damage, Zombie zombieToDamage)
    {
        //zombieToDamage = zombieLookup[SelectedZombie];

        if (zombieToDamage == null)
        {
            Debug.Log("Tried to damage invalid zombie, try again!");
            ApplyDamage(damage, zombieToAimAt());
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

            preferenceZombie = nulledPreference;

            ZombieKilled?.Invoke();

            if (HasPlayerCompletedTheEncounter())
            {
                playerKilledAllZombies?.Invoke();
            }
        }
        else
        {
            zombieToKill.hp = 0;

            Debug.Log($"Zombie with id {zombieToKill.id} took fatal damage and now has {zombieToKill.hp} hp.");
            ZombieKilled?.Invoke();
            SelectedZombie = ZombieList.First().id;
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
            selectedUIButtons.Clear();
            lineCanvas.MarkDirtyRepaint();  // Apaga a linha da tela
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
            selectedUIButtons.Clear();
            lineCanvas.MarkDirtyRepaint();  // Apaga a linha da tela
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
            selectedUIButtons.Clear();
            lineCanvas.MarkDirtyRepaint();  // Apaga a linha da tela

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
        selectedUIButtons.Clear();
        lineCanvas.MarkDirtyRepaint();  // Apaga a linha da tela
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
        foreach (var item in list)
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

        //Need to fix, because these calculations on account for x and y and doesnt detect diagonal neighbours.

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

                    // NOVIDADE AQUI:   /////////////////////////////////////////////////////////////////
                    selectedUIButtons.Add(selectedElement);
                    lineCanvas?.MarkDirtyRepaint();
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

    /// <summary>
    /// // NewFunction Draw In the UI //////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>

    public void OnDrawLines(MeshGenerationContext context)
    {
        // Se tivermos menos de 2 botões selecionados, não tem como traçar uma linha
        if (selectedUIButtons.Count < 2) return;
        var painter2D = context.painter2D;

        painter2D.lineWidth = lineWidth;
        painter2D.strokeColor = lineColor;
        painter2D.lineJoin = LineJoin.Round; // Deixa as quinas da linha arredondadas
        painter2D.lineCap = LineCap.Round; // Deixa a ponta da linha arredondada

        painter2D.BeginPath();

        for (int i = 0; i < selectedUIButtons.Count; i++)
        {
            var button = selectedUIButtons[i];

            // Pega o centro exato do botão e converte para as coordenadas do Canvas de linha
            Vector2 buttonCenterInWorld = button.worldBound.center;
            Vector2 localPos = lineCanvas.WorldToLocal(buttonCenterInWorld);

            if(i == 0)
            {
                // Move o "pincel" para o primeiro botão
                painter2D.MoveTo(localPos);
            }
            else
            {
                // Traça a linha até os próximos botões
                painter2D.LineTo(localPos);
            }
        }

        // Pinta a linha de fato
        painter2D.Stroke();
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
        InitializeEncounter();
        Debug.Log("I should activate now!");
    }
    public void DeactivateMinigame()
    {
        ui.visible = false;
        Debug.Log("I should deactivate now!");
    }



    #endregion
    
    public void InitializeEncounter()
    {
        enemyDefeatTarget = UnityEngine.Random.Range(4, 8);
        zombiesSpawned = 0;
    }


}
