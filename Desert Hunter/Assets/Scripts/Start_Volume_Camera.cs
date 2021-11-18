using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Start_Volume_Camera : MonoBehaviour
{
    public GameObject StartCamera;
    public GameObject FollowCamera;

    private void OnTriggerStay(Collider other) {
        if(other.gameObject.CompareTag("Player")){
            StartCamera.SetActive(true);
            FollowCamera.SetActive(false);
        }
    }
    private void OnTriggerExit(Collider other) {
        if(other.gameObject.CompareTag("Player")){
            StartCamera.SetActive(false);
            FollowCamera.SetActive(true);
        }
    }
}
