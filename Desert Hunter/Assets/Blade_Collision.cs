using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blade_Collision : MonoBehaviour
{
    private void OnTriggerStay(Collider other) {
        Debug.Log("Working");
        FindObjectOfType<Aim_And_Shoot>()._bladeTimer = 0f;
    }

}
