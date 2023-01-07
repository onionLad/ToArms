/*
 * UnitController.cs
 *
 * Purpose: Handles operations that deal with a single unit. These include
 *          movement operations and combat functions.
 */

/* ======================================================================== *\
 *  IMPORTS                                                                 *
\* ======================================================================== */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

/* ======================================================================== *\
 * CLASS: UnitController                                                    *
 * Description: An instance of this class is needed for each unit in each   *
 *              battle scene. It gets attached to a unit GameObject.        *
\* ======================================================================== */
public class UnitController : MonoBehaviour
{
    /* ==================================================================== *\
     *  MEMBER VARIABLES                                                    *
    \* ==================================================================== */

    /* Objects. */
    private Grid theGrid;
    private GridController gridControl;

    /* 
     * List of coordinates cooresponding to move range tiles associated with
     * this unit.
     */
    private List<Vector3Int> markedTiles = new List<Vector3Int>();

    /* Bools. */
    private bool myTurn;

    /* ==================================================================== *\
     *                                                                      *
     *  CLASS METHODS                                                       *
     *                                                                      *
    \* ==================================================================== */

    /* 
     * On start, initialize member variables, center the unit to the nearest
     * tile, and add its position to the GridController's list of occupied
     * tiles.
     */
    void Start()
    {
        theGrid = GameObject.Find("Grid").GetComponent<Grid>();
        gridControl = GameObject.Find("Grid").GetComponent<GridController>();
        myTurn = false;

        gameObject.transform.position = getCellPos();

        gridControl.addOccupiedTile(get_currGridPos());
    }

    /* Every frame, check for user inputs. */
    void Update()
    {
        /* If left-click detected... */
        if (Input.GetButtonDown("Fire1")) {

            /*
             * If it's the unit's turn and the selected tile is a legal move,
             * relocate the unit.
             */
            if (myTurn && isLegalMove()) {
                Reloc( GetCursorPosition(), get_currGridPos() );
            } 

            /* Temporary code.
             * If the move isn't legal, end the unit's turn.
             */
            else if (myTurn && (GetCursorPosition() != get_currGridPos())) {
                toggleTurn();
            }
        }
    }

    /* Unit clicked --> Toggle turn status. */
    void OnMouseDown()
    {
        toggleTurn();
    }

    /* ==================================================================== *\
     *  Helper Functions                                                    *
    \* ==================================================================== */

    /*
     * Returns the world position of the cell that the unit should center
     * itself on.
     */
    Vector3 getCellPos()
    {
        Vector3 coords = theGrid.GetCellCenterWorld(theGrid.WorldToCell(transform.position));
        return new Vector3(coords.x, (float)(coords.y - 0.25), coords.z);
    }

    /* Obtains character's current transform position. */
    Vector3 get_currPos()
    {
        return gameObject.transform.position;
    }

    /* Gets unit's position in relation to theGrid. The function's primary job
       is to account for grid offset. */
    Vector3Int get_currGridPos()
    {
        int offsetx = -1;
        int offsety = -1;

        Vector3Int gridpos = theGrid.WorldToCell(get_currPos());
        int x_coord = gridpos.x + offsetx;
        int y_coord = gridpos.y + offsety;

        Vector3Int offsetCoord = new Vector3Int(x_coord, y_coord, gridpos.z);

        return offsetCoord;
    }

    /* Switches myTurn on/off. */
    void toggleTurn()
    {
        myTurn = !myTurn;

        if (myTurn) {
            markedTiles.AddRange( gridControl.instantiateMoveRange(gridControl.CursorGridCoords(), 2) );
        } else {
            Debug.Log("UNINSTANTIATE");
            gridControl.unInstantiateMoveRange(markedTiles);
            markedTiles.Clear();
        }
    }

    /* Function that checks if a given move is legal or not. Basically, does it
       fall within the highlighted zone? */
    bool isLegalMove()
    {
        return gridControl.hasOverlayTile(GetCursorPosition());
    }

    /* Obtains grid coordinates of curent cursor position. */
    Vector3Int GetCursorPosition()
    {
        return gridControl.CursorGridCoords();
    }

    /* Moves unit to clicked tile. */
    void Reloc(Vector3Int newPos, Vector3Int currGridPos)
    {
        /* Determine how far in each direction the unit must be moved. */
        int x_dif = newPos.x - currGridPos.x;
        int y_dif = newPos.y - currGridPos.y;

        /*
         * If the difference isn't 0 in any direction, perform the relocation.
         *
         * Update occupied tiles in GridController.
         * Transform coordinate difference with offset and relocate.
         * Finally, toggle turn to instantiate move range.
         */
        if (x_dif != 0 || y_dif != 0) {
            gridControl.remOccupiedTile(currGridPos);
            gridControl.addOccupiedTile(newPos);

            Vector3 offset = new Vector3( (float)(x_dif * 0.5 - y_dif * 0.5), (float)(x_dif * 0.25 + y_dif * 0.25), 0 );
            gameObject.transform.position += offset;

            toggleTurn();
        }
    }
}
