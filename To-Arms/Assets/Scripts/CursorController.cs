using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class CursorController : MonoBehaviour
{
    private Grid theGrid;
    private Vector3Int previousMousePos = new Vector3Int();
    private GridController gridControl;

    // Start is called before the first frame update
    void Start()
    {
        // theGrid refers to the Grid object within which all tilemaps reside.
        theGrid = GameObject.Find("Grid").GetComponent<Grid>();
        gridControl = GameObject.Find("Grid").GetComponent<GridController>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3Int mousePos = gridControl.GetGridPos();

        /* If the mouse is moving and we aren't hovering over a void tile,
           we get the cursor's position on the grid corrected for offset
           (truePos) and set the position of the cursor sprite to that
           position. */
        if (!mousePos.Equals(previousMousePos) && gridControl.get_TileAtCursor("battleMap") != "null") 
        {
            Vector3 truePos = calc_cursorPos(mousePos);
            gameObject.transform.position = truePos;
        }
    }

    /*
     * For some reason, our grid is offset from the calculated positions, so
     * we need this function to obtain the true position of the CURSOR.
     */
    Vector3 calc_cursorPos(Vector3Int gridPos)
    {
        // Find a way to automate offset.
        int offsetx = -1;
        int offsety = -1;

        // Translating grid coords to actual coords.
        int x_coord = gridPos.x - offsetx;
        int y_coord = gridPos.y - offsety;

        /* This calculation converts the isometric grid position to the 
           coordinate system transform.position uses. */
        Vector3 pos = new Vector3( (float)(x_coord * 0.5 - y_coord * 0.5), (float)(x_coord * 0.25 + y_coord * 0.25 - 0.5), 0 );
        return pos;
    }
}
