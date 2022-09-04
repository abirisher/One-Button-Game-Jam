using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    private Animator[] anim;
    private Animator spaceman;
    private Animator text;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentsInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach (Animator child in anim)
            {
                child.SetBool("fadeOut", true);
            }
        }
        if (transform.GetChild(4).GetChild(0).gameObject.GetComponent<Animator>().GetBool("fadeIn"))
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene("Level 1");
            }
        }
    }
}