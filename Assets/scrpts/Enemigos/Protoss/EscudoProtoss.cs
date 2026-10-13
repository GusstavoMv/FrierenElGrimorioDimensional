using System.Collections;
using UnityEngine;

public class EscudoProtoss : MonoBehaviour
{
    [Header("Ajustes de Escudo")]
    [SerializeField] private int escudoMaximo = 2; // Soporta 2 disparos
    private int escudoActual;

    [Header("Efectos Visuales de Impacto")]
    [SerializeField] private SpriteRenderer visualBurbuja;
    [Tooltip("Tiempo en segundos que brilla el escudo al recibir el golpe")]
    [SerializeField] private float duracionBrilloImpacto = 0.2f; 
    [SerializeField] private float tiempoRegeneracion = 4f;

    private float temporizadorRegen = 0f;
    private Coroutine rutinaBrillo;

    private void Awake()
    {
        escudoActual = escudoMaximo;
        // Inicia apagado para que no sea visible todo el tiempo
        if (visualBurbuja != null)
        {
            visualBurbuja.enabled = false;
        }
    }

    private void Update()
    {
        // Regenera el escudo si pasas un rato sin dispararle
        if (escudoActual < escudoMaximo)
        {
            temporizadorRegen += Time.deltaTime;
            if (temporizadorRegen >= tiempoRegeneracion)
            {
                escudoActual = escudoMaximo;
                temporizadorRegen = 0f;
            }
        }
    }

    public bool TieneEscudoActivo()
    {
        return escudoActual > 0;
    }

    public void RecibirImpacto(int danio)
    {
        temporizadorRegen = 0f;
        escudoActual -= danio;
        if (escudoActual < 0) escudoActual = 0;

        Debug.Log($"¡Escudo golpeado! Puntos restantes: {escudoActual}");

        // Si todavía le quedaba escudo, muestra el destello azul de absorción
        if (visualBurbuja != null)
        {
            if (rutinaBrillo != null) StopCoroutine(rutinaBrillo);
            rutinaBrillo = StartCoroutine(DestelloEscudo());
        }
    }

    private IEnumerator DestelloEscudo()
    {
        visualBurbuja.enabled = true; // Se enciende solo al ser impactado
        yield return new WaitForSeconds(duracionBrilloImpacto);
        visualBurbuja.enabled = false; // Se apaga de inmediato
    }
}