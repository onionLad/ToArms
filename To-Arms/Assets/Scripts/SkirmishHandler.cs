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

    /* ==================================================================== *\
     *                                                                      *
     *  CLASS METHODS                                                       *
     *                                                                      *
    \* ==================================================================== */

    /* On start, initialize class members. */
    void Start()
    {
        unitContainer = GameObject.Find("Units");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
