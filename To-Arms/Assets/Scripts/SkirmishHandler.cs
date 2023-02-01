/*
 * SkirmishHandler.cs
 *
 * Purpose: Contains the SkirmishHandler class, which controls operations that
 *          manage skirmishes.
 */

/* ======================================================================== *\
 *  IMPORTS                                                                 *
\* ======================================================================== */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* ======================================================================== *\
 * CLASS: SkirmishHandler                                                   *
 * Description: A single instance of these is needed for battle scenes. It  *
 *              gets attached to a gamehandler object.                      *
\* ======================================================================== */
public class SkirmishHandler : MonoBehaviour
{
    /* ==================================================================== *\
     *  MEMBER VARIABLES                                                    *
    \* ==================================================================== */

    /* Game Objects */
    private GameObject unitContainer;
    private List<GameObject> units = new List<GameObject>();

    /* Turn tracker. */
    private int currTurn;
    private int maxTurn;

    /* ==================================================================== *\
     *                                                                      *
     *  CLASS METHODS                                                       *
     *                                                                      *
    \* ==================================================================== */

    /* On start, initialize class members. */
    void Start()
    {
        unitContainer = GameObject.Find("Units");
        GameObject[] unitArray = GameObject.FindGameObjectsWithTag("unit");

        /*
         * At some point, we will want a better system for determining turn
         * order.
         */
        for (int i = 0; i < unitArray.Length; i++) {
            units.Add(unitArray[i]);
            // Debug.Log("Unit Added");
        }
        // Debug.Log(units.Count);

        currTurn = 0;
    }

    /* Every frame, update display tiles according to current turn. */
    void Update()
    {
        /* Updating display tiles. */
        UnitController currUnitController = units[currTurn].GetComponent<UnitController>();
        if (!currUnitController.isTurn()) {
            // Debug.Log("uhoh");
            currUnitController.enableTurn();
            for (int i = 0; i < units.Count; i++) {
                if (i != currTurn) {
                    units[i].GetComponent<UnitController>().disableTurn();
                }
            }
        }
    }

    /* ==================================================================== *\
     *  Helper Functions                                                    *
    \* ==================================================================== */

    /* Moves currTurn to the next unit in the turn order. */
    public void incrementTurn()
    {
        currTurn++;
        if (currTurn >= units.Count) {
            currTurn = 0;
        }

        units[currTurn].GetComponent<UnitController>().setUpdateCounter();
    }

    /* Removes a unit from the unit array. Gets called when a unit dies. */
    public void Units_rem(GameObject unit)
    {
        units.Remove(unit);
    }
}
