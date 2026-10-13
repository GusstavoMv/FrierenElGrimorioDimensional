using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 12f;

    private Rigidbody2D rb2D;
    private SpriteRenderer spriteRenderer;

    public float MoveInput { get; private set; }
    public bool IsGrounded { get; private set; }
    public bool EstaMuerto { get; private set; }

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (EstaMuerto) return;

        // 1. Movimiento horizontal
        MoveInput = Input.GetAxisRaw("Horizontal");

        // 2. Volteo de sprite
        if (MoveInput > 0) spriteRenderer.flipX = false;
        else if (MoveInput < 0) spriteRenderer.flipX = true;

        // 3. Salto
        if (Input.GetButtonDown("Jump") && IsGrounded)
        {
            rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, jumpForce);
            IsGrounded = false;
        }
    }

    private void FixedUpdate()
    {
        if (EstaMuerto) return;

        rb2D.linearVelocity = new Vector2(MoveInput * speed, rb2D.linearVelocity.y);
    }

    // Permite que PlayerCollision actualice el estado del suelo con las normales
    public void SetGrounded(bool state)
    {
        IsGrounded = state;
    }

    public void DesactivarPorMuerte()
    {
        EstaMuerto = true;
        rb2D.linearVelocity = Vector2.zero;
        rb2D.bodyType = RigidbodyType2D.Kinematic;
    }
}