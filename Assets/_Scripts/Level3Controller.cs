﻿// Level3Controller.cs
// Created by: Winnie Chung
// Last Modified: Dec. 13 by Winnie Chung
// Description: Keeps track of the game state for the third level, and the transition to the next scene
// Revision History:
// Oct. 17: File creation
// Oct. 18: Code refactoring
// Oct. 20: Added handling for key
// Oct. 21: Added internal documentation
// Dec. 12: Modified handling for keys, UI elements
// Dec. 13: Modified header

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

// keeps track of the game state for the third level, and the transition to the next scene
public class Level3Controller : LevelController {

    // private instance variables
    private GameObject _finish;

    protected override void Start()
    {
        this._keysNeeded = GameObject.FindGameObjectsWithTag("key").Length;
        base.Start();
        
        this._finish = GameObject.FindGameObjectWithTag("Finish");
        this._finish.SetActive(false);
    }

    public override void GetKey()
    {
        base.GetKey();
        if (base.HasAllKeys) {
            this._finish.SetActive(true);
        }
    }

    /* instead of starting a new level,
     * present the game clear text and
     * restart button */
    public override void ExitLevel()
    {
        this._isOver = true;

        this._gameOverText2.GetComponent<Text>().text = "Clear";

        this._gameOverText1.SetActive(true);
        this._gameOverText2.SetActive(true);
        this._restartButton.SetActive(true);
        this._menuButton.SetActive(true);
        this._player.SetActive(false);
    }
}
