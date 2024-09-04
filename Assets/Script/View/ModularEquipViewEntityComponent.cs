using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ComponentsAndContainers;
public class ModularEquipViewEntityComponent : ComponentOfContainer<Entity>
{
    [System.Serializable]
    public class Data
    {
        public Vector3 offset;        
        public Vector3 forwardRotation;
        public Transform transform;

        public Vector3 Position => transform.position + transform.TransformDirection(offset);

        public Quaternion Rotation => transform.rotation * Quaternion.Euler(forwardRotation);

        public Vector3 Right => Rotation * Vector3.right;
        public Vector3 Forward => Rotation * Vector3.forward;
        public Vector3 Up => Rotation * Vector3.up;
    }

    public Data this[string str]
    {
        get => _whereToEquip[str];
    }

    [SerializeField]
    Pictionarys<string, Data> whereToEquip = new();

    Dictionary<string, Data> _whereToEquip = new();

    [SerializeField]
    bool showInModel = true;

    public override void OnEnterState(Entity param)
    {
        _whereToEquip = whereToEquip.ToDictionary();
        //whereToEquip.Clear();

    }

    public override void OnExitState(Entity param)
    {
    }

    public override void OnStayState(Entity param)
    {
    }

    private void OnDrawGizmosSelected()
    {
        if (!showInModel)
            return;

        foreach (var item in whereToEquip)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(item.value.Position, 0.05f);

            Gizmos.color = Color.blue;
            Utilitys.DrawArrowRay(item.value.Position, item.value.Forward * 0.2f);

            Gizmos.color = Color.green;
            Utilitys.DrawArrowRay(item.value.Position, item.value.Up * 0.2f);

            Gizmos.color = Color.red;
            Utilitys.DrawArrowRay(item.value.Position, item.value.Right * 0.2f);
        }
    }
}
