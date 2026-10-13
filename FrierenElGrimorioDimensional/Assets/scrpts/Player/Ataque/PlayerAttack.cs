using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Configuración de Ataque")]
    [SerializeField] private GameObject prefabProyectil;
    [SerializeField] private float cadenciaDisparo = 0.35f;
    [SerializeField] private float distanciaSeparacionX = 0.8f;
    [SerializeField] private float alturaDisparoY = 0.0f;

    private SpriteRenderer spriteRenderer;
    private PlayerMovement playerMovement;
    private Collider2D[] misColliders;
    private float tiempoSiguienteDisparo = 0f;
    private float direccionDisparoX = 1f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerMovement = GetComponent<PlayerMovement>();
        misColliders = GetComponentsInChildren<Collider2D>();
    }

    private void Update()
    {
        if (playerMovement != null && playerMovement.EstaMuerto) return;

        // 1. Detectar dirección horizontal activa por teclado
        float inputHorizontal = Input.GetAxisRaw("Horizontal");
        if (inputHorizontal > 0.05f)
        {
            direccionDisparoX = 1f;
        }
        else if (inputHorizontal < -0.05f)
        {
            direccionDisparoX = -1f;
        }
        else if (spriteRenderer != null)
        {
            direccionDisparoX = spriteRenderer.flipX ? -1f : 1f;
        }

        // 2. Disparar con Clic Izquierdo o Tecla J
        if ((Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.J)) && Time.time >= tiempoSiguienteDisparo)
        {
            Disparar();
            tiempoSiguienteDisparo = Time.time + cadenciaDisparo;
        }
    }

    private void Disparar()
    {
        if (prefabProyectil == null) return;

        Vector2 direccion = new Vector2(direccionDisparoX, 0f);
        Vector3 posicionSpawn = transform.position + new Vector3(direccionDisparoX * distanciaSeparacionX, alturaDisparoY, 0f);

        GameObject balaObj = Instantiate(prefabProyectil, posicionSpawn, Quaternion.identity);

        // Ignora colisión con todos los colliders del jugador
        Collider2D balaCol = balaObj.GetComponent<Collider2D>();
        if (balaCol != null && misColliders != null)
        {
            foreach (Collider2D col in misColliders)
            {
                if (col != null)
                {
                    Physics2D.IgnoreCollision(col, balaCol, true);
                }
            }
        }

        PlayerProyectil proyectil = balaObj.GetComponent<PlayerProyectil>();
        if (proyectil != null)
        {
            proyectil.Inicializar(direccion);
        }
    }
}