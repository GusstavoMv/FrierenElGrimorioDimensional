using UnityEngine;

[RequireComponent(typeof(PlayerMovement), typeof(PlayerHealth))]
public class PlayerCollision : MonoBehaviour
{
    [Header("Efectos")]
    [SerializeField] private CameraShakePerlin cameraShake;

    private PlayerMovement movement;
    private PlayerHealth playerHealth;
    private bool wasInAir = false;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. Detección física del suelo REAL (contacto desde abajo)
        foreach (ContactPoint2D punto in collision.contacts)
        {
            if (punto.normal.y > 0.6f)
            {
                if (movement != null)
                {
                    movement.SetGrounded(true);
                }

                // 2. Sacudida de cámara al aterrizar en el piso
                if (wasInAir)
                {
                    if (cameraShake != null)
                    {
                        cameraShake.Shake(2.0f, 0.2f);
                    }
                    wasInAir = false;
                }
                break;
            }
        }

        // 3. Daño al chocar con cualquier enemigo físico
        if (collision.gameObject.CompareTag("Enemigo"))
        {
            if (playerHealth != null)
            {
                playerHealth.RecibirDanio(1);
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // Solo renueva el estado en el suelo si sigues pisando hacia abajo
        foreach (ContactPoint2D punto in collision.contacts)
        {
            if (punto.normal.y > 0.6f)
            {
                if (movement != null)
                {
                    movement.SetGrounded(true);
                }
                break;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (movement != null)
        {
            movement.SetGrounded(false);
        }
        wasInAir = true;
    }
}