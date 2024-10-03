using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTrailByHandSpeed : MonoBehaviour
{
    public ParticleSystem particle;
    public float speedThreshold = 5f;
    [SerializeField]
    private Transform handTransform;
    private Vector3 lastPosition;
    private float currentSpeed;

    void Start()
    {
        // Busca el transform de la mano (derecha o izquierda) en los ancestros del arma
        handTransform = SearchInParentForMultipleNames(transform, new string[] { "QuickRigCharacter_RightHand", "QuickRigCharacter_LeftHand" });

        if (handTransform != null)
        {
            //Debug.Log("Transform de la mano encontrado: " + handTransform.name);
            lastPosition = handTransform.position;
        }
        //else
        //{
        //    Debug.LogError("No se pudo encontrar el Transform del hueso de la mano.");
        //}
    }

    void Update()
    {
        if (handTransform == null) return;

        // Calcula la velocidad en base al cambio de posición de la mano
        currentSpeed = (handTransform.position - lastPosition).magnitude / Time.deltaTime;

        // Actualiza la posición anterior
        lastPosition = handTransform.position;

        // Activa o desactiva las partículas en función de la velocidad
        if (currentSpeed >= speedThreshold)
        {
            EnableParticleWithTrail();
        }
        else
        {
            DisableParticleWithTrail();
        }
    }

    // Función para buscar múltiples nombres en los padres del arma
    Transform SearchInParentForMultipleNames(Transform current, string[] names)
    {
        while (current != null)
        {
            //Debug.Log("Buscando en: " + current.name); // Mostrar cada nombre de Transform por el que pasa

            // Verifica si el nombre actual es alguno de los buscados
            foreach (string name in names)
            {
                if (current.name == name)
                {
                    return current;
                }
            }
            current = current.parent;
        }
        return null;
    }


    void EnableParticleWithTrail()
    {
        if (!particle.isPlaying)
        {
            particle.Play();
        }
    }

    void DisableParticleWithTrail()
    {
        if (particle.isPlaying)
        {
            particle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }
}
