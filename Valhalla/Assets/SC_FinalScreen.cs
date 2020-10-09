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
        SC_UiManager.single.QuitToMainMenu();
    }

    public void QuitGame()
    {
        SC_UiManager.single.QuitGame();
    }

}
