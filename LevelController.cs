using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class LevelController : MonoBehaviour
{
    public GameObject menuScreen;
    public GameObject HTPScreen;
    public AudioSource menuMusic;
    private float originalVolume;
    public float soundReduction;

    void Start()
    {
        originalVolume = menuMusic.volume;
    }
    public void StartLevel()
    {
        SceneManager.LoadScene("Level1");
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
