using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class InventoryDisplay : MonoBehaviour
{

    public GameObject settingsCanvas;
    public GameObject[] tabObjects = new GameObject[5];

    public bool canChangeTabs = true;

    //public Image slotImage;

    public enum tab
    {
        equipped,
        weapons,
        shields,
        armor,
        misc
    }

    public tab currentTab = tab.equipped;

    public InventoryManager inventoryManager;

    // Start is called before the first frame update
    void Start()
    {
        inventoryManager = GetComponent<InventoryManager>();
        closeInventory();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void showInventory()
    {
        settingsCanvas.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        loadTab();
    }

    public void closeInventory()
    {
        settingsCanvas.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public List<GameObject> slotObjects;

    public void loadTab()
    {
        canChangeTabs = false;

        foreach (GameObject tab in tabObjects)
        {
            tab.SetActive(false);
        }

        tabObjects[(int)currentTab].SetActive(true);

        string tabName = currentTab.ToString();

        slotObjects = new List<GameObject>();
        
        int count = 1;
        do
        {
            slotObjects.Add(GameObject.Find(tabName + " Item " + count));
            count++;
        }
        while (GameObject.Find(tabName + " Item " + count) != null);

        int numOfItemsInTab = inventoryManager.inventory[tabName].Count;

        for (int i = 0; i < numOfItemsInTab; i++)
        {
            if (inventoryManager.inventory[tabName][i] == null)
            {
                clearImageSource(slotObjects[i].GetComponent<Image>());
                continue;
            }
            setImageByName(inventoryManager.inventory[tabName][i], slotObjects[i].GetComponent<Image>());
        }

        for (int i = numOfItemsInTab; i < slotObjects.Count; i++)
        {
            clearImageSource(slotObjects[i].GetComponent<Image>());
        }

        canChangeTabs = true;
    }

    public void changeTab(int newTab)
    {
        if (canChangeTabs)
        {
            currentTab = (tab)newTab;

            loadTab();
        }
    }

    public void setImageByName(string name, Image slotImage)
    {
        // Path to the folder containing the images
        string folderPath = Application.dataPath + "/_Prefabs/Inventory/Inventory Sprites/";

        // Check if the folder exists
        if (Directory.Exists(folderPath))
        {
            // Search for the image file with the specified name
            string[] files = Directory.GetFiles(folderPath, name + ".*");

            // Check if any files match the name
            if (files.Length > 0)
            {
                
                // Load the first matching image
                string imagePath = "file://" + files[0]; // Unity requires file:// protocol for local files
                StartCoroutine(loadImage(imagePath, slotImage));
            }
            else
            {
                Debug.LogError("Image with name '" + name + "' not found in folder: " + folderPath);
            }
        }
        else
        {
            Debug.Log("Directory does not exist");
        }
    }

    // Coroutine to load the image asynchronously
    public IEnumerator loadImage(string imagePath, Image slotImage)
    {
        slotImage.color = new Color(slotImage.color.r, slotImage.color.g, slotImage.color.b, 1f);

        using (WWW www = new WWW(imagePath))
        {
            yield return www;

            if (string.IsNullOrEmpty(www.error))
            {
                // Set the loaded texture as the source for the Image component
                Texture2D texture = www.texture;
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
                slotImage.sprite = sprite;
            }
            else
            {
                Debug.LogError("Failed to load image: " + www.error);
            }
        }
    }

    public void clearImageSource(Image slotImage)
    {
        slotImage.sprite = null;

        slotImage.color = new Color(slotImage.color.r, slotImage.color.g, slotImage.color.b, 0f);
    }
}