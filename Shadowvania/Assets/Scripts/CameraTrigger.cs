using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            var fake = GameObject.FindGameObjectWithTag("FakePlayer").transform;
            StartCoroutine(ChangeTarget(fake));
        }
    }

    private IEnumerator ChangeTarget(Transform target)
    {
        var cam = GameObject.FindGameObjectWithTag("StateCamera").GetComponent<Cinemachine.CinemachineVirtualCameraBase>();
        var defaultLook = cam.LookAt;
        var defaultFollow = cam.Follow;
        cam.LookAt = target;
        cam.Follow = target;
        yield return new WaitForSeconds(1.5f);
        cam.Follow = defaultFollow;
        cam.LookAt = defaultLook;
        GameObject.FindGameObjectWithTag("FakePlayer").SetActive(false);
        gameObject.SetActive(false);
    }
}
