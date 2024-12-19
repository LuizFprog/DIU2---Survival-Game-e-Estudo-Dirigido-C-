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

    public float dashCooldown = 3f;
    private float dashCooldownTime;
    private bool isImmune;
    private float immuneTime;
    private Collider2D playerCollider;
    private int originalLayer;

    public float croachCooldown = 10f;
    private float croachCooldownTime;
    private string originalTag;

    public Weapon weapon;
    public Transform aimTransform;

    public PauseMenu pauseMenu;

    Vector2 spawnPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerCollider = GetComponent<Collider2D>();
        originalLayer = gameObject.layer;
        originalTag = gameObject.tag;
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
                dashCooldownTime = dashCooldown;
            }
        }
        else if (isCroach)
        {
            croachTime -= Time.deltaTime;
            if (croachTime <= 0)
            {
                isCroach = false;
                croachCooldownTime = croachCooldown;
                gameObject.tag = originalTag;
            }
        }
        else
        {
            dashCooldownTime -= Time.deltaTime;
            croachCooldownTime -= Time.deltaTime;

            speedX = Input.GetAxisRaw("Horizontal") * movSpeed;
            speedY = Input.GetAxisRaw("Vertical") * movSpeed;
            input = new Vector2(speedX, speedY);
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) && dashCooldownTime <= 0)
            {
                isDashing = true;
                dashTime = dashDuration;
                input *= dashSpeed;
                isImmune = true;
                immuneTime = 1.5f;
                gameObject.layer = LayerMask.NameToLayer("ImmunePlayer");
            }
            if (Input.GetKeyDown(KeyCode.C) && croachCooldownTime <= 0)
            {
                isCroach = true;
                croachTime = croachDuration;
                input *= croachSpeed;
                gameObject.tag = "Untagged";
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

        if (isImmune)
        {
            immuneTime -= Time.deltaTime;
            if (immuneTime <= 0)
            {
                isImmune = false;
                gameObject.layer = originalLayer;
            }
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
        if (collision.gameObject.CompareTag("Zombie") && !isImmune)
        {
            pauseMenu.GameOver();
        }
    }

}
