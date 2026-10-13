using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MarineAtack : MonoBehaviour
{
    [Header("Configuración de Disparo Recto")]
    [SerializeField] private GameObject prefabBala;
    [SerializeField] private float cadenciaDisparo = 2.0f;
    [SerializeField] private float rangoVision = 9.0f;
    [SerializeField] private Vector2 offsetDisparo = new Vector2(0.6f, 0.1f);

    [Header("Orientación del Sprite Original")]
    [Tooltip("Marca esto si el dibujo original de tu Marine mira hacia la izquierda por defecto")]
    [SerializeField] private bool spriteOriginalMiraIzquierda = false;

    private Transform jugador;
    private SpriteRenderer spriteRenderer;
    private float temporizadorDisparo = 0f;
    private bool estaMuerto = false;

    private void Awake()
    {
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
        if (estaMuerto || jugador == null) return;

        temporizadorDisparo += Time.deltaTime;

        float distancia = Vector2.Distance(transform.position, jugador.position);

        // 1. Mirar siempre hacia la Piedra si está en rango
        if (distancia <= rangoVision)
        {
            bool jugadorALaDerecha = jugador.position.x >= transform.position.x;
            ActualizarOrientacion(jugadorALaDerecha);

            // 2. Disparar recto horizontal en esa misma dirección
            if (temporizadorDisparo >= cadenciaDisparo && prefabBala != null)
            {
                DispararRecto(jugadorALaDerecha);
                temporizadorDisparo = 0f;
            }
        }
    }

    private void ActualizarOrientacion(bool haciaDerecha)
    {
        if (spriteRenderer == null) return;

        if (spriteOriginalMiraIzquierda)
        {
            // Si el dibujo original mira a la izquierda: se voltea para mirar a la derecha
            spriteRenderer.flipX = haciaDerecha;
        }
        else
        {
            // Si el dibujo original mira a la derecha: se voltea para mirar a la izquierda
            spriteRenderer.flipX = !haciaDerecha;
        }
    }

    private void DispararRecto(bool haciaDerecha)
    {
        // Calcula la salida del proyectil desde el cañón según el lado al que apunta
        float posX = haciaDerecha ? Mathf.Abs(offsetDisparo.x) : -Mathf.Abs(offsetDisparo.x);
        Vector3 posicionSalida = transform.position + new Vector3(posX, offsetDisparo.y, 0f);

        // Rotación horizontal pura: 0° hacia la derecha, 180° hacia la izquierda
        float angulo = haciaDerecha ? 0f : 180f;
        Quaternion rotacion = Quaternion.Euler(0, 0, angulo);

        Instantiate(prefabBala, posicionSalida, rotacion);
    }

    // Si la Piedra lo toca físicamente: la Piedra pierde 1 vida
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (estaMuerto) return;

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
        Gizmos.DrawWireSphere(transform.position, rangoVision);
    }
}