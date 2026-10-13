using UnityEngine;

public class SaludEnemigo : MonoBehaviour
{
    [SerializeField] private int vida = 2;

    public void RecibirDanio(int danio)
    {
        vida -= danio;
        Debug.Log($"{gameObject.name} recibió daño. Vida restante: {vida}");

        if (vida <= 0)
        {
            Morir();
        }
    }

    private void Morir()
    {
        Destroy(gameObject);
    }
}