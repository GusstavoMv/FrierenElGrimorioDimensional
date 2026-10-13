using UnityEngine;
using System.Collections;

public class SonidosHydralisco : MonoBehaviour
{
    [Header("Audios del Hydralisco")]
    public AudioClip[] rugidos; // Lista para meter todos los sonidos que quieras
    private AudioSource audioSource;

    [Header("Tiempos de espera (Segundos)")]
    public float tiempoMinimo = 3f; // Lo mínimo que espera para sonar
    public float tiempoMaximo = 7f; // Lo máximo que espera

    void Start()
    {
        // Obtiene o crea el reproductor de audio
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Configuración para que no suene de golpe ni se repita en bucle
        audioSource.playOnAwake = false;
        audioSource.loop = false;

        // Inicia la rutina de sonidos
        StartCoroutine(RutinaRugidosAleatorios());
    }

    IEnumerator RutinaRugidosAleatorios()
    {
        while (true)
        {
            // 1. Espera un tiempo aleatorio entre el mínimo y el máximo
            float tiempoEspera = Random.Range(tiempoMinimo, tiempoMaximo);
            yield return new WaitForSeconds(tiempoEspera);

            // 2. Si hay audios cargados, elige uno al azar y lo reproduce
            if (rugidos.Length > 0)
            {
                int indiceAleatorio = Random.Range(0, rugidos.Length);
                audioSource.PlayOneShot(rugidos[indiceAleatorio]);
            }
        }
    }
}