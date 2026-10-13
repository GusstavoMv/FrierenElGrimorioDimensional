using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemigoZealot : MonoBehaviour
{
    [Header("Patrulla por Distancia Fija")]
    [SerializeField] private float distanciaPatrulla = 3.5f; // Metros a la izquierda y derecha
    [SerializeField] private float velocidadPatrulla = 1.2f;
    [SerializeField] private float velocidadPersecucion = 2.2f;

    [Header("Detección de Piedra")]
    [SerializeField] private float rangoVision = 8.0f;
    [SerializeField] private float distanciaAtaque = 1.3f;
    [SerializeField] private float toleranciaAlturaY = 1.2f; // Máxima diferencia de altura para considerarlo su mismo piso
    [SerializeField] private float tiempoEntreGolpes = 1.5f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Transform jugador;

    private float limiteIzquierdoX;
    private float limiteDerechoX;
    private bool moviendoHaciaDerecha = true;
    private float temporizadorAtaque = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb.freezeRotation = true;
    }

    private void Start()
    {
        // 1. Define sus límites fijos automáticamente según donde lo soltaste en la escena
        limiteIzquierdoX = transform.position.x - distanciaPatrulla;
        limiteDerechoX = transform.position.x + distanciaPatrulla;

        // 2. Localiza al jugador por Tag
        GameObject objJugador = GameObject.FindGameObjectWithTag("Player");
        if (objJugador != null)
        {
            jugador = objJugador.transform;
        }
    }

    private void Update()
    {
        temporizadorAtaque += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        bool jugadorValidoEnSuPiso = false;

        if (jugador != null)
        {
            float distHorizontal = Mathf.Abs(jugador.position.x - transform.position.x);
            float distVertical = Mathf.Abs(jugador.position.y - transform.position.y);
            float distTotal = Vector2.Distance(transform.position, jugador.position);

            // Solo interactúa si la Piedra está a su misma altura (mismo piso) y dentro de sus límites
            bool enMismoPiso = distVertical <= toleranciaAlturaY;
            bool dentroDeSuRango = jugador.position.x >= limiteIzquierdoX && jugador.position.x <= limiteDerechoX;

            if (enMismoPiso && dentroDeSuRango && distTotal <= rangoVision)
            {
                jugadorValidoEnSuPiso = true;

                // 1. Si está a rango cuerpo a cuerpo: se detiene y ataca
                if (distHorizontal <= distanciaAtaque)
                {
                    rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                    MirarHacia(jugador.position.x);

                    if (temporizadorAtaque >= tiempoEntreGolpes)
                    {
                        EjecutarAtaque();
                        temporizadorAtaque = 0f;
                    }
                }
                // 2. Si está en rango pero más lejos: persigue respetando su piso
                else
                {
                    float dirX = Mathf.Sign(jugador.position.x - transform.position.x);
                    rb.linearVelocity = new Vector2(dirX * velocidadPersecucion, rb.linearVelocity.y);
                    MirarHacia(jugador.position.x);
                }
            }
        }

        // 3. Si el jugador NO está en su piso o está fuera de sus límites: patrulla tranquilo
        if (!jugadorValidoEnSuPiso)
        {
            Patrullar();
        }
    }

    private void Patrullar()
    {
        float dirX = moviendoHaciaDerecha ? velocidadPatrulla : -velocidadPatrulla;
        rb.linearVelocity = new Vector2(dirX, rb.linearVelocity.y);

        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = !moviendoHaciaDerecha;
        }

        // Cambios de sentido al alcanzar la distancia máxima
        if (transform.position.x >= limiteDerechoX && moviendoHaciaDerecha)
        {
            moviendoHaciaDerecha = false;
        }
        else if (transform.position.x <= limiteIzquierdoX && !moviendoHaciaDerecha)
        {
            moviendoHaciaDerecha = true;
        }
    }

    private void MirarHacia(float objetivoX)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = objetivoX < transform.position.x;
        }
    }

    private void EjecutarAtaque()
    {
        if (jugador == null) return;

        PlayerHealth salud = jugador.GetComponent<PlayerHealth>();
        if (salud != null)
        {
            salud.RecibirDanio(1);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Rango de patrulla fija (Línea verde)
        Gizmos.color = Color.green;
        Vector3 inicio = Application.isPlaying ? new Vector3(limiteIzquierdoX, transform.position.y, 0) : transform.position + Vector3.left * distanciaPatrulla;
        Vector3 fin = Application.isPlaying ? new Vector3(limiteDerechoX, transform.position.y, 0) : transform.position + Vector3.right * distanciaPatrulla;
        Gizmos.DrawLine(inicio, fin);

        // Rango de visión (Amarillo)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoVision);

        // Rango de ataque (Rojo)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaAtaque);
    }
}