using UnityEngine;

public class BalaMarine : MonoBehaviour
{
    [SerializeField] private float velocidad = 6f;
    [SerializeField] private float tiempoVida = 4f;

    private void Start()
    {
        // Se destruye sola a los 4 segundos si no choca con nada
        Destroy(gameObject, tiempoVida);
    }

    private void Update()
    {
        // Avanza hacia donde esté apuntando
        transform.Translate(Vector2.right * velocidad * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si choca con el jugador
        if (collision.CompareTag("Player") || collision.GetComponent<PlayerMovement>() != null)
        {
            PlayerHealth salud = collision.GetComponent<PlayerHealth>();
            if (salud != null)
            {
                salud.RecibirDanio(1);
            }
            Destroy(gameObject);
        }
        // Si choca con una pared o suelo
        else if (!collision.isTrigger && !collision.CompareTag("Enemigo"))
        {
            Destroy(gameObject);
        }
    }
}