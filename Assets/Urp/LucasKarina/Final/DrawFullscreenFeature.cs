namespace UnityEngine.Rendering.Universal
{
 /*   public enum BufferType
    {
        CameraColor,
        Custom 
    }

    public class DrawFullscreenFeature : ScriptableRendererFeature
    {
        
        [System.Serializable]
        public class Settings
        {
            public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingOpaques;

            public Material blitMaterial = null;
            public int blitMaterialPassIndex = -1;
            public BufferType sourceType = BufferType.CameraColor;
            public BufferType destinationType = BufferType.CameraColor;
            public string sourceTextureId = "_SourceTexture";
            public string destinationTextureId = "_DestinationTexture";
            public LayerMask mask;
        }

        public Settings settings = new Settings();
        DrawFullscreenPass blitPass;

        public override void Create()
        {
            blitPass = new DrawFullscreenPass(name);
            
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (settings.blitMaterial == null)
            {
                Debug.LogWarningFormat("Missing Blit Material. {0} blit pass will not execute. Check for missing reference in the assigned renderer.", GetType().Name);
                return;
            }

            blitPass.renderPassEvent = settings.renderPassEvent;
            blitPass.settings = settings;
            blitPass.MaskFilter(settings.mask);
            renderer.EnqueuePass(blitPass);
           
        }
    }

}*/

    public class Blit : ScriptableRendererFeature
    {
        public class BlitPass : ScriptableRenderPass
        {

            private BlitSettings settings;

            private RTHandle source;
            private RTHandle destination;
            private RTHandle temp;

            //private RTHandle srcTextureId;
            private RTHandle srcTextureObject;
            private RTHandle dstTextureId;
            private RTHandle dstTextureObject;

            private string m_ProfilerTag;

            private static RTHandle s_TempRTHandle;

            public BlitPass(RenderPassEvent renderPassEvent, BlitSettings settings, string tag)
            {
                this.renderPassEvent = renderPassEvent;
                this.settings = settings;
                m_ProfilerTag = tag;
                if (settings.srcType == Target.RenderTextureObject && settings.srcTextureObject)
                    srcTextureObject = RTHandles.Alloc(settings.srcTextureObject);
                if (settings.dstType == Target.RenderTextureObject && settings.dstTextureObject)
                    dstTextureObject = RTHandles.Alloc(settings.dstTextureObject);

                if (s_TempRTHandle == null)
                {
                    s_TempRTHandle = RTHandles.Alloc(Vector2.one, TextureXR.slices, dimension: TextureXR.dimension, colorFormat: settings.graphicsFormat, enableRandomWrite: true, useDynamicScale: true, name: "TempRT");
                }

                temp = s_TempRTHandle;
            }

            public void Setup(ScriptableRenderer renderer)
            {
                if (settings.requireDepthNormals)
                    ConfigureInput(ScriptableRenderPassInput.Normal);
            }

            public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
            {
                if (renderingData.cameraData.camera.gameObject.layer != settings.mask)
                    return;

                var desc = renderingData.cameraData.cameraTargetDescriptor;
                desc.depthBufferBits = 0; // Color and depth cannot be combined in RTHandles

                //RenderingUtils.ReAllocateIfNeeded(ref temp, Vector2.one, desc, name: "_TemporaryColorTexture");
                // These resizable RTHandles seem quite glitchy when switching between game and scene view :\
                // instead,
                RenderingUtils.ReAllocateIfNeeded(ref temp, desc, name: "_TemporaryColorTexture");

                var renderer = renderingData.cameraData.renderer;
                if (settings.srcType == Target.CameraColor)
                {
                    source = renderer.cameraColorTargetHandle;
                }
                else if (settings.srcType == Target.TextureID)
                {
                    //RenderingUtils.ReAllocateIfNeeded(ref srcTextureId, Vector2.one, desc, name: settings.srcTextureId);
                    //source = srcTextureId;
                    /*
                    Doesn't seem to be a good way to get an existing target with this new RTHandle system.
                    The above would work but means I'd need fields to set the desc too, which is just messy. If they don't match completely we get a new target
                    Previously we could use a RenderTargetIdentifier... but the Blitter class doesn't have support for those in 2022.1 -_-
                    Instead, I guess we'll have to rely on the shader sampling the global textureID
                    */
                    source = temp;
                }
                else if (settings.srcType == Target.RenderTextureObject)
                {
                    source = srcTextureObject;
                }

                if (settings.dstType == Target.CameraColor)
                {
                    destination = renderer.cameraColorTargetHandle;
                }
                else if (settings.dstType == Target.TextureID)
                {
                    desc.graphicsFormat = settings.graphicsFormat;
                    RenderingUtils.ReAllocateIfNeeded(ref dstTextureId, Vector2.one, desc, name: settings.dstTextureId);
                    destination = dstTextureId;
                }
                else if (settings.dstType == Target.RenderTextureObject)
                {
                    destination = dstTextureObject;
                }
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                if (renderingData.cameraData.cameraType == CameraType.Preview || renderingData.cameraData.camera.gameObject.layer != settings.mask) return;

                CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);
                if (settings.setInverseViewMatrix)
                {
                    cmd.SetGlobalMatrix("_InverseView", renderingData.cameraData.camera.cameraToWorldMatrix);
                }

                //Debug.Log("blit : src = " + source.name + ", dst = " + destination.name);
                if (source == destination)
                {
                    Blitter.BlitCameraTexture(cmd, source, temp, settings.blitMaterial, settings.blitMaterialPassIndex);
                    Blitter.BlitCameraTexture(cmd, temp, destination, Vector2.one);
                }
                else
                {
                    Blitter.BlitCameraTexture(cmd, source, destination, settings.blitMaterial, settings.blitMaterialPassIndex);
                }

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }

            public override void OnCameraCleanup(CommandBuffer cmd)
            {
                source = null;
                destination = null;
            }

            public void Dispose()
            {
                temp?.Release();
                dstTextureId?.Release();
            }
        }

        [System.Serializable]
        public class BlitSettings
        {
            public RenderPassEvent Event = RenderPassEvent.AfterRenderingOpaques;

            public Material blitMaterial = null;
            public int blitMaterialPassIndex = 0;
            public bool setInverseViewMatrix = false;
            public bool requireDepthNormals = false;

            public Target srcType = Target.CameraColor;
            //public string srcTextureId = "_CameraColorTexture";
            public RenderTexture srcTextureObject;

            public Target dstType = Target.CameraColor;
            public string dstTextureId = "_BlitPassTexture";
            public RenderTexture dstTextureObject;

            public int mask;

            public bool overrideGraphicsFormat = false;
            public UnityEngine.Experimental.Rendering.GraphicsFormat graphicsFormat;
        }

        public enum Target
        {
            CameraColor,
            TextureID,
            RenderTextureObject
        }

        public BlitSettings settings = new BlitSettings();
        public BlitPass blitPass;

        public override void Create()
        {
            var passIndex = settings.blitMaterial != null ? settings.blitMaterial.passCount - 1 : 1;
            settings.blitMaterialPassIndex = Mathf.Clamp(settings.blitMaterialPassIndex, -1, passIndex);
            blitPass = new BlitPass(settings.Event, settings, name);

            if (settings.graphicsFormat == UnityEngine.Experimental.Rendering.GraphicsFormat.None)
            {
                settings.graphicsFormat = SystemInfo.GetGraphicsFormat(UnityEngine.Experimental.Rendering.DefaultFormat.LDR);
            }
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (settings.blitMaterial == null)
            {
                Debug.LogWarningFormat("Missing Blit Material. {0} blit pass will not execute. Check for missing reference in the assigned renderer.", GetType().Name);
                return;
            }

            if (renderingData.cameraData.camera.gameObject.layer != settings.mask) 
                return;

            renderer.EnqueuePass(blitPass);
        }

        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            if (renderingData.cameraData.camera.gameObject.layer != settings.mask)
                return;

            blitPass.Setup(renderer);
        }

        protected override void Dispose(bool disposing)
        {
            blitPass.Dispose();
        }
    }
}



