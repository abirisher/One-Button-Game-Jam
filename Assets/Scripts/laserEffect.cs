using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class laserEffect : MonoBehaviour
{
    public float speed;
    private bool positive;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 scaleTemp = transform.localScale;
        if (scaleTemp.x > 0)
        {
            positive = true;
            scaleTemp.x -= 0.15f * Time.deltaTime;
            transform.Translate(Vector2.right * Time.deltaTime * speed);
        }
        else if (scaleTemp.x < 0)
        {
            positive = false;
            scaleTemp.x += 0.15f * Time.deltaTime;
            transform.Translate(Vector2.left * Time.deltaTime * speed);
        }
        if (scaleTemp.y > 0)
            scaleTemp.y -= 0.15f * Time.deltaTime;
        else if (scaleTemp.y < 0)
            scaleTemp.y += 0.15f * Time.deltaTime;
        transform.localScale = scaleTemp;

        if (positive)
        {
            if (transform.localScale.x <= 1)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            if (transform.localScale.x >= -1)
            {
                Destroy(gameObject);
            }
        }
    }
}
