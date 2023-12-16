using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewObjectModel : MonoBehaviour
{
    public IViewController[] controllers;

    [SerializeField]
    public Renderer originalRender;

    [field: SerializeField]
    public bool isTransparent { get; private set; }

    [SerializeField]
    protected Material transparentMaterial;

    [field: SerializeField]
    public bool defaultRight { get; private set; }

    EventGeneric eventGeneric;

    bool _isTransparent;

    protected virtual void Awake()
    {
        //originalRender.material = transparentMaterial;

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

