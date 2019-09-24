using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomExit : MonoBehaviour
{
    [SerializeField]
    private float RoomLoadDelaySeconds = .1f;

    [SerializeField]
    private string SceneNameToLoad;

    [SerializeField]
    private string name;
    public string Name
    {
        get { return name; }
        set { name = value; }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!(collision is CapsuleCollider2D))
        {
            return;
        }
        FindObjectOfType<GameSession>().HasEntered = true;
        FindObjectOfType<GameSession>().ExitUsed = Name;

        StartCoroutine(LoadRoom());
    }


    private IEnumerator LoadRoom()
    {
        yield return new WaitForSeconds(RoomLoadDelaySeconds);

        SceneManager.LoadScene(SceneNameToLoad);
    }
}
