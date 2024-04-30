using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{

    public Dictionary<string, List<string>> inventory = new Dictionary<string, List<string>>();

    public InventoryDisplay inventoryDisplay;

    public float darkenAmount = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        inventoryDisplay = GetComponent<InventoryDisplay>();

        inventory.Add("equipped", new List<string>());
        inventory.Add("weapons", new List<string>());
        inventory.Add("shields", new List<string>());
        inventory.Add("armor", new List<string>());
        inventory.Add("misc", new List<string>());

        for (int i = 0; i < 6; i++)
        {
            inventory["equipped"].Add(null);
        }
    }

    // Update is called once per frame
    private Color originalColor; // Store the original color of the sprite

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        RaycastHit hit; 
        
        if (Physics.Raycast(ray, out hit))
        {
            GameObject hitObject = hit.collider.gameObject;

            SpriteRenderer spriteRenderer = hitObject.GetComponent<SpriteRenderer>();

            if (spriteRenderer != null)
            {
                // Check if the mouse is currently hovering over the object
                bool isMouseOver = hitObject == gameObject; // Assuming this script is attached to the same GameObject

                // If the mouse is hovering over the object, darken the sprite
                if (isMouseOver)
                {
                    // Store the original color if it hasn't been stored yet
                    if (originalColor == default(Color))
                    {
                        originalColor = spriteRenderer.color;
                    }

                    // Darken the color by reducing its RGB values
                    float darkenAmount = 0.5f; // Adjust this value to control the darkness
                    Color darkenedColor = new Color(
                        originalColor.r * darkenAmount,
                        originalColor.g * darkenAmount,
                        originalColor.b * darkenAmount,
                        originalColor.a
                    );

                    // Assign the darkened color to the sprite renderer
                    spriteRenderer.color = darkenedColor;
                }
                // If the mouse is not hovering over the object, reset the color
                else
                {
                    // Restore the original color
                    spriteRenderer.color = originalColor;
                    // Reset the originalColor variable
                    originalColor = default(Color);
                }   
            }
        }
    }


    public void addItem(string itemType, string itemName)
    {
        if (inventory.ContainsKey(itemType))
        {
            inventory[itemType].Add(itemName);
            Debug.Log(inventory[itemType][inventory[itemType].Count - 1]);
        }
        else
        {
            Debug.LogWarning("Item type '" + itemType + "' does not exist in the inventory.");
        }
    }

    public void unEquip(int index)
    {
        if (inventory["equipped"][index] == null)
        {
            return;
        }
        if (index == 0)
        {
            inventory["weapons"].Add(inventory["equipped"][0]);
        }
        if (index == 1)
        {
            inventory["shields"].Add(inventory["equipped"][1]);
        }

        if (GameObject.Find(inventory["equipped"][index]) != null)
        {
            GameObject.Find(inventory["equipped"][index]).transform.localScale = Vector3.zero;
        }

        inventory["equipped"][index] = null;

        inventoryDisplay.loadTab();
    }

    public string itemTypeToEquip;

    public void setEquipItemType(string itemType)
    {
        itemTypeToEquip = itemType;
    }

    public void equipItem(int index)
    {
        if (index >= inventory[itemTypeToEquip].Count)
        {
            return;
        }
        int indexToAddTo = 0;

        if (itemTypeToEquip == "weapons")
        {
            indexToAddTo = 0;
        }
        if (itemTypeToEquip == "shields")
        {
            indexToAddTo = 1;
        }
        if (itemTypeToEquip == "armor")
        {
            indexToAddTo = 2;
        }
        if (itemTypeToEquip == "armor")
        {
            indexToAddTo = 3;
        }
        if (itemTypeToEquip == "armor")
        {
            indexToAddTo = 4;
        }
        if (itemTypeToEquip == "armor")
        {
            indexToAddTo = 5;
        }

        if (GameObject.Find(inventory["equipped"][indexToAddTo]) != null)
        {
            GameObject.Find(inventory["equipped"][indexToAddTo]).transform.localScale = Vector3.zero;
        }

        string item = inventory[itemTypeToEquip][index]; // Retrieve the item to be equipped
        inventory["equipped"][indexToAddTo] = item; // Add the item to the "equipped" list

        GameObject.Find(inventory["equipped"][indexToAddTo]).transform.localScale = GameObject.Find(inventory["equipped"][indexToAddTo]).GetComponent<Item>().originalSize;

        if (itemTypeToEquip == "weapons")
        {
            inventory["weapons"].RemoveAt(index);
        }
        if (itemTypeToEquip == "shields")
        {
            inventory["shields"].RemoveAt(index);
        }

        inventoryDisplay.loadTab();
    }
}
