using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{

    public string itemType;

    public int attackPower;
    public int reachDistance;

    public bool isInInventory = false;
    public bool isBeingHeld;

    public Vector3 originalSize;

    public GameObject player;
    public GameObject playerRightArm;
    public GameObject playerLeftArm;

    public PlayerController playerController;

    public InventoryManager inventoryManager;

    // Start is called before the first frame update
    void Start()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        boxCollider.enabled = false;
        originalSize = transform.localScale;
        player = GameObject.Find("Player");
        playerRightArm = GameObject.Find("mixamorig:RightHandMiddle1");
        playerLeftArm = GameObject.Find("mixamorig:LeftHandMiddle1");
        playerController = player.GetComponent<PlayerController>();
        inventoryManager = GameObject.Find("Inventory Manager").GetComponent<InventoryManager>();
    }

    // Update is called once per frame
    public float distance = 0f;

    void Update()
    {
        distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance < 1f && Input.GetKeyDown(playerController.pickUpItem) && !isInInventory)
        {
            inventoryManager.addItem(itemType, transform.name);
            isInInventory = true;
            if (itemType == "weapons")
            {
                transform.SetParent(playerRightArm.transform);
                transform.localPosition = new Vector3(0, 0, 0);
                transform.localRotation = Quaternion.Euler(0, -90f, -90f);
            }
            else if (itemType == "shields")
            {
                transform.SetParent(playerLeftArm.transform);
                transform.localPosition = new Vector3(0, 0, 0);
                transform.localRotation = Quaternion.Euler(0, 180f, 180f);
            }
            transform.localScale = Vector3.zero;
        }
    }
}