using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : SingletonMono<MainCamera>
{
    [System.Serializable]
    public class MapTransform
    {
        public Transform[] parents = new Transform[6];

        public Camera[] cameras = new Camera[6];

        public int Length => cameras.Length;

        public Transform this[int index]
        {
            get
            {
                return cameras[index].transform;
            }
        }
    }    
    
    public MapTransform rendersOverlay;

    

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


    Plane plane;

    public void SetProyections(Hexagone hexagone)
    {
        hexagone.SetProyections(main.transform.parent, rendersOverlay.parents);
    }


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

        for (int i = 0; i < rendersOverlay.Length; i++)
        {
            rendersOverlay.cameras[i].orthographic = main.orthographic;
        }

        if (!perspective)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).rotation = Quaternion.identity;

                transform.GetChild(i).GetChild(0).localPosition = Vector3.zero;
            }

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
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).rotation = Quaternion.Euler(rotationPerspective);

                transform.GetChild(i).GetChild(0).localPosition = vectorPerspective;
            }

            for (int i = 0; i < pointsInScreen.Length; i++)
            {
                _points[i] = main.ViewportToWorldPoint(new Vector3(pointsInScreen[i].x, pointsInScreen[i].y, main.nearClipPlane));

                Ray ray = new Ray(main.transform.position, _points[i] - main.transform.position);

                plane.Raycast(ray, out float distance);

                _points2[i] = ray.GetPoint(distance) - main.transform.position;
            }
        }

        LoadSystem.AddPostLoadCorutine(()=>
        {

            SetProyections(HexagonsManager.arrHexCreados[0]);
            
        });
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
