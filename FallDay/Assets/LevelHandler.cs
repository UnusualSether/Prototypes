using System;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

[Serializable]
public class LevelHandler
{
    public Level current_level;

    public int rooms_to_complete;

    public int rooms_completed = 0;

    

    public void InitLevelManager(GameHandler handler, ThreeDGameHandler three_d)
    {
        handler.level_handler.current_level = current_level;

        three_d.possibleRooms = current_level.possible_rooms;

        handler.possible_rewards = current_level.possible_rewards;

        rooms_to_complete = current_level.number_of_rooms_to_complete;

        rooms_completed = 0;

        handler.possible_rewards = current_level.possible_rewards;
    }

    public void LoadLevel(Level level)
    {

        SceneManager.LoadScene(level.level_scene);


        


    }

    public void RoomCleared(out bool level_cleared)
    {
        rooms_completed++;

        if (rooms_completed >= rooms_to_complete)
        {
            level_cleared = true;
        }

        else
        {
            level_cleared = false;
        }
    }




}
