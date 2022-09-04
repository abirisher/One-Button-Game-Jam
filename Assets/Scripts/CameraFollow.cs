using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Vector3 offset;
    public float smoothSpeed;
    private bool swapCamera;
    private Vector3 updatedTranslate;
    private PlayerController player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (player != null && !player.GetComponent<PlayerController>().gameOver)
        {
            Vector3 desiredPosition = player.transform.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
            if (player.swapCamera == true)
            {
                offset.x *= -1;
                player.swapCamera = false;
            }

            if (player.GetComponent<PlayerController>().gravityFlipped)
            {
                offset.y = -Mathf.Abs(offset.y);
            }
            else
            {
                offset.y = Mathf.Abs(offset.y);
            }
        }
    }
}
