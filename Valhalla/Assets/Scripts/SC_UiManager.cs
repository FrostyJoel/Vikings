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
    public bool loading;

    [Header("Main")]
    public GameObject hud;
    public GameObject menu;
    public GameObject endScreen;

    [Header("Screens")]
    public GameObject startScreenMenu;
    public GameObject loadingScreen;
    public GameObject optionScreenMenu;
    public GameObject pauseScreenMenu;
    public GameObject wonScreen;
    public GameObject lostScreen;

    [Header("Sliders")]
    public Slider healthBar;
    public Slider lightningStrike;

    [Header("Texts")]
    public Text lightningStrikeCoolDown;

    [Header("DropDowns")]
    public Dropdown resolutionOptions;

    [Header("BackGrounds")]
    public Image startScreenBackGround;
    public Image pauseScreenBackGround;

    [Header("HideInInspector")]
    public bool getAttackInput;

    private void Awake()
    {
        if (single == null)
        {
            single = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(single.gameObject);
            return;
        }

        if(startScreenBackGround == null)
        {
            Debug.Log("No Start BackGround Assigned");
            return;
        }

        if (pauseScreenBackGround == null)
        {
            Debug.Log("No PauseScreen BackGround Assigned");
            return;
        }

        if (startScreenMenu == null && startScreen)
        {
            Debug.LogWarning("No Start Menu Assigned");
            return;
        }

        if (loadingScreen == null)
        {
            Debug.LogWarning("No Loading Screen Assigned");
            return;
        }

        if (pauseScreenMenu == null)
        {
            Debug.LogWarning("No Pause Menu Assigned");
            return;
        }

        if (optionScreenMenu == null)
        {
            Debug.LogWarning("No Option Menu Assigned");
            return;
        }

        if (healthBar == null)
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

    public void StartGame()
    {
        startScreen = false;
        startScreenMenu.SetActive(false);
        GetNextRoom();
        loading = true;
        loadingScreen.SetActive(true);
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public  void GetNextRoom()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ResumeGame()
    {
        pauseScreenMenu.SetActive(false);
        optionScreenMenu.SetActive(false);
    }

    public void OptionScreen()
    {
        ResetResolutions();
        if (startScreen)
        {
            if (startScreenMenu.activeSelf)
            {
                startScreenMenu.SetActive(false);
                
            }
        }
        else
        {
            if (pauseScreenMenu.activeSelf)
            {
                pauseScreenMenu.SetActive(false);
            }
        }
        optionScreenMenu.SetActive(true);
    }

    public void ReturnMenu()
    {
        if (startScreen)
        {
            if (optionScreenMenu.activeSelf)
            {
                startScreenMenu.SetActive(true);
            }
        }
        else
        {
            if (optionScreenMenu.activeSelf)
            {
                pauseScreenMenu.SetActive(true);
            }
        }
        optionScreenMenu.SetActive(false);
    }

    public void QuitGame()
    {
        if (startScreenMenu.activeSelf)
        {
            Application.Quit();
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (startScreen || loading) 
        {
            hud.SetActive(false);
            if(lostScreen.activeSelf == false || wonScreen.activeSelf == false)
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    loading = false;
                    loadingScreen.SetActive(false);
                }
            }
            startScreenBackGround.gameObject.SetActive(true);
            pauseScreenBackGround.gameObject.SetActive(false);
            return;
        }
        else
        {
            hud.SetActive(true);
            startScreenBackGround.gameObject.SetActive(false);
        }
        if (SC_GameManager.single != null && SC_GameManager.single.gameStart)
        {
            if (Input.GetButtonDown("Cancel"))
            {
                if (!pauseScreenMenu.activeSelf)
                {
                    pauseScreenMenu.SetActive(true);
                }
                else
                {
                    ResumeGame();
                }
                optionScreenMenu.SetActive(false);
            }

            if (pauseScreenMenu.activeSelf || optionScreenMenu.activeSelf)
            {
                Time.timeScale = 0;
                getAttackInput = false;
                if (!Cursor.visible)
                {
                    Cursor.visible = true;
                }
                if (pauseScreenBackGround.gameObject.activeSelf == false)
                {
                    pauseScreenBackGround.gameObject.SetActive(true);
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
                if(pauseScreenBackGround.gameObject.activeSelf == true)
                {
                    pauseScreenBackGround.gameObject.SetActive(false);
                }
            }

            HealthBarUpdate();
            LightninStrikeCoolDownUpdate();
        
        }
    }

    //In Game Functions

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

    public void HealthBarUpdate()
    {
        healthBar.minValue = 0;
        healthBar.value = SC_TopDownController.single.curHealth;
        healthBar.maxValue = SC_TopDownController.single.maxHealth;
    }
   

    //For The Options

    Resolution[] resolutions;
    public void ResetResolutions()
    {
        resolutions = Screen.resolutions;
        resolutionOptions.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);
            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionOptions.AddOptions(options);
        resolutionOptions.value = currentResolutionIndex;
        resolutionOptions.RefreshShownValue();
    }

    public void ToggleFullScreen()
    {
        if (Screen.fullScreen)
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        }
    }
   
    public void GetGameWonScreen()
    {
        if (!Cursor.visible)
        {
            Cursor.visible = true;
        }
        endScreen.SetActive(true);
        wonScreen.SetActive(true);
    }

    public void GetGameLostScreen()
    {
        Debug.Log("Getting Lost Screen");
        if (!Cursor.visible)
        {
            Cursor.visible = true;
        }
        endScreen.SetActive(true);
        lostScreen.SetActive(true);
    }

}
