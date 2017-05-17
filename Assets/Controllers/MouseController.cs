using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour {

    public GameObject circleCursorPrefab;

    bool buildModeIsObjects = false;
    TileType buildModeTile = TileType.Empty;
    string buildModeObjectType;

    // Keep track of the mouse position last and current frame
    Vector3 lastFramePosition;
    Vector3 currentFramePosition;

    // Start position of a mouse drag
    Vector3 dragStartPosition;
    List<GameObject> dragPreviewGameObjects;

	// Use this for initialization
	void Start () {
        // A list of objects to show as a preview when dragging the mouse
        dragPreviewGameObjects = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
        // Get the mouse current position from the Camera
        currentFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Force mouse Z axis to 0
        currentFramePosition.z = 0;

        UpdateDragging();
        UpdateCameraMovement();

        // Update where the mouse position was this frame
        lastFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lastFramePosition.z = 0;
	}

    void UpdateDragging()
    {
        // Cancel out if over a UI element
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        // Start mouse drag
        if (Input.GetMouseButtonDown(0))
        {
            dragStartPosition = currentFramePosition;
        }

        // Calculate the area dragged
        int start_x = Mathf.FloorToInt(dragStartPosition.x);
        int end_x = Mathf.FloorToInt(currentFramePosition.x);
        int start_y = Mathf.FloorToInt(dragStartPosition.y);
        int end_y = Mathf.FloorToInt(currentFramePosition.y);

        // Catch in case of dragging backwards
        if (end_x < start_x)
        {
            int tmp = end_x;
            end_x = start_x;
            start_x = tmp;
        }
        if (end_y < start_y)
        {
            int tmp = end_y;
            end_y = start_y;
            start_y = tmp;
        }

        // Remove old drag previews
        while(dragPreviewGameObjects.Count > 0)
        {
            GameObject go = dragPreviewGameObjects[0];
            dragPreviewGameObjects.RemoveAt(0);
            SimplePool.Despawn(go);
        }

        if(Input.GetMouseButton(0))
        {
            //Display a preview of the dragged area
            for (int x = start_x; x <= end_x; x++)
                {
                for (int y = start_y; y <= end_y; y++)
                {
                    Tile t = WorldController.Instance.World.GetTileAt(x, y);
                    if (t != null)
                    {
                        // Dplay the building hint on top of this tile position
                        GameObject go = SimplePool.Spawn(circleCursorPrefab, new Vector3(x, y, 0), Quaternion.identity);
                        go.transform.SetParent(this.transform, true);
                        dragPreviewGameObjects.Add(go);
                    }
                }
            }
        }

        if(Input.GetMouseButtonUp(0))
        {
            // End the drag
            // Loop through all the tiles
            for (int x = start_x; x <= end_x; x++)
            {
                for (int y = start_y; y <= end_y; y++)
                {
                    Tile t = WorldController.Instance.World.GetTileAt(x, y);

                    if (t != null)
                    {
                        if (buildModeIsObjects == true)
                        {
                            // Create the Building and assign it to the tile
                            // FIXME: Right now, we're just going to assume walls.
                            // WorldController.Instance.World.PlaceBuilding(buildModeObjectType, t);
                            string buildingType = buildModeObjectType;
                            Job j = new Job(t, (theJob) =>
                            {
                                OnBuildingJobComplete(buildingType, theJob.tile);
                            });

                            WorldController.Instance.World.jobQueue.Enqueue( j );
                        }
                        else
                        {
                            // We are in tile-changing mode.
                            t.Type = buildModeTile;
                        }
                    }
                }
            }
        }
    }

    void UpdateCameraMovement()
    {
        // Handle screen panning
        if (Input.GetMouseButton(1) || Input.GetMouseButton(2))
        {   // Right or Middle Mouse Button

            Vector3 diff = lastFramePosition - currentFramePosition;
            Camera.main.transform.Translate(diff);

        }

        Camera.main.orthographicSize -= Camera.main.orthographicSize * Input.GetAxis("Mouse ScrollWheel");

        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 3f, 25f);
    }

    // Set the current build mode to building Floors
    public void SetMode_BuildFloor()
    {
        buildModeIsObjects = false;
        buildModeTile = TileType.Floor;
    }

    // Set the current TileType to Empty
    public void SetMode_Bulldoze()
    {
        buildModeIsObjects = false;
        buildModeTile = TileType.Empty;
    }

    // Place a building under mouse (Can't handle more than 1x1 buildings at the moment
    public void SetMode__BuildBuilding(string objectType)
    {
        buildModeIsObjects = true;
        buildModeObjectType = objectType;
    }

    void OnBuildingJobComplete(string buildingType, Tile t)
    {
        WorldController.Instance.World.PlaceBuilding(buildingType, t);
    }
}
