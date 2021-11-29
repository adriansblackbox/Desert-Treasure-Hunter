using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbScript : MonoBehaviour
{
    private Blade_Controller _bladeScript;
    private void Start(){
        _bladeScript = FindObjectOfType<Blade_Controller>();
    }
    private void Update() {
        // Here, we would check if the level is reset by player death.
        // In that case, we renable the meshrenderer and the collider.s
    }
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Blade")){
            _bladeScript.t = 0.0f;
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<Collider>().enabled = false;
        }
    }
}
