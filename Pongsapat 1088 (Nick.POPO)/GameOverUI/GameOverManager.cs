using UnityEngine;
using UnityEngine.SceneManagement; 
public class GameOverManager : MonoBehaviour
{

    public void RetryGame()
    {
       
        Time.timeScale = 1f;

        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    
    {
        Time.timeScale = 1f;

       
        SceneManager.LoadScene("PlayScreen");
    }
}