using UnityEngine;

// This script handles interaction with pickup items (Sword and Health)
public class ItemPickup : MonoBehaviour
{
    // Enum to easily select the type of item in the Inspector
    public enum ItemType { Sword, Health }
    public ItemType itemType;

    [Header("Health Settings (if itemType is Health)")]
    // Amount of health to restore/add
    public int healthAmount = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Try to get the player component from the object that touched the item
        PlayerMovementUnderwater_Combined_v3 player = other.GetComponent<PlayerMovementUnderwater_Combined_v3>();

        if (player != null)
        {
            if (itemType == ItemType.Sword)
            {
                // Call the player's function to mark that they have the sword
                player.PickUpSword();
                // Destroy the item object
                Destroy(gameObject);
            }
            else if (itemType == ItemType.Health)
            {
                // Call the player's function to increase health
                player.PickUpHealth(healthAmount);
                // Destroy the item object
                Destroy(gameObject);
            }
        }
    }
}