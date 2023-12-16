using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MainCamera : SingletonMono<MainCamera>
{
    [System.Serializable]
    public class MapTransform
    {
        public Camera[] cameras = new Camera[6];
        

        public int RealLength => cameras.Length;

        public int Length => RealLength - offsetIndex;

        public IEnumerable<Transform> Parents => parents.Skip(offsetIndex);

        public Transform GetParent(int i) => parents[i + offsetIndex];

        [SerializeField]
        Transform[] parents = new Transform[6];

        [SerializeField]
        int offsetIndex = 2;

        public Transform this[int index]
        {
            get
            {
                return cameras[index+2].transform;
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

    Vector3 centerPoint;

    public void SetProyections(Hexagone hexagone)
    {
        hexagone.SetProyections(main.transform.parent, rendersOverlay.Parents);

        centerPoint = hexagone.transform.position;
    }


    protected override void Awake()
    {
        base.Awake();
        main = Camera.main;
        plane = new Plane(Vector3.forward, 0);
    }

    private void OnEnable()
    {
        points = new Vector3[pointsInScreen.Length];

        _points = new Vector3[pointsInScreen.Length];

        _points2 = new Vector3[pointsInScreen.Length];

        for (int i = 0; i < rendersOverlay.RealLength; i++)
        {
            rendersOverlay.cameras[i].orthographic = !perspective;
        }

        if (!perspective)
        {
            for (int i = -2; i < rendersOverlay.Length; i++)
            {
                rendersOverlay.GetParent(i).rotation = Quaternion.identity;

                rendersOverlay[i].transform.localPosition = Vector3.zero;
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
            for (int i = -2; i < rendersOverlay.Length; i++)
            {
                rendersOverlay.GetParent(i).rotation = Quaternion.Euler(rotationPerspective);

                rendersOverlay[i].transform.localPosition = vectorPerspective;
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

        for (int i = 0; i < rendersOverlay.Length; i++)
        {
            rendersOverlay[i].SetActiveGameObject(false);
        }

        transform.position  = obj.position.Vect3_Z(transform.position.z);

        for (int i = 0; i < points.Length; i++)
        {
            points[i] = _points2[i] + main.transform.position;

            var translatedPoint = points[i] - centerPoint;

            int lado = HexagonsManager.CalcEdge(translatedPoint);

            //print($"El punto {i}, se encuentra en el lado {lado}");

            translatedPoint = (Quaternion.Euler(0, 0, lado * 60) * translatedPoint);

            if (translatedPoint.y >= HexagonsManager.apotema)
            {
                //print($"El punto {i}, se encuentra fuera del hexagono con el punto transladado a {aux} siendo el original {_points2[i]}");

                rendersOverlay[lado].SetActiveGameObject(true);
            }
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
