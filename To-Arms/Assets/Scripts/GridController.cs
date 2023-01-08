/*
 * GridController.cs
 *
 * Purpose: Handles operations that deal with the tilemap. These include
 *          obtaining grid positions, identifying tile types, and displaying
 *          movement ranges.
 */

/* ======================================================================== *\
 *  IMPORTS                                                                 *
\* ======================================================================== */
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

/* ======================================================================== *\
 * CLASS: GridController                                                    *
 * Description: A single instance of these is needed for battle scenes. It  *
 *              gets attached to container for all tilemaps in the scene.   *
\* ======================================================================== */
public class GridController : MonoBehaviour
{
    /* ==================================================================== *\
     *  MEMBER VARIABLES                                                    *
    \* ==================================================================== */

    /* These are for displaying movement range. */
    public Tilemap overlayGrid;
    public Tile overlayTile;

    /* 
     * Also used for displaying movement range. Gets filled when tiles are
     * instantiated and emptied when the tiles are uninstantiated.
     */
    private List<Vector3Int> markedTiles = new List<Vector3Int>();

    /* List of occupied coordinates. */
    private List<Vector3Int> occupiedTiles = new List<Vector3Int>();

    /* 
     * These are for keeping track of the grid object and the battlemap 
     * (terrain).
     */
    private Grid grid;
    private Tilemap battleMap;

    /* ==================================================================== *\
     *                                                                      *
     *  CLASS METHODS                                                       *
     *                                                                      *
    \* ==================================================================== */

    /* On start, initialize private gameobject members. */
    void Start()
    {
        grid = gameObject.GetComponent<Grid>();
        battleMap = GameObject.Find("Tilemap_Base").GetComponent<Tilemap>();
    }

    /* ==================================================================== *\
     *  Public Getter Functions                                             *
    \* ==================================================================== */

    /* Returns the position of the cursor on the grid. */
    public Vector3Int CursorGridCoords()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellCoords = grid.WorldToCell(mouseWorldPos);
        return new Vector3Int(cellCoords.x, cellCoords.y, 0);
    }

    /*
     *  Purpose: Finds the name of the tile that the user is hovering over. 
     *  Input:  Accepts the name of the tilemap being checked.
     *  Output: The name of the tile at the current cursor position on the
     *          terrain tilemap.
     */
    public string get_TileAtCursor()
    {
        // Vector3Int coord = new Vector3Int(CursorGridCoords().x, CursorGridCoords().y, 0);
        Vector3Int coord = CursorGridCoords();

        if (battleMap.GetTile(coord) == null) {
            return "null";
        } else {
            return battleMap.GetTile(coord).ToString();
        }
    }

    /*
     * Determines if a given position on the movement range tilemap has an
     * overlay tile. That is, whether or not the given position is a legal
     * move.
     */
    public bool hasOverlayTile(Vector3Int pos)
    {
        // pos = new Vector3Int(pos.x, pos.y, 0);
        return overlayGrid.GetTile(pos) != null;
    }

    /* ==================================================================== *\
     *  Public Setter Functions                                             *
    \* ==================================================================== */

    /* Adds a coordinate to the occupiedTiles list. */
    public void addOccupiedTile(Vector3Int coord)
    {
        coord = new Vector3Int(coord.x, coord.y, 0);
        if (!occupiedTiles.Contains(coord)) {
            occupiedTiles.Add(coord);
        }
    }

    /* Removes a coordinate from the occupiedTiles list. */
    public void remOccupiedTile(Vector3Int coord)
    {
        coord = new Vector3Int(coord.x, coord.y, 0);
        if (occupiedTiles.Contains(coord)) {
            occupiedTiles.Remove(coord);
        }
    }

    /* ==================================================================== *\
     *  Move Range Display Functions                                        *
     *                                                                      *
     *  These functions display the highlighted tiles which appear when a   *
     *  player wants to move a character.                                   *
    \* ==================================================================== */

    /*
     * Function: instantiateMoveRange()
     * Description: Primary control function. Highlights all legal moves
     *              within range of a starting postion.
     * Input:  A position on the terrain grid and a move range.
     * Output: Highlights legal moves from the starting point.
     */
    public List<Vector3Int> instantiateMoveRange(Vector3Int start, int depth)
    {
        Debug.Log("Instantiating from: " + start);

        /* Obtaining a list of legal moves. */
        List<Vector3Int> coords = get_legalMoves(start, depth);
        // foreach (Vector3Int c in coords) {
        //     Debug.Log(c);
        // }

        /* Iterating over that list, highlighting each of the coordinates. */
        int coords_size = coords.Count;
        for (int i = 0; i < coords_size; i++) {
            overlayGrid.SetTile(coords[i], overlayTile);
        }

        /* Storing highlighted coords so they can be uninstantiated later. */
        markedTiles.AddRange(coords);

        return coords;
    }

    /*
     * Container function for retrieving legal moves. It calls its recursive
     * counterpart and returns a list of coordinates.
     */
    List<Vector3Int> get_legalMoves(Vector3Int start, int depth)
    {
        List<Vector3Int> marked = new List<Vector3Int>();
        // start = new Vector3Int(start.x, start.y, 0);
        // marked.Add( start );
        marked.Add( new Vector3Int(start.x, start.y, 0) );
        // List<Vector3Int> moves = get_legalMoves(start, depth, marked).Distinct().ToList();
        List<Vector3Int> moves = get_legalMoves(start, depth, marked).ToList();
        moves.Remove( start );
        return moves;
    }

    /*
     * Recursive function for finding legal moves from a starting position. It
     * utilizes breadth-first search, first checking the legality of neighbors
     * before moving onto the next layer of potential moves. 
     *
     * marked is a list of coordinates for tiles that have already been
     * recorded as legal. We don't need to consider moves in marked.
     */
    List<Vector3Int> get_legalMoves(Vector3Int start, int depth, List<Vector3Int> marked)
    {
        /*
         * When we reach the outer limit of the move range, we return a list
         * with a single value: the current coordinate.
         */
        if (depth == 0) {
            List<Vector3Int> self = new List<Vector3Int>();
            self.Add( start );
            return self;
        }

        /* Getting legal moves to adjacent tiles. */
        List<Vector3Int> adj = get_legalAdj(start, marked);

        /* Iterating over legal adjacent moves, calling get_legalMoves on each
           of them. As we check moves, we keep track of legal moves. We keep
           marked seperate from coordlist because marked needs to accumulate
           elements between iterations, while coordList only needs to return
           legal moves for the current iteration. */
        int listsize = adj.Count;
        List<Vector3Int> coordList = new List<Vector3Int>();
        for (int i = 0; i < listsize; i++) {
            List<Vector3Int> coordBranch = get_legalMoves(adj[i], depth-1, marked);
            marked.AddRange(coordBranch);
            coordList.AddRange(coordBranch);
        }

        return coordList;
    }

    /*
     * This function checks all coordinates adjacent to a given starting point
     * and returns all that are legal.
     *
     * Note: This function can be modified so that adjacency doesn't count
     *       diagonal tiles.
     */
    List<Vector3Int> get_legalAdj(Vector3Int start, List<Vector3Int> marked)
    {
        List<Vector3Int> adjList = new List<Vector3Int>();
        for (int y = -1; y <= 1; y++) {
            for (int x = -1; x <= 1; x++) {
                Vector3Int coord = new Vector3Int(start.x + x, start.y + y, 0);
                // Vector3Int coord = new Vector3Int(start.x + x, start.y + y, start.z);

                if (!isIllegal(coord, marked)) {
                    // Debug.Log(coord);
                    adjList.Add( coord );
                }
            }
        }

        return adjList;
    }

    /*
     * A move is illegal if the tile is occupied, it's a void tile, or if it's
     * already been marked.
     */
    bool isIllegal(Vector3Int coord, List<Vector3Int> marked)
    {
        bool is_occupied = occupiedTiles.Contains(coord);
        bool is_void = battleMap.GetTile(coord) == null;
        bool is_marked = marked.Contains(coord);

        return  is_occupied || is_void || is_marked;
    }

    /*
     * This function is called when it's time to get rid of all highlighted
     * tiles associated with a single unit.
     */
    public void unInstantiateMoveRange(List<Vector3Int> marked)
    {
        int listsize = marked.Count;
        for (int i = 0; i < listsize; i++) {
            markedTiles.Remove(marked[i]);
            if (!markedTiles.Contains(marked[i])) {
                overlayGrid.SetTile(marked[i], null);
            }
        }
        // markedTiles.Clear();
    }
}
