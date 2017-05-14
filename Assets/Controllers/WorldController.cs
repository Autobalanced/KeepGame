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
    public World World { get; protected set; }

	// Use this for initialization
	void Start () {

        LoadSprites();

        if (Instance != null)
        {
            Debug.LogError("There should never be two world controlelrs");
        }
        Instance = this;

        // Create a world with Empty tiles
        World = new World();

        World.RegisterBuildingCreated(OnBuildingCreated);

        // Instantiate the Dictionaries for tracking tiless and GameObjects
        tileGameObjectMap = new Dictionary<Tile, GameObject>();
        buildingGameObjectMap = new Dictionary<Building, GameObject>();

        // Create a GameObject for each of our tiles so they show visually.
        for (int x = 0; x < World.Width; x++)
        {
            for (int y = 0; y < World.Height; y++)
            {
                // Get the tile data
                Tile tile_data = World.GetTileAt(x, y);

                // Create a new GameObject for this tile;
                GameObject tile_go = new GameObject();

                // Add the tile and GameObject to the Dictionary
                tileGameObjectMap.Add(tile_data, tile_go);

                // Give the GameObject the name of the Tile co ords
                tile_go.name = "Tile_" + x + "_" + y;
                // Place the GameObject at the Tile co ords
                tile_go.transform.position = new Vector3(tile_data.X, tile_data.Y, 0);
                tile_go.transform.SetParent(this.transform, true);

                // Add a Sprite Renderer to the tile
                tile_go.AddComponent<SpriteRenderer>().sprite = emptySprite;

                // Register a callback so whenver the tile is updated the GameObject reflects changes
                tile_data.RegisterTileTypeChangedCallBack(OnTileTypeChanged);
            }
        }

        // Center the camera
        Camera.main.transform.position =  new Vector3(World.Width / 2, World.Height / 2, Camera.main.transform.position.z);
	}

    void LoadSprites()
    {
        buildingSprites = new Dictionary<string, Sprite>();
        Sprite[] sprites = Resources.LoadAll<Sprite>("Images/Buildings/");

        Debug.Log("LOADED RESOURCE:");
        foreach (Sprite s in sprites)
        {
            Debug.Log(s);
            buildingSprites[s.name] = s;
        }
    }

    // Update is called once per frame
    void Update () {
		
	}

    // Callback for whenever a tile changes type
    void OnTileTypeChanged( Tile tile_data)
    {
        if (tileGameObjectMap.ContainsKey(tile_data) == false)
        {
            Debug.LogError("tileGameObjectMap doesn't contain the tile_data -- did you forget to add the tile to the dictionary? Or maybe forget to unregister a callback?");
            return;
        }

        GameObject tile_go = tileGameObjectMap[tile_data];

        if (tile_go == null)
        {
            Debug.LogError("tileGameObjectMap's returned GameObject is null -- did you forget to add the tile to the dictionary? Or maybe forget to unregister a callback?");
            return;
        }

        if (tile_data.Type == TileType.Floor)
        {
            tile_go.GetComponent<SpriteRenderer>().sprite = floorSprite;
        }
        else if (tile_data.Type == TileType.Empty)
        {
            tile_go.GetComponent<SpriteRenderer>().sprite = null;
        }
        else
        {
            Debug.LogError("OnTileTypeChanged - Unrecognized tile type.");
        }
    }

    // Get the Tile at these co ordinates
    public Tile GetTileAtWorldCoord(Vector3 coord)
    {
        int x = Mathf.FloorToInt(coord.x);
        int y = Mathf.FloorToInt(coord.y);

        return World.GetTileAt(x, y);
    }

    // Create a gameobject and Sprite for a building when created
    public void OnBuildingCreated( Building building)
    {
        GameObject building_go = new GameObject();

        // Add this building to the Dictionary for the WorldController
        buildingGameObjectMap.Add(building, building_go);

        // Name the building by it's tile co ordinate and type
        building_go.name = building.objectType + "_" + building.tile.X + "_" + building.tile.Y;
        // Place the builing at the Tile's location
        building_go.transform.position = new Vector3(building.tile.X, building.tile.Y, 0);
        building_go.transform.SetParent(this.transform, true);

        // Add a sprite renderer to this GameObject
        building_go.AddComponent<SpriteRenderer>().sprite = GetSpriteForBuilding(building);

        // Register callback for whenever this GameObject is updated
        building.RegisterOnChangedCallback(OnBuildingCreated);
    }

    public void OnBuildingChanged( Building building)
    {
        // Make sure the buildings graphics are correct.
        if(buildingGameObjectMap.ContainsKey(building) == false)
        {
            Debug.LogError("OnBuildingChanged -- trying to change visuals but buidling is not in the map");
            return;
        }

        GameObject building_go = buildingGameObjectMap[building];

        building_go.GetComponent<SpriteRenderer>().sprite = GetSpriteForBuilding(building);
    }

    Sprite GetSpriteForBuilding(Building obj)
    {
        // To be expanded if Sprite change if their have Neighbours
        return buildingSprites[obj.objectType];
    }

}
