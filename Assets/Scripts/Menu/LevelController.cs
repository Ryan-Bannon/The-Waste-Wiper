using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class LevelController : MonoBehaviour
{
    public GameObject menuScreen;
    // How to play screen
    public GameObject HTPScreen;
    public AudioSource menuMusic;
    private float originalVolume;
    // How much the music will be reduced by when the user opens a different screen
    public float soundReduction;
    // The screen that contains the different levels
    public GameObject levelScreen;

    void Start()
    {
        originalVolume = menuMusic.volume;
    }
    public void StartLevel1()
    {
        SceneManager.LoadScene("Level1");
    }

    public void StartLevel2()
    {
        SceneManager.LoadScene("Level2");
    }

    public void OpenLevels()
    {
        levelScreen.SetActive(true);
        menuMusic.volume *= soundReduction;
    }

    public void CloseLevels()
    {
        levelScreen.SetActive(false);
        menuMusic.volume = originalVolume;
    }

    public void OpenHTP()
    {
        HTPScreen.SetActive(true);
        menuMusic.volume *= soundReduction;

    }

    public void CloseHTP()
    {
        HTPScreen.SetActive(false);
        menuMusic.volume = originalVolume;
    }

    


    
}
