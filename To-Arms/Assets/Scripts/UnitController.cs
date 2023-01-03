using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class UnitController : MonoBehaviour
{
    //-- VARIABLES --//

    // Objects.
    private Grid theGrid;
    private GridController gridControl;
    // private Tilemap battleMap;

    // Bools.
    private bool myTurn;
    
    //-- FUNCTIONS --//

    // Start is called before the first frame update
    void Start()
    {
        theGrid = GameObject.Find("Grid").GetComponent<Grid>();
        gridControl = GameObject.Find("Grid").GetComponent<GridController>();
        // battleMap = GameObject.Find("Tilemap_Base").GetComponent<Tilemap>();

        myTurn = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Click -> Display Move Range.
        if (Input.GetButtonDown("Fire1"))
        {
            if (myTurn && isLegalMove()) {
                Reloc( GetPosition(), get_currGridPos() );
            } else if (myTurn) {
                toggleTurn();
            }
        }
    }

    // Click -> Select.
    void OnMouseDown()
    {
        toggleTurn();
    }

    //-- HELPERS --//

    // Gets character's position.
    Vector3 get_currPos()
    {
        return gameObject.transform.position;
    }

    /* Gets character's position in relation to theGrid. The function's
       primary job is to account for grid offset. */
    Vector3 get_currGridPos()
    {
        // Find a way to automate offset.
        int offsetx = -1;
        int offsety = -1;

        Vector3 gridpos = theGrid.WorldToCell(get_currPos());
        float x_coord = gridpos.x + offsetx;
        float y_coord = gridpos.y + offsety;

        Vector3 offsetCoord = new Vector3(x_coord, y_coord, gridpos.z);

        return offsetCoord;
    }

    // Moves unit to clicked tile.
    void Reloc(Vector3Int newPos, Vector3 currGridPos)
    {
        float x_dif = newPos.x - currGridPos.x;
        float y_dif = newPos.y - currGridPos.y;

        if (x_dif != 0 || y_dif != 0)
        {
            Vector3 offset = new Vector3( (float)(x_dif * 0.5 - y_dif * 0.5), (float)(x_dif * 0.25 + y_dif * 0.25), 0 );

            gameObject.transform.position += offset;

            toggleTurn();
        }
    }

    // Switches myTurn on/off.
    void toggleTurn()
    {
        myTurn = !myTurn;

        // Switch other units' myTurn to false.

        if (myTurn)
        {
            gridControl.instantiateMoveRange(GetPosition(), 2);
        } else {
            gridControl.unInstantiateMoveRange();
        }
    }

    // Function that checks if a given move is legal or not. Basically, does it
    // fall within the highlighted zone?
    bool isLegalMove()
    {
        return gridControl.hasOverlayTile(GetPosition());
    }

    Vector3Int GetPosition()
    {
        return gridControl.GetGridPos();
    }
}
