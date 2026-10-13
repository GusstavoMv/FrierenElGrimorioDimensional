using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPausa : MonoBehaviour
{
    [Header("Panel Contenedor")]
    public GameObject menuPausaPanel;

    private bool pausado = false;

    void Start()
    {
        // Nos aseguramos de que empiece oculto por código
        if (menuPausaPanel != null)
        {
            menuPausaPanel.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pausado)
            {
                Continuar();
            }
            else
            {
                Pausar();
            }
        }
    }

    public void Pausar()
    {
        if (menuPausaPanel != null)
        {
            menuPausaPanel.SetActive(true); // Enciende la interfaz
        }
        Time.timeScale = 0f;            // Congela físicas y enemigos
        pausado = true;
    }

    public void Continuar()
    {
        if (menuPausaPanel != null)
        {
            menuPausaPanel.SetActive(false); // Apaga la interfaz
        }
        Time.timeScale = 1f;             // Devuelve el tiempo normal
        pausado = false;
    }

    public void Reiniciar()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SalirAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu"); // Tu escena de la portada
    }
}