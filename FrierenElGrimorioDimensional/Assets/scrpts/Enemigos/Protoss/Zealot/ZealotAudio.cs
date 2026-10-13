using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ZealotAudio : MonoBehaviour
{
    [Header("Voces del Zealot")]
    [SerializeField] private AudioClip[] frasesZealot;

    [Header("Efectos de Ataque")]
    [SerializeField] private AudioClip[] sonidosAtaque; // Sonidos de cuchillas/ataque Protoss

    [Header("Rango de Audición")]
    [SerializeField] private float radioAudicion = 8.0f;

    [Header("Silencio entre frases")]
    [SerializeField] private float pausaMinima = 3.0f;
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
        audioSource.spatialBlend = 0f; // 2D limpio
    }

    private void Start()
    {
        GameObject objJugador = GameObject.FindGameObjectWithTag("Player");
        if (objJugador != null)
        {
            jugador = objJugador.transform;
        }

        if (frasesZealot != null && frasesZealot.Length > 0)
        {
            rutinaVoces = StartCoroutine(CicloVoces());
        }
    }

    private IEnumerator CicloVoces()
    {
        while (!estaMuerto)
        {
            float tiempoEspera = Random.Range(pausaMinima, pausaMaxima);
            yield return new WaitForSeconds(tiempoEspera);

            if (estaMuerto) yield break;

            if (EstaEnRangoDeAudicion())
            {
                int nuevoIndice = ElegirIndiceAleatorio();
                AudioClip clip = frasesZealot[nuevoIndice];

                if (clip != null)
                {
                    audioSource.clip = clip;
                    audioSource.Play();
                    yield return new WaitForSeconds(clip.length);
                }
            }
        }
    }

    // Llama a esta función para que suene el corte/cuchillada psiónica
    public void ReproducirSonidoAtaque()
    {
        if (estaMuerto) return;

        if (sonidosAtaque != null && sonidosAtaque.Length > 0)
        {
            int index = Random.Range(0, sonidosAtaque.Length);
            if (sonidosAtaque[index] != null)
            {
                audioSource.pitch = Random.Range(0.9f, 1.1f);
                audioSource.PlayOneShot(sonidosAtaque[index], 0.9f);
                audioSource.pitch = 1.0f;
            }
        }
    }

    // Si la Piedra choca físicamente contra el Zealot, suena el ataque y te resta 1 corazón
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (estaMuerto) return;

        if (collision.gameObject.CompareTag("Player") || collision.gameObject.GetComponent<PlayerMovement>() != null)
        {
            ReproducirSonidoAtaque();

            PlayerHealth salud = collision.gameObject.GetComponent<PlayerHealth>();
            if (salud != null)
            {
                salud.RecibirDanio(1);
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
        if (frasesZealot.Length <= 1) return 0;
        int indice;
        do
        {
            indice = Random.Range(0, frasesZealot.Length);
        } while (indice == ultimoIndice);

        ultimoIndice = indice;
        return indice;
    }

    public void Morir()
    {
        if (estaMuerto) return;
        estaMuerto = true;

        if (rutinaVoces != null) StopCoroutine(rutinaVoces);
        audioSource.Stop();

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = false;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        MonoBehaviour scriptAtaque = GetComponent("ZealotAtack") as MonoBehaviour;
        if (scriptAtaque != null) scriptAtaque.enabled = false;

        if (sonidoMuerte != null)
        {
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
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radioAudicion);
    }
}