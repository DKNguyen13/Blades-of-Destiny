using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float strength = 5f;
    private float horizontalInput;
    private Rigidbody2D rb;
    private Animator ani;
    private Collider2D colli;
    private bool isGround = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
        colli = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        if(isGround)
        {
            if (horizontalInput != 0)
            {
                ani.Play("walk");
            }
            else
            {
                ani.Play("idle");
            }
        }
        Move();
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Jump();
        }
    }

    //Move
    public void Move()
    {
        rb.velocity = new Vector2(speed * horizontalInput, rb.velocity.y);
        if(horizontalInput > 0.1f)
        {
            transform.localScale = Vector3.one;
        }
        else if(horizontalInput < -0.1f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    //Jump
    public void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, strength);
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGround = true ;
        }
    }
}
