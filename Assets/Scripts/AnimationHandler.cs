using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    public bool killed;
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (killed)
        {
            Destroy(this.gameObject, anim.GetCurrentAnimatorStateInfo(0).length);
        }
    }
}
