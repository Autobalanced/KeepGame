using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Hero : Character {

    // Parent class for all Hero units, a Hero cannot be a Hero class, must be a subset of Hero such as Fighter, Wizard, etc.

    int mana; //Not all heroes will need mana but may need it for Prestige hero classes or gained abilities

    // Extended statistics:
    // STR, DEX, CON, INT, WIS, CHA?
    // Melee Damage, Ranged Damage, Magic Damage, Melee Resist etc?

    // Personality statistics:
    // Loyalty (How easily will a Hero answer the players call)
    // Resolve (How easily will the Hero flee from a fight)
    // Happiness or Tiredness (How easily will the hero rest or use a entertainment building)
    // Inquisitiveness (How easily will a hero explore unexplored terrain, open unidentified items or try experimental spells)

    // Heroes may be able to carry more items, so extend or override here:
    // Dictionary<slot, item> equipment;
    // List<item> inventory;
    // int gold; Currency carried by Hero.

    // Hero mundane abilities:
    // Melee attack/damage
    // Accept Quest
    // Complete Quest

    // Hero special abilities:
    // Blank but to be extended by Heroes leveled class
}
