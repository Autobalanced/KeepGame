using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Types of tiles, so far just empty or Floor tiles
public enum TileType { Empty, Floor };

public class Tile {

    // Default the TileType to empty
    private TileType _type = TileType.Empty;

    public TileType Type
    {
        get { return _type; }
        set
        {
            TileType oldType = _type;
            _type = value;

            // Call the callback to alert changes
            if (cbTileTypeChanged != null && oldType != _type)
            {
                cbTileTypeChanged(this);
            }
        }
    }

    // If there is an item on this tile.
    Item item;

    // If there is a building on this tile.
    public Building building
    {
        get; protected set;
    }

    // Get context for the game world
    public World world
    {
        get; protected set;
    }

    // This tiles x and y co ordinates
    public int X
    {
        get; protected set;
    }
    public int Y
    {
        get; protected set;
    }

    // Call this function whenever the tile type changes
    Action<Tile> cbTileTypeChanged;

    // Initialise the Tile
    public Tile ( World world, int x, int y )
    {
        this.world = world;
        this.X = x;
        this.Y = y;
    }

    // Register a call back for the tile type changing
    public void RegisterTileTypeChanged(Action<Tile> callback)
    {
        cbTileTypeChanged += callback;
    }

    // Unregister a call back for the tile type changing
    public void UnRegisterTileTypeChanged(Action<Tile> callback)
    {
        cbTileTypeChanged -= callback;
    }

    // Attempt to place a building on this tile
    public bool PlaceBuilding(Building objInstance)
    {
        if (objInstance == null )
        {
            // Remove what was here before
            building = null;
            return true;
        }

        // If there is an object that is valid to be placed
        if ( building != null )
        {
            Debug.LogError("Something already exists on this tile");
            return false;
        }

        // No errors:
        building = objInstance;
        return true;
    }
}
