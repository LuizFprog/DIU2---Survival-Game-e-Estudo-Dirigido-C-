using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AIChase : MonoBehaviour
{
    public Transform playerTransform;
    public float speed;
    private float distance;
    private Animator animator;
    public float stopDistance = 0.1f;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            distance = Vector2.Distance(transform.position, playerTransform.position);
            Vector2 direction = playerTransform.position - transform.position;
            direction.Normalize();
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            if (distance > stopDistance)
            {
                animator.SetBool("isMoving", true);
                transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, speed * Time.deltaTime);

                animator.SetFloat("X", direction.x);
                animator.SetFloat("Y", direction.y);
            }
            else
            {
                animator.SetBool("isMoving", false);
            }
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            ScoreManager.instance.AddScore(5);
            Destroy(gameObject);
        }
    }
}
