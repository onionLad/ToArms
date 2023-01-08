/*
 * CursorController.cs
 *
 * Purpose: Handles operations that deal with the game cursor.
 */

/* ======================================================================== *\
 *  IMPORTS                                                                 *
\* ======================================================================== */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

/* ======================================================================== *\
 * CLASS: CursorController                                                  *
 * Description: A single instance of these is needed for battle scenes. It  *
 *              gets attached to the cursor GameObject.                     *
\* ======================================================================== */
public class CursorController : MonoBehaviour
{
    /* ==================================================================== *\
     *  MEMBER VARIABLES                                                    *
    \* ==================================================================== */

    private GridController gridControl;
    private Vector3Int previousMousePos = new Vector3Int();

    /* ==================================================================== *\
     *                                                                      *
     *  CLASS METHODS                                                       *
     *                                                                      *
    \* ==================================================================== */

    /* On start, obtain the scene's GridController. */
    void Start()
    {
        gridControl = GameObject.Find("Grid").GetComponent<GridController>();
    }

    /*
     * Every frame, update the position of the cursor such that it follow's
     * the user's cursor.
     */
    void Update()
    {
        Vector3Int mousePos = gridControl.CursorGridCoords();

        /* If the mouse is moving and we aren't hovering over a void tile,
           we get the cursor's position on the grid corrected for offset
           (truePos) and set the position of the cursor sprite to that
           position. */
        if (!mousePos.Equals(previousMousePos) && gridControl.get_TileAtCursor() != "null") {

            /* Update position of game cursor. */
            Vector3 truePos = calc_cursorPos(mousePos);
            gameObject.transform.position = truePos;

            /* Update previous mouse position. */
            previousMousePos = mousePos;
        }
    }

    /*
     * For some reason, our grid is offset from the calculated positions, so
     * we need this function to obtain the true position of the user's cursor.
     */
    Vector3 calc_cursorPos(Vector3Int gridPos)
    {
        /* Translating grid coords to actual coords. */
        int offsetx = 1;
        int offsety = 1;
        int x_coord = gridPos.x + offsetx;
        int y_coord = gridPos.y + offsety;

        /*
         * This calculation converts the isometric grid position to the 
         * coordinate system transform.position uses.
         */
        Vector3 pos = new Vector3( (float)(x_coord * 0.5 - y_coord * 0.5), (float)(x_coord * 0.25 + y_coord * 0.25 - 0.5), 0 );
        return pos;
    }
}
