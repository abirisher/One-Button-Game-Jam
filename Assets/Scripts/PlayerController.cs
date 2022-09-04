using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public bool swapCamera;
    public float timeResolution;
    public float maxTime;
    public float jumpForce;
    public float maxJump;
    public float minJump;
    public float timeToMax;
    public float gravityModifier;
    public float speed;
    public float goopSlowMod;
    public LayerMask layerMask = -1;

    private float tempSpeed;
    private bool isOnGround = false;
    public bool gameOver = false;
    private bool rising = true;
    private bool keyDown = true;
    public bool gravityFlipped = false;

    private Rigidbody2D playerRb;
    private LineRenderer lineRenderer;
    private Animator playerAnim;
    private Collider2D playerCollider;
    private AudioSource jumpSound;
    private AudioSource bounceSound;
    private AudioSource m2DeathSound;
    private AudioSource m1DeathSound;
    private AudioSource runSound;
    private AudioSource gameOverMusic;
    private AudioSource pdeathSound;



    // Start is called before the first frame update
    void Start()
    {
        getComponents();
        Physics2D.gravity *= gravityModifier;
        jumpForce = minJump;
        tempSpeed = speed;
    }

    // Update is called once per frame
    private void Update()
    {
        jump();
        if (gameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                gameOverMusic.Stop();
                runSound.UnPause();
                Physics2D.gravity /= gravityModifier;
                Vector3 temp = Physics2D.gravity;
                temp.y = -Mathf.Abs(Physics2D.gravity.y);
                Physics2D.gravity = temp;
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }

    private void FixedUpdate()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }
    private void OnCollisionStay2D(Collision2D other)
    {
        tileInteract(other);
        enemyCollision(other);
        gravPlate(other);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        goop(other);
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Tile"))
        {
            isOnGround = false;
        }
    }
    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.CompareTag("Goop"))
        {
            jumpForce *= goopSlowMod;
            maxJump *= goopSlowMod;
        }
    }

    private void jump()
    {
        if (isOnGround && !gameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                speed = 0;
                keyDown = true;
            }
            else if (Input.GetKey(KeyCode.Space) && keyDown)
            {
                runSound.Pause();
                playerAnim.SetBool("isJumping", true);
                renderArc();
                if (rising)
                {
                    jumpForce += (maxJump - minJump) / (60 * timeToMax);
                    if (jumpForce >= maxJump)
                    {
                        rising = false;
                    }
                }
                else
                {
                    jumpForce -= (maxJump - minJump) / (60 * timeToMax);
                    if (jumpForce <= minJump)
                    {
                        rising = true;
                    }
                }
            }
            else if (Input.GetKeyUp(KeyCode.Space) && keyDown)
            {
                jumpSound.Play();
                clearArc();
                keyDown = false;
                isOnGround = false;
                playerRb.AddForce(Vector2.up * jumpForce * (-Physics2D.gravity.y / Mathf.Abs(Physics2D.gravity.y)), ForceMode2D.Impulse);
                jumpForce = minJump;
                speed = tempSpeed;
            }
        }
    }

    private void getComponents()
    {
        playerRb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        lineRenderer = GetComponent<LineRenderer>();
        playerAnim = GetComponent<Animator>();
        jumpSound = GetComponents<AudioSource>()[0];
        bounceSound = GetComponents<AudioSource>()[1];
        m1DeathSound = GetComponents<AudioSource>()[2];
        m2DeathSound = GetComponents<AudioSource>()[3];
        runSound = GetComponents<AudioSource>()[4];
        gameOverMusic = GetComponents<AudioSource>()[5];
        pdeathSound = GetComponents<AudioSource>()[6];
    }

    private void enemyCollision(Collision2D other)
    {
        {
            if (other.gameObject.CompareTag("Hit Enemy"))
            {
                Vector2 playerDistance = transform.position - other.transform.position;
                playerDistance.x = Mathf.Abs(playerDistance.x);
                if (Mathf.Abs(Mathf.Acos(playerDistance.x / playerDistance.magnitude)) >= 1 && playerDistance.y > 0)
                {
                    GameObject target = other.gameObject;
                    Animator targetAnim = target.GetComponent<Animator>();
                    if (targetAnim != null)
                    {
                        AnimationHandler animationScript = target.GetComponent<AnimationHandler>();
                        if (animationScript != null)
                        {
                            targetAnim.SetBool("killed", true);
                            animationScript.killed = true;
                            
                        }
                        else
                        {
                            Destroy(other.gameObject);
                        }
                    }
                    else
                    {
                        Destroy(other.gameObject);
                    }
                    bounceSound.Play();
                    int rand = Random.Range(1, 3);
                    if (rand == 1)
                    {
                        m1DeathSound.Play();
                    }
                    else
                    {
                        m2DeathSound.Play();
                    }
                    playerRb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                }
                //Enemy is higher than or at same height as player
                else
                {
                    gameOver = true;
                    runSound.Pause();
                    //bgm.Pause();
                    pdeathSound.Play();
                    gameOverMusic.Play();
                    speed = 0;
                    clearArc();
                    gravityFlipped = false;
                    transform.position = new Vector3(-500000, 0, 0);
                    GameObject.Find("Canvas").GetComponentInChildren<Animator>().SetBool("dead", true);
                    Debug.Log("Game Over");
                }
            }
            else if (other.gameObject.CompareTag("Avoid Enemy"))
            {
                gameOver = true;
                runSound.Pause();
                //bgm.Pause();
                pdeathSound.Play();
                gameOverMusic.Play();
                speed = 0;
                clearArc();
                gravityFlipped = false;
                transform.position = new Vector3(-500000, 0, 0);
                GameObject.Find("Canvas").GetComponentInChildren<Animator>().SetBool("dead", true);
                Debug.Log("Game Over");
            }

        }
    }

    private void tileInteract(Collision2D other)
    {
        if (other.gameObject.CompareTag("Tile"))
        {
            RaycastHit2D hit = Physics2D.Raycast(playerCollider.bounds.center, Vector2.down);
            if (hit.collider != null && !gravityFlipped)
            {
                if (Mathf.Abs(hit.distance) < playerCollider.bounds.extents.y + 0.1)
                {
                    runSound.UnPause();
                    isOnGround = true;
                    playerAnim.SetBool("isJumping", false);
                }
            }
            hit = Physics2D.Raycast(playerCollider.bounds.center, Vector2.up);
            if (hit.collider != null && gravityFlipped)
            {
                if (Mathf.Abs(hit.distance) < playerCollider.bounds.extents.y + 0.1)
                {
                    runSound.UnPause();
                    isOnGround = true;
                    playerAnim.SetBool("isJumping", false);
                }
            }
            hit = Physics2D.Raycast(playerCollider.bounds.center, Vector2.right);
            if (hit.collider != null && speed > 0)
            {
                if (Mathf.Abs(hit.distance) < playerCollider.bounds.extents.x + 0.1)
                {
                    speed *= -1;
                    tempSpeed *= -1;
                    swapCamera = true;
                    flipSpriteX();
                }
            }
            hit = Physics2D.Raycast(playerCollider.bounds.center, Vector2.left);
            if (hit.collider != null && speed < 0)
            {
                if (Mathf.Abs(hit.distance) < playerCollider.bounds.extents.x + 0.1)
                {
                    speed *= -1;
                    tempSpeed *= -1;
                    swapCamera = true;
                    flipSpriteX();
                }
            }
        }
        /*
                    //Right side
                    RaycastHit2D topHit = Physics2D.Raycast(new Vector2(playerCollider.transform.position.x + playerCollider.bounds.size.x, playerCollider.transform.position.y - playerCollider.bounds.size.y / 2), Vector2.down);
                    //Left side
                    RaycastHit2D botHit = Physics2D.Raycast(new Vector2(playerCollider.transform.position.x, playerCollider.transform.position.y - playerCollider.bounds.size.y / 2), Vector2.down);

        Debug.Log("Down left: " + botHit.collider + " " + botHit.distance);
        Debug.Log("Down right: " + topHit.collider + " " + topHit.distance);

                    if ((topHit.collider != null || botHit.collider != null) && !gravityFlipped)
                    {
                        if (topHit.collider != null)
                        {
                            if (Mathf.Abs(topHit.distance) < playerCollider.bounds.size.y + 1.5)
                            {
                                isOnGround = true;
                                playerAnim.SetBool("isJumping", false);
                            }
                        }
                        else if (botHit.collider != null)
                        {
                            if (Mathf.Abs(botHit.distance) < playerCollider.bounds.size.y + 1.5)
                            {
                                isOnGround = true;
                                playerAnim.SetBool("isJumping", false);
                            }
                        }
                    }

                    //Right side
                    topHit = Physics2D.Raycast(new Vector2(playerCollider.transform.position.x + playerCollider.bounds.size.x, playerCollider.transform.position.y + playerCollider.bounds.size.y / 2), Vector2.up);
                    //Left side
                    botHit = Physics2D.Raycast(new Vector2(playerCollider.transform.position.x, playerCollider.transform.position.y + playerCollider.bounds.size.y / 2), Vector2.up);

        Debug.Log("Up left: " + botHit.collider + " " + botHit.distance);
        Debug.Log("Up right: " + topHit.collider + " " + topHit.distance);

                    if ((topHit.collider != null || botHit.collider != null) && !gravityFlipped)
                    {
                        if (topHit.collider != null)
                        {
                            if (Mathf.Abs(topHit.distance) < playerCollider.bounds.size.y + 1.2)
                            {
                                isOnGround = true;
                                playerAnim.SetBool("isJumping", false);
                            }
                        }
                        else if (botHit.collider != null)
                        {
                            if (Mathf.Abs(botHit.distance) < playerCollider.bounds.size.y + 1.2)
                            {
                                isOnGround = true;
                                playerAnim.SetBool("isJumping", false);
                            }
                        }
                    }

                    topHit = Physics2D.Raycast(new Vector2(transform.position.x, playerCollider.transform.position.y + playerCollider.bounds.size.y / 2), Vector2.right);
                    botHit = Physics2D.Raycast(new Vector2(transform.position.x, playerCollider.transform.position.y), Vector2.right);

        Debug.Log("Mid Right: " + botHit.collider + " " + botHit.distance);
        Debug.Log("Top Right: " + topHit.collider + " " + topHit.distance);

                    if ((topHit.collider != null || botHit.collider) && speed > 0)
                    {
                        if (topHit.collider != null)
                        {
                            if (Mathf.Abs(topHit.distance) < playerCollider.bounds.extents.x + 0.25)
                            {
                                speed *= -1;
                                tempSpeed *= -1;
                                swapCamera = true;
                                flipSpriteX();
                            }
                        }
                        else if (botHit.collider != null)
                        {
                            if (Mathf.Abs(botHit.distance) < playerCollider.bounds.extents.x + 0.25)
                            {
                                speed *= -1;
                                tempSpeed *= -1;
                                swapCamera = true;
                                flipSpriteX();
                            }
                        }
                    }

                    topHit = Physics2D.Raycast(new Vector2(playerCollider.bounds.size.x, playerCollider.transform.position.y + playerCollider.bounds.size.y / 2), Vector2.left);
                    botHit = Physics2D.Raycast(new Vector2(playerCollider.bounds.size.x, playerCollider.transform.position.y), Vector2.left);

        Debug.Log("Mid Left: " + botHit.collider + " " + botHit.distance);
        Debug.Log("Top Left: " + topHit.collider + " " + topHit.distance);

                    if ((topHit.collider != null || botHit.collider) && speed > 0)
                    {
                        if (topHit.collider != null && speed < 0)
                        {
                            if (Mathf.Abs(topHit.distance) < playerCollider.bounds.extents.x + 0.25)
                            {
                                speed *= -1;
                                tempSpeed *= -1;
                                swapCamera = true;
                                flipSpriteX();
                            }
                            else if (Mathf.Abs(botHit.distance) < playerCollider.bounds.extents.x + 0.25)
                            {
                                speed *= -1;
                                tempSpeed *= -1;
                                swapCamera = true;
                                flipSpriteX();
                            }
                        }
                    }*/
    }

    public void flipSpriteX()
    {
        Vector3 lTemp = gameObject.transform.localScale;
        lTemp.x *= -1;
        gameObject.transform.localScale = lTemp;
    }

    private void renderArc()
    {
        Vector2 veclocityVector = transform.right * tempSpeed + transform.up * jumpForce * (-Physics2D.gravity.y / Mathf.Abs(Physics2D.gravity.y));
        lineRenderer.positionCount = (int)(maxTime / timeResolution) + 1;
        int index = 0;
        Vector2 currentPosition = playerCollider.transform.position;
        if (!gravityFlipped)
        {
            currentPosition.y -= playerCollider.bounds.extents.y;
        }
        else
        {
            currentPosition.y += playerCollider.bounds.extents.y;
        }

        for (float t = 0.0f; t < maxTime; t += timeResolution)
        {
            lineRenderer.SetPosition(index, currentPosition);
            RaycastHit hit;

            if (Physics.Raycast(currentPosition, veclocityVector, out hit, veclocityVector.magnitude * timeResolution, layerMask))
            {
                lineRenderer.positionCount = index + 2;

                lineRenderer.SetPosition(index + 1, hit.point);

                break;
            }

            currentPosition += veclocityVector * timeResolution;
            veclocityVector += Physics2D.gravity * timeResolution;
            index++;
        }
    }

    private void clearArc()
    {
        lineRenderer.positionCount = 0;
    }

    private void gravPlate(Collision2D other)
    {
        if (other.gameObject.CompareTag("Grav"))
        {
            Physics2D.gravity *= -1;
            gravityFlipped = !gravityFlipped;
            Vector3 temp = transform.localScale;
            temp.y *= -1;
            transform.localScale = temp;
        }
    }

    private void goop(Collider2D other)
    {
        if (other.gameObject.CompareTag("Goop"))
        {
            jumpForce *= 1 / goopSlowMod;
            maxJump *= 1 / goopSlowMod;
        }
    }
}