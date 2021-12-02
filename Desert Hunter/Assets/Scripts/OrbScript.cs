using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
//using UnityEditor.VFX;
//using UnityEditor.VFX.UI;

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
            this.gameObject.SetActive(false);
        }
    }
    public void Reset() {
        this.gameObject.SetActive(true);
    }
}
