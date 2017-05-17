using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World {

    // A two dimensional array that holds all our tile data.
    Tile[,] tiles;

    // A Dictionary that holds all Building types
    Dictionary<string, Building> buildingPrototypes;

    // The total width of the world in tiles.
    public int Width { get; protected set; }

    // The total height of the world in tiles.
    public int Height { get; protected set; }

    // Callback for when a building is placed;
    Action<Building> cbBuildingCreated;
    Action<Tile> cbTileChanged;

    // Queue of all available jobs (Replace with dedicated class for managing job queues of differing types)
    public Queue<Job> jobQueue;

    // Initialise the World (Currently set to 100x100 tiles
    public World (int width = 100, int height = 100)
    {
        Width = width;
        Height = height;

        jobQueue = new Queue<Job>();

        // Set the tile array to be as large as the game world size
        tiles = new Tile[Width, Height];

        // Fill the tile array with Tiles
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                tiles[x, y] = new Tile(this, x, y);
            }
        }

        // Report World completion to the Log
        Debug.Log("World create with " + Width + " x " + Height + " dimension.");

        // Initialise all building prototypes
        CreateBuildingPrototypes();
    }

    // Create a default building type
    void CreateBuildingPrototypes()
    {
        buildingPrototypes = new Dictionary<string, Building>();

        buildingPrototypes.Add("BuildingPlaceholder_0",
            Building.CreatePrototype(
                "BuildingPlaceholder_0",
                0, // Impassable to movement
                1, // Width of the building
                1, // Height of the building
                false // Links to neighbour objects
                )
         );
    }

    // Randomly generate tiles to test system
    public void RandomGeneration()
    {
        Debug.Log("Randomising game Tiles");
        for(int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++)
            {

                if (UnityEngine.Random.Range(0, 2) == 0)
                {
                    tiles[x, y].Type = TileType.Empty;
                }
                else
                {
                    tiles[x, y].Type = TileType.Floor;
                }

            }
        }
    }

    // Return the tile data queried
    public Tile GetTileAt(int x, int y)
    {
        if (x > Width || x < 0 || y > Height || y < 0)
        {
            Debug.LogError("Tile (" + x + "," + y + ") is out of range.");
            return null;
        }
        return tiles[x, y];
    }

    // Place a building
    public void PlaceBuilding(string objectType, Tile t)
    {
        if( buildingPrototypes.ContainsKey(objectType) == false)
        {
            Debug.LogError("buildingPrototypes doesn't contain a proto for key: " + objectType);
            return;
        }

        Building obj = Building.PlaceInstance(buildingPrototypes[objectType], t);

        if (obj == null)
        {
            // Failed to place building
            return;
        }

        if (cbBuildingCreated != null)
        {
            cbBuildingCreated(obj);
        }
    }

    public void RegisterBuildingCreated(Action<Building> callbackfunc)
    {
        cbBuildingCreated += callbackfunc;
    }

    public void UnregisterBuildingCreated(Action<Building> callbackfunc)
    {
        cbBuildingCreated -= callbackfunc;
    }

    public bool IsBuildingPlacementValid (string buildingType, Tile t)
    {
        return buildingPrototypes[buildingType].funcPositionValidation(t);

    }
}
