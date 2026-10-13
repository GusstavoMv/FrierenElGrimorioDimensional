using UnityEngine;

public class ZonaMuerte : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Detecta al jugador por su Tag o buscando su componente de físicas
        if (collision.CompareTag("Player") || collision.GetComponent<PlayerMovement>() != null)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.DerrotaJugador();
            }
            else
            {
                Debug.LogError("Error: No existe el objeto GameManager en la Hierarchy con el script asignado.");
            }
        }
    }
}