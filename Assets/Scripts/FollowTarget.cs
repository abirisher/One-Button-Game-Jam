using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public float moveSpeed;
    private Collider2D collision;
    private void Start()
    {
        collision = GetComponent<Collider2D>();
    }
    private void Update()
    {
        transform.Translate(Vector2.right * Time.deltaTime * moveSpeed);
        RaycastHit2D left = Physics2D.Raycast(new Vector2(collision.transform.position.x, collision.transform.position.y), Vector2.down);
        RaycastHit2D right = Physics2D.Raycast(new Vector2(collision.transform.position.x + collision.bounds.size.x, collision.transform.position.y), Vector2.down);
        if (left.collider != null && right.collider != null)
        {
            if ((left.distance > collision.bounds.size.y + 1 && moveSpeed < 0) || (right.distance > collision.bounds.size.y + 1 && moveSpeed > 0))
            {
                moveSpeed *= -1;
            }
        }
        else
        {
            moveSpeed *= -1;
        }
        
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Tile"))
        {
            moveSpeed *= -1;
        }
    }
}
