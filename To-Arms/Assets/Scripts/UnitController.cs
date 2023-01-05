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

    // Objects.
    private Grid theGrid;
    private GridController gridControl;

    // Bools.
    private bool myTurn;

    /* ==================================================================== *\
     *                                                                      *
     *  CLASS METHODS                                                       *
     *                                                                      *
    \* ==================================================================== */

    /* On start, initialize member variables. */
    void Start()
    {
        theGrid = GameObject.Find("Grid").GetComponent<Grid>();
        gridControl = GameObject.Find("Grid").GetComponent<GridController>();

        myTurn = false;
    }

    /* Every frame, check for user inputs. */
    void Update()
    {
        /* If l-click detected... */
        if (Input.GetButtonDown("Fire1")) {

            /*
             * If it's the unit's turn and the selected tile is a legal move,
             * relocate the unit.
             */
            if (myTurn && isLegalMove()) {
                Reloc( GetPosition(), get_currGridPos() );
            } 

            /* Temporary code.
             * If the move isn't legal, end the unit's turn.
             */
            else if (myTurn) {
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

    /* Obtains character's current transform position. */
    Vector3 get_currPos()
    {
        return gameObject.transform.position;
    }

    /* Gets unit's position in relation to theGrid. The function's primary job
       is to account for grid offset. */
    Vector3 get_currGridPos()
    {
        int offsetx = -1;
        int offsety = -1;

        Vector3 gridpos = theGrid.WorldToCell(get_currPos());
        float x_coord = gridpos.x + offsetx;
        float y_coord = gridpos.y + offsety;

        Vector3 offsetCoord = new Vector3(x_coord, y_coord, gridpos.z);

        return offsetCoord;
    }

    /* Switches myTurn on/off. */
    void toggleTurn()
    {
        myTurn = !myTurn;

        if (myTurn) {
            gridControl.instantiateMoveRange(gridControl.CursorGridCoords(), 2);
        } else {
            gridControl.unInstantiateMoveRange();
        }
    }

    /* Function that checks if a given move is legal or not. Basically, does it
       fall within the highlighted zone? */
    bool isLegalMove()
    {
        return gridControl.hasOverlayTile(GetPosition());
    }

    /* Obtains grid coordinates of curent cursor position. */
    Vector3Int GetPosition()
    {
        return gridControl.CursorGridCoords();
    }

    /* Moves unit to clicked tile. */
    void Reloc(Vector3Int newPos, Vector3 currGridPos)
    {
        /* Determine how far in each direction the unit must be moved. */
        float x_dif = newPos.x - currGridPos.x;
        float y_dif = newPos.y - currGridPos.y;

        /* 
         * Convert grid coordinate difference into transform coordinate
         * difference.
         */
        if (x_dif != 0 || y_dif != 0) {
            Vector3 offset = new Vector3( (float)(x_dif * 0.5 - y_dif * 0.5), (float)(x_dif * 0.25 + y_dif * 0.25), 0 );

            gameObject.transform.position += offset;

            toggleTurn();
        }
    }
}
