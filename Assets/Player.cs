using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    public GameObject rock;
    public Transform spawnpoint;
    public Image healthbar;
    public GameObject player;
    public GameObject skull;

    public AudioSource hit;
    public AudioSource heal;
    public AudioSource slash;

    Rigidbody2D rb;
    Animator animator;
 //   ArrayList arrayList;

    private Vector2 move;
    private float intialY;

    private float speed;
    public float Speed
    {
        get { return speed; }
        set { speed = value; }
    }

    private float heath;
    public float Health
    {
        get { return heath; }
        set { heath = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();


        intialY = transform.position.y;

        spawnpoint = transform;
        Speed = 100;
        move = new Vector2();
        Health = 100;

        animator.SetBool("hit", false);

        InvokeRepeating("SpawnSkull", 2, 20);
    }

    // Update is called once per frame
    void Update()
    {
        Inputs();

        float yPos = transform.position.y;
       // float changeScaleBy = 0.005f;

        Vector3 firstScale = transform.localScale;


        if (Input.GetKeyDown(KeyCode.R))
        {
            Instantiate(rock, new Vector3(player.transform.position.x, 5, player.transform.position.z), Quaternion.identity);

        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetBool("cast", true);
            slash.Play();
        }

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow))
        {
            animator.SetBool("walk", true);
            if (yPos > intialY)
            {   
                if (firstScale.x < 0)
                {
                    transform.localScale -= new Vector3(-0.002f, 0.002f, 0f); 
                }
                else
                {
                    transform.localScale -= new Vector3(0.002f, 0.002f, 0f);
                }
                
            }
            else if (yPos < intialY)
            {
                if (firstScale.x < 0)
                {
                    transform.localScale += new Vector3(-0.002f, 0.002f, 0f);
                }
                else
                {
                    transform.localScale += new Vector3(0.002f, 0.002f, 0f);
                }
            }
            else
            {
                slash.Stop();
            }
            intialY = yPos;
        }
        else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
        {
            animator.SetBool("walk", true);
            Flip();
        }
        else
        {
            animator.SetBool("walk", false);
        }
    }

    public void Flip()
    {
        Transform canvas = player.transform.Find("Canvas");
        Vector3 scale = transform.localScale;
        if (Input.GetKey(KeyCode.RightArrow))
        {
            scale.x = (0f - Mathf.Abs(scale.x));
            transform.localScale = scale;
            canvas.transform.localScale = (-1)*-scale;
        }
        else
        {
            scale.x = Mathf.Abs(scale.x);
            transform.localScale = scale;
            canvas.transform.localScale = (-1)*-scale;
        }

    }

    public void Inputs()
    {
        move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

    }

    public void Crumble(Transform t)
    {
        Animator aniT = t.GetComponent<Animator>();
        if (aniT != null)
            aniT.enabled = false;

        foreach (Transform child in t.GetComponentInChildren<Transform>())
        {
            if(child == t.Find("Hip"))
            {
                child.SetParent(null, true);
            }
         //   if (child != t.Find("Canvas")){
                Crumble(child);
          //  }
        }
        Rigidbody2D rb1 = t.GetComponent<Rigidbody2D>();
        if (rb1 == null)
            rb1 = t.gameObject.AddComponent<Rigidbody2D>();

        Collider2D col = t.GetComponent<Collider2D>();
        if (col == null)
            col = t.gameObject.AddComponent<CapsuleCollider2D>();

        rb1.AddForce(new Vector3(0, -1, 0), ForceMode2D.Impulse);

    }
  
    public void FixedUpdate()
    {
        rb.velocity = move * Speed * Time.fixedDeltaTime;

        float clampedX = Mathf.Clamp(transform.position.x, -9.152168f, 9.216416f);
        float clampedY = Mathf.Clamp(transform.position.y, -5.423787f, 3.105064f);

        transform.position = new Vector3(clampedX, clampedY, 11.498f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.CompareTag("Rock"))
        {
            Damage(20);
            animator.SetBool("hit", true);
            hit.Play();

            if (healthbar.fillAmount == 0 || healthbar.fillAmount == 0.0f)
            {
                Crumble(transform);
            }
        }
        else if (collision.collider.gameObject.CompareTag("Skull"))
        {
            Regen(10);
            heal.Play();
            Destroy(collision.collider.gameObject);
        }
        else{
            hit.Stop();
            heal.Stop();
        }
    }

    public void Damage(float damage)
    {
        Health -= damage;
        healthbar.fillAmount = Health/100;  
    }

    public void Regen(float amount)
    {
        Health += amount;
        healthbar.fillAmount = Health/100;
    }

    public void SpawnSkull()
    {
        int x = Random.Range(-8, 8);
        int y = Random.Range(-5, 3);
        Instantiate(skull, new Vector3(x, y, 11.498f), Quaternion.identity); 
    }
}