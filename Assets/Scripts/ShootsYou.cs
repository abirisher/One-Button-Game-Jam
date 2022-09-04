using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootsYou : MonoBehaviour
{
    public int timeToShoot;
    public GameObject laser;
    private bool shooting;
    private float timer;
    private int seconds;
    private Animator anim;
    private AudioSource sound;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        sound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        seconds = (int)timer % 60;
        if (seconds == 1 && shooting)
        {
            shooting = false;
            anim.SetBool("shooting", false);
        }
        if (seconds == timeToShoot)
        {
            anim.SetBool("shooting", true);
            shooting = true;
            Vector3 gunLocation = new Vector3(-5, 2, 0);
            gunLocation.x *= transform.parent.localScale.x / Mathf.Abs(transform.parent.localScale.x);
            Instantiate(laser, transform.position + gunLocation, Quaternion.identity);
            sound.Play();
            Vector3 laserVect = laser.transform.localScale;
            laserVect.x *= transform.parent.localScale.x / Mathf.Abs(transform.parent.localScale.x);
            laser.transform.localScale = laserVect;
            timer = 0;
        }
    }
}