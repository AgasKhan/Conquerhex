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
                return cameras[index+ offsetIndex].transform;
            }
        }
    }

    [System.Serializable]
    public class Tracker
    {
        public Transform obj;

        public Vector3 rotationEulerPerspective;

        public Vector3 offsetObjPosition ; 

        public Quaternion rotationPerspective ; 

        public Vector3 vectorPerspective ; 

        [SerializeField]
        float velocityTransition = 1;

        [SerializeField]
        float smoothColision = 1;

        Character character;

        Timer transitionsSet;

        [SerializeField]
        int cameraSet;

        [SerializeField]
        int prevCameraSet;

        ref Vector3 setOffsetObjPosition => ref character.aiming.sets[cameraSet].offsetObjPosition;

        ref Quaternion setRotationPerspective => ref character.aiming.sets[cameraSet].rotationPerspective;

        ref Vector3 setVectorPerspective => ref character.aiming.sets[cameraSet].vectorPerspective;


        ref Vector3 prevOffsetObjPosition => ref character.aiming.sets[prevCameraSet].offsetObjPosition;

        ref Quaternion prevRotationPerspective => ref character.aiming.sets[prevCameraSet].rotationPerspective;

        ref Vector3 prevVectorPerspective => ref character.aiming.sets[prevCameraSet].vectorPerspective;

        Vector3 CameraPosition => Position + rotationPerspective * vectorPerspective;

        public Vector3 Position => (obj.position + offsetObjPosition).Vect3Copy_Y(offsetObjPosition.y);

        public float Fov => character?.aiming.sets[cameraSet].fov ?? 60;

        float distanceToObjective;

        Vector3 ToTrackPosition => toTrack.position + offsetToTrack;

        Vector3 offsetToTrack;

        Transform toTrack;

        RaycastHit hitInfo;

        public void OnCharacterSelected(Character character)
        {
            if (this.character != null)
            {
                this.character.moveEventMediator.quaternionOffset = null;
                this.character.attackEventMediator.quaternionOffset = null;
                this.character.dashEventMediator.quaternionOffset = null;
                this.character.abilityEventMediator.quaternionOffset = null;
                this.character.aimingEventMediator.eventPress -= AimingEventMediatorEventPress;
                this.character.aiming.onMode -= Aiming_onMode;
            }

            obj = character?.transform;

            this.character = character;

            if (character == null)
                return;

            this.character.moveEventMediator.quaternionOffset = RotationCamera;

            this.character.dashEventMediator.quaternionOffset = RotationCamera;


            this.character.attackEventMediator.quaternionOffset = CameraAiming;

            this.character.abilityEventMediator.quaternionOffset = CameraAiming;


            this.character.aimingEventMediator.eventPress += AimingEventMediatorEventPress;

            this.character.aiming.onMode += Aiming_onMode;

            Aiming_onMode(this.character.aiming.mode);
        }

        private void Aiming_onMode(AimingEntityComponent.Mode obj)
        {
            prevCameraSet = cameraSet;

            cameraSet = (int)obj;

            transitionsSet.Reset();
        }

        private void AimingEventMediatorEventPress(Vector2 arg1, float arg2)
        {
            if (!transitionsSet.Chck)
                return;

            if (character.aiming.mode == AimingEntityComponent.Mode.perspective)
            {
                rotationEulerPerspective.x -= arg1.y;

                rotationEulerPerspective.x = Mathf.Clamp(rotationEulerPerspective.x, -20, 89);
            }

            if (character.aiming.mode == AimingEntityComponent.Mode.perspective || Input.GetKey(KeyCode.LeftControl))
                rotationEulerPerspective.y += arg1.x;
        }

        Quaternion RotationCamera()
        {
            return Quaternion.Euler(0, 0, -rotationEulerPerspective.y);
        }

        Quaternion CameraAiming()
        {
            if (character.aiming.mode != AimingEntityComponent.Mode.perspective)
            {
                return RotationCamera();
            }          

            //Debug.DrawRay(ray.origin,ray.direction, Color.red);
               
            if(hitInfo.transform!=null)
            {
                Vector3 aux = hitInfo.point - character.transform.position;

                //Debug.DrawRay(character.transform.position, aux, Color.green);

                character.aiming.ObjectivePosition = hitInfo.point;

                aux.y = 0;

                float angleY = Mathf.Atan2(aux.x, aux.z) * Mathf.Rad2Deg;

                return Quaternion.AngleAxis(-angleY, Vector3.forward);
            }


            return RotationCamera();
        }

        public void Update()
        {

            //if (!transitionsSet.Chck || character==null || character.aiming.mode != AimingEntityComponent.Mode.perspective)
            //  return;

            if (!transitionsSet.Chck && character != null)
                return;

            if (character.aiming.mode == AimingEntityComponent.Mode.perspective)
            {
                if (Physics.SphereCast(Position, 0.5f, rotationPerspective * setVectorPerspective, out hitInfo, distanceToObjective, Physics.AllLayers, QueryTriggerInteraction.Ignore))
                {
                    vectorPerspective = Vector3.Lerp(vectorPerspective, setVectorPerspective.normalized * (hitInfo.distance - 0.1f), Time.deltaTime * smoothColision);
                }
                else
                {
                    vectorPerspective = Vector3.Lerp(vectorPerspective, setVectorPerspective, Time.deltaTime * smoothColision);
                }

                Ray ray = new Ray(CameraPosition, rotationPerspective * Vector3.forward);

                if (Physics.Raycast(ray, out hitInfo, float.PositiveInfinity, Physics.AllLayers & ~(1 << 15), QueryTriggerInteraction.Ignore))
                {
                    if (Input.GetKeyDown(KeyCode.LeftControl)) //seteo del trackeo
                    {
                        if (toTrack != null)
                        {
                            toTrack = null;
                        }
                        else
                        {
                            toTrack = hitInfo.transform;
                            offsetToTrack = hitInfo.point - hitInfo.transform.position;
                        }
                    }
                }

                if (toTrack != null)//trackeo
                    rotationEulerPerspective = Quaternion.LookRotation(ToTrackPosition - CameraPosition).eulerAngles;
            }

            setRotationPerspective = Quaternion.Euler(rotationEulerPerspective);

            prevRotationPerspective = Quaternion.Euler(prevRotationPerspective.eulerAngles.x, rotationEulerPerspective.y, prevRotationPerspective.eulerAngles.z);

            rotationPerspective = setRotationPerspective;
        }

        void EnterMenu()
        {

        }

        void ExitMenu()
        {

        }

        public void Init()
        {
            GameManager.onEnterMenuUnityEvent.AddListener(EnterMenu);

            GameManager.onExitMenuUnityEvent.AddListener(ExitMenu);

            transitionsSet = TimersManager.Create(velocityTransition, () =>
            {
                if (character == null)
                    return;

                rotationPerspective = Quaternion.Slerp(prevRotationPerspective, setRotationPerspective, transitionsSet.InversePercentage());
                vectorPerspective = Vector3.Lerp(prevVectorPerspective, setVectorPerspective, transitionsSet.InversePercentage());
                offsetObjPosition = Vector3.Lerp(prevOffsetObjPosition, setOffsetObjPosition, transitionsSet.InversePercentage());

                distanceToObjective = vectorPerspective.magnitude;
                rotationEulerPerspective = rotationPerspective.eulerAngles;

            }, () =>
            {
                if (character == null)
                    return;

                rotationPerspective = setRotationPerspective;
                vectorPerspective = setVectorPerspective;
                offsetObjPosition = setOffsetObjPosition;

                distanceToObjective = vectorPerspective.magnitude;
                rotationEulerPerspective = rotationPerspective.eulerAngles;
            }).Stop();
        }
    }

    [Header("Configuracion general")]

    public Shake shake = new Shake();

    public Vector3[] pointsInWorld;


    [SerializeField]
    Tracker tracker = new Tracker();

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

       
        for (int i = -2; i < rendersOverlay.Length; i++)
        {
            rendersOverlay.GetParent(i).rotation = tracker.rotationPerspective;

            rendersOverlay[i].transform.localPosition = tracker.vectorPerspective;

            rendersOverlay.cameras[i + 2].fieldOfView = tracker.Fov;
        }

        for (int i = 0; i < pointsInScreen.Length; i++)
        {
            _points[i] = main.ViewportToWorldPoint(new Vector3(pointsInScreen[i].x, pointsInScreen[i].y, main.nearClipPlane));

            Ray ray = new Ray(main.transform.position, _points[i] - main.transform.position);

            plane.Raycast(ray, out float distance);

            _points2[i] = ray.GetPoint(distance) - main.transform.position;
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

        tracker.Init();

        eventManager.events.SearchOrCreate<SingleEvent<Character>>("Character").delegato += tracker.OnCharacterSelected;

        pointsInWorld = new Vector3[rendersOverlay.cameras.Length];       

        LoadSystem.AddPostLoadCorutine(() =>
        {
            if (HexagonsManager.instance != null && HexagonsManager.instance.automaticRender)
                SetProyections(HexagonsManager.arrHexCreados?[0]);
        });
    }


    private void LateUpdate()
    {
        if (tracker.obj == null)
            return;

        for (int i = 0; i < camerasEdge.Length; i++)
        {
            camerasEdge[i] = (false);
        }

        //rotationPerspective = Vector3.Slerp(rotationPerspective, aimingPerspective, Time.deltaTime * velocityRotation);

        tracker.Update();

        transform.position = tracker.Position;

        for (int i = -2; i < rendersOverlay.Length; i++)
        {
            rendersOverlay.GetParent(i).rotation = tracker.rotationPerspective;

            rendersOverlay[i].transform.localPosition = tracker.vectorPerspective;

            rendersOverlay.cameras[i+2].fieldOfView = tracker.Fov;
        }

        for (int i = 0; i < pointsInScreen.Length; i++)
        {
            _points[i] = main.ViewportToWorldPoint(new Vector3(pointsInScreen[i].x, pointsInScreen[i].y, main.nearClipPlane));

            Ray ray = new Ray(main.transform.position, _points[i] - main.transform.position);

            plane.Raycast(ray, out float distance);

            _points2[i] = ray.GetPoint(distance) - main.transform.position;
        }

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

        for (int i = 0; i < rendersOverlay.cameras.Length; i++)
        {
            pointsInWorld[i] = rendersOverlay.cameras[i].ViewportToWorldPoint(new Vector3(0.5f,0.5f,1));

            Ray ray = new Ray(rendersOverlay.cameras[i].transform.position, pointsInWorld[i] - rendersOverlay.cameras[i].transform.position);

            plane.Raycast(ray, out float distance);

            pointsInWorld[i] = ray.GetPoint(distance);
        }

        RefreshMaterial();
    }

    private void OnDestroy()
    {
        RefreshMaterial(false);
        eventManager.events.SearchOrCreate<SingleEvent<Health>>("Damage").delegato -= ShakeStart;
        eventManager.events.SearchOrCreate<SingleEvent<Character>>("Character").delegato -= tracker.OnCharacterSelected;
    }


    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < points.Length; i++)
        {
            Gizmos.color = Color.red;

            Gizmos.DrawSphere(points[i], 0.1f);
        }

        for (int i = 0; i < pointsInWorld.Length; i++)
        {
            Gizmos.color = Color.red;

            Gizmos.DrawSphere(pointsInWorld[i], 0.1f);
        }
    }
}




/*
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
                return cameras[index+ offsetIndex].transform;
            }
        }
    }

    [System.Serializable]
    public class Tracker
    {
        [System.Serializable]
        public struct CameraSet
        {
            public Vector3 offsetObjPosition;

            public Vector3 rotationPerspective;

            public Vector3 vectorPerspective;
        }

        public Transform obj;

        public Vector3 offsetObjPosition;

        public Vector3 rotationPerspective;

        public Vector3 vectorPerspective;

        [SerializeField]
        CameraSet[] sets;

        [SerializeField]
        float velocityTransition = 1;

        int indexSetConf;

        int prevIndexSetConf;

        Character character;

        Timer transitionsSet;


        ref Vector3 setOffsetObjPosition => ref sets[indexSetConf].offsetObjPosition;

        ref Vector3 setRotationPerspective => ref sets[indexSetConf].rotationPerspective;

        ref Vector3 setVectorPerspective => ref sets[indexSetConf].vectorPerspective;


        ref Vector3 prevOffsetObjPosition => ref sets[prevIndexSetConf].offsetObjPosition;

        ref Vector3 prevRotationPerspective => ref sets[prevIndexSetConf].rotationPerspective;

        ref Vector3 prevVectorPerspective => ref sets[prevIndexSetConf].vectorPerspective;

        Vector3 position => obj.position + offsetObjPosition;

        public Vector3 Position => position.Vect3Copy_Y(offsetObjPosition.y);

        public void OnCharacterSelected(Character character)
        {
            if (this.character != null)
            {
                this.character.moveEventMediator.quaternionOffset = null;
                this.character.attackEventMediator.quaternionOffset = null;
                this.character.dashEventMediator.quaternionOffset = null;
                this.character.abilityEventMediator.quaternionOffset = null;
                this.character.aimingEventMediator.eventPress -= AimingEventMediatorEventPress;
                this.character.aiming.onMode -= Aiming_onMode;
            }

            obj = character?.transform;

            this.character = character;

            if (character == null)
                return;

            this.character.moveEventMediator.quaternionOffset = RotationCamera;

            this.character.attackEventMediator.quaternionOffset = RotationCamera;

            this.character.dashEventMediator.quaternionOffset = RotationCamera;

            this.character.abilityEventMediator.quaternionOffset = RotationCamera;

            this.character.aimingEventMediator.eventPress += AimingEventMediatorEventPress;

            this.character.aiming.onMode += Aiming_onMode;

            Aiming_onMode(this.character.aiming.mode);
        }

        private void Aiming_onMode(AimingEntityComponent.Mode obj)
        {
            prevIndexSetConf = indexSetConf;
            indexSetConf = (int)obj;

            transitionsSet.Reset();
        }

        private void AimingEventMediatorEventPress(Vector2 arg1, float arg2)
        {
            if (character.aiming.mode == AimingEntityComponent.Mode.perspective)
            {
                setRotationPerspective.y += arg1.x;
                setRotationPerspective.x -= arg1.y;

                setRotationPerspective.x = Mathf.Clamp(setRotationPerspective.x, -20, 89);

                rotationPerspective = setRotationPerspective;
            }
        }

        Quaternion RotationCamera()
        {
            return Quaternion.Euler(0, 0, -rotationPerspective.y);
        }

        public void Init()
        {
            transitionsSet = TimersManager.Create(velocityTransition, () =>
            {
                if (character == null)
                    return;

                rotationPerspective = Vector3.Slerp(prevRotationPerspective, setRotationPerspective, transitionsSet.InversePercentage());
                vectorPerspective = Vector3.Lerp(prevVectorPerspective, setVectorPerspective, transitionsSet.InversePercentage());
                offsetObjPosition = Vector3.Lerp(prevOffsetObjPosition, setOffsetObjPosition, transitionsSet.InversePercentage());

            }, () =>
            {
                if (character == null)
                    return;

                prevRotationPerspective.y = 0;
                rotationPerspective = setRotationPerspective;
                vectorPerspective = setVectorPerspective;
                offsetObjPosition = setOffsetObjPosition;
            });
        }
    }

    [Header("Configuracion general")]

    public Shake shake = new Shake();

    public bool perspective;

    public Vector3[] pointsInWorld;

    [SerializeField]
    Tracker tracker = new Tracker();

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
                rendersOverlay.GetParent(i).rotation = Quaternion.Euler(tracker.rotationPerspective);

                rendersOverlay[i].transform.localPosition = tracker.vectorPerspective;
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

        tracker.Init();

        eventManager.events.SearchOrCreate<SingleEvent<Character>>("Character").delegato += tracker.OnCharacterSelected;

        pointsInWorld = new Vector3[rendersOverlay.cameras.Length];       

        LoadSystem.AddPostLoadCorutine(() =>
        {
            if (HexagonsManager.instance != null && HexagonsManager.instance.automaticRender)
                SetProyections(HexagonsManager.arrHexCreados?[0]);
        });
    }


    private void LateUpdate()
    {
        if (tracker.obj == null)
            return;

        for (int i = 0; i < camerasEdge.Length; i++)
        {
            camerasEdge[i] = (false);
        }

        //rotationPerspective = Vector3.Slerp(rotationPerspective, aimingPerspective, Time.deltaTime * velocityRotation);

        transform.position = tracker.Position;

        for (int i = -2; i < rendersOverlay.Length; i++)
        {
            rendersOverlay.GetParent(i).rotation = Quaternion.Euler(tracker.rotationPerspective);

            rendersOverlay[i].transform.localPosition = tracker.vectorPerspective;
        }

        for (int i = 0; i < pointsInScreen.Length; i++)
        {
            _points[i] = main.ViewportToWorldPoint(new Vector3(pointsInScreen[i].x, pointsInScreen[i].y, main.nearClipPlane));

            Ray ray = new Ray(main.transform.position, _points[i] - main.transform.position);

            plane.Raycast(ray, out float distance);

            _points2[i] = ray.GetPoint(distance) - main.transform.position;
        }

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

        for (int i = 0; i < rendersOverlay.cameras.Length; i++)
        {
            pointsInWorld[i] = rendersOverlay.cameras[i].ViewportToWorldPoint(new Vector3(0.5f,0.5f,1));

            Ray ray = new Ray(rendersOverlay.cameras[i].transform.position, pointsInWorld[i] - rendersOverlay.cameras[i].transform.position);

            plane.Raycast(ray, out float distance);

            pointsInWorld[i] = ray.GetPoint(distance);
        }

        RefreshMaterial();
    }

    private void OnDestroy()
    {
        RefreshMaterial(false);
        eventManager.events.SearchOrCreate<SingleEvent<Health>>("Damage").delegato -= ShakeStart;
        eventManager.events.SearchOrCreate<SingleEvent<Character>>("Character").delegato -= tracker.OnCharacterSelected;
    }


    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < points.Length; i++)
        {
            Gizmos.color = Color.red;

            Gizmos.DrawSphere(points[i], 0.1f);
        }

        for (int i = 0; i < pointsInWorld.Length; i++)
        {
            Gizmos.color = Color.red;

            Gizmos.DrawSphere(pointsInWorld[i], 0.1f);
        }
    }
}

 
 */