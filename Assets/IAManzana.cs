using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAManzana : MonoBehaviour
{
    [SerializeField]
    Detect<Entity> detect;
    [SerializeField]
    Entity param;

    private void Update()
    {
        var depredadores = detect.Area(param.transform.position, (algo) => { return Team.hervivoro == algo.team; });
        Debug.Log("depredadores " + depredadores.Length);
        foreach (var depredador in depredadores)
        {
            var aux = depredador.GetComponent<IABoid>();
            aux.steerings["frutas"].targets.Remove(transform);
            Debug.Log("depredador " + depredador.name);
            gameObject.SetActive(false);
        }
    }


}
