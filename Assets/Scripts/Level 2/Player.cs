using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;




[System.Serializable]
// Custom UnityEvent for int parameters
public class IntEvent : UnityEvent<int, int> { } 

public class Player : MonoBehaviour
{
    // Holds the player's rigidbody
    private Rigidbody2D rb;  
    
    // Holds the player's spriteRenderer
    private SpriteRenderer sr;
    
    // How fast the player moves vertically
    public float vertSpeed;
    
    // How fast the player moves horizontally
    public float horizSpeed;
    
    // Stores the player's movement on the horizontal axis
    private float horizMoveInput; 
    
    // Stores the player's movement on the horizontal axis
    private float moveInputX; 
    
    // Stores the player's movement on the vertical axis
    private float moveInputY;
    
    // Stores the player's score
    public int score = 0;
    
    // Stores the text for the score
    public TMP_Text scoreText;
    
    // Stores the empty game object that the trash will go when the player collects it
    public GameObject trashSpwn;
    
    // Stores the coordinates for where the trash will spawn after the player collects it
    private float trashSpwnX, trashSpwnY;
    
    // Stores the number of hits the player can take before dying
    public int health;
    // The heart icon that will represent the player's health
    public GameObject heartPrefab; 
    // The container that will hold all the hearts
    public Transform heartContainer;
    // Stores heart icons
    private List<GameObject> hearts = new List<GameObject>(); 
    
    [SerializeField]
    // Stores how long the player will blink red when they take damage
    private float blinkTime;
    
    [SerializeField]
    // Stores the number of times the player will blink red when they take damage
    private float numOfBlinks;

    // Whether the player is in a grace period after taking damage
    private bool gracePeriod = false;

    // TEXTS ------------------------------------------------------------------------------------------------------  
    
    // The text that shows when the player loses
    public TMP_Text losingText;
    // The game object that holds the losing text
    private GameObject losingTextGO;
    // ------------------------------------------------------------------------------------------------------------
    
    // Whether the player is dead
    public bool isDead = false;
    [HideInInspector]
    // Event that increases scroll speed when player collects an item
    public UnityEvent increaseScrollSpeed;
    [HideInInspector]
    // Event that lets the stage script know when to change the min/max for the number of trash to spawn
    public IntEvent changeTrshMinMax;
    
    [HideInInspector]
    // Event that lets the stage script know when to change the min/max for the number of enemies to spawn
    public IntEvent changeEnmyMinMax;
    // Event that updates the health text when the player takes damage
    // public UnityEvent healthTextUpdate;
    // How much the scroll speed and player speed will increase by when the score reaches a checkpoint
    public float percentIncrease;
    // Will store the minimum/maximum number of trash/enemies to spawn
    public int trashMin, trashMax, enemyMin, enemyMax;
    // Whether the player has reached a number of points that will increase the speed and number of entities
    private bool isCheckpoint;

    // MIN/MAX TRASH SPAWN (the # indicates the score before the min/max changes)
    // public int minTrash5, maxTrash5, minTrash10, maxTrash10, minTrash15, maxTrash15, minTrash20, maxTrash20, minTrash25, maxTrash25, minTrash30, maxTrash30;
    
    // The min/maxes for how much trash/enemies/rocks spawn after each checkpoint. Ecah index responds to a checkpoint, I.E. trashMinBounds[0] and trashMaxBounds[0] is the min/max trash to spawn after the first checkpoint
    public int[] trashMinArr;
    public int[] trashMaxArr;
    public int[] enemyMinArr;
    public int[] enemyMaxArr;

    // MIN/MAX ENEMY SPAWN (the # indicates the score before the min/max changes)
    // public int minEnemies5, maxEnemies5, minEnemies10, maxEnemies10, minEnemies15, maxEnemies15, minEnemies20, maxEnemies20, minEnemies25, maxEnemies25, minEnemies30, maxEnemies30;
    // AUDIO ------------------------------------------------------------------------------------------------------
    // The sound that plays when the player hits a shark
    public AudioClip sharkBiteSound;
    // The sound that plays when the player collects trash
    public AudioClip trashCollectSound;
    // The sound that plays when the player jumps
    public AudioClip jumpSound;
    // The sound that plays when the player flaps their wings in their jump animation
    public AudioClip flapSound;
    // The sound that playws when the player hits a rock
    public AudioClip rockHitSound;
    // ------------------------------------------------------------------------------------------------------------

    // The button that will restart the level
    public Button retryButton;
    // The button that will take the player back to the menu
    public Button menuButton;

    private Animator animator;

    private bool isGrounded = true;

    // Stores the layer the player begins on
    int originalLayer;


    

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        // originalColor = sr.color;


        // Set the coordinates of where the trash will spawn after the player collects it
        trashSpwnX = trashSpwn.transform.position.x;
        trashSpwnY = trashSpwn.transform.position.y;
        losingTextGO = losingText.gameObject;
        losingTextGO.SetActive(false);

        // healthText.text = "Health: " + health;

        
        CameraScript2 cameraScript = GameObject.Find("Main Camera").GetComponent<CameraScript2>();
        percentIncrease = cameraScript.percentIncrease/2;
        increaseScrollSpeed.AddListener(cameraScript.IncreaseScrollSpeed);

        Stage stage = GameObject.Find("Stage 1").GetComponent<Stage>();
        // The stage will change the min/max for the number of trash to spawn when invoked
        changeTrshMinMax.AddListener(stage.ChangeTrashMinMax);
        // The stage will change the min/max for the number of enemies to spawn when invoked
        changeEnmyMinMax.AddListener(stage.ChangeEnemyMinMax);
        // This lets the player know the min/max set in the inspector for the stage
        trashMin = stage.minTrash;
        trashMax = stage.maxTrash;
        enemyMin = stage.minEnemies;
        enemyMax = stage.maxEnemies;

        // Creates the initial hearts based on the player's health
        UpdateHearts();

        animator = GetComponent<Animator>();

        originalLayer = gameObject.layer;

       


    }

    // Update is called once per frame
    void Update()
    {
        if(!isDead) 
        {
             // Get the horizontal input (A/D or LeftArrow/RightArrow)
            moveInputX = Input.GetAxisRaw("Horizontal");
            // Get the vertical input (W/S or UpArrow/DownArrow)
            moveInputY = Input.GetAxisRaw("Vertical");
            // Move the character based on the input
            rb.velocity = new Vector2(moveInputX * horizSpeed, moveInputY * vertSpeed);
            
            // This will cause the bounds and speed to change every increment of 5 up to 30
            if (score == 5 && !isCheckpoint) 
                Checkpoint(trashMinArr[0], trashMaxArr[0], enemyMinArr[0], enemyMaxArr[0]);
            else if (score == 10 && !isCheckpoint)
                Checkpoint(trashMinArr[1], trashMaxArr[1], enemyMinArr[1], enemyMaxArr[1]);
            else if (score == 15 && !isCheckpoint)
                Checkpoint(trashMinArr[2], trashMaxArr[2], enemyMinArr[2], enemyMaxArr[2]);
            else if (score == 20 && !isCheckpoint)
               Checkpoint(trashMinArr[3], trashMaxArr[3], enemyMinArr[3], enemyMaxArr[3]);
            else if (score == 25 && !isCheckpoint) 
                Checkpoint(trashMinArr[4], trashMaxArr[4], enemyMinArr[4], enemyMaxArr[4]);
            else if (score == 30 && !isCheckpoint)
                Checkpoint(trashMinArr[5], trashMaxArr[5], enemyMinArr[5], enemyMaxArr[5]);    
            
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                Jump();
            }
        }
       
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        GameObject entity = other.gameObject;
        string tag = entity.tag;
        // This happens when the player collects trash
        if (tag == "trash")
        {
            PlaySoundEffect(trashCollectSound);
            entity.transform.position = new Vector2(trashSpwnX, trashSpwnY);
            score++;
            scoreText.text = "Score: " + score;
            // When the player gets a point while in a checkpoint, they must be out of a checkpoint
            if (isCheckpoint)
                isCheckpoint = false;
            
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        GameObject entity = other.gameObject;
        string tag = entity.tag;
        // This happens when the player collides with an enemy
        if (tag == "enemy" && !gracePeriod)
        {
            PlaySoundEffect(sharkBiteSound);
            TakeDamage();
        }
        else if (tag == "rock" && !gracePeriod)
        {
            PlaySoundEffect(rockHitSound);
            TakeDamage();
        }
            
    }

    public void TakeDamage() 
    {
        if(!isGrounded)
        {
            Land();
        } 
        StartCoroutine(Blink()); 
        UpdateHearts();       
         if (health == 0)
                EndGame();
        Debug.Log("Health: " + health);
        
    }
    public void EndGame()
    {
        /// Shows Game over text
        losingTextGO.SetActive(true);
        isDead = true;
        Destroy(gameObject);
        retryButton.gameObject.SetActive(true);
        menuButton.gameObject.SetActive(true);
    }
    // Makes the player blink/become invincible/phase when they take damage
    IEnumerator Blink()
    {
        gracePeriod = true;  

        gameObject.layer = LayerMask.NameToLayer("Invincibility");
        // This way, the player can only take damage after they're done blinking
        health--;
        for (int i = 0; i < numOfBlinks; i++)
        {
            // Make semi-transparent
            sr.color = new Color(1f, 1f, 1f, 0.5f); 
            yield return new WaitForSeconds(blinkTime);
            
            // Fully opaque again
            sr.color = new Color(1f, 1f, 1f, 1f); 
            yield return new WaitForSeconds(blinkTime);
        }
        gracePeriod = false;
        gameObject.layer = originalLayer;
    }

    
    // This happens when the player reaches a checkpoint (increases speed, cahnges min/max of entities)
    private void Checkpoint(int minTrash, int maxTrash, int minEnemies, int maxEnemies)
    {
        increaseSpeed();
        changeTrshMinMax.Invoke(minTrash, maxTrash);
        changeEnmyMinMax.Invoke(minEnemies, maxEnemies);
        isCheckpoint = true;
    }

    private void increaseSpeed()
    {
        increaseScrollSpeed.Invoke();
        vertSpeed++;
        horizSpeed += 0.5f;
    }
    private void PlaySoundEffect(AudioClip clip)
    {
         // Create a temporary GameObject for the sound
        GameObject tempGO = new GameObject("TempAudio");
        // Create temporary audio source
        AudioSource audioSource = tempGO.AddComponent<AudioSource>(); 
        // Assign the audio clip
        audioSource.clip = clip;
        // Play the audio clip
        audioSource.Play();

        // Automatically destroy the GameObject after the sound finishes
        Destroy(tempGO, clip.length);
    }

    private void UpdateHearts()
    {
        // Clear all previous hearts
        foreach (GameObject heart in hearts)
        {
            // Destroy old heart GameObjects
            Destroy(heart); 
        }
        hearts.Clear();

        // Create new hearts based on health
        for (int i = 0; i < health; i++)
        {
            // Add heart to container
            GameObject heart = Instantiate(heartPrefab, heartContainer); 
            // Store reference for cleanup later
            hearts.Add(heart); 
        }
    }

    private void Jump()
    {
        // Play jump animation
        animator.Play("Player_Jump");
        // Set layer to interact with sharks and borders - not trash or rocks
        gameObject.layer = LayerMask.NameToLayer("Airborne");
        // Play jump sound      
        PlaySoundEffect(jumpSound);
        // While wings flap, play wing flapping sound
        isGrounded = false;

    }
    // This happens when the player lands after a jump
    private void Land()
    {
        // Play splash sound
        // Switch back to original layer
        gameObject.layer = originalLayer;
        // Switch back to swimming animation
        animator.Play("Player_Swim");
        isGrounded = true;
    }

   
    

}
