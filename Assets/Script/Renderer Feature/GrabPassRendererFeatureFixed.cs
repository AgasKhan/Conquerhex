using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GrabPassRendererFeatureFixed : URPGrabPass.Runtime.GrabPassRendererFeature
{
    [SerializeField]
    bool SceneCameraView;

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType == CameraType.Preview || renderingData.cameraData.cameraType == CameraType.Reflection || (!SceneCameraView && renderingData.cameraData.cameraType == CameraType.SceneView))
            return;

        base.AddRenderPasses(renderer, ref renderingData);
    }
}
