using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSystem : MonoBehaviour
{
    public void Jugar()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void Salir()
    {
        Application.Quit();
        Debug.Log("Juego cerrado");
    }
}