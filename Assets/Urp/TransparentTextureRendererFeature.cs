using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TransparentTextureRendererFeature : ScriptableRendererFeature
{
    private class TransparentTextureRenderPass : ScriptableRenderPass
    {
        private RTHandle transparentTextureHandle;

        private const string ProfilerTag = "Transparent Texture";

        List<ShaderTagId> shaderTagList = new List<ShaderTagId>();

        public TransparentTextureRenderPass(List<string> shaderTagList)
        {
            foreach (var item in shaderTagList)
            {
                this.shaderTagList.Add(new ShaderTagId(item));
            }

            renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
  
            transparentTextureHandle = RTHandles.Alloc("_CameraTransparentTexture");
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(ProfilerTag);

            var descriptor = renderingData.cameraData.cameraTargetDescriptor;
            descriptor.msaaSamples = 1;
            descriptor.depthBufferBits = 0;

            // Obtén el identificador único de la textura temporal
            var tempRTID = Shader.PropertyToID(transparentTextureHandle.name);

            cmd.GetTemporaryRT(tempRTID, descriptor);

            RenderTargetIdentifier destinationIdentifier = transparentTextureHandle;
            cmd.Blit(BuiltinRenderTextureType.CameraTarget, destinationIdentifier);
            cmd.SetGlobalTexture(tempRTID, destinationIdentifier);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);


            // Draw the objects that are using materials associated with this pass.
            var drawingSettings = CreateDrawingSettings(shaderTagList, ref renderingData, SortingCriteria.CommonTransparent);

            var filteringSettings = new FilteringSettings(RenderQueueRange.transparent, ~0);

            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            var tempRTID = Shader.PropertyToID(transparentTextureHandle.name);

            cmd.ReleaseTemporaryRT(tempRTID);
        }
    }

    public List<string> shaderTagList = new List<string>();

    private TransparentTextureRenderPass pass;

    public override void Create()
    {
        pass = new TransparentTextureRenderPass(shaderTagList);
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(pass);
    }
}