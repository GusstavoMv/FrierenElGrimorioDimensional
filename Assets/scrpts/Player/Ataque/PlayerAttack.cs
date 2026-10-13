using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Configuración de Ataque")]
    [SerializeField] private GameObject prefabProyectil;
    [SerializeField] private Transform puntoDisparo;
    [SerializeField] private float cadenciaDisparo = 0.4f;

    private SpriteRenderer spriteRenderer;
    private PlayerMovement playerMovement;
    private float tiempoSiguienteDisparo = 0f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (playerMovement != null && playerMovement.EstaMuerto) return;

        // Disparo con Clic izquierdo o tecla J
        if ((Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.J)) && Time.time >= tiempoSiguienteDisparo)
        {
            Disparar();
            tiempoSiguienteDisparo = Time.time + cadenciaDisparo;
        }
    }

    private void Disparar()
    {
        if (prefabProyectil == null) return;

        // Determinar dirección horizontal
        Vector2 direccion = (spriteRenderer != null && spriteRenderer.flipX) ? Vector2.left : Vector2.right;
        Vector3 posicionSpawn = (puntoDisparo != null) ? puntoDisparo.position : transform.position;

        GameObject proyectilObj = Instantiate(prefabProyectil, posicionSpawn, Quaternion.identity);
        
        // Aquí se usa PlayerProyectil para coincidir con tu archivo
        PlayerProyectil proyectil = proyectilObj.GetComponent<PlayerProyectil>();
        if (proyectil != null)
        {
            proyectil.Inicializar(direccion);
        }
    }
}