using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RenderToCameraFeature : FullScreenPassRendererFeature
{
    public bool MainCamera;


    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        bool mainCamera = renderingData.cameraData.camera.CompareTag("MainCamera");

        if ((MainCamera && mainCamera) || ( !mainCamera && !MainCamera))
            base.AddRenderPasses(renderer, ref renderingData);
    }
}
