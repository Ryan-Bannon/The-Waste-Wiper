using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Trigger : MonoBehaviour
{
    // The location where the stage will be spawned (the spawn point)
    public GameObject stageSP;

    // Create a bool that keeps track of what stage you are on. 
    // When you are on stage 1, you will spawn stage 2, when you are on stage 2, you will spawn stage 1
    public bool isStage1;
    // Stages
    public GameObject stage1;
    public GameObject stage2;
    // Event that will be called when the stage changes
    public UnityEvent stageChange;
    // Checks if this is the first stage the player is on
    private bool isFirstStage = true;


    // Start is called before the first frame update
    void Start()
    {
        // The player starts on stage 1
        isStage1 = true;
        // This is so the stage script knows which stage it is on
        stageChange.AddListener(GameObject.Find("Stage 1").GetComponent<Stage>().SetStage);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D (Collider2D benchmark) {
        string tag = benchmark.gameObject.tag;
        if (tag == "benchmark")
            SpawnStage();
    }

    private void SpawnStage() {
        if (isStage1)
        {
            stage2.SetActive(false);
            stage2.SetActive(true);
        }
        else
        {
            stage1.SetActive(false);
            stage1.SetActive(true);
        }
        isStage1 = !isStage1;
        // We only want to call the state change if this is not the first spawning of the stage
        if (!isFirstStage)
            stageChange.Invoke();
        else
            isFirstStage = false;
    }
}
