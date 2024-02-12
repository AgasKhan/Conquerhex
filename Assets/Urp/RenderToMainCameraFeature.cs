using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RenderToMainCameraFeature : FullScreenPassRendererFeature
{
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if(renderingData.cameraData.camera.CompareTag("MainCamera"))
            base.AddRenderPasses(renderer, ref renderingData);
    }
}
