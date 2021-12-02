using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blade_Collision : MonoBehaviour
{
    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.layer == 9 || other.gameObject.layer == 10){
            FindObjectOfType<Blade_Controller>().t = FindObjectOfType<Blade_Controller>().BladeTime;
        }
    }
}
