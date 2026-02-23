using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine.Jobs;
using Unity.VisualScripting;
using JetBrains.Annotations;
using System;

public class GachaSystem : MonoBehaviour
{
    


    static int UltraRare = 10;
    static int Rare = 40;
    static int Common = 50;

    [SerializeField] public WeightedTable<CharacterData> characterTable = new WeightedTable<CharacterData>();



    [SerializeField] public List<WeightedEntry<string>> basicBannerEntries;



    [Serializable]
    public class WeightedEntry<T>
    {
        public T value; //Received Item/Char
        public int rarity; // Likeliness
    }

    [Serializable]
    public class WeightedTable<T>
    {
        public List<WeightedEntry<T>> Entries;
    }



    public static class WeightedSelector
    {
        public static T Select<T>(WeightedTable<T> table)
        {


            int weightCutoff;

            int selectedPullIndex;

            weightCutoff = UnityEngine.Random.Range(UltraRare, Common);

            selectedPullIndex = UnityEngine.Random.Range(0, table.Entries.Count);
            
            if (table.Entries[selectedPullIndex].rarity < weightCutoff)
            {
                Debug.Log("Selected Item is out of weight range, reselecting...");
                selectedPullIndex = UnityEngine.Random.Range(0, table.Entries.Count);
            }


            return table.Entries[selectedPullIndex].value;
        }
    }

    public void Start()
    {
        //Create Basic Banner


        


        //Create Special Banner
        
    }

    [ContextMenu("Pull")]
    public void Pull()
    {
        
    }

    public void AddCharacters()
    {
        
    }

    public void AddItems()
    {

    }

    public void AddFragments()
    {

    }

    

    
}
