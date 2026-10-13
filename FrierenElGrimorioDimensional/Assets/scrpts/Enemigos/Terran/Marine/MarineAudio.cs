using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MarineAudio : MonoBehaviour
{
    [Header("Banco de Voces Aleatorias")]
    [SerializeField] private AudioClip[] audiosAleatorios;

    [Header("Rango de Audición")]
    [Tooltip("Distancia máxima a la que debe estar la Piedra para que el Marine hable")]
    [SerializeField] private float radioAudicion = 7.0f;

    [Header("Silencio entre frases")]
    [Tooltip("Tiempo mínimo de espera después de que termine de hablar")]
    [SerializeField] private float pausaMinima = 3.0f;
    [Tooltip("Tiempo máximo de espera después de que termine de hablar")]
    [SerializeField] private float pausaMaxima = 7.0f;

    [Header("Sonido de Muerte")]
    [SerializeField] private AudioClip sonidoMuerte;

    private AudioSource audioSource;
    private Transform jugador;
    private bool estaMuerto = false;
    private Coroutine rutinaVoces;
    private int ultimoIndice = -1;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D puro para sonar limpio y consistente
    }

    private void Start()
    {
        GameObject objJugador = GameObject.FindGameObjectWithTag("Player");
        if (objJugador != null)
        {
            jugador = objJugador.transform;
        }

        if (audiosAleatorios != null && audiosAleatorios.Length > 0)
        {
            rutinaVoces = StartCoroutine(CicloVocesAleatorias());
        }
    }

    private IEnumerator CicloVocesAleatorias()
    {
        while (!estaMuerto)
        {
            // 1. Pausa de espera antes de la siguiente oportunidad de hablar
            float tiempoEspera = Random.Range(pausaMinima, pausaMaxima);
            yield return new WaitForSeconds(tiempoEspera);

            if (estaMuerto) yield break;

            // 2. Comprobar si el jugador está dentro del radio de audición
            if (EstaEnRangoDeAudicion())
            {
                int nuevoIndice = ElegirIndiceAleatorio();
                AudioClip clipSeleccionado = audiosAleatorios[nuevoIndice];

                if (clipSeleccionado != null)
                {
                    audioSource.pitch = Random.Range(0.95f, 1.05f);
                    audioSource.clip = clipSeleccionado;
                    audioSource.Play();

                    // Espera exactamente a que termine la frase antes de continuar
                    yield return new WaitForSeconds(clipSeleccionado.length);
                    audioSource.pitch = 1.0f;
                }
            }
        }
    }

    private bool EstaEnRangoDeAudicion()
    {
        if (jugador == null) return false;
        return Vector2.Distance(transform.position, jugador.position) <= radioAudicion;
    }

    private int ElegirIndiceAleatorio()
    {
        if (audiosAleatorios.Length <= 1) return 0;

        int indice;
        do
        {
            indice = Random.Range(0, audiosAleatorios.Length);
        } while (indice == ultimoIndice);

        ultimoIndice = indice;
        return indice;
    }

    public void Morir()
    {
        if (estaMuerto) return;
        estaMuerto = true;

        // 1. Corta de inmediato cualquier frase que esté reproduciendo
        if (rutinaVoces != null)
        {
            StopCoroutine(rutinaVoces);
        }
        audioSource.Stop();

        // 2. Apagar componentes visuales y físicos para que desaparezca al instante
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = false;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        MonoBehaviour scriptAtaque = GetComponent("MarineAtack") as MonoBehaviour;
        if (scriptAtaque != null) scriptAtaque.enabled = false;

        // 3. El grito de muerte siempre suena a volumen completo y se destruye el objeto al finalizar
        if (sonidoMuerte != null)
        {
            audioSource.pitch = 1.0f;
            audioSource.PlayOneShot(sonidoMuerte, 1.0f);
            Destroy(gameObject, sonidoMuerte.length);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualizar el radio de audición en la vista Scene (Color Cyan)
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radioAudicion);
    }
}