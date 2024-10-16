using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using CameraComponents;
using Unity.Collections;
using Unity.Jobs;

public class MainCamera : SingletonMono<MainCamera>
{
    [System.Serializable]
    public class Culling
    {
        struct FrustrumJob : IJobParallelFor
        {
            // Jobs declare all data that will be accessed in the job
            // By declaring it as read only, multiple jobs are allowed to access the data in parallel
            [ReadOnly]
            public NativeArray<(Vector3 origin, Vector3 direction)> rays;

            [ReadOnly]
            public NativeArray<Plane> planes;

            // By default containers are assumed to be read & write
            public NativeArray<bool> isEnter;

            // The code actually running on the job
            public void Execute(int i)
            {
                Ray ray = new Ray(rays[i].origin, rays[i].direction);

                for (int p = 0; p < planes.Length; p++)
                {
                    if(!planes[p].GetSide(rays[i].origin))
                    {
                        if (planes[p].Raycast(ray, out var enter) && enter * enter < rays[i].direction.sqrMagnitude)
                        {
                            continue;
                        }

                        isEnter[i] = false;
                        return;
                    }
                }

                isEnter[i] = true;
            }
        }

        HashSet<Vector3> pointsCalculated = new();

        List<(Vector3 origin, Vector3 direction)> rays = new();

        public Plane[] planes = new Plane[6]; //Array size must be exactly 6 elements

        public void Update()
        {
            GeometryUtility.CalculateFrustumPlanes(Main, planes);
        }

        public bool IsInFrustrum(IEnumerable<Vector3> points)
        {
            pointsCalculated.Clear();
            rays.Clear();

            foreach (var pointA in points)
            {
                
                foreach (var pointB in points)
                {
                    if (pointsCalculated.Contains(pointB))
                        continue;

                    rays.Add(new(pointA, pointB - pointA));

                    //Debug.DrawRay(rays[rays.Count - 1].origin, rays[rays.Count-1].direction, Color.green);
                }

                pointsCalculated.Add(pointA);
            }

            // Initialize the job data
            var job = new FrustrumJob()
            {
                planes = new(planes, Allocator.TempJob),
                rays = new(rays.ToArray(), Allocator.TempJob),
                isEnter = new(rays.Count, Allocator.TempJob)
            };

            var jobHandler = job.Schedule(rays.Count, 32);


            jobHandler.Complete();

            var inFrustrum = false;

            for (int i = 0; i < job.isEnter.Length; i++)
            {
                if(job.isEnter[i])
                {
                    inFrustrum = true;
                    break;
                }
            }

            job.rays.Dispose();
            job.planes.Dispose();
            job.isEnter.Dispose();

            return inFrustrum;
        }
    }

    static public Camera Main => instance?._main;

    static Plane plane;

    [Header("Configuracion general")]

    public Shake shake = new Shake();

    //public Vector3[] pointsInWorld;

    [SerializeField]
    Tracker tracker = new Tracker();

    [Header("Configuracion interna")]

    [SerializeField]
    Camera _main;

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
    Culling culling;

    [SerializeField]
    UnityEngine.Experimental.Rendering.Universal.RenderObjects renderObjects;

    Hexagone hexagone;

    bool[] camerasEdge = new bool[6];

    public static Vector3 GetScreenToWorld(Vector3 point)
    {
        var ray = Main.ScreenPointToRay(point);

        //ray.direction *= -1;

        plane.Raycast(ray, out float enter);

        return ray.GetPoint(enter);
    }

    public void SetProyections(Hexagone hexagone)
    {
        if (hexagone == null)
            return;

        hexagone.SetProyections(Main.transform.parent, rendersOverlay.Parents);

        this.hexagone= hexagone;
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
        shake.Execute(1 - (health.actualLife / health.maxLife));
    }

    private void Shake_position(Vector3 obj)
    {
        shakeTr.localPosition = obj;
    }

    /*
    private void OnValidate()
    {
        Refresh();
    }
    */

    private void OnEnable()
    {
        UpdateRenderers();
    }

    private void UpdateRenderers()
    {
        for (int i = -2; i < rendersOverlay.Length; i++)
        {
            rendersOverlay.GetParent(i).rotation = tracker.rotationPerspective;

            rendersOverlay[i].transform.localPosition = tracker.vectorPerspective;
        }
    }

    private void TrackerOnFov(float obj)
    {
        for (int i = 0; i < rendersOverlay.cameras.Length; i++)
        {
            rendersOverlay.cameras[i].fieldOfView = obj;
        }
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

        tracker.transitionsSet.AddToEnd(()=> 
        {
            for (int i = -1; i < rendersOverlay.Length; i++)
            {
                var originalLayer = rendersOverlay.cameras[i + 2].cullingMask;

                if(tracker.setDetectLayer)
                    originalLayer |= (1 << 6);
                else
                    originalLayer  &= ~(1 << 6);

                rendersOverlay.cameras[i + 2].cullingMask = originalLayer;

                renderObjects.SetActive(tracker.setEntitiesOverlay);
            }
        });

        tracker.onFov += TrackerOnFov;

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

        UpdateRenderers();

        culling.Update();

        if (HexagonsManager.instance == null)
        {
            return;
        }

        for (int i = 0; i < hexagone.ladosArray.Length; i++)
        {
            camerasEdge[i] = culling.IsInFrustrum(hexagone.GetEquivalentPoints(i, 20));
        }

        RefreshMaterial();
    }

    private void OnDestroy()
    {
        RefreshMaterial(false);
        eventManager.events.SearchOrCreate<SingleEvent<Health>>("Damage").delegato -= ShakeStart;
        eventManager.events.SearchOrCreate<SingleEvent<Character>>("Character").delegato -= tracker.OnCharacterSelected;
        tracker.Destroy();
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawSphere(tracker.hitInfo.point, 0.05f);

        foreach (var item in culling.planes)
        {
            Utilitys.DrawArrowRay(-item.normal * item.distance, item.normal*10);
        }
    }

    
}

namespace CameraComponents
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
                return cameras[index + offsetIndex].transform;
            }
        }
    }

    [System.Serializable]
    public class Tracker
    {
        public Transform obj;

        [SerializeField]
        LayerMask maskToAiming;

        [SerializeField]
        LayerMask maskToCollisionCamera;

        public Vector3 rotationEulerPerspective;

        public Vector3 offsetObjPosition;

        public Quaternion rotationPerspective;

        public Vector3 vectorPerspective;

        [SerializeField]
        float velocityRotate=20;

        public float Dist = 10;

        Vector3 lastVectorPers, lastVectorObjOffset;
        Quaternion lastRot;

        [SerializeField]
        float velocityTransition = 1;

        [SerializeField]
        float smoothColision = 1;

        Character character;

        [HideInInspector]
        public TimedCompleteAction transitionsSet;

        [SerializeField]
        int cameraSet;

        [SerializeField]
        int prevCameraSet;

        [SerializeField]
        Material aimingMaterial;

        public bool setDetectLayer => character.aiming.sets[cameraSet].areaFeedBack;

        public bool setEntitiesOverlay => character.aiming.sets[cameraSet].entitiesOverlay;

        ref Vector3 setOffsetObjPosition => ref character.aiming.sets[cameraSet].offsetObjPosition;

        ref Quaternion setRotationPerspective => ref character.aiming.sets[cameraSet].rotationPerspective;

        ref Vector3 setVectorPerspective => ref character.aiming.sets[cameraSet].vectorPerspective;


        ref Vector3 prevOffsetObjPosition => ref character.aiming.sets[prevCameraSet].offsetObjPosition;

        ref Quaternion prevRotationPerspective => ref character.aiming.sets[prevCameraSet].rotationPerspective;

        ref Vector3 prevVectorPerspective => ref character.aiming.sets[prevCameraSet].vectorPerspective;

        Vector3 CameraPosition => Position + rotationPerspective * vectorPerspective;

        public Vector3 Position => (obj.position + offsetObjPosition).Vect3Copy_Y(offsetObjPosition.y);

        public event System.Action<float> onFov;

        float distanceToObjective;

        Vector3 ToTrackPosition => toTrack.position + offsetToTrack;

        Vector3 offsetToTrack;

        Transform toTrack;

        public RaycastHit hitInfo;

        System.Action _update;

        Ability ability;

        public void Destroy()
        {
            transitionsSet.Destroy();
        }

        Quaternion RotationCamera()
        {
            return Quaternion.Euler(0, 0, -rotationEulerPerspective.y);
        }

        public void OnCharacterSelected(Character character)
        {
            if (this.character != null)
            {
                this.character.moveEventMediator.quaternionOffset = null;
                this.character.aiming.onMode -= Aiming_onMode;
                this.character.caster.onEnterCasting -= Caster_onEnterCasting;
                this.character.caster.onExitCasting -= Caster_onExitCasting;
            }

            obj = character?.transform;

            this.character = character;

            if (character == null)
                return;

            character.caster.onEnterCasting += Caster_onEnterCasting;
            character.caster.onExitCasting += Caster_onExitCasting;

            this.character.moveEventMediator.quaternionOffset = RotationCamera;
            this.character.aiming.onMode += Aiming_onMode;

            Aiming_onMode(this.character.aiming.mode);
        }

        private void Caster_onEnterCasting(Ability obj)
        {
            if (obj is WeaponKata kata && kata.Weapon is RangeWeapon && character.aiming.mode == AimingEntityComponent.Mode.perspective)
            {
                TimersManager.Create(Color.white.ChangeAlphaCopy(0), Color.white, 0.5f, Color.Lerp, (color) => aimingMaterial.SetColor("_Color", color));
                ability = kata;
            }
        }

        private void Caster_onExitCasting(Ability obj)
        {
            if (ability != null)
                TimersManager.Create(Color.white, Color.white.ChangeAlphaCopy(0), 0.5f, Color.Lerp, (color) => aimingMaterial.SetColor("_Color", color));
            ability = null;
        }



        private void CameraBlockPerspectiveDown(Vector2 arg1, float arg2)
        {
            if (!transitionsSet.Chck)
                return;

            if (toTrack != null)
            {
                toTrack = null;
            }
            else if (hitInfo.transform != null && hitInfo.transform.TryGetComponent<Entity>(out var entity))
            {
                toTrack = entity.transform;
                offsetToTrack = hitInfo.point - entity.transform.position;
            }
        }

        private void CameraBlockTopDownStay(Vector2 arg1, float arg2)
        {
            if (!transitionsSet.Chck)
                return;

            rotationEulerPerspective.y += arg1.x;

            character.aiming.AimingToObjective2D = RotationCamera() * Vector2.up;
        }

        private void AimingEventMediatorEventPress(Vector2 arg1, float arg2)
        {
            if (!transitionsSet.Chck)
                return;

            //arg1 *= (1/Time.deltaTime) * (1/velocityRotate);

            rotationEulerPerspective.x -=  arg1.y;

            rotationEulerPerspective.x = Mathf.Clamp(rotationEulerPerspective.x, -20, 89);

            rotationEulerPerspective.y += arg1.x;
        }

        private void AimingUpdate()
        {
            if (hitInfo.transform != null)
            {
                character.aiming.ObjectivePosition = hitInfo.point;

                return;
            }

            character.aiming.ObjectivePosition = (rotationPerspective * Vector3.forward * 100) + CameraPosition;
        }

        private void Aiming_onMode(AimingEntityComponent.Mode obj)
        {
            prevCameraSet = cameraSet;

            cameraSet = (int)obj;

            transitionsSet.Reset();

            VirtualControllers.CameraBlock.eventPress -= CameraBlockTopDownStay;
            VirtualControllers.CameraBlock.eventDown -= CameraBlockPerspectiveDown;

            VirtualControllers.Camera.eventDown -= AimingEventMediatorEventPress;
            VirtualControllers.Camera.eventPress -= AimingEventMediatorEventPress;
            VirtualControllers.Camera.eventUp -= AimingEventMediatorEventPress;
            _update -= AimingUpdate;

            switch (obj)
            {
                case AimingEntityComponent.Mode.topdown:

                    VirtualControllers.CameraBlock.eventPress += CameraBlockTopDownStay;

                    break;


                case AimingEntityComponent.Mode.perspective:

                    VirtualControllers.CameraBlock.eventDown += CameraBlockPerspectiveDown;

                    VirtualControllers.Camera.eventDown += AimingEventMediatorEventPress;
                    VirtualControllers.Camera.eventPress += AimingEventMediatorEventPress;
                    VirtualControllers.Camera.eventUp += AimingEventMediatorEventPress;

                    _update += AimingUpdate;

                    break;

                case AimingEntityComponent.Mode.focus:

                    break;

                default:
                    break;
            }
        }

        public void Update()
        {
            _update();
        }

        void MyUpdate()
        {

            //if (!transitionsSet.Chck || character==null || character.aiming.mode != AimingEntityComponent.Mode.perspective)
            //  return;

            if (!transitionsSet.Chck || character == null)
                return;

            if (character.aiming.mode == AimingEntityComponent.Mode.perspective)
            {
                if (Physics.SphereCast(Position, 0.5f, rotationPerspective * setVectorPerspective, out hitInfo, distanceToObjective, maskToCollisionCamera, QueryTriggerInteraction.Ignore))
                {
                    vectorPerspective = Vector3.Lerp(vectorPerspective, setVectorPerspective.normalized * (hitInfo.distance - 0.1f), Time.deltaTime * smoothColision);
                }
                else
                {
                    vectorPerspective = Vector3.Lerp(vectorPerspective, setVectorPerspective, Time.deltaTime * smoothColision);
                }

                Ray ray = new Ray(CameraPosition, rotationPerspective * Vector3.forward);

                if (ability != null)
                {
                    var angle = ability.Angle / 2;

                    angle = Mathf.Clamp(angle, 0, 30);

                    aimingMaterial.SetFloat("_Dispersion", angle);

                    //ray.direction = Quaternion.Euler(Random.Range(angle / -2, angle / 2), Random.Range(angle / -2, angle / 2), 0) * (ray.direction);
                }

                Physics.Raycast(ray, out hitInfo, float.PositiveInfinity, maskToAiming, QueryTriggerInteraction.Ignore);

                if (toTrack != null)//trackeo
                {
                    if (hitInfo.distance > 25 || !toTrack.gameObject.activeInHierarchy)//distancia limite hardcodeada
                    {
                        toTrack = null;
                    }
                    else
                    {
                        rotationEulerPerspective = Quaternion.LookRotation(ToTrackPosition - CameraPosition).eulerAngles;
                        rotationEulerPerspective.x = Mathf.Clamp(rotationEulerPerspective.x, -20, 20);
                    }
                }
            }

            setRotationPerspective = Quaternion.Euler(rotationEulerPerspective);

            prevRotationPerspective = Quaternion.Euler(prevRotationPerspective.eulerAngles.x, rotationEulerPerspective.y, prevRotationPerspective.eulerAngles.z);

            rotationPerspective = setRotationPerspective;
        }

        void EnterMenu()
        {
            lastVectorPers = vectorPerspective;
            lastRot = rotationPerspective;
            lastVectorObjOffset = offsetObjPosition;

            offsetObjPosition = Vector3.zero;
            var p = character.move.direction * -Dist;
            vectorPerspective = new(-0.05f, .85f, -6f);
            rotationEulerPerspective = new(6, Mathf.Atan2(p.x, p.z) * Mathf.Rad2Deg, 0);
            rotationPerspective = Quaternion.Euler(rotationEulerPerspective);

            Cursor.visible = true;
        }

        void ExitMenu()
        {
            vectorPerspective = lastVectorPers;
            rotationPerspective = lastRot;
            offsetObjPosition = lastVectorObjOffset;
            rotationEulerPerspective = rotationPerspective.eulerAngles;
        }

        public void Init()
        {
            GameManager.onEnterMenuUnityEvent.AddListener(EnterMenu);

            GameManager.onExitMenuUnityEvent.AddListener(ExitMenu);

            _update = MyUpdate;

            transitionsSet = (TimedCompleteAction)TimersManager.Create(velocityTransition, () =>
            {
                if (character == null)
                    return;

                rotationPerspective = Quaternion.Slerp(prevRotationPerspective, setRotationPerspective, transitionsSet.InversePercentage());
                vectorPerspective = Vector3.Lerp(prevVectorPerspective, setVectorPerspective, transitionsSet.InversePercentage());
                offsetObjPosition = Vector3.Lerp(prevOffsetObjPosition, setOffsetObjPosition, transitionsSet.InversePercentage());

                distanceToObjective = vectorPerspective.magnitude;
                rotationEulerPerspective = rotationPerspective.eulerAngles;

                onFov?.Invoke(Mathf.Lerp(character.aiming.sets[prevCameraSet].fov, character.aiming.sets[cameraSet].fov, transitionsSet.InversePercentage()));

            }, () =>
            {
                if (character == null)
                    return;

                rotationPerspective = setRotationPerspective;
                vectorPerspective = setVectorPerspective;
                offsetObjPosition = setOffsetObjPosition;

                distanceToObjective = vectorPerspective.magnitude;
                rotationEulerPerspective = rotationPerspective.eulerAngles;

                onFov?.Invoke(character.aiming.sets[cameraSet].fov);
            }).Stop();
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