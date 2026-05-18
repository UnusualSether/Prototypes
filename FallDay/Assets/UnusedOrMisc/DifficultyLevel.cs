using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using static GameHandler.Encounter;

public class DifficultyLevel : MonoBehaviour
{
    public static RadioButtonGroup DiffSelection;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        var select = root.Q<RadioButtonGroup>("DiffSelection");

        var choose = new List<string> { "Easy", "Medium", "Hard"};

        select.choices = choose;

        select.value = Zombie.DifficultyValue;

        select.RegisterValueChangedCallback((evt) =>
        {
            if(evt.newValue == 0) 
            {
                Zombie.DifficultyValue = 1;
                Debug.Log("Dificuldade virou Facil");
            }

            if (evt.newValue == 1)
            {
                Zombie.DifficultyValue = 2;
                Debug.Log("Dificuldade virou Medio");
            }

            if (evt.newValue == 2)
            {
                Zombie.DifficultyValue = 3;
                Debug.Log("Dificuldade virou Dificil");
            }

        });

    }



    
}
