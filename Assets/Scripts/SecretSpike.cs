using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretSpike : MonoBehaviour
{

    public int phaseSeconds;
    private float timer;
    private int seconds;
    private Animator anim;
    private AudioSource openSound;
    private AudioSource closeSound;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        openSound = GetComponents<AudioSource>()[0];
        closeSound = GetComponents<AudioSource>()[1];
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        seconds = (int)timer % 60;
        if (seconds == phaseSeconds)
        {
            if (gameObject.CompareTag("Hit Enemy"))
            {
                anim.SetBool("timer", true);
                gameObject.tag = "Avoid Enemy";
                openSound.Play();
            }
            else if (gameObject.CompareTag("Avoid Enemy"))
            {
                anim.SetBool("timer", false);
                gameObject.tag = "Hit Enemy";
                closeSound.Play();
            }
            timer = 0;
        }
    }
}