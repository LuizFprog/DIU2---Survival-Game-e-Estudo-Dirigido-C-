using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AIChase : MonoBehaviour
{
    public Transform playerTransform;
    public float speed;
    private float distance;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        distance = Vector2.Distance(transform.position, playerTransform.transform.position);
        Vector2 direction = playerTransform.transform.position - transform.position;
        direction.Normalize();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        animator.SetBool("isMoving", true);
        transform.position = Vector2.MoveTowards(transform.position, playerTransform.transform.position, speed * Time.deltaTime);

        animator.SetFloat("X", direction.x);
        animator.SetFloat("Y", direction.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Bullet"))
        {
            Destroy(gameObject);
        }
    }
}
