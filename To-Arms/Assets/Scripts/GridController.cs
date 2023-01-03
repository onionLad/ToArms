using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class GridController : MonoBehaviour
{
    public Tilemap overlayGrid;
    public Tile overlayTile;

    private Grid grid;
    private Tilemap battleMap;
    private List<Vector3Int> markedTiles = new List<Vector3Int>();

    // Start is called before the first frame update
    void Start()
    {
        grid = gameObject.GetComponent<Grid>();
        battleMap = GameObject.Find("Tilemap_Base").GetComponent<Tilemap>();
    }

    /*
     * This function returns the position of the cursor on the grid.
     */
    public Vector3Int GetGridPos() {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return grid.WorldToCell(mouseWorldPos);
    }

    /*
     * This function finds the name of the tile we're hovering over. The
     * function accepts the name of a tilemap so it knows which one to compare
     * the current mouse position to.
     *
     * If the tile being hovered over is a void tile, we just return "null".
     */
    public string get_TileAtCursor(string mapName)
    {
        // Find a way to automate offset.
        int offsetx = 0;
        int offsety = 0;

        Vector3Int coord = new Vector3Int(GetGridPos().x - offsetx, GetGridPos().y - offsety, 0);

        if (mapName == "battleMap") {
            if (battleMap.GetTile(coord) == null) {
                return "null";
            } else {
                return battleMap.GetTile(coord).ToString();
            }
        }

        else if (mapName == "overlay") {
            if (overlayGrid.GetTile(coord) == null) {
                return "null";
            } else {
                return overlayGrid.GetTile(coord).ToString();
            }
        }

        else {
            return "null";
        }
    }

    // DISPLAY MOVE RANGE FUNCTIONS.
    //
    // These functions display the highlighted tiles which appear when a
    // player wants to move a character.
    //-----------------------------------------------------------------------//

    /*
     * Primary control function. It takes a character's current position 
     * (start) and its move range (depth) as parameters, and highlights all
     * legal moves within range of the starting postion.
     */
    public void instantiateMoveRange(Vector3Int start, int depth)
    {
        Debug.Log("Instantiating");
        
        // // Accounting for offset.
        // start = new Vector3Int(start.x - 5, start.y - 5, 0);
        
        // Getting a list of legal moves.
        List<Vector3Int> coords = get_legalMoves(start, depth);
        
        // Iterating over that list, highlighting each of the coordinates.
        int coords_size = coords.Count;
        for (int i = 0; i < coords_size; i++)
        {
            overlayGrid.SetTile(coords[i], overlayTile);
        }

        // Storing the highlighted tile coords so we know which tiles to 
        // unhighlight when we stop displaying the move range.
        markedTiles.AddRange(coords);
    }

    /*
     * Container function for retrieving legal moves. It calls its recursive
     * counterpart and returns a list of coordinates.
     */
    List<Vector3Int> get_legalMoves(Vector3Int start, int depth)
    {
        List<Vector3Int> marked = new List<Vector3Int>();
        List<Vector3Int> moves = get_legalMoves(start, depth, marked).Distinct().ToList();
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
        /* When we reach the outer limit of the move range, we return a list
           with a single value: the current coordinate. */
        if (depth == 0)
        {
            List<Vector3Int> self = new List<Vector3Int>();
            self.Add( start );
            return self;
        }

        // Getting legal moves to adjacent tiles.
        List<Vector3Int> adj = get_legalAdj(start, marked);

        /* Iterating over legal adjacent moves, calling get_legalMoves on each
           of them. As we check moves, we keep track of legal moves. We keep
           marked seperate from coordlist because marked needs to accumulate
           elements between iterations, while coordList only needs to return
           legal moves for the current iteration. */
        int listsize = adj.Count;
        List<Vector3Int> coordList = new List<Vector3Int>();
        for (int i = 0; i < listsize; i++)
        {
            List<Vector3Int> coordBranch = get_legalMoves(adj[i], depth-1, marked);
            marked.AddRange(coordBranch);
            coordList.AddRange(coordBranch);
        }

        return coordList;
    }

    /*
     * This function checks all coordinates adjacent to a given starting point
     * and returns all that are legal.
     */
    List<Vector3Int> get_legalAdj(Vector3Int start, List<Vector3Int> marked)
    {
        List<Vector3Int> adjList = new List<Vector3Int>();
        for (int y = -1; y < 2; y++)
        {
            for (int x = -1; x < 2; x++)
            {
                Vector3Int coord = new Vector3Int(start.x + x, start.y + y, 0);

                if (!isIllegal(coord, marked))
                {
                    adjList.Add( coord );
                }
            }
        }

        return adjList;
    }

    // A move is illegal if the tile is occupied, it's a void tile, or if it's
    // already been marked.
    bool isIllegal(Vector3Int coord, List<Vector3Int> marked)
    {
        bool is_marked = marked.Contains(coord);
        bool is_void = battleMap.GetTile(coord) == null;

        return is_marked || is_void;
    }

    /*
     * This function is called when it's time to get rid of all highlighted
     * tiles.
     */
    public void unInstantiateMoveRange()
    {
        int listsize = markedTiles.Count;
        for (int i = 0; i < listsize; i++)
        {
            overlayGrid.SetTile(markedTiles[i], null);
        }
    }

    //-----------------------------------------------------------------------//

    /*
     * This function checks if a given position has a non-void tile.
     */
    public bool hasOverlayTile(Vector3Int pos)
    {
        pos = new Vector3Int(pos.x, pos.y, 0);
        return overlayGrid.GetTile(pos) != null;
    }
}
