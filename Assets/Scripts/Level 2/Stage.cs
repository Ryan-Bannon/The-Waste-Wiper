using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;


public class Stage : MonoBehaviour
{
    public GameObject spawnPoint;
    public Vector3 spPosition;
    // Checks if this is the beginning stage the player spawns on
    private bool isBegStage;

    // Holds the boundaries entities can spawn within
    public GameObject[] boundaries;
    // The maximum y-value at which an entity can spawn
    private float topBound;
    // The maximum x-value at which an entity can spawn
    private float rightBound;
    // The minimum y-value at which an entity can spawn
    private float bottomBound;
    // The minimum x-value at which an entity can spawn
    private float leftBound;
    // Holds the stage 1 trash to pick up
    public GameObject[] trash;
    // Stores whether it's stage 1 or 2. This will cause the corresponding trash to be used.
    private bool isStage1;
    // Stores the shark enemy
    public GameObject shark;
    // Stores the rock obstacle
    public GameObject rock;
    [SerializeField]
    // How long an entity stays on screen before it despawns
    private float entityTime;
    [SerializeField]
    // Stores the minimum and maximum amount of trash/enemies/rocks that can be on screen
    public int minTrash, maxTrash, minEnemies, maxEnemies, minRocks, maxRocks;
    // Stores a reference to the player script
    private Player player;

    // Whether the game is paused
    public bool isPaused;

    // The text that shows when the player pauses the game
    public TMP_Text pauseText;

    // The button that will restart the level
    public Button retryButton;
    // The button that will take the player back to the menu
    public Button menuButton;
    

    
    void Awake()
    {
       
        // This can only be the beginning stage if it is stage 1, and this is the first time it is called
        if(gameObject.name == "Stage 1") {
            isStage1 = true;
            isBegStage = true;
        }
        player = GameObject.Find("Player").GetComponent<Player>();


    }

    // Update is called once per frame
    void Update ()
    {
        // If the player presses tab, the game will be paused
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!isPaused) 
            {
                PauseGame();
                Debug.Log("Game is paused");
            }
                
            else
            {
                ResumeGame();
                Debug.Log("Game is unpaused");
            }
               
        }
    }
    void OnEnable() 
    {
        // Gets the position of the spawn point
        spPosition = spawnPoint.transform.position;
        // If this is not the first stage, spawn in stage and entities
        if(!isBegStage) 
        {
            transform.position = spPosition;
            topBound = boundaries[0].gameObject.transform.position.y;
            rightBound = boundaries[1].gameObject.transform.position.x;
            bottomBound = boundaries[2].gameObject.transform.position.y;
            leftBound = boundaries[3].gameObject.transform.position.x;
            // This will make it so the game over screen shows an endless scroll with no entities spawning
            if(!player.isDead)
                SpawnEntities();
        }        
        else 
        {
            isBegStage = false; 
        }

            
    }
    // Spawns all of the entities for this stage
    private void SpawnEntities() 
    {
        Spawn("shark", minEnemies, maxEnemies);
        Spawn("rock", minRocks, maxRocks);
        Spawn("trash", minTrash, maxTrash);              
    }
   

    private void Spawn(string type, int min, int max)
    {
         // The number of enemies to spawn
        int num = Random.Range(min, max+1);
        // Stores whether the position of the enemy is valid
        bool validPos = false;
        // Will stop an infinite loop
        int maxAttempts = 50;
        int attempts = 0;
        // Stores where the entitiy will spawn
        Vector3 spawnPoint = Vector3.zero;
        // Checks whether the entity is hitting something
        Collider2D hitCollider = null;
        // Stores the shark/trash/rock
        GameObject entity = null;
        while(num > 0)
        {
            while (!validPos && attempts < maxAttempts)
            {
                float randX = Random.Range(leftBound, rightBound);
                float randY = Random.Range(bottomBound, topBound);
                spawnPoint = new Vector3(randX, randY, 0f);
                if (type == "shark")
                {
                    entity = shark;
                    hitCollider = Physics2D.OverlapCircle(spawnPoint, 2.5f);
                }
                else if (type == "trash")
                {
                    int randIndx = Random.Range(0, trash.Length);
                    entity = trash[randIndx];
                    Vector2 boxSize = entity.GetComponent<BoxCollider2D>().size;
                    hitCollider = Physics2D.OverlapBox(spawnPoint, boxSize, 0f);
                }
                else if (type == "rock")
                {
                    entity = rock;
                    Vector2 boxSize = entity.GetComponent<BoxCollider2D>().size;
                    hitCollider = Physics2D.OverlapBox(spawnPoint, boxSize, 0f);
                }

                if (hitCollider == null)
                    validPos = true;
                attempts++;
            }
            // If we cannot find a valid placement for the entity, we will stop trying to spawn in entities
            if(attempts >= maxAttempts)
                break;
            GameObject entityClone = Instantiate(entity, spawnPoint, Quaternion.identity);
            num--;
            Destroy(entityClone, entityTime);
            // This will make sure we check if the next entity we spawn in has a valid position
            validPos = false;
            // This will reset our attempts for the next spawn
            attempts = 0;       
        }      
    }
    private void SpawnRocks (int min, int max)
    {

    }

    public void SetStage() 
    {
        isStage1 = !isStage1;
    }

    public void ChangeTrashMinMax(int min, int max) 
    {
        minTrash = min;
        maxTrash = max;
    }

    public void ChangeEnemyMinMax(int min, int max) 
    {
        minEnemies = min;
        maxEnemies = max;
    }

    public void RestartLevel()
    {
        // If the player restarts the level from the pause menu, we must unpause the game
        if (isPaused)
            ResumeGame();
        SceneManager.LoadScene("Level2");
    }
    public void BackToMenu()
    {
        // If the player goes to the menu from the pause menu, we must unpause the game
        if (isPaused)
            ResumeGame();
        SceneManager.LoadScene("Menu");
    }

     // This happens when the player pauses the game
    private void PauseGame()
    {
        Time.timeScale = 0;
        retryButton.gameObject.SetActive(true);
        menuButton.gameObject.SetActive(true);
        isPaused = true;
        pauseText.gameObject.SetActive(true);
    }

    // This happens when the player resumes the game
    private void ResumeGame()
    {
        Time.timeScale = 1;
        retryButton.gameObject.SetActive(false);
        menuButton.gameObject.SetActive(false);
        isPaused = false;
        pauseText.gameObject.SetActive(false);
    }
}
