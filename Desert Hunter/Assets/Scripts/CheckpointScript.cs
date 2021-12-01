using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointScript : MonoBehaviour
{
    public GameObject player;
    public GameObject[] resetables;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Blade") || other.gameObject.CompareTag("Player")) {
            player.GetComponent<Player_Controller>().lastCheckpoint = gameObject;
        }
    }

    public void Reset() {
        for (int i = 0; i < resetables.Length; i++) {
            if (resetables[i].GetComponent<OrbScript>() != null) resetables[i].GetComponent<OrbScript>().Reset();
            if (resetables[i].GetComponent<RopeScript>() != null) resetables[i].GetComponent<RopeScript>().Reset();
        }
    }
}
