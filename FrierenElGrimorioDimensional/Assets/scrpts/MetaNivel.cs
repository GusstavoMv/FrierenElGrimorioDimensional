using UnityEngine;
using UnityEngine.SceneManagement;

public class MetaNivel : MonoBehaviour
{
    private bool nivelCargado = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si entra la Piedra (o cualquier objeto con tag Player)
        if (!nivelCargado && (collision.CompareTag("Player") || collision.GetComponent<PlayerMovement>() != null))
        {
            nivelCargado = true;
            SceneManager.LoadScene("Nivel_02");
        }
    }
}