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

        if (direccion.x < 0)
        {
            Vector3 escala = transform.localScale;
            escala.x = -Mathf.Abs(escala.x);
            transform.localScale = escala;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.GetComponent<PlayerMovement>() != null) return;
        if (collision.GetComponent<ZonaMuerte>() != null) return;

        // Si choca con un enemigo
        if (collision.CompareTag("Enemigo"))
        {
            // 1. Primero intenta impactar en el Escudo Protoss si lo tiene
            EscudoProtoss escudo = collision.GetComponent<EscudoProtoss>();
            if (escudo != null && escudo.TieneEscudoActivo())
            {
                escudo.RecibirImpacto(danio);
                Destroy(gameObject);
                return;
            }

            // 2. Si el escudo está roto o no tiene, daña la vida del enemigo
            SaludEnemigo vidaEnemigo = collision.GetComponent<SaludEnemigo>();
            if (vidaEnemigo != null)
            {
                vidaEnemigo.RecibirDanio(danio);
            }
            else
            {
                // Si no tiene script de vida aún, destruye al enemigo directamente
                Destroy(collision.gameObject);
            }

            Destroy(gameObject);
            return;
        }

        // Choca contra suelo o plataformas
        if (!collision.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}