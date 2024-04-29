using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BishopKnight : MonoBehaviour
{

    public float health;
    public float maxHealth = 100f;
    public float attackReachDistance = 2f;
    public float noticeDistance = 10f;
    public float loseInterestDistance = 25f;
    public float attackPower = 2f;
    public float moveSpeed = 2f;

    public Transform player;
    public GameObject weapon;

    public Animator anim;

    public enum state
    {
        idle,
        chasing,
        attacking,
        defending,
        dead,
    }

    public state currentState = state.idle;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        anim = GetComponent<Animator>();
        player = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (currentState == state.idle && distance < noticeDistance)
        {
            currentState = state.chasing;
        }

        if (currentState == state.chasing && distance < attackReachDistance)
        {
            currentState = state.attacking;
        }

        if (currentState == state.attacking && distance > attackReachDistance + 2f)
        {
            currentState = state.chasing;
        }

        if (currentState == state.chasing && distance > loseInterestDistance)
        {
            anim.SetBool("isRunning", false);
            currentState = state.idle;
        }

        if (health < 10f)
        {
            currentState = state.defending;
        }

        switch (currentState)
        {
            case state.idle:
                StartCoroutine(idle());
                break;
            case state.chasing:
                StartCoroutine(chasing());
                break;
            case state.attacking:
                StartCoroutine(attacking());
                break;
            case state.defending:
                StartCoroutine(defending());
                break;
            case state.dead:
                break;
        }
    }

    public IEnumerator idle()
    {
        yield return null;
    }

    public IEnumerator chasing()
    {
        anim.SetBool("isRunning", true);
        anim.SetBool("battleIdle", true);

        Vector3 direction = player.position - transform.position;
        direction.y = 0; // Ignore changes in the y-coordinate
        transform.rotation = Quaternion.LookRotation(direction);

        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        yield return null;
    }

    public IEnumerator attacking()
    {
        if (!isAttacking)
        {
            Vector3 direction = player.position - transform.position;
            direction.y = 0; // Ignore changes in the y-coordinate
            transform.rotation = Quaternion.LookRotation(direction);

        }

        anim.SetBool("isRunning", false);
        anim.SetBool("battleIdle", true);

        if (Random.Range(0, 1500) == 69)
        {
            if (Random.Range(0, 2) == 0)
            {
                anim.SetTrigger("attackOne");
                StartCoroutine(checkForHit());
            }
            else
            {
                anim.SetTrigger("attackTwo");
                 StartCoroutine(checkForHit());
            }
        }
        yield return null;
    }

    public IEnumerator defending()
    {
        if (Random.Range(0, 2500) == 420)
        {
            if (Random.Range(0, 2) == 0)
            {
                anim.SetTrigger("attackOne");
                StartCoroutine(checkForHit());
            }
            else
            {
                anim.SetTrigger("attackTwo");
                StartCoroutine(checkForHit());
            }
        }
        yield return null;
    }

    public bool isAttacking = false;

    public IEnumerator checkForHit()
    {
        isAttacking = true;
        yield return new WaitForSeconds(0.3f);
        isAttacking = false;
    }

    public void subHealth(int toSub)
    {
        health -= toSub;

        if (health < 1f)
        {
            StartCoroutine(die());
        }
    }

    public void addHealth(int toAdd)
    {
        health += toAdd;
        Mathf.Clamp(health, 1f, maxHealth);
    }

    public IEnumerator die()
    {
        StopCoroutine("checkForHit");
        transform.Rotate(0, 180, 0);
        anim.SetTrigger("die");

        yield return new WaitForSeconds(5f);

        gameObject.SetActive(false);
    }
}
