using UnityEngine;


public class ViewEquipWeapon : ViewEquipElement<ViewEquipWeapon>
{
    public AnimationInfo animations;

    public Texture2D positionsPCache, normalsPCache;

    public int countPCache;

    /*
    #if UNITY_EDITOR

    [SerializeField]
    UnityEditor.Experimental.VFX.Utility.PointCacheAsset pointCache;

    private void OnValidate()
    {
        if(pointCache!=null)
        {
            positionsPCache = pointCache.surfaces[0];
            normalsPCache = pointCache.surfaces[1];
            countPCache = pointCache.PointCount;
        }
    }

    #endif
    */
   
}
