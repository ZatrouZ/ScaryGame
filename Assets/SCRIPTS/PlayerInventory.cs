using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private List<int> keys = new List<int>();  // List to store collected keys

    // Method to add a key to the inventory
    public void AddKey(int keyID)
    {
        if (!keys.Contains(keyID))
        {
            keys.Add(keyID);
            Debug.Log("Key with ID " + keyID + " added to inventory.");
        }
    }

    // Method to check if the player has a key
    public bool HasKey(int keyID)
    {
        return keys.Contains(keyID);
    }
}
