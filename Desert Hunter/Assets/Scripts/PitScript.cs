using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitScript : MonoBehaviour
{
    public GameObject player;
    
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
            FindObjectOfType<Blade_Controller>().t = FindObjectOfType<Blade_Controller>().BladeTime;
            player.GetComponent<Player_Controller>().Reset();
        }
    }
}
