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
    public GameObject winScreen;
    public GameObject lostScreen;

    [Header("Sliders")]
    public Slider healthBar;
    public Slider loadingBar;
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
        if (single != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            single = this;
            DontDestroyOnLoad(this);
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

        if (loadingBar == null)
        {
            Debug.LogWarning("No Loading Bar Assigned");
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
        SetUpLoading();
    }

    private void SetUpLoading()
    {
        loading = true;
        loadingScreen.SetActive(true);
        loadingBar.minValue = 0;
        loadingBar.gameObject.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public  void GetNextRoom()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void OnLevelWasLoaded(int level)
    {
        if (!startScreen)
        {
            if(endScreen.activeSelf == true)
            {
                endScreen.SetActive(false);
                lostScreen.SetActive(false);
                winScreen.SetActive(false);
            }

            getAttackInput = false;
            SC_GameManager.single.tempStarterCam = FindObjectOfType<Camera>();

            SC_AttackManager.single.ResetManager();
            SC_GameManager.single.ResetManager();
            SC_RoomPooler.single.ResetManager();
            SC_RoomManager.single.ResetManager();
        }
    }

    public void ResumeGame()
    {
        pauseScreenMenu.SetActive(false);
        optionScreenMenu.SetActive(false);
    }

    public void OptionScreen()
    {
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
            startScreen = true;
            Time.timeScale = 1f;
            pauseScreenMenu.SetActive(false);
            startScreenMenu.SetActive(true);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        SC_AudioManager aMan = SC_AudioManager.single;
        if (startScreen || loading) 
        {
            if (startScreen && !aMan.IsPlayingMusic(MusicType.MainMenuTheme))
            {
                aMan.StopAllMusic();
                aMan.PlayMusic(MusicType.MainMenuTheme);
            }

            hud.SetActive(false);
            if(loading)
            {
                if (!aMan.IsPlayingMusic(MusicType.LoadingScreenAmbient))
                {
                    aMan.PlayMusic(MusicType.LoadingScreenAmbient);
                }
                SetLoadingBar();
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
            if (!aMan.IsPlayingMusic(MusicType.CombatTheme))
            {
                aMan.StopAllMusic();
                aMan.PlayMusic(MusicType.CombatTheme);
            }
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
                if (pauseScreenMenu.activeSelf)
                {
                    if (aMan.IsPlayingMusic(MusicType.CombatTheme) && !aMan.volumeHalfed)
                    {
                        aMan.HalfAllMusic();
                    }
                }
                else
                {
                    if (aMan.IsPlayingMusic(MusicType.CombatTheme) && aMan.volumeHalfed)
                    {
                        aMan.DoubleAllMusic();
                    }
                }


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
                if (aMan.IsPlayingMusic(MusicType.CombatTheme) && aMan.volumeHalfed)
                {
                    aMan.DoubleAllMusic();
                }


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
    private void SetLoadingBar()
    {
        if (SC_RoomPooler.single != null && SC_RoomManager.single != null)
        {
            loadingBar.value = SC_RoomManager.single.currentAmountOfRooms;
            loadingBar.maxValue = SC_RoomManager.single.maxAmountOfRooms;

            if (loadingBar.value >= loadingBar.maxValue)
            {
                loadingBar.gameObject.SetActive(false);
                if (Input.GetButtonDown("Fire1"))
                {
                    loading = false;
                    loadingScreen.SetActive(false);
                }
            }
            else if(loadingBar.gameObject.activeSelf == false)
            {
                loadingBar.gameObject.SetActive(true);
            }
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
    public void Start()
    {
        resolutions = Screen.resolutions;
        resolutionOptions.ClearOptions();

        List<string> options = new List<string>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);
        }

        resolutionOptions.AddOptions(options);
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void ToggleFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }
   
    public void GetGameWonScreen()
    {
        if (!Cursor.visible)
        {
            Cursor.visible = true;
        }
        if (!SC_AudioManager.single.IsPlayingMusic(MusicType.VictoryScreenTheme))
        {
            SC_AudioManager.single.StopAllMusic();
            SC_AudioManager.single.PlayMusic(MusicType.VictoryScreenTheme);
        }
        endScreen.SetActive(true);
        winScreen.SetActive(true);
    }

    public void GetGameLostScreen()
    {
        Debug.Log("Getting Lost Screen");
        if (!Cursor.visible)
        {
            Cursor.visible = true;
        }
        if (!SC_AudioManager.single.IsPlayingMusic(MusicType.GameOverScreenTheme))
        {
            SC_AudioManager.single.StopAllMusic();
            SC_AudioManager.single.PlayMusic(MusicType.GameOverScreenTheme);
        }
        endScreen.SetActive(true);
        lostScreen.SetActive(true);
    }

}
