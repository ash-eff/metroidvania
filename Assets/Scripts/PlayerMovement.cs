using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public LayerMask enemyMask;
    [SerializeField]
    private CharacterController2D controller;
    [SerializeField]
    private GameObject projectile;
    [SerializeField]
    private GameObject explosion;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float fireRate;

    private float horizontal;
    private float nextFire;
    private bool jump;

    private Vector3 startPos;

    Rigidbody2D rb2d;
    private LineRenderer lr;
    private Animator anim;

    private void Awake()
    {
        startPos = transform.position;
        rb2d = GetComponent<Rigidbody2D>();
        lr = GetComponent<LineRenderer>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal") * speed;

        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }

        if (Input.GetButton("Fire1") && Time.time > nextFire)
        {
            StartCoroutine(ShootLaser());
        }

        // running
        if(Mathf.Abs(horizontal) > 0 && controller.m_Grounded)
        {
            anim.SetBool("Run", true);
            anim.SetBool("Jump", false);
            anim.SetBool("Idle", false);
        }
        else if (Mathf.Abs(horizontal) > 0 && !controller.m_Grounded || Mathf.Abs(horizontal) <= 0 && !controller.m_Grounded) // jumping
        {
            anim.SetBool("Jump", true);
            anim.SetBool("Run", false);
            anim.SetBool("Idle", false);
        }
        else if(Mathf.Abs(horizontal) <= 0 && controller.m_Grounded) // idle
        {
            anim.SetBool("Idle", true);
            anim.SetBool("Jump", false);
            anim.SetBool("Run", false);
        }
    }

    private void FixedUpdate()
    {
        controller.Move(horizontal * Time.fixedDeltaTime, false, jump);
        jump = false;
    }

    IEnumerator ShootLaser()
    {
        nextFire = Time.time + fireRate;
        anim.SetTrigger("Shoot");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector3(transform.localScale.x, 0f, 0f), 10f, enemyMask);
        Debug.DrawRay(transform.position, new Vector3(transform.localScale.x, 0f, 0f) * 10f, Color.red);
        if (hit)
        {
            lr.SetPosition(0, transform.position + new Vector3(transform.localScale.x, 0f, 0f));
            lr.SetPosition(1, hit.point);
            if (hit.collider.tag == "Enemy")
            {
                GameObject exp = Instantiate(explosion, hit.point + new Vector2(0, .5f), Quaternion.identity);
                Destroy(exp, 1);
                Destroy(hit.collider.gameObject);
            }
            if (hit.collider.tag == "Walls")
            {
                Vector2 hitPos = new Vector2(hit.point.x, hit.point.y);
            }
        }
        else
        {
            lr.SetPosition(0, transform.position + new Vector3(transform.localScale.x, 0f, 0f));
            lr.SetPosition(1, transform.position + new Vector3(transform.localScale.x * 10, 0, 0));
        }
        yield return new WaitForSecondsRealtime(.08f);
        lr.SetPosition(0, Vector3.zero);
        lr.SetPosition(1, Vector3.zero);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "MovingPlatform")
        {
            transform.parent = collision.gameObject.transform;
        }

        if (collision.gameObject.tag == "Hole")
        {
            Debug.Log("Dead");
            transform.position = startPos;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "MovingPlatform")
        {
            transform.parent = null;
        }
    }
}
