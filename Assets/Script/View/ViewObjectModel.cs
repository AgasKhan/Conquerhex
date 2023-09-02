using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewObjectModel : MonoBehaviour
{
    [SerializeReference]
    public IViewController[] controllers;

    [SerializeField]
    public Renderer originalRender;

    [field: SerializeField]
    public bool isTransparent { get; private set; }

    [SerializeField]
    protected Material transparentMaterial;

    [field: SerializeField]
    public bool defaultRight { get; private set; }

    public System.Action<ViewObjectModel, Transform[]> onCloneAndSuscribe;

    EventGeneric eventGeneric;

    public virtual Transform[] Proyections()
    {
        var ret = new Transform[6];

        for (int i = 0; i < ret.Length; i++)
        {
            ret[i] = new GameObject("Proyection " + i).transform;
        }

        onCloneAndSuscribe?.Invoke(this, ret);

        return ret;
    }

    protected virtual void Awake()
    {
        originalRender.material = transparentMaterial;

        originalRender.sortingOrder = Mathf.RoundToInt(transform.position.y * -100);

        eventGeneric = EventManager.events.SearchOrCreate<EventGeneric>("move");

        controllers = GetComponents<IViewController>();

        for (int i = 0; i < controllers.Length; i++)
        {
            controllers[i].OnEnterState(this);
        }
    }

    protected virtual void OnEnable()
    {
        if (isTransparent)
            eventGeneric.action += UpdateTransparent;
    }

    private void OnDisable()
    {
        if (isTransparent)
            eventGeneric.action -= UpdateTransparent;
    }

    private void UpdateTransparent(params object[] param)
    {
        if (!gameObject.activeSelf)
            return;

        Vector3 posPlayer = (Vector3)param[0];

        if (posPlayer.y > transform.position.y)
        {
            originalRender.material.SetInt("_transparent", 1);
        }
        else
        {
            originalRender.material.SetInt("_transparent", 0);
        }
    }

    public interface IViewController : IState<ViewObjectModel> { }
}

