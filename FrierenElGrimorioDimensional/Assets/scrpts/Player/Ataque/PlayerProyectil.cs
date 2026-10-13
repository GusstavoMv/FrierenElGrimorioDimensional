using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerProyectil : MonoBehaviour
{
    [Header("Ajustes Físicos")]
    [SerializeField] private float velocidad = 12f;
    [SerializeField] private float tiempoVida = 3f;
    [SerializeField] private int danio = 1;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
    }

    private void Start()
    {
        Destroy(gameObject, tiempoVida);
    }

    public void Inicializar(Vector2 direccion)
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = direccion.normalized * velocidad;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Ignorar jugador, sus componentes y zonas de muerte
        if (collision.CompareTag("Player") ||
            collision.GetComponent<PlayerMovement>() != null ||
            collision.GetComponent<PlayerHealth>() != null)
        {
            return;
        }

        if (collision.GetComponent<ZonaMuerte>() != null) return;

        // 2. Colisión con enemigos
        if (collision.CompareTag("Enemigo"))
        {
            // Escudo Protoss
            EscudoProtoss escudo = collision.GetComponent<EscudoProtoss>();
            if (escudo != null && escudo.TieneEscudoActivo())
            {
                escudo.RecibirImpacto(danio);
                Destroy(gameObject);
                return;
            }

            // Marine Audio
            MarineAudio audioMarine = collision.GetComponent<MarineAudio>();
            if (audioMarine != null)
            {
                audioMarine.Morir();
                Destroy(gameObject);
                return;
            }

            // Salud genérica de enemigo
            SaludEnemigo vidaEnemigo = collision.GetComponent<SaludEnemigo>();
            if (vidaEnemigo != null)
            {
                vidaEnemigo.RecibirDanio(danio);
            }
            else
            {
                Destroy(collision.gameObject);
            }

            Destroy(gameObject);
            return;
        }

        // 3. Destrucción contra paredes/suelo del Tilemap
        if (!collision.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}