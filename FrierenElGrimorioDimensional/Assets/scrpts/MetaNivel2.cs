using UnityEngine;
using UnityEngine.SceneManagement;

public class MetaNivel2 : MonoBehaviour
{
    [Header("Configuración de Escena")]
    [SerializeField] private string nombreSiguienteNivel = "Nivel_03";

    private bool nivelCargado = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Detecta la llegada de la Piedra mediante Tag o componente físico
        if (!nivelCargado && (collision.CompareTag("Player") || collision.GetComponent<PlayerMovement>() != null))
        {
            nivelCargado = true;
            SceneManager.LoadScene(nombreSiguienteNivel); // ✅ Usa la variable correctamente
        }
    }
}