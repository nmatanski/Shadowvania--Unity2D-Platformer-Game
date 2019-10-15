using UnityEngine;
using UnityEngine.SceneManagement;

public class CoinsPersist : MonoBehaviour
{
    private int startingSceneIndex;


    private void Awake()
    {
        int coinsPersistsCount = FindObjectsOfType<CoinsPersist>().Length;

        if (coinsPersistsCount > 1)
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
