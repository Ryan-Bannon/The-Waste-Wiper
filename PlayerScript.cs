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
    //--------------------------------------------
    //SOUND EFFECTS ------------
    public AudioSource correctSE;
    public AudioSource incorrectSE;
    public AudioSource pickUpSE;
    public AudioSource footstepsSE;
    // ----------------------------
    private int throwOutCount;

    public GameObject buttons;



    //const string playerJump = "penguin_jump";

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
            if(moveInput != 0)
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

            selectedObject = touchingObject && Input.GetKeyDown(KeyCode.E);
            //Debug.Log("selectedObject: " + selectedObject + " holdingObject: " +holdingObject );

            if ( selectedObject && !holdingObject)
            {
                //Debug.Log("SELECTEDOBJECT/!HOLDINGOBJECT CONDITIONAL WORKS!");
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
            else if(selectedObject && holdingObject && (currentTrashCan != heldItemType) )
                UpdateScore(false);
            //Debug.Log("CurrentTrashCan: " + currentTrashCan + " heldItemType: " + heldItemType);
       
            if(incorrectTextTimer > 0 && incorrectText.text != "")
                incorrectTextTimer -= Time.deltaTime;
            else if(incorrectTextTimer <= 0)
            {
                incorrectText.text = "";
                incorrectTextTimer = incorrectTextTimerSetter;
                onScoreCooldown = false;
            }

            if(correctTextTimer > 0 && correctText.text != "")
                correctTextTimer -= Time.deltaTime;
            else if(correctTextTimer <= 0)
            {
                correctText.text = "";
                correctTextTimer = correctTextTimerSetter;   
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
        // Debug.Log(heldItem.ToString());
        heldItem.SetActive(false);
        holdingObject = false;
        heldItemType = "";
        currentItem = "";
        UpdateScore(true);
        correctSE.Play();
    }
    private void UpdateScore(bool rightCan)
    {
        Debug.Log("UPDATESCORE WORKS!");
        
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
        SceneManager.LoadScene("Menu");
    }
    public void Restart()
    {
        SceneManager.LoadScene("Level1");
    }
}
