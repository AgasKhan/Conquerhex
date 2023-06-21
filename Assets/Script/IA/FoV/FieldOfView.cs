using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [Range(0, 360)] public float viewAngle;

    [SerializeField]
    Detect<Transform> detect;

    [HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();

    private void Start()
    {
        detect.dot = Mathf.Cos(viewAngle);
        StartCoroutine("FindTargetsWithDelay", .2f);
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while(true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    void FindVisibleTargets()
    {
        visibleTargets.Clear();

        visibleTargets.AddRange(detect.ConeWithRay(transform, (toChck)=> { return toChck != transform;}));
    }
}
