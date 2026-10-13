using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlantaMayor_Atack : MonoBehaviour
{
    private Animator animator;
    private PlayerHealth jugadorObjetivo;
    private bool atacando = false;

    [Header("Sincronización de Mordisco")]
    [Tooltip("Tiempo en segundos desde que arranca la animación hasta que cierra la boca")]
    [SerializeField] private float retardoImpacto = 0.35f; 
    [SerializeField] private float cadenciaAtaque = 1.4f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.GetComponent<PlayerMovement>() != null)
        {
            jugadorObjetivo = collision.GetComponent<PlayerHealth>();
            if (!atacando)
            {
                StartCoroutine(SecuenciaAtaque());
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.GetComponent<PlayerMovement>() != null)
        {
            if (jugadorObjetivo == null)
            {
                jugadorObjetivo = collision.GetComponent<PlayerHealth>();
            }

            if (!atacando)
            {
                StartCoroutine(SecuenciaAtaque());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.GetComponent<PlayerMovement>() != null)
        {
            jugadorObjetivo = null; // Si te retiras a tiempo, esquivas el mordisco
        }
    }

    private IEnumerator SecuenciaAtaque()
    {
        atacando = true;

        // 1. Inicia la animación de estirar el cuello
        if (animator != null)
        {
            animator.SetTrigger("Morder");
        }

        // 2. Espera a que la boca llegue al frente y cierre las mandíbulas
        yield return new WaitForSeconds(retardoImpacto);

        // 3. Si el jugador sigue en rango en el momento del mordisco, recibe el daño
        if (jugadorObjetivo != null)
        {
            jugadorObjetivo.RecibirDanio(1);
            Debug.Log("¡Mordisco sincronizado! -1 Corazón");
        }

        // 4. Pausa antes de poder volver a atacar
        yield return new WaitForSeconds(cadenciaAtaque - retardoImpacto);
        atacando = false;
    }

    // Daño si caminas y te estrellas de lleno contra el tallo
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.GetComponent<PlayerMovement>() != null)
        {
            PlayerHealth salud = collision.gameObject.GetComponent<PlayerHealth>();
            if (salud != null)
            {
                salud.RecibirDanio(1);
            }
        }
    }

    public void ImpactoMordida() { }
}