using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// using Pathfinding

public abstract class Character : MonoBehaviour {

    // This is the base class for all Dynamic Units in the game

    // GameController requirements:
    // xPos
    // yPos
    // zPos

    // Base statistics for all Units
    int health; // private int with a TakeDamage() function?
    float movespeed;

    // Base personality statistics:
    int currentFear;
    int maxFear; // How much Fear a unit can take before attempting to flee to safety

    // Base item slots for all Units:

    // Enumerator for all slot types that a character may have
    public enum SlotType { Head, Weapon, Armour, Accessory1, Accessory2 };

    // Dictionary for all items the character has equipped such as weapons or armour.
    Dictionary<SlotType, Item> equipment = new Dictionary<SlotType, Item>()
    {
        { SlotType.Head, null }, // Each slot has a name and what item is in that slot
        { SlotType.Weapon, null },
        { SlotType.Armour, null },
        { SlotType.Accessory1, null },
        { SlotType.Accessory2, null }
    };

    // List for all items the character is carrying but has not equipped.
    List<Item> inventory = new List<Item>();

    int gold; // Currency carried by the Unit (Perhaps move to Inventory?)

    // Pathfinding

    // Base actions for all Units:
    // Pick up items
    // Equip items
    // Enter building
    // Exit building
}
