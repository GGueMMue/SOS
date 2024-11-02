using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClearColider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            // 씬 전환 필요.

            SceneManager.LoadScene("ScoreBoard");
            Debug.Log("Clear");
        }
    }
}
