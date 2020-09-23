using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SC_MenuManager : MonoBehaviour
{
    public static SC_MenuManager single;
    public int levelScene;
    private void Awake()
    {
        if (single == null)
        {
            single = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void GetNextRoom()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(levelScene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
