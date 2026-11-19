using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement; 


namespace BetterSceneManagement
{


    public static class BetterSceneManager
    {

        public static event Action<AnimationType> StartTransition;

        public static event Action DoneLoading;

       

        public static void LoadScene(string scene_name, AnimationType animation)
        {
            StartTransition?.Invoke(animation);

            scene_to_load = scene_name;

            GeneralAnimator.AnimationDone += LoadAssetsAndScene;
        }

        private static string scene_to_load;
        private static void LoadAssetsAndScene()
        {
            SceneManager.LoadScene(scene_to_load);

            DoneLoading.Invoke();

            scene_to_load = "";


            GeneralAnimator.AnimationDone -= LoadAssetsAndScene;

        }

    }
}