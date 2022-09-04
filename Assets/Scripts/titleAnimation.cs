using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class titleAnimation : MonoBehaviour
{
    private GameObject parent;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void spaceMan()
    {
        parent = transform.parent.gameObject;
        Animator[] anim = parent.GetComponentsInChildren<Animator>();
        foreach (Animator child in anim)
            {
                if (child.gameObject.name == "Astronaut")
                {
                    child.SetBool("moveRight", true);
                }
            }
    }

    public void text()
    {
        parent = transform.parent.gameObject;
        Animator[] anim = parent.GetComponentsInChildren<Animator>();
        foreach (Animator child in anim)
            {
                if (child.gameObject.name == "Story Text")
                {
                    child.SetBool("moveUp", true);
                }
            }
    }

    public void play()
    {
        Animator anim = transform.GetChild(0).GetComponent<Animator>();
        anim.SetBool("fadeOut", false);
        anim.SetBool("fadeIn", true);

    }
}
