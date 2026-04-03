using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HerCollisions : MonoBehaviour
{
    public GameObject cleric;

    Animator aniB;
    Animator aniC;
    public Image healthbarB;

    public float teleportCooldown = 4f;
    private float lastTeleportTime = -Mathf.Infinity;

    private float distance;
    public float Distance
    {
        get { return distance; }
        set { distance = value; }
    }

    private float healthB;
    public float HealthB
    {
        get { return healthB; }
        set { healthB = value; }
    }


    // Start is called before the first frame update
    void Start()
    {
        aniB = GetComponent<Animator>();
        aniC = cleric.GetComponent<Animator>();

        HealthB = 100;

    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(transform.position, cleric.transform.position);
        TeleportingEnemy();
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.CompareTag("Sword") && gameObject.tag == "Barby") 
        {
            Damage(10f);
            aniB.SetTrigger("hit");
            if (healthbarB.fillAmount == 0 || healthbarB.fillAmount == 0.0f)
            {
                EnemyCrumble(transform);
            }
        }

    }

    public void Damage(float damage)
    {
        HealthB -= damage;
        healthbarB.fillAmount = HealthB / 100;

    }

    public void TeleportingEnemy()
    {
        if (Time.time - lastTeleportTime < teleportCooldown)
            return;

        int rand = Random.Range(0, 10);

        int newX = Random.Range(-7, 7);
        float newY = Random.Range(-0.2f, -1.88f);


        if(rand%3 == 0 && distance < 12f)
        {
            transform.position = new Vector3(newX, newY, 11.498f);
            lastTeleportTime = Time.time;
            print("teleporting");
        }
    }

    public void EnemyCrumble(Transform t)
    {
        Animator aniT = t.GetComponent<Animator>();
        if (aniT != null)
            aniT.enabled = false;

        foreach (Transform child in t.GetComponentInChildren<Transform>())
        {
            if (child == t.Find("Hip"))
            {
                child.SetParent(null, true);
            }
            if (child != t.Find("Canvas"))
            {
                EnemyCrumble(child);
            }
        }
        Rigidbody2D rb1 = t.GetComponent<Rigidbody2D>();
        if (rb1 == null)
            rb1 = t.gameObject.AddComponent<Rigidbody2D>();

        Collider2D col = t.GetComponent<Collider2D>();
        if (col == null)
            col = t.gameObject.AddComponent<CapsuleCollider2D>();

        rb1.AddForce(new Vector3(0, -1, 0), ForceMode2D.Impulse);

        Destroy(t.gameObject, 2f); 
    }

}
