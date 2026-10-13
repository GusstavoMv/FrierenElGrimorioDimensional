using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HUDVidas : MonoBehaviour
{
    [Header("Iconos de Corazones (En orden 1, 2, 3)")]
    [SerializeField] private Image[] corazones;

    [Header("Animación de Desvanecimiento")]
    [SerializeField] private float duracionEfecto = 0.3f;

    private void OnEnable()
    {
        PlayerHealth.OnVidasModificadas += ActualizarCorazones;
    }

    private void OnDisable()
    {
        PlayerHealth.OnVidasModificadas -= ActualizarCorazones;
    }

    private void ActualizarCorazones(int vidasRestantes)
    {
        if (corazones == null) return;

        for (int i = 0; i < corazones.Length; i++)
        {
            if (corazones[i] == null) continue;

            // Si el corazón debe apagarse y sigue visible en pantalla
            if (i >= vidasRestantes && corazones[i].gameObject.activeSelf)
            {
                StartCoroutine(EfectoDesaparecer(corazones[i]));
            }
            // Si el nivel se reinicia o tiene vida completa
            else if (i < vidasRestantes)
            {
                corazones[i].gameObject.SetActive(true);
                corazones[i].color = Color.white;
                corazones[i].transform.localScale = Vector3.one;
            }
        }
    }

    private IEnumerator EfectoDesaparecer(Image imgCorazon)
    {
        Transform trans = imgCorazon.transform;
        Vector3 escalaBase = trans.localScale;
        Color colorBase = imgCorazon.color;
        float tiempo = 0f;

        // 1. Pop y destello rojo
        trans.localScale = escalaBase * 1.3f;
        imgCorazon.color = new Color(1f, 0.2f, 0.2f, 1f);
        yield return new WaitForSeconds(0.06f);

        // 2. Encogimiento y reducción de opacidad (Alpha)
        while (tiempo < duracionEfecto)
        {
            tiempo += Time.deltaTime;
            float progreso = tiempo / duracionEfecto;

            trans.localScale = Vector3.Lerp(escalaBase * 1.3f, Vector3.zero, progreso);
            imgCorazon.color = new Color(colorBase.r, colorBase.g, colorBase.b, Mathf.Lerp(1f, 0f, progreso));
            yield return null;
        }

        imgCorazon.gameObject.SetActive(false);
    }
}