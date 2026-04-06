using UnityEngine;
using UnityEngine.SceneManagement;

public partial class ThreeDGameHandlerMinigameManaging : MonoBehaviour
{
       public GameObject UIDocument;

    private void Update()
    {
        WhenToDeactivateMinigame();
        WhenToActivateMinigame();
    }
    private void WhenToActivateMinigame()
        {
          if (Input.GetKeyDown(KeyCode.G))
          {
              UIDocument.SetActive(true);
              Debug.Log("Apareceu!");
          }

    }

        private void WhenToDeactivateMinigame()
        {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if(UIDocument != null)
            {
                {
                    UIDocument.SetActive(false);
                    Debug.Log("Sumiu!");
                }
            }
        }
    } 
} 
