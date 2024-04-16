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

    [Header("Configuracion general")]

    public Shake shake = new Shake();

    public Transform obj;

    public bool perspective;

    [SerializeField]
    Vector3 rotationPerspective;

    [SerializeField]
    Vector3 vectorPerspective;

    [Header("Configuracion interna")]

    [SerializeField]
    Camera main;

    [SerializeField]
    EventManager eventManager;

    [SerializeField]
    Transform shakeTr;

    [SerializeField]
    Transform offsetTr;

    [SerializeField]
    Material CameraRenderer;

    [SerializeField]
    MapTransform rendersOverlay;

    [SerializeField]
    Vector2[] pointsInScreen;

    [SerializeField]
    Vector3[] points;

    Vector3[] _points;

    Vector3[] _points2;

    Plane plane;

    Vector3 centerPoint;

    bool[] camerasEdge = new bool[6];

    public void SetProyections(Hexagone hexagone)
    {
        if (hexagone == null)
            return;

        hexagone.SetProyections(main.transform.parent, rendersOverlay.Parents);

        centerPoint = hexagone.transform.position;
    }

    private void RefreshMaterial(bool on = true)
    {
        for (int i = 0; i < rendersOverlay.Length; i++)
        {
            rendersOverlay[i].SetActiveGameObject(camerasEdge[i]);
            CameraRenderer.SetFloat("_position" + (1 + i), camerasEdge[i] && on ? 1 : 0);
        }
    }

    public void OnCharacterSelected(Character character)
    {
        obj = character.transform;
    }
    

    void ShakeStart(Health health)
    {
        shake.Execute(1 - (health.actualLife/health.maxLife));
    }

    private void Shake_position(Vector3 obj)
    {
        shakeTr.localPosition = obj;
    }

    void Refresh()
    {
        if (main == null)
            return;

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
    }

    private void OnValidate()
    {
        Refresh();
    }

    private void OnEnable()
    {
        Refresh();
    }

    protected override void Awake()
    {
        base.Awake();
        plane = new Plane(Vector3.up, 0);

        shake.position += Shake_position;
        shake.Init(shakeTr.localPosition);

        eventManager.events.SearchOrCreate<SingleEvent<Health>>("Damage").delegato += ShakeStart;

        eventManager.events.SearchOrCreate<SingleEvent<Character>>("Character").delegato += OnCharacterSelected;

        LoadSystem.AddPostLoadCorutine(() =>
        {
            if (HexagonsManager.instance != null)
                SetProyections(HexagonsManager.arrHexCreados?[0]);
        });
    }


    private void LateUpdate()
    {
        if (obj == null)
            return;

        for (int i = 0; i < camerasEdge.Length; i++)
        {
            camerasEdge[i] = (false);
        }

        transform.position  = obj.position.Vect3Copy_Y(transform.position.y);

        if (HexagonsManager.instance == null)
        {
            return;
        }

        for (int i = 0; i < points.Length; i++)
        {
            points[i] = _points2[i] + main.transform.position;

            var translatedPoint = (points[i] - centerPoint).Vect3To2XZ();

            int lado = HexagonsManager.CalcEdge(translatedPoint);

            //print($"El punto {i}, se encuentra en el lado {lado}");

            translatedPoint = (Quaternion.Euler(0, 0, lado * 60) * translatedPoint);

            if (translatedPoint.y >= HexagonsManager.apotema)
            {
                //print($"El punto {i}, se encuentra fuera del hexagono con el punto transladado a {aux} siendo el original {_points2[i]}");

                camerasEdge[lado] = (true);
            }
        }

        RefreshMaterial();
    }

    private void OnDestroy()
    {
        RefreshMaterial(false);
        eventManager.events.SearchOrCreate<SingleEvent<Health>>("Damage").delegato -= ShakeStart;
        eventManager.events.SearchOrCreate<SingleEvent<Character>>("Character").delegato -= OnCharacterSelected;
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
