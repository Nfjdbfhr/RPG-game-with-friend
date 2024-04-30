using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float jumpForce = 1f;
    public float rollForce = 50f;
    public float health;
    public float maxHealth = 100;

    public bool isOnGround;
    public bool isRunning = true;
    public bool canMove = true;
    public bool isAttacking = false;

    public KeyCode forward = KeyCode.W;
    public KeyCode left = KeyCode.A;
    public KeyCode back = KeyCode.S;
    public KeyCode right = KeyCode.D;
    public KeyCode sprint = KeyCode.LeftControl;
    public KeyCode jump = KeyCode.Space;
    public KeyCode openInventory = KeyCode.X;
    public KeyCode pickUpItem = KeyCode.E;
    public KeyCode attack = KeyCode.Mouse0;
    public KeyCode rollKey = KeyCode.R;

    public InventoryDisplay invenDisplayer;
    public InventoryManager invenManager;
    public Animator playerAnim;

    public AnimationClip swordAnimation;

    public Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        invenDisplayer = GameObject.Find("Inventory Manager").GetComponent<InventoryDisplay>();
        invenManager = GameObject.Find("Inventory Manager").GetComponent<InventoryManager>();
        isOnGround = false;
        playerAnim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isRunning)
        {
            if (canMove)
            {
                checkForMovement();
            }

            if (Input.GetKeyDown(openInventory))
            {
                isRunning = false;
                playerAnim.SetBool("walking", false);
                invenDisplayer.showInventory();
            }

            if (Input.GetKeyDown(attack))
            {
                if (invenManager.inventory["equipped"][0] != null && isOnGround && !isAttacking)
                {
                    StartCoroutine(swordAttack());
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(openInventory))
            {
                isRunning = true;
                invenDisplayer.closeInventory();
            }
        }
    }

    public IEnumerator swordAttack()
    {
        playerAnim.SetTrigger("swordAttack");
        Debug.Log(invenManager.inventory["equipped"][0]);
        GameObject.Find(invenManager.inventory["equipped"][0]).GetComponent<BoxCollider>().enabled = true;
        isAttacking = true;

        yield return new WaitForSeconds(0.09f);

        checkForSwordHit();

        yield return new WaitForSeconds(1f);

        isAttacking = false;
        playerAnim.ResetTrigger("swordAttack");
        GameObject.Find(invenManager.inventory["equipped"][0]).GetComponent<BoxCollider>().enabled = false;
    }

    public void checkForSwordHit()
    {
        Vector3 rayOrigin = new Vector3(transform.position.x, transform.position.y + (transform.localScale.y / 2), transform.position.z);
        Vector3 rayDirection = transform.forward;
        float raycastDistance = GameObject.Find(invenManager.inventory["equipped"][0]).GetComponent<Item>().reachDistance;

        // Perform the raycast
        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, raycastDistance))
        {
            if (hit.collider.gameObject.tag == "bishopKnight")
            {
                Debug.Log("hit");
                BishopKnight enemyHit = hit.collider.gameObject.GetComponent<BishopKnight>();

                if (enemyHit.health > 0)
                {
                    enemyHit.subHealth(GameObject.Find(invenManager.inventory["equipped"][0]).GetComponent<Item>().attackPower);
                }
                else
                {
                    Debug.Log("Missed");
                }
            }
            else
            {
                Debug.Log("Missed");
            }
        }
        else
        {
            // If the raycast doesn't hit anything, you can handle that here
            Debug.Log("Missed");
        }
    }

    public void checkForMovement()
    {
        if (Input.GetKeyDown(rollKey))
        {
            StartCoroutine(roll());
            if (Input.GetKey(forward))
            {
                RotateObjectTowardsCamera(Vector3.forward);
            }
            if (Input.GetKey(left))
            {
                RotateObjectTowardsCamera(Vector3.left);
            }
            if (Input.GetKey(back))
            {
                RotateObjectTowardsCamera(Vector3.back);
            }
            if (Input.GetKey(right))
            {
                RotateObjectTowardsCamera(Vector3.right);
            }

            rb.AddForce(transform.forward * rollForce, ForceMode.Impulse);

            return;
        }

        if (playerAnim.GetCurrentAnimatorStateInfo(0).IsName("Sword Attack"))
        {
            return;
        }

        float horizontalInput = 0f;
        float verticalInput = 0f;

        // get the camera's forward direction
        Vector3 cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0f;
        cameraForward.Normalize();

        if (Input.GetKey(sprint))
        {   
            moveSpeed = 5f;
            playerAnim.SetBool("running", true);
        }
        else
        {
            moveSpeed = 3f;
            playerAnim.SetBool("running", false);
        }   

        
        Vector3 moveDirection = Vector3.zero;
        
        if (Input.GetKey(forward))
        {
            moveDirection += cameraForward;
        }
        if (Input.GetKey(back))
        {
            moveDirection -= cameraForward;
        }
        if (Input.GetKey(right))
        {
            moveDirection += Camera.main.transform.right;
        }
        if (Input.GetKey(left))
        {
            moveDirection -= Camera.main.transform.right;
        }

        // normalize the movement direction to keep speed on diagonals
        if (moveDirection.magnitude > 1f)
        {
            moveDirection.Normalize();
        }

        // move the player based off of movement input
        if (moveDirection != Vector3.zero)
        {
            // set players rotation to face the movement direction
            Quaternion newRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * 10f);

            // move the player in the calculated movement direction
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
        
            // Set walking animation
            playerAnim.SetBool("walking", true);
        }
        else
        {
            // play idle animation
            playerAnim.SetBool("walking", false);
        }

        // check for jump input
        if (Input.GetKeyDown(jump) && isOnGround)
        {
            isOnGround = false;
            playerAnim.SetTrigger("jump");
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }   
    }

    public void RotateObjectTowardsCamera(Vector3 direction)
    {
        // Get the camera's rotation
        Quaternion cameraRotation = Camera.main.transform.rotation;

        // Ignore rotation around the x and z axes (pitch and roll)
        cameraRotation.x = 0;
        cameraRotation.z = 0;

        // Rotate the direction vector according to the camera's rotation
        Vector3 newDirection = cameraRotation * direction;

        // Calculate the rotation angle based on the new direction
        float angle = Mathf.Atan2(newDirection.x, newDirection.z) * Mathf.Rad2Deg;

        // Set the object's rotation only on the y-axis
        transform.rotation = Quaternion.Euler(0, angle, 0);
    }

    public IEnumerator roll()
    {
        playerAnim.SetTrigger("roll");
        canMove = false;
        yield return new WaitForSeconds(1.01f);
        canMove = true;
    }

    public IEnumerator land()
    {
        playerAnim.SetTrigger("land");

        yield return new WaitForSeconds(0.34f);

        isOnGround = true;
        playerAnim.ResetTrigger("land");
    }

    public void subHealth(int toSub)
    {
        health -= toSub;
    }

    public void addHealth(int toAdd)
    {
        health += toAdd;
        Mathf.Clamp(health, 1f, maxHealth);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "ground")
        {
            StartCoroutine(land());
        }
    }
}
