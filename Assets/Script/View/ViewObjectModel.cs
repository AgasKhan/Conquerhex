using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewObjectModel : MonoBehaviour
{
    public class OptimizedView : MonoBehaviour
    {
        public ViewObjectModel parent;

        private void OnBecameVisible()
        {
            parent.SetActiveChildren(true);
        }

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

    public Renderer originalRender => originalRenders.Length == 1 ? originalRenders[0] : null;

    [field: SerializeField]
    public bool defaultRight { get; private set; }

    Animator[] animators;

    public void SetActiveChildren(bool b)
    {
        foreach (var item in animators)
        {
            item.enabled = b;
        }
    }

    private void Awake()
    {
        animators = GetComponentsInChildren<Animator>();

        controllers = GetComponents<IViewController>();

        if(animators!=null && animators.Length>0)
        {
            for (int i = 0; i < originalRenders.Length; i++)
            {
                var optimized = originalRenders[i].gameObject.AddComponent<OptimizedView>();

                optimized.parent = this;
            }

            SetActiveChildren(false);
        }
            

        for (int i = 0; i < controllers.Length; i++)
        {
            controllers[i].OnEnterState(this);
        }
    }

    public interface IViewController : IState<ViewObjectModel> { }
}



