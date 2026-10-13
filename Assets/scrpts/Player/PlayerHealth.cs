using UnityEngine;
using System;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Configuracion de Vidas")]
    [SerializeField] private int maxVidas = 3;
    private int vidasActuales;

    // Evento C# que escucha HUDVidas
    public static event Action<int> OnVidasModificadas;

    private PlayerMovement movement;
    private SpriteRenderer spriteRenderer;
    private bool esInvulnerable = false;
    [SerializeField] private float tiempoInvulnerabilidad = 1.0f;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        vidasActuales = maxVidas;
    }

    private void Start()
    {
        OnVidasModificadas?.Invoke(vidasActuales);
    }

public void RecibirDanio(int cantidad = 1)
    {
        if (esInvulnerable || vidasActuales <= 0) return;

        vidasActuales -= cantidad;
        OnVidasModificadas?.Invoke(vidasActuales);

        if (vidasActuales > 0)
        {
            StartCoroutine(RutinaInvulnerabilidad());
        }
        else
        {
            // 1. Apaga el movimiento para que el jugador no pueda seguir caminando
            if (movement != null)
            {
                movement.DesactivarPorMuerte();
            }

            // 2. Dispara la animación de muerte del Animator
            PlayerAnimations anim = GetComponent<PlayerAnimations>();
            if (anim != null)
            {
                anim.EjecutarMuerte();
            }

            // 3. Espera 1.2 segundos para que se reproduzca el clip antes de reiniciar
            if (GameManager.Instance != null)
            {
                GameManager.Instance.Invoke(nameof(GameManager.Instance.DerrotaJugador), 1.2f);
            }
        }
    }

    // Efecto de parpadeo visual en el personaje
    private IEnumerator RutinaInvulnerabilidad()
    {
        esInvulnerable = true;

        for (int i = 0; i < 3; i++)
        {
            if (spriteRenderer != null) spriteRenderer.color = new Color(1f, 1f, 1f, 0.25f);
            yield return new WaitForSeconds(tiempoInvulnerabilidad / 6f);
            if (spriteRenderer != null) spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(tiempoInvulnerabilidad / 6f);
        }

        esInvulnerable = false;
    }
}