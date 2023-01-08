/*
 * Unit.cs
 *
 * Purpose: Contains the Unit class, with contains all information about a
 *          given unit, including its health, combat modifiers, and stats.
 */

/* ======================================================================== *\
 *  IMPORTS                                                                 *
\* ======================================================================== */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* ======================================================================== *\
 * CLASS: Unit                                                              *
 * Description: An instance of this class is needed for each unit in each   *
 *              battle scene. It gets attached to a unit GameObject.        *
\* ======================================================================== */
public class Unit : MonoBehaviour
{
    /* ==================================================================== *\
     *  MEMBER VARIABLES                                                    *
    \* ==================================================================== */

    /*
     * Unit Type Info.
     * UnitName determines the base stats, modifer alters the base stats to
     * adjust difficulty, and team determines which units this unit can
     * attack.
     */
    public string unitName;
    public float modifier;
    public string team;

    /* Unit Stats. */
    private int maxHealth;
    private int currHealth;
    private int moveRange;

    private int strength;
    private int armor;
    private int baseDamage;

    private string weapon;

    /* ==================================================================== *\
     *                                                                      *
     *  CLASS METHODS                                                       *
     *                                                                      *
    \* ==================================================================== */

    /* On start, initialize member variables. */
    void Start()
    {
        switch (unitName)
        {
            default:
                maxHealth = 10;
                currHealth = maxHealth;
                moveRange = 2;

                strength = 5;
                armor = 5;
                baseDamage = 5;

                weapon = "None";
                break;
        }
    }

    /* Debugging function. Spits out unit stats to console. */
    void DisplayStats()
    {
        Debug.Log
        (
            "Name: " + unitName + "\n" +
            "Health: " + currHealth + "/" + maxHealth + "\n" +
            "Move: " + moveRange + "\n" +
            "Strength: " + strength + "\n" +
            "Armor: " + armor + "\n" +
            "Damage: " + baseDamage + "\n" +
            "Weapon: " + weapon
        );
    }
}
