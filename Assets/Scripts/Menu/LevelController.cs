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
    // The screen for how to play the first level
    public GameObject HTPScreenL1;
    // The screen for how to play the second level
    public GameObject HTPScreenL2;
    // The audio source that plays the background music
    public AudioSource menuMusic;
    // The audio source that plays the button click sound
    public AudioSource buttonClick;
    // The sound that plays when the user clicks a button
    public AudioClip buttonSound;
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
        PlaySoundEffect(buttonSound);
        SceneManager.LoadScene("Level1");
    }

    public void StartLevel2()
    {
        PlaySoundEffect(buttonSound);
        SceneManager.LoadScene("Level2");
    }

    public void OpenLevels()
    {
        PlaySoundEffect(buttonSound);
        levelScreen.SetActive(true);
        menuMusic.volume *= soundReduction;
    }

    public void CloseLevels()
    {
        PlaySoundEffect(buttonSound);
        levelScreen.SetActive(false);
        menuMusic.volume = originalVolume;
    }

    public void OpenHTP()
    {
        PlaySoundEffect(buttonSound);
        HTPScreen.SetActive(true);
        menuMusic.volume *= soundReduction;

    }

    public void CloseHTP()
    {
        PlaySoundEffect(buttonSound);
        HTPScreen.SetActive(false);
        menuMusic.volume = originalVolume;
    }

    public void OpenHTPL1()
    {
        PlaySoundEffect(buttonSound);
        HTPScreenL1.SetActive(true);

    }

    public void CloseHTPL1()
    {
        PlaySoundEffect(buttonSound);
        HTPScreenL1.SetActive(false);
    }

    public void OpenHTPL2()
    {
        PlaySoundEffect(buttonSound);
        HTPScreenL2.SetActive(true);
    }

    public void CloseHTPL2()
    {
        PlaySoundEffect(buttonSound);
        HTPScreenL2.SetActive(false);
    }

     private void PlaySoundEffect(AudioClip clip)
    {
        buttonClick.clip = clip;
        buttonClick.Play();
    }

    


    
}
