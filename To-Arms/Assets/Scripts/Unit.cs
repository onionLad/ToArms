/*
 * Unit.cs
 *
 * Purpose: Contains the Unit class, which contains all information about a
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

                strength = 2;
                armor = 2;
                baseDamage = 2;

                weapon = "None";
                break;
        }
    }

    /*
     * Every frame, destroy the Unit's gameobject if it doesn't have any
     * health left.
     */
    void Update()
    {
        if (currHealth <= 0) {
            Destroy(gameObject);
            /* Update SkirmishHandler */
        }
    }

    /* ==================================================================== *\
     *  Helper Functions                                                    *
    \* ==================================================================== */

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

    /* ==================================================================== *\
     *  Combat Helper Functions                                             *
    \* ==================================================================== */

    /*
     * Accepts an enemy unit, calculates damage, then deals damage. We may
     * want to break this function into a damage calculator and a seperate
     * damage dealer.
     */
    public void CalculateDamage(Unit attacker)
    {
        Debug.Log(attacker.gameObject.name + " attacked " + gameObject.name);

        int attackForce = (int)(attacker.strength * attacker.baseDamage * attacker.modifier);
        int defendForce = (int)(strength * armor * modifier);

        int totalDamage = attackForce + defendForce;

        int attackDamage = (int)(((float)attackForce / totalDamage) * attacker.strength * 5);

        currHealth -= attackDamage;

        // Debug.Log("Attack Force: " + attackForce + "\n" +
        //             "Defend Force: " + defendForce + "\n" +
        //             "Total Damage: " + totalDamage + "\n" +
        //             "Attack Damage: " + attackDamage);

        Debug.Log(gameObject.name + " now has " + currHealth + " HP");
    }
}
