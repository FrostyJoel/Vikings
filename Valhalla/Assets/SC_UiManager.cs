using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SC_UiManager : MonoBehaviour
{
    public static SC_UiManager single;

    public bool startScreen;

    [Header("Screens")]
    public GameObject startScreenMenu;
    public GameObject loadingScreen;
    public GameObject optionScreenMenu;
    public GameObject pauseScreenMenu;

    [Header("Sliders")]
    public Slider loadingBar;

    public Slider healthBar;
    public Slider lightningStrike;

    [Header("Texts")]
    public Text lightningStrikeCoolDown;

    [Header("HideInInspector")]
    public bool getAttackInput;

    private void Awake()
    {
        if(single == null)
        {
            single = this;
        }
        else
        {
            Destroy(single.gameObject);
            return;
        }

        if(startScreenMenu == null && startScreen)
        {
            Debug.LogWarning("No Start Menu Assigned");
            return;
        }

        if (loadingScreen == null)
        {
            Debug.LogWarning("No Loading Screen Assigned");
            return;
        }

        if(loadingBar == null)
        {
            Debug.LogWarning("No Loading Bar Assigned");
        }

        if (pauseScreenMenu == null)
        {
            Debug.LogWarning("No Pause Menu Assigned");
            return;
        }

        if(optionScreenMenu == null)
        {
            Debug.LogWarning("No Option Menu Assigned");
            return;
        }

        if(healthBar == null)
        {
            Debug.LogWarning("No Healthbar Assigned");
            return;
        }

        if (lightningStrike == null)
        {
            Debug.LogWarning("No LightningStrikeSlider Assigned");
            return;
        }

        if (lightningStrikeCoolDown == null)
        {
            Debug.LogWarning("No Lightning Strike Cooldown Text Assigned");
            return;
        }

    }

    #region StartScreenFunctionality

    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void OptionScreen()
    {
        if (startScreenMenu.activeSelf)
        {
            startScreenMenu.SetActive(false);
            optionScreenMenu.SetActive(true);
        }
    }

    public void ReturnToMainMenu()
    {
        if (optionScreenMenu.activeSelf)
        {
            optionScreenMenu.SetActive(false);
            startScreenMenu.SetActive(true);
        }
    }

    public void QuitGame()
    {
        if (startScreenMenu.activeSelf)
        {
            Application.Quit();
        }
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        #region StartOfRoom
        
        #endregion

        #region InGameUIFunctionality
        if (startScreen) { return; }

        if (Input.GetButtonDown("Cancel"))
        {
            if (!pauseScreenMenu.activeSelf)
            {
                pauseScreenMenu.SetActive(true);
            }
            else
            {
                pauseScreenMenu.SetActive(false);
            }
            optionScreenMenu.SetActive(false);
        }

        if (pauseScreenMenu.activeSelf)
        {
            Time.timeScale = 0;
            getAttackInput = false;
            if (!Cursor.visible)
            {
                Cursor.visible = true;
            }
        }
        else
        {
            Time.timeScale = 1f;
            if (!IsInvoking(nameof(ResetAttackInput)))
            {
                Invoke(nameof(ResetAttackInput), 0.1f);
            }
            if (Cursor.visible)
            {
                Cursor.visible = false;
            }
        }

        HealthBarUpdate();
        LightninStrikeCoolDownUpdate();
        #endregion
    }

    #region inGameUIFunctionality
    public void ResetAttackInput()
    {
        getAttackInput = true;
    }

    private void LightninStrikeCoolDownUpdate()
    {
        SC_AttackManager aMan = SC_AttackManager.single;
        float currCooldown = aMan.currLightningCooldown;
        lightningStrike.minValue = 0;
        lightningStrike.value = currCooldown;
        lightningStrike.maxValue = SC_AttackManager.single.maxLightningCooldown;
        if (currCooldown > 0)
        {
            lightningStrikeCoolDown.text = Mathf.RoundToInt(currCooldown).ToString();
        }
        else
        {
            lightningStrikeCoolDown.text = "";
        }
    }

    private void HealthBarUpdate()
    {
        healthBar.minValue = 0;
        healthBar.value = SC_TopDownController.single.curHealth;
        healthBar.maxValue = SC_TopDownController.single.maxHealth;
    }

    public void ReturnToGame()
    {
        if (pauseScreenMenu.activeSelf)
        {
            pauseScreenMenu.SetActive(false);
            optionScreenMenu.SetActive(false);
        }
    }

    public void OptionsInGame()
    {
        if (pauseScreenMenu.activeSelf)
        {
            optionScreenMenu.SetActive(true);
            pauseScreenMenu.SetActive(false);
        }
    }

    public void BackToPauseMenu()
    {
        if (optionScreenMenu.activeSelf)
        {
            optionScreenMenu.SetActive(false);
            pauseScreenMenu.SetActive(true);
        }
    }

    public void QuitToMainMenu()
    {
        if (pauseScreenMenu.activeSelf)
        {
            SceneManager.LoadScene(0);
        }
    }
    #endregion

}
