using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePersist : MonoBehaviour
{
    private int startingSceneIndex;


    private void Awake()
    {
        int scenePersistCount = FindObjectsOfType<ScenePersist>().Length;

        if (scenePersistCount > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    //Do this to respawn everything in the scene after changing room and not dying in the same one
    //private void Start()
    //{
    //    startingSceneIndex = SceneManager.GetActiveScene().buildIndex;
    //}

    //private void Update()
    //{
    //    int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
    //    if (currentSceneIndex != startingSceneIndex)
    //    {
    //        Destroy(gameObject);
    //    }
    //}
}
