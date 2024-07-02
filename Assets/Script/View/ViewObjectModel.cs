using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewObjectModel : MonoBehaviour
{
    public class OptimizedView : MonoBehaviour
    {
        public ViewObjectModel parent;

        private void OnBecameInvisible()
        {
            parent.SetActiveChildren(false);
        }
    }

    [SerializeField]
    EventManager eventsManager;

    public IViewController[] controllers;

    [SerializeField]
    public Renderer[] originalRenders;

    /*
    [field: SerializeField]
    public bool isTransparent { get; private set; }
    */

    public Renderer originalRender => originalRenders.Length == 1 ? originalRenders[0] : null;

    /*
    [SerializeField]
    protected Material transparentMaterial;
    */

    [field: SerializeField]
    public bool defaultRight { get; private set; }

    SingleEvent<Vector3> eventGeneric;

    //bool _isTransparent;

    Animator[] animators;

    Timer inViewTimer;

    public void SetActiveChildren(bool b)
    {
        foreach (var item in originalRenders)
        {
            item.enabled = b;
        }
        foreach (var item in animators)
        {
            item.enabled = b;
        }
    }

    bool CheckInView()
    {
        foreach (var item in MainCamera.instance.pointsInWorld)
        {
            if (this.SqrDistance(item) < 25*25)
                return true;

            /*
            Vector3 pos = item.WorldToViewportPoint(transform.position);

            if ((pos.z > 0 && pos.x >= -1f && pos.x <= 2f && pos.y >= -1f && pos.y <= 2f))
                return true;
            */
        }

        return false;
    }

    void Check()
    {
        if (CheckInView())
            SetActiveChildren(true);
        else
            SetActiveChildren(false);
    }

    private void OnEnable()
    {
        inViewTimer?.Reset();
    }

    private void OnDisable()
    {
        inViewTimer?.Stop();
    }

    private void Awake()
    {
        //originalRender.material = transparentMaterial;

        //originalRender.sortingOrder = Mathf.RoundToInt(transform.position.y * -100);

        //eventGeneric = eventsManager.events.SearchOrCreate<SingleEvent<Vector3>>("move");

        animators = GetComponentsInChildren<Animator>();

        controllers = GetComponents<IViewController>();

        for (int i = 0; i < controllers.Length; i++)
        {
            controllers[i].OnEnterState(this);
        }

        /*
        for (int i = 0; i < originalRenders.Length; i++)
        {
            var optimized = originalRenders[i].gameObject.AddComponent<OptimizedView>();

            optimized.parent = this;
        }
        */

        /*
        if (isTransparent && originalRender!=null)
            eventGeneric.delegato += UpdateTransparent;
        */

        //SetActiveChildren(false);

        inViewTimer = TimersManager.Create(0.5f, Check).SetLoop(true).Stop();
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


    /*
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

        //originalRender.material.SetInt("_transparent", b? 1:0);
    }
    */


    

    public interface IViewController : IState<ViewObjectModel> { }
}



