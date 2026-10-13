using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class CameraShakePerlin : MonoBehaviour
{
    [SerializeField] private CinemachineCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin noise;

    void Start()
    {
        // 1. Si no arrastraste la cámara a la casilla, la busca en este mismo objeto
        if (virtualCamera == null)
        {
            virtualCamera = GetComponent<CinemachineCamera>();
        }

        // 2. Busca el componente Perlin en este objeto o en sus hijos
        if (virtualCamera != null)
        {
            noise = virtualCamera.GetComponentInChildren<CinemachineBasicMultiChannelPerlin>();
        }

        if (noise == null)
        {
            Debug.LogError("No se encontró CinemachineBasicMultiChannelPerlin en " + gameObject.name);
        }
    }

    public void Shake(float intensity, float duration)
    {
        if (noise != null)
        {
            // Detiene rutinas previas por si saltas varias veces seguidas
            StopAllCoroutines();
            StartCoroutine(ShakeRoutine(intensity, duration));
        }
    }

    private IEnumerator ShakeRoutine(float intensity, float duration)
    {
        noise.AmplitudeGain = intensity;
        noise.FrequencyGain = 10f; // <-- OBLIGATORIO: Frecuencia alta para que vibre rápido
        
        yield return new WaitForSeconds(duration);
        
        noise.AmplitudeGain = 0f;
        noise.FrequencyGain = 0f; // Apaga la oscilación
    }
}