using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building {

	// Register the base tile for this building
    public Tile tile
    {
        get; protected set;
    }

    // ObjectType returns what type of object this is for visual sprite allocation
    public string objectType
    {
        get; protected set;
    }

    // Movement cost for walking through/on this building (0 = impassable)
    float movementCost;

    // Dimensions for this building
    int width;
    int height;

    // Can this building be connected to another
    public bool linksToNeighbour
    {
        get; protected set;
    }

    // Callback if this building is changed in anyway
    Action<Building> cbOnChanged;

    // Check if this building is in a valid position
    public Func<Tile, bool> funcPositionValidation;


    // Restrict instantiation from outside the Building class, now Buildings can only be created from Building.PlaceInstance
    protected Building()
    {

    }

    static public Building CreatePrototype( string objectType, float movementCost = 1f, int width = 1, int height = 1, bool linksToNeighbour = false)
    {
        Building obj = new Building();

        obj.objectType = objectType;
        obj.movementCost = movementCost;
        obj.width = width;
        obj.height = height;
        obj.linksToNeighbour = linksToNeighbour;

        obj.funcPositionValidation = obj.IsValidPosition;

        return obj;
    }

    static public Building PlaceInstance ( Building proto, Tile tile)
    {
        // Check if position is valid
        if ( proto.funcPositionValidation(tile) == false)
        {
            Debug.LogError("There was an error placing this building");
            return null;
        }

        // If position valid
        Building obj = new Building();

        obj.objectType = proto.objectType;
        obj.movementCost = proto.movementCost;
        obj.width = proto.width;
        obj.height = proto.height;
        obj.linksToNeighbour = proto.linksToNeighbour;

        obj.tile = tile;

        // FIXME: This assumes we are 1x1!
        if (tile.PlaceBuilding(obj) == false)
        {
            // For some reason, we weren't able to place our object in this tile.
            // (Probably it was already occupied.)

            // Do NOT return our newly instantiated object.
            // (It will be garbage collected.)
            return null;
        }

        if (obj.linksToNeighbour)
        {
            // This type of furniture links itself to its neighbours,
            // so we should inform our neighbours that they have a new
            // buddy.  Just trigger their OnChangedCallback.

            Tile t;
            int x = tile.X;
            int y = tile.Y;

            t = tile.world.GetTileAt(x, y + 1);
            if (t != null && t.building != null && t.building.objectType == obj.objectType)
            {
                // We have a Northern Neighbour with the same object type as us, so
                // tell it that it has changed by firing is callback.
                t.building.cbOnChanged(t.building);
            }
            t = tile.world.GetTileAt(x + 1, y);
            if (t != null && t.building != null && t.building.objectType == obj.objectType)
            {
                t.building.cbOnChanged(t.building);
            }
            t = tile.world.GetTileAt(x, y - 1);
            if (t != null && t.building != null && t.building.objectType == obj.objectType)
            {
                t.building.cbOnChanged(t.building);
            }
            t = tile.world.GetTileAt(x - 1, y);
            if (t != null && t.building != null && t.building.objectType == obj.objectType)
            {
                t.building.cbOnChanged(t.building);
            }
        }

        return obj;
    }

    public void RegisterOnChangedCallback(Action<Building> callbackFunc)
    {
        cbOnChanged += callbackFunc;
    }

    public void UnregisterOnChangedCallback(Action<Building> callbackFunc)
    {
        cbOnChanged -= callbackFunc;
    }

    public bool IsValidPosition(Tile t)
    {
        // Make sure tile is FLOOR
        if (t.Type != TileType.Floor)
        {
            return false;
        }

        // Make sure tile doesn't already have a building
        if (t.building != null)
        {
            return false;
        }

        return true;
    }

    // Unneccassary for now, only for custom doors, can be re used for gates
    public bool IsValidPosition_Door(Tile t)
    {
        if (IsValidPosition(t) == false)
            return false;

        // Make sure we have a pair of E/W walls or N/S walls
        return true;
    }
}
