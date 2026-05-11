using System.Runtime.CompilerServices;
using UnityEngine;

public partial class GameHandler
{
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

        
        public Encounter(EncounterData data)
        {
            difficulty = data.difficulty;

            numberOfZombies = data.numberOfZombies;

            zombieTimer = data.zombieTimer;
        }

    }

}
