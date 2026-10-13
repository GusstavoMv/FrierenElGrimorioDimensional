using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class PlayerAnimations : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private PlayerMovement playerMovement;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        // 1. Obtener la velocidad horizontal del Rigidbody2D o del input
        float velocidad = Mathf.Abs(rb.linearVelocity.x);

        // 2. Si no está en el piso, apagamos la velocidad para que no mueva los pies en el aire
        if (playerMovement != null && !playerMovement.IsGrounded)
        {
            velocidad = 0f;
        }

        // 3. Enviamos el valor a Speed
        if (animator != null)
        {
            animator.SetFloat("Speed", velocidad);
        }

        // 4. Volteo del personaje respetando la escala (Scale 3x3)
        if (rb.linearVelocity.x > 0.05f)
        {
            spriteRenderer.flipX = false;
        }
        else if (rb.linearVelocity.x < -0.05f)
        {
            spriteRenderer.flipX = true;
        }
    }

    public void EjecutarMuerte()
    {
        if (animator != null)
        {
            animator.SetTrigger("Muerte");
        }
    }
}