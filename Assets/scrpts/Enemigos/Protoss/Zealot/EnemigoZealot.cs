using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemigoZealotIA : MonoBehaviour
{
    [Header("Límites de la Plataforma")]
    [SerializeField] private Transform puntoIzquierda;
    [SerializeField] private Transform puntoDerecha;
    [SerializeField] private float velocidadPatrulla = 2f;
    [SerializeField] private float velocidadPersecucion = 3.5f;

    [Header("Detección y Combate")]
    [SerializeField] private float rangoDeteccion = 3.5f;
    [SerializeField] private float distanciaAtaque = 1.8f;
    [SerializeField] private float toleranciaAlturaY = 1.2f;
    [SerializeField] private float tiempoEntreAtaques = 1.5f;

    private float tiempoSiguienteAtaque = 0f;
    private bool moviendoHaciaDerecha = true;
    private Rigidbody2D rb;
    private Transform jugador;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        GameObject objJugador = GameObject.FindGameObjectWithTag("Player");
        if (objJugador != null)
        {
            jugador = objJugador.transform;
        }
    }

    private void Update()
    {
        if (jugador == null) return;

        float distTotal = Vector2.Distance(transform.position, jugador.position);
        float distY = Mathf.Abs(jugador.position.y - transform.position.y);
        
        // Comprobación estricta: ¿El jugador está en la misma plataforma física?
        bool jugadorEnMiPlataforma = JugadorEstaEnMiPlataforma() && (distY <= toleranciaAlturaY);

        if (jugadorEnMiPlataforma && distTotal <= distanciaAtaque)
        {
            // Frena y ataca con sus cuchillas
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            IntentarAtacar();
        }
        else if (jugadorEnMiPlataforma && distTotal <= rangoDeteccion)
        {
            // Te persigue solo porque estás pisando SU plataforma
            float dirX = Mathf.Sign(jugador.position.x - transform.position.x);
            rb.linearVelocity = new Vector2(dirX * velocidadPersecucion, rb.linearVelocity.y);

            if (spriteRenderer != null)
                spriteRenderer.flipX = dirX < 0;
        }
        else
        {
            // Si estás en otra plataforma o volando, patrulla sin volverse loco
            Patrullar();
        }
    }

    private bool JugadorEstaEnMiPlataforma()
    {
        if (puntoIzquierda == null || puntoDerecha == null || jugador == null) return false;
        float minX = Mathf.Min(puntoIzquierda.position.x, puntoDerecha.position.x);
        float maxX = Mathf.Max(puntoIzquierda.position.x, puntoDerecha.position.x);
        return jugador.position.x >= minX && jugador.position.x <= maxX;
    }

    private void Patrullar()
    {
        if (puntoIzquierda == null || puntoDerecha == null) return;

        float minX = Mathf.Min(puntoIzquierda.position.x, puntoDerecha.position.x);
        float maxX = Mathf.Max(puntoIzquierda.position.x, puntoDerecha.position.x);

        if (moviendoHaciaDerecha)
        {
            rb.linearVelocity = new Vector2(velocidadPatrulla, rb.linearVelocity.y);
            if (spriteRenderer != null) spriteRenderer.flipX = false;

            if (transform.position.x >= maxX)
                moviendoHaciaDerecha = false;
        }
        else
        {
            rb.linearVelocity = new Vector2(-velocidadPatrulla, rb.linearVelocity.y);
            if (spriteRenderer != null) spriteRenderer.flipX = true;

            if (transform.position.x <= minX)
                moviendoHaciaDerecha = true;
        }
    }

    private void IntentarAtacar()
    {
        if (Time.time >= tiempoSiguienteAtaque)
        {
            tiempoSiguienteAtaque = Time.time + tiempoEntreAtaques;
            PlayerHealth salud = jugador.GetComponent<PlayerHealth>();
            if (salud != null)
            {
                salud.RecibirDanio(1);
            }
        }
    }

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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoDeteccion);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaAtaque);
    }
}