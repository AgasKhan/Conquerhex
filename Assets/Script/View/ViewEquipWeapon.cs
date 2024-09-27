using UnityEngine;


public class ViewEquipWeapon : ViewEquipElement<ViewEquipWeapon>
{
    public enum HandHandling
    {
        Normal,
        Inversed
    }

    public HandHandling handHandling;

    [SerializeField]
    Vector3 leftOffset;

    public Texture2D positionsPCache, normalsPCache;

    public int countPCache;

    [SerializeField]
    Transform nailedPoint;

    [SerializeField]
    AnimationInfo animations;

    [SerializeField]
    new Animation animation;

    public void NailInTransform(Transform transform, Vector3 offset, Vector3 direction)
    {
        this.transform.SetParent(transform);
        this.transform.localPosition = offset;
        this.transform.localPosition -= this.transform.TransformDirection(nailedPoint.position);

        Debug.LogWarning("Falta terminar de implementar");
    }

    public void PlayAction(string name)
    {
        if(animation!=null)
            animation.Play(name);
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        Gizmos.color = new Color(0.75f,0,0,1);

        Gizmos.DrawWireSphere(transform.position + transform.TransformDirection(leftOffset), 0.1f);

        /////////////////////////////
        if (nailedPoint == null)
            return;

        Gizmos.color = Color.green;

        Gizmos.DrawSphere(nailedPoint.position, 0.02f);

        Utilitys.DrawArrowRay(nailedPoint.position, nailedPoint.up*0.5f);
    }

    #if UNITY_EDITOR
    private void OnValidate()
    {
        if(animation != null && animations != null && animations.animClips.Count != animation.GetClipCount())
        {
            //DestroyImmediate(animation, true);

            //animation = gameObject.AddComponent<Animation>();

            foreach (var animClip in animations.animClips)
            {
                animation.AddClip(animClip.value.animationClip, animClip.key, 0, (int)(animClip.value.animationClip.frameRate * animClip.value.animationClip.length), animClip.value.inLoop);
            }
        }
    }
    #endif
}