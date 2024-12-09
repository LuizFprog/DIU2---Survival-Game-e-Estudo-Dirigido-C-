using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerCtrl : MonoBehaviour
{
    Animator anim;
    bool moving;
    public float movSpeed;
    public float dashSpeed;
    public float dashDuration;
    public float croachSpeed;
    public float croachDuration;
    float speedX, speedY;
    Vector2 input;
    Vector2 mousePosition;
    Rigidbody2D rb;
    bool isDashing;
    float dashTime;
    bool isCroach;
    float croachTime;

    public Weapon weapon;
    public Transform aimTransform;

    Vector2 spawnPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spawnPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDashing)
        {
            dashTime -= Time.deltaTime;
            if (dashTime <= 0)
            {
                isDashing = false;
            }
        }
        else if (isCroach)
        {
            croachTime -= Time.deltaTime;
            if(croachTime <= 0)
            {
                isCroach = false;
            }
        }
        else
        {
            speedX = Input.GetAxisRaw("Horizontal") * movSpeed;
            speedY = Input.GetAxisRaw("Vertical") * movSpeed;
            input = new Vector2(speedX, speedY);
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            {
                isDashing = true;
                dashTime = dashDuration;
                input *= dashSpeed;
            }
            if(Input.GetKeyDown(KeyCode.C))
            {
                isCroach = true;
                croachTime = croachDuration;
                input *= croachSpeed;
            }
            if (Input.GetMouseButtonDown(0))
            {
                weapon.Fire();
                anim.SetBool("isShooting", true);
                Vector2 aimDirection = (mousePosition - rb.position).normalized; 
                anim.SetFloat("X", aimDirection.x); 
                anim.SetFloat("Y", aimDirection.y);
            }
            else
            {
                anim.SetBool("isShooting", false);
            }

            rb.linearVelocity = input;
            Animate();
        }
    }

    private void FixedUpdate() 
    { 
        Vector2 aimDirection = mousePosition - rb.position; 
        float aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg - 90f; 
        aimTransform.rotation = Quaternion.Euler(0, 0, aimAngle); 
    }

    private void Animate()
    {
        if (input.magnitude > 0.1f || input.magnitude < -0.1f)
        {
            moving = true;
        }
        else
        {
            moving = false;
        }
        if (moving)
        {
            anim.SetFloat("X", speedX);
            anim.SetFloat("Y", speedY);
        }
        anim.SetBool("Moving", moving);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Zombie"))
        {
            transform.position = spawnPosition;
        }
    }
}
