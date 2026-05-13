using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoBackMainGame : MonoBehaviour
{

        public void GoBackButton()          // Button to go back
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);       // Goes back to main game
    }
}
