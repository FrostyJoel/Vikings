using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_StartScreen : MonoBehaviour
{
    public void StartGame()
    {
        SC_MenuManager.single.GetNextRoom();
    }
    public void QuitGame()
    {
        SC_MenuManager.single.QuitGame();
    }
}
