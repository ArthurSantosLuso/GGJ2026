using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
   public void StartGame()
   {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        SceneManager.LoadScene(sceneIndex+1);
   }

    public void endGame()
    {
        Application.Quit();
    }

    public void FireScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void PoisonScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void IceScene(int index)
    {
        SceneManager.LoadScene(index);
    }
}
