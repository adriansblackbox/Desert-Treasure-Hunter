using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeScript : MonoBehaviour {

    public GameObject[] suspended;

    private Vector3[] savedPositions;
    private Quaternion[] savedRotations;
    private Blade_Controller _bladeScript;

    private void Start() {
        savedPositions = new Vector3[suspended.Length];
        savedRotations = new Quaternion[suspended.Length];
        for (int i = 0; i < suspended.Length; i++) {
            if (suspended[i].GetComponent<Rigidbody>() != null) suspended[i].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            savedPositions[i] = suspended[i].transform.position;
            savedRotations[i] = suspended[i].transform.rotation;
        }

        _bladeScript = FindObjectOfType<Blade_Controller>();
    }

    private void Update() {

    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Blade")) {
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<Collider>().enabled = false;

            for (int i = 0; i < suspended.Length; i++) {
                if (suspended[i].GetComponent<Rigidbody>() != null) suspended[i].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            }
        }
    }

    public void Reset() {
        GetComponent<MeshRenderer>().enabled = true;
        GetComponent<Collider>().enabled = true;

        for (int i = 0; i < suspended.Length; i++) {
            if (suspended[i].GetComponent<Rigidbody>() != null) suspended[i].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            suspended[i].transform.position = savedPositions[i];
            suspended[i].transform.rotation = savedRotations[i];
        }
    }
}
