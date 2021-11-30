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
        
    }
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Blade")){
            _bladeScript.t = 0.0f;
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<Collider>().enabled = false;
        }
    }
    public void Reset() {
        GetComponent<MeshRenderer>().enabled = true;
        GetComponent<Collider>().enabled = true;
    }
}
