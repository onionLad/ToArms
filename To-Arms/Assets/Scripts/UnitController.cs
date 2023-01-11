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

    /* Objects & Scripts. */
    private Grid theGrid;
    private GridController gridControl;
    private SkirmishHandler skirmish;

    /* 
     * Lists of coordinates cooresponding to move range tiles and attack
     * target tiles associated with this unit.
     */
    private List<Vector3Int> markedTiles = new List<Vector3Int>();
    private List<Vector3Int> targetTiles = new List<Vector3Int>();

    /* Bools. */
    private bool myTurn;
    private bool hasMoved;
    private bool hasAttacked;

    /* Poor Solution. */
    private int tilesNotUpdated;

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
        skirmish = GameObject.Find("GameHandler").GetComponent<SkirmishHandler>();

        gameObject.transform.position = get_CellInWorldPos();

        gridControl.addOccupiedTile(get_currGridPos());

        myTurn = false;
        hasMoved = false;
        hasAttacked = false;

        tilesNotUpdated = 0;
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

            // /* Temporary code.
            //  * If the move isn't legal, end the unit's turn.
            //  */
            // else if (myTurn && (GetCursorPosition() != get_currGridPos())) {
            //     toggleTurn();
            // }
        }

        /* Temp Solution: Update move & target tiles a few times. */
        if (myTurn && (tilesNotUpdated > 0)) {
            disableTurn();
            enableTurn();
            tilesNotUpdated--;
        }
    }

    // /* Unit clicked --> Toggle turn status. */
    // void OnMouseDown()
    // {
    //     toggleTurn();
    // }

    /* ==================================================================== *\
     *  Coordinate Getter Functions                                         *
    \* ==================================================================== */

    /* Gets the WORLD position of the UNIT in relation to the GRID. */
    Vector3 get_CellInWorldPos()
    {
        Vector3 coords = theGrid.GetCellCenterWorld(theGrid.WorldToCell(transform.position));
        return new Vector3(coords.x, (float)(coords.y - 0.25), coords.z);
    }

    /* Gets the GRID position of the UNIT. */
    Vector3Int get_currGridPos()
    {
        int offsetx = -1;
        int offsety = -1;

        Vector3Int gridpos = theGrid.WorldToCell(gameObject.transform.position);
        int x_coord = gridpos.x + offsetx;
        int y_coord = gridpos.y + offsety;

        Vector3Int offsetCoord = new Vector3Int(x_coord, y_coord, gridpos.z);

        return offsetCoord;
    }

    /* Gets the GRID positon of the CURSOR. */
    Vector3Int GetCursorPosition()
    {
        return gridControl.CursorGridCoords();
    }

    /* ==================================================================== *\
     *  Turn Management Functions                                           *
    \* ==================================================================== */

    /* Switches myTurn on/off. */
    void toggleTurn()
    {
        myTurn = !myTurn;

        if (myTurn) {
            // Debug.Log("INSTANTIATE");
            markedTiles.AddRange( gridControl.instantiateMoveRange(gridControl.CursorGridCoords(), 2) );
            targetTiles.AddRange( gridControl.instantiateTargets(gridControl.CursorGridCoords(), 1) );

            tilesNotUpdated = 30;
        } else {
            // Debug.Log("UNINSTANTIATE");
            gridControl.unInstantiateMoveRange(markedTiles);
            markedTiles.Clear();

            gridControl.unInstantiateTargets(targetTiles);
            targetTiles.Clear();
        }
    }

    /* Switches myTurn to true and updates display tiles. */
    public void enableTurn()
    {
        myTurn = true;

        markedTiles.AddRange( gridControl.instantiateMoveRange(get_currGridPos(), 2) );
        targetTiles.AddRange( gridControl.instantiateTargets(get_currGridPos(), 1) );

        tilesNotUpdated = 30;
    }

    /* Switches myTurn to false and updates display tiles. */
    public void disableTurn()
    {
        myTurn = false;

        gridControl.unInstantiateMoveRange(markedTiles);
        markedTiles.Clear();

        gridControl.unInstantiateTargets(targetTiles);
        targetTiles.Clear();
    }

    /* Returns this unit's turn status. Used by other scripts. */
    public bool isTurn()
    {
        return myTurn;
    }

    /*
     * Returns true if the unit is ready to move. That is, it has exhausted
     * all position actions it could have taken on the current turn.
     */
    public bool turnOver()
    {
        return hasMoved && hasAttacked;
    }

    /* ==================================================================== *\
     *  Movement Helper Functions                                           *
    \* ==================================================================== */

    /* Function that checks if a given move is legal or not. Basically, does it
       fall within the highlighted zone? */
    bool isLegalMove()
    {
        return gridControl.hasOverlayTile(GetCursorPosition());
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

            skirmish.incrementTurn();
        }
    }
}
