using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewObjectModel : MonoBehaviour
{
    [SerializeField]
    NewEventManager eventsManager;

    public IViewController[] controllers;

    [SerializeField]
    public Renderer originalRender;

    [field: SerializeField]
    public bool isTransparent { get; private set; }

    [SerializeField]
    protected Material transparentMaterial;

    [field: SerializeField]
    public bool defaultRight { get; private set; }

    EventParam<Vector3> eventGeneric;

    bool _isTransparent;

    protected virtual void Start()
    {
        //originalRender.material = transparentMaterial;

        originalRender.sortingOrder = Mathf.RoundToInt(transform.position.y * -100);

        eventGeneric = eventsManager.events.SearchOrCreate<EventParam<Vector3>>("move");

        controllers = GetComponents<IViewController>();

        for (int i = 0; i < controllers.Length; i++)
        {
            controllers[i].OnEnterState(this);
        }

        if (isTransparent)
            eventGeneric.delegato += UpdateTransparent;
    }

    /*
    protected virtual void OnEnable()
    {
        if (isTransparent && eventGeneric != null)
            eventGeneric.delegato += UpdateTransparent;
    }

    private void OnDisable()
    {
        if (isTransparent && eventGeneric!= null)
            eventGeneric.delegato -= UpdateTransparent;
    }
    */

    private void UpdateTransparent(Vector3 posPlayer)
    {
        if (!gameObject.activeSelf)
            return;

        SetTransparent(posPlayer.y > transform.position.y);
    }

    private void SetTransparent(bool b)
    {
        if (_isTransparent == b)
            return;

        _isTransparent = b;

        originalRender.material.SetInt("_transparent", b? 1:0);
    }

    public interface IViewController : IState<ViewObjectModel> { }
}

