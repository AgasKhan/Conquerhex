using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : SingletonMono<MainCamera>
{
    [System.Serializable]
    public class MapTransform
    {
        public RenderTextureHex[] renders;

        public Transform this[int index]
        {
            get
            {
                return renders[index].cameraRelated.transform;
            }
        }
    }    
    
    public MapTransform cameras;    

    public Transform obj;

    public bool perspective;

    public Vector2[] pointsInScreen;

    public Vector3[] points;

    Vector3[] _points;

    Vector3[] _points2;

    [SerializeField]
    Vector3 rotationPerspective;

    [SerializeField]
    Vector3 vectorPerspective;

    Camera main;

    public RenderTextureHex[] renders
    {
        get => cameras.renders;
    }

    Plane plane;

    protected override void Awake()
    {
        base.Awake();
        main = Camera.main;
        plane = new Plane(Vector3.forward, 0);
    }

    private void OnEnable()
    {
        main.orthographic = !perspective;

        points = new Vector3[pointsInScreen.Length];

        _points = new Vector3[pointsInScreen.Length];

        _points2 = new Vector3[pointsInScreen.Length];

        if (!perspective)
        {
            transform.rotation = Quaternion.identity;
            transform.GetChild(0).localPosition = Vector3.zero;

            for (int i = 0; i < pointsInScreen.Length; i++)
            {
                _points[i] = main.ViewportToWorldPoint(new Vector3(pointsInScreen[i].x, pointsInScreen[i].y, 0));

                _points2[i] = main.ViewportToWorldPoint(new Vector3(pointsInScreen[i].x, pointsInScreen[i].y, main.nearClipPlane));
                Ray ray = new Ray(_points[i], _points2[i] - _points[i]);

                plane.Raycast(ray, out float distance);

                _points2[i] = ray.GetPoint(distance) - main.transform.position;
            }
        }
        else
        {
            transform.rotation = Quaternion.Euler(rotationPerspective);
            transform.GetChild(0).localPosition = vectorPerspective;

            for (int i = 0; i < pointsInScreen.Length; i++)
            {
                _points[i] = main.ViewportToWorldPoint(new Vector3(pointsInScreen[i].x, pointsInScreen[i].y, main.nearClipPlane));

                Ray ray = new Ray(main.transform.position, _points[i] - main.transform.position);

                plane.Raycast(ray, out float distance);

                _points2[i] = ray.GetPoint(distance) - main.transform.position;
            }
        }

        /*
        for (int i = 0; i < cameras.renders.Length; i++)
        {
            cameras[i].SetParent(main.transform);

            cameras[i].position = main.transform.position;

            cameras[i].rotation = main.transform.rotation;
        }
        */

        /*
        LoadSystem.AddPostLoadCorutine(()=>
        {

            HexagonsManager.arrHexCreados[0].SetProyections(main.transform, cameras.renders);
            //for (int i = 0; i < cameras.renders.Length; i++)
            //{
            //    cameras.renders[i].SetRender(0, i);

            //}
        });*/
    }

    
    private void LateUpdate()
    {

        if (obj == null)
            return;
        transform.position  = obj.position.Vect3To2().Vec2to3(transform.position.z);

        for (int i = 0; i < points.Length; i++)
        {
            points[i] = _points2[i] + main.transform.position;
        }
    }
   

    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < points.Length; i++)
        {
            Gizmos.color = Color.red;

            Gizmos.DrawSphere(points[i], 0.1f);
        }
    }
}
