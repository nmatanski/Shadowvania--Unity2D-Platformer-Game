using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
    [SerializeField]
    private int playerLivesCapacity = 3;
    public int PlayerLivesCapacity
    {
        get { return playerLivesCapacity; }
        set { playerLivesCapacity = value; }
    }


    [SerializeField]
    private int currentPlayerLives = 3;
    public int CurrentPlayerLives
    {
        get { return currentPlayerLives; }
        set { currentPlayerLives = value; }
    }


    [SerializeField]
    private string lastCheckpointScene = "Main Menu";
    public string LastCheckpointScene
    {
        get { return lastCheckpointScene; }
        set { lastCheckpointScene = value; }
    }



    private void Awake()
    {
        int sessionsCount = FindObjectsOfType<GameSession>().Length;
        if (sessionsCount > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }


    public void ProcessPlayerDeath()
    {
        if (CurrentPlayerLives > 1)
        {
            TakeLife();
        }
        else
        {
            ResetSessionToLastCheckpoint(); //respawn the player with full health on the last checkpoint
        }
    }

    private void ResetSessionToLastCheckpoint()
    {
        SceneManager.LoadScene(LastCheckpointScene); ///TODO: Respawn on/near the checkpoint
        //Destroy(gameObject); ///TODO: Do not destroy the progress, just respawn
        CurrentPlayerLives = PlayerLivesCapacity;
        ///TODO: gold /= 2;
    }

    private void TakeLife()
    {
        CurrentPlayerLives--;
        var currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex); ///TODO: Load near the hazard/enemy
    }
}
