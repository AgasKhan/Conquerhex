using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ComponentsAndContainers
{
    public abstract class Container<ChildContainer> : MyScripts, ISearchComponent<ChildContainer>
        where ChildContainer : Container<ChildContainer> //ChildContainer es el hijo de Container
    {
        public ChildContainer container => (ChildContainer)this; //el hijo

        SuperDickLite<IComponent<ChildContainer>> _container = new SuperDickLite<IComponent<ChildContainer>>();

        //Dictionary<System.Type, IComponent<ChildContainer>> _container = new Dictionary<System.Type, IComponent<ChildContainer>>();

        public bool TryGetInContainer<T>(out T component) where T : IComponent<ChildContainer>
        {
            var b = _container.TryGetValue(typeof(T), out var comp);

            component = (T)comp;

            return b;
        }

        public T GetInContainer<T>() where T : IComponent<ChildContainer>
        {
            return (T)_container[typeof(T)];
        }

        public void RemoveInContainer<T>() where T : IComponent<ChildContainer>
        {
            _container[typeof(T)].OnExitState(container);

            _container[typeof(T)] = null;

            //var component = GetInContainer<T>();

            //_container.Remove(component.GetType());

            //component.OnExitState(container);
        }

        public void AddInContainer<T>(T component) where T : IComponent<ChildContainer>
        {
            _container[component.GetType()] = component;

            //_container.Add(component.GetType(), component);

            component.OnEnterState(container);
        }

        protected override void Config()
        {
            MyAwakes += MyAwake;
            //MyStarts += MyStart;
        }

        void MyAwake()
        {
            foreach (var component in GetComponents<IComponent<ChildContainer>>())
            {
                _container[component.GetType()] = component;

                component.OnSetContainer(container);

                //Debug.Log(component.name);

                //_container.Add(component.GetType(), component);
            }

            foreach (var component in _container)
            {
                component.OnEnterState(container);
            }
        }

        /*
        void MyStart()
        {
            foreach (var component in _container)
            {
                component.OnEnterState(container);
            }
        }*/
    }

    public interface ISearchComponent<Container> 
        where Container : Container<Container>
    {
        public Container container { get; }

        public T GetInContainer<T>() where T : IComponent<Container>;

        public void RemoveInContainer<T>() where T : IComponent<Container>;

        public void AddInContainer<T>(T component) where T : IComponent<Container>;

        public bool TryGetInContainer<T>(out T component) where T : IComponent<Container>;
    }

    public interface IComponent<Container> : ISearchComponent<Container>, IState<Container> 
        where Container : Container<Container>
    {
        public void OnSetContainer(Container param);
    }


    public abstract class ComponentOfContainer<Container> : MonoBehaviour, IComponent<Container> 
        where Container : Container<Container>
    {
        public Container container { get; protected set; }

        public virtual void OnSetContainer(Container param)
        {
            container = param;
        }

        public T GetInContainer<T>() where T : IComponent<Container> => container.GetInContainer<T>();

        public void RemoveInContainer<T>() where T : IComponent<Container> => container.RemoveInContainer<T>();

        public void AddInContainer<T>(T component) where T : IComponent<Container> => container.AddInContainer(component);

        public bool TryGetInContainer<T>(out T component) where T : IComponent<Container> => container.TryGetComponent(out component);

        public abstract void OnEnterState(Container param);

        public abstract void OnStayState(Container param);

        public abstract void OnExitState(Container param);
    }


}


