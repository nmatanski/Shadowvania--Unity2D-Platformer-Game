using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    [SerializeField]
    private int goldAmount = 1;

    [SerializeField]
    private AudioClip coinPickupSFX;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision is CapsuleCollider2D == false)
        {
            return;
        }

        AudioSource.PlayClipAtPoint(coinPickupSFX, Camera.main.transform.position);

        FindObjectOfType<GameSession>().AddToGold(goldAmount);

        Destroy(gameObject);
    }
}
