using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RenderToTextureFeature : ScriptableRendererFeature
{
    class RenderToTexturePass : ScriptableRenderPass
    {
        private RenderTexture _renderTexture;
        private LayerMask _overrideLayerMask;

        Camera _targetCam;

        public RenderToTexturePass(RenderTexture renderTexture, LayerMask layerMask, Camera cam)
        {
            _renderTexture = renderTexture;
            _overrideLayerMask = layerMask;
            _targetCam = cam;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {

            CommandBuffer cmd = CommandBufferPool.Get("RenderToTexture");

            cmd.SetRenderTarget(_renderTexture);
            cmd.ClearRenderTarget(true, true, Color.clear);

            DrawingSettings drawingSettings = CreateDrawingSettings(new ShaderTagId("UniversalForward"), ref renderingData, SortingCriteria.CommonOpaque);
            FilteringSettings filteringSettings = new(RenderQueueRange.all, _overrideLayerMask);

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }

    RenderToTexturePass renderToTexturePass;
    public LayerMask layerToRender;
    public RenderTexture renderTexture;
    public Camera cam;

    public override void Create()
    {
        renderToTexturePass = new RenderToTexturePass(renderTexture, layerToRender, cam)
        {
            renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing
        };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (!renderingData.cameraData.camera.CompareTag("MainCamera")) return;

        renderer.EnqueuePass(renderToTexturePass);
    }
}
