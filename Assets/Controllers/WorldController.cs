using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour {

    public static WorldController Instance { get; protected set; }

    // Set the Sprites for Tiles
    public Sprite floorSprite;
    public Sprite emptySprite;

    // Create a collection of all Tiles and Buildings and their related GameObjects
    Dictionary<Tile, GameObject> tileGameObjectMap;
    Dictionary<Building, GameObject> buildingGameObjectMap;

    // Dictionary of all Buildings and their associated Sprites
    Dictionary<string, Sprite> buildingSprites;

    // The full world generated
    public World world { get; protected set; }

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
