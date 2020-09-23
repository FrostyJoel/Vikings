using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_FinalScreen : MonoBehaviour
{
    private void Awake()
    {
        Cursor.visible = true;
    }
    public void RestartGame()
    {
        SC_MenuManager.single.RestartGame();
    }

    public void QuitGame()
    {
        SC_MenuManager.single.QuitGame();
    }

}
