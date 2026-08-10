using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public partial class GameHandler
{
    [SerializeField] private DifficultyLevel DiffSelection;

    public class Encounter
    {

        /// <summary>
        /// Even when creating encounters from scratch, still use the most applicable difficult for easier managing and organization.
        /// </summary>
        public enum Difficulty
        {
            Easy,
            Medium,
            Difficult
        }

        public Difficulty difficulty;

        public int numberOfZombies;

        public float zombieTimer;

        public void Updatediff()
        {
            if (DifficultyLevel.DiffSelection.value == 0 || difficulty == Difficulty.Easy)
            {
                Zombie.DifficultyValue = 1;
            }

            else if (DifficultyLevel.DiffSelection.value == 1 || difficulty == Difficulty.Medium)
            {
                Zombie.DifficultyValue = 2;
            }

            else if (DifficultyLevel.DiffSelection.value == 2 || difficulty == Difficulty.Difficult)
            {
                Zombie.DifficultyValue = 3;
            }
        }


        public Encounter(EncounterData data)
        {
            difficulty = data.difficulty;

            numberOfZombies = data.numberOfZombies;

            zombieTimer = data.zombieTimer;
        }



       
    }

}
