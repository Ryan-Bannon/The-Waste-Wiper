using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
    public float moveSpeed;
    //PLAYER COMPONENTS ------
    private Animator animator;
    private Rigidbody2D rb;
    // -----------------------

    //Stores the player's movement on the horizontal axis
    private float moveInput; 

    //Stores the animation the player is currnently doing
    private string currentAnimation;
    //Stores the player's original scale (this is used to flip the player when he changes directions)
    private Vector3 oScale; 

    //PLAYER ANIMATIONS ---------------------
    const string playerIdle = "penguin_idle";
    const string playerRun = "penguin_walk";
    // ------------------------------------

    //Checks whether the player is currently standing on an object (their colliders are touching)
    private bool touchingObject; 

    //Stores the tag of the current object
    private string currentItem; 

    //Holds the current colliding object's tag
    private string tag;

    //ITEMS -------------------------------------------------

    public GameObject cokeCanR;
    public GameObject heldCokeCanR;

    public GameObject cokeCanB;
    public GameObject heldCokeCanB;

    public GameObject cokeCanG;
    public GameObject heldCokeCanG;

    public GameObject newspaper;
    public GameObject heldNewspaper;

    public GameObject bottle;
    public GameObject heldBottle;

    public GameObject chipsY;
    public GameObject heldChipsY;

    public GameObject chipsB;
    public GameObject heldChipsB;

    //-------------------------------------------------

    //Checks whether a player is holding an object
    private bool holdingObject;

    //This stores if the player is currently standing on an object and has pressed E
    private bool selectedObject;

    //Will store the item the player is currently holding
    private GameObject heldItem;

    //These will determine whether the player has a recyclable, paper, or landfill item 
    private string holdingType;

    //This will determine what the type of the item you are holding is (recyclable, landifll, or paper)
    private string heldItemType;

    //Stores the tag of the current trash can's tag
    private string currentTrashCan;
   
    private int score; 
    //TEXTS ------------------------------------
    public TMP_Text scoreText; 

    public TMP_Text incorrectText;
    public float incorrectTextTimer; 
    private float incorrectTextTimerSetter;

    private bool onScoreCooldown;

    public TMP_Text correctText; 
    public float correctTextTimer;
    private float correctTextTimerSetter;

    public TMP_Text winText;

    public TMP_Text pauseText;
    //--------------------------------------------
    //SOUND EFFECTS ------------
    public AudioSource music;
    public AudioSource correctSE;
    public AudioSource incorrectSE;
    public AudioSource pickUpSE;
    public AudioSource footstepsSE;
    public AudioSource buttonSE;
    // ----------------------------
    private int throwOutCount;

    public GameObject buttons;
    public GameObject pauseButtons;

    private bool isPaused;
    // The background music before the game is pasued
    private float originalVolume;
    // How much the music will be reduced by when the user opens a different screen
    public float soundReduction;

    // Start is called before the first frame update
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        //Stores the original scale
        oScale = transform.localScale;
        incorrectTextTimerSetter = incorrectTextTimer;

        incorrectText.text = "";
        correctText.text = "";
        correctTextTimerSetter = correctTextTimer;
        winText.text = "";
        buttons.SetActive(false);
        originalVolume = music.volume;
         

    }

    // Update is called once per frame
    void Update()
    {
        if(throwOutCount < 7)
        {
            // Get the horizontal input (A/D or LeftArrow/RightArrow)
            moveInput = Input.GetAxis("Horizontal");

            // Move the character based on the input
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

            // Animate the character
            if(moveInput != 0 && !isPaused)
            {
                ChangeAnimationState(playerRun);
                if(!footstepsSE.isPlaying)
                    footstepsSE.Play();
            }
            else
            {
                ChangeAnimationState(playerIdle);
                footstepsSE.Stop();
            }
            
        
            // Flip the character based on the movement direction
            if (moveInput < 0)
            {
                transform.localScale = transform.localScale = new Vector3(-Mathf.Abs(oScale.x), oScale.y, oScale.z);
            }
            else if (moveInput > 0)
            {
                 transform.localScale = new Vector3(Mathf.Abs(oScale.x), oScale.y, oScale.z);
            }
            // if the player is over an object, and they press E, they will pick up an object
            selectedObject = touchingObject && Input.GetKeyDown(KeyCode.E);

            if ( selectedObject && !holdingObject)
            {
                PickUpObject();
            }
            // If the player is pressing e and standing on an object and holding an object and the trash can they 
            // are standing on is the same type as the one they are holding, it will throw it out
        
            else if (selectedObject && holdingObject && (currentTrashCan == heldItemType) )
            {
                Debug.Log("Conditional with throwOut works!");
                ThrowOut();
            }
            //If the player tries to put the item into a trash can that is not the same category, they will lose a point
            else if (selectedObject && holdingObject && (currentTrashCan != heldItemType) )
                UpdateScore(false);
       
            if (incorrectTextTimer > 0 && incorrectText.text != "")
                incorrectTextTimer -= Time.deltaTime;
            else if(incorrectTextTimer <= 0)
            {
                incorrectText.text = "";
                incorrectTextTimer = incorrectTextTimerSetter;
                onScoreCooldown = false;
            }

            if (correctTextTimer > 0 && correctText.text != "")
                correctTextTimer -= Time.deltaTime;
            else if (correctTextTimer <= 0)
            {
                correctText.text = "";
                correctTextTimer = correctTextTimerSetter;   
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if(!isPaused)
                    PauseGame();
                else
                    ResumeGame();
            }
        }
        else
        {
            winText.text = "YOU WIN! \nFinal Score: " + score;
            rb.velocity = new Vector3(0f, 0f, 0f);
            ChangeAnimationState(playerIdle);
            buttons.SetActive(true);
        }

            
        
    }
    //If the player is touching an object's box collider, it will show that they are touching an item
    void OnTriggerStay2D(Collider2D thing)
    {
        tag = thing.gameObject.tag;
        if (tag == "canR" || tag == "canG"|| tag == "canB" || tag == "newspaper" || tag == "chipsB"|| tag == "chipsY"|| tag == "bottle")
        {
            SetObject(true);
        } 
        else if(tag == "landfill" || tag == "recycle" || tag == "paper")
        {
            currentTrashCan = tag;
            touchingObject = true;
        }
    }
    //This will make it so it shows the player not touching an object once they leave the object's box collider
    void OnTriggerExit2D(Collider2D thing)
    {
        tag = thing.gameObject.tag;
        if (tag == "canR" || tag == "canG"|| tag == "canB" || tag == "newspaper" || tag == "chipsB"|| tag == "chipsY"|| tag == "bottle")
        {
            SetObject(false);
        }   
        else if (tag == "landfill" || tag == "recycle" || tag == "paper")
        {
            currentTrashCan = "";
            touchingObject = false;
        }
    }
    
    //Changes the player's animation state
    private void ChangeAnimationState(string animation)
    {
        if(currentAnimation == animation)
            return;      
        
        animator.Play(animation);

        currentAnimation = animation; 
    }

    //This will set the object the player is currently holding to false, and will then set the object he is supposed to be holding to true
    private void PickUpObject()
    {
        pickUpSE.Play();
        //Debug.Log("Current Item: " + currentItem);
        //Debug.Log("PICKUPWORKED!");

        if(currentItem == "canR")
        {
            cokeCanR.SetActive(false);
            heldCokeCanR.SetActive(true);
            heldItem = heldCokeCanR;
            heldItemType = "recycle";
            holdingObject = true;
        }  
        else if(currentItem == "canG")
        {
            cokeCanG.SetActive(false);
            heldCokeCanG.SetActive(true);
            heldItem = heldCokeCanG;
            heldItemType = "recycle";
            holdingObject = true;
        }  
        else if(currentItem == "canB")
        {
            cokeCanB.SetActive(false);
            heldCokeCanB.SetActive(true);
            heldItem = heldCokeCanB;
            heldItemType = "recycle";
            holdingObject = true;
        }  
        else if(currentItem == "chipsY")
        {
            Debug.Log("CHIPSYWORKED!");
            chipsY.SetActive(false);
            heldChipsY.SetActive(true);
            heldItem = heldChipsY;
            heldItemType = "landfill";
            holdingObject = true;
        }  
        else if(currentItem == "chipsB")
        {
            Debug.Log("CHIPSBWORKED!");
            chipsB.SetActive(false);
            heldChipsB.SetActive(true);
            heldItem = heldChipsB;
            heldItemType = "landfill";
            holdingObject = true;
        }  
        else if(currentItem == "bottle")
        {
            bottle.SetActive(false);
            heldBottle.SetActive(true);
            heldItem = heldBottle;
            heldItemType = "recycle";
            holdingObject = true;
        }
        else if (currentItem == "newspaper")
        {
            newspaper.SetActive(false);
            heldNewspaper.SetActive(true);
            heldItem = heldNewspaper;
            heldItemType = "paper";
            holdingObject = true;
        }
    }

    //Sets if the player is touching an object, and if he is, then the tag of that item gets stored in the currentItem tag
    private void SetObject(bool isTouching)
    {     
        touchingObject = isTouching;  
        
        if (touchingObject)
            currentItem = tag;
    }

    //Will make the current item the player is holding unactive. This will simulate throwing trash in a trash can.
    private void ThrowOut()
    {
        throwOutCount++;
        heldItem.SetActive(false);
        holdingObject = false;
        heldItemType = "";
        currentItem = "";
        UpdateScore(true);
        correctSE.Play();
    }
    private void UpdateScore(bool rightCan)
    {   
        if(rightCan)
        {
            scoreText.text = "Score: " + ++score;
            correctText.text = "<color=green>Correct!</color>";
            incorrectText.text = "";
            onScoreCooldown = false;
        }    
       
        else if(!onScoreCooldown && (currentTrashCan == "landfill" || currentTrashCan == "recycle" || currentTrashCan == "paper") )
        {
            incorrectSE.Play();
            scoreText.text = "Score: " + --score;
            incorrectText.text = "<color=red>Incorrect</color> bin, try again!";
            correctText.text = "";
            onScoreCooldown = true;
        }           
    }

    public void BackToMenu()
    {
        buttonSE.Play();
        // If the player goes back to the menu from the pause menu, we must unpause the game before going back to the menu
        if(isPaused)
            Time.timeScale = 1;
        SceneManager.LoadScene("Menu");
    }
    public void Restart()
    {
        StartCoroutine(RestartWithDelay());
    }

    public void PauseGame()
    {
        buttonSE.Play();
        music.volume *= soundReduction;
        Time.timeScale = 0;
        pauseText.gameObject.SetActive(true);
        pauseButtons.SetActive(true);
        isPaused = true;       
    }
    public void ResumeGame()
    {
        buttonSE.Play();
        Time.timeScale = 1;
        pauseText.gameObject.SetActive(false);
        pauseButtons.SetActive(false);
        isPaused = false;
        music.volume = originalVolume;
    }
    // This method makes it so we can hear the button click sound when the player restarts the game
    IEnumerator RestartWithDelay()
    {
        buttonSE.Play();
        // The game won't restart until the button sound effect is done playing
        yield return new WaitForSecondsRealtime(buttonSE.clip.length);
        if (isPaused)
            Time.timeScale = 1;
        SceneManager.LoadScene("Level1");
    }
}

    
