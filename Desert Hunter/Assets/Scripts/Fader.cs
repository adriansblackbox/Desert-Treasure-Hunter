using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
    public float fadeSpeed = 6f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator Fade(Player_Controller player_Controller) {
        for (Color squareColor = gameObject.GetComponent<Image>().color; squareColor.a < 1; squareColor = gameObject.GetComponent<Image>().color) {
            gameObject.GetComponent<Image>().color = new Color(squareColor.r, squareColor.g, squareColor.b, squareColor.a + (fadeSpeed * Time.deltaTime));
            yield return null;
        }
        for (Color squareColor = gameObject.GetComponent<Image>().color; squareColor.a > 0; squareColor = gameObject.GetComponent<Image>().color) {
            gameObject.GetComponent<Image>().color = new Color(squareColor.r, squareColor.g, squareColor.b, squareColor.a - (fadeSpeed * Time.deltaTime));
            yield return null;
        }
        player_Controller.enabled = true;
        yield return null;
    }
}
