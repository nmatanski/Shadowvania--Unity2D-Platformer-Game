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


    private void OnTriggerEnter2D(Collider2D collision)
    {
        StartCoroutine(LoadRoom());
    }

    private IEnumerator LoadRoom()
    {
        yield return new WaitForSeconds(RoomLoadDelaySeconds);
        SceneManager.LoadScene(SceneNameToLoad);
    }
}
