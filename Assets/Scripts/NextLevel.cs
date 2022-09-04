using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{
    private GameObject BGM;
    public string scene;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.name == "Canvas")
        {
            gameObject.GetComponentInChildren<Animator>().SetBool("dead", true);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                BGM = GameObject.Find("BGM");
                if (BGM != null)
                {
                    BGM.GetComponent<AudioSource>().UnPause();
                }
                SceneManager.LoadScene(scene);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Player")
        {
            if (gameObject.name == "Button")
            {
                BGM = GameObject.Find("BGM");
                if (BGM != null)
                {
                    BGM.GetComponent<AudioSource>().Pause();
                }
                gameObject.GetComponent<AudioSource>().Play();
                DontDestroyOnLoad(gameObject);
            }
            Physics2D.gravity /= other.gameObject.GetComponent<PlayerController>().gravityModifier;
            other.gameObject.GetComponent<PlayerController>().gravityFlipped = false;
            SceneManager.LoadScene(scene);
        }
    }
}