﻿using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/*
 * Blit Renderer Feature                                                https://github.com/Cyanilux/URP_BlitRenderFeature
 * ------------------------------------------------------------------------------------------------------------------------
 * Based on the Blit from the UniversalRenderingExamples
 * https://github.com/Unity-Technologies/UniversalRenderingExamples/tree/master/Assets/Scripts/Runtime/RenderPasses
 * 
 * Extended to allow for :
 * - Specific access to selecting a source and destination (via current camera's color / texture id / render texture object
 * - (Pre-2021.2/v12) Automatic switching to using _AfterPostProcessTexture for After Rendering event, in order to correctly handle the blit after post processing is applied
 * - Setting a _InverseView matrix (cameraToWorldMatrix), for shaders that might need it to handle calculations from screen space to world.
 * 		e.g. Reconstruct world pos from depth : https://www.cyanilux.com/tutorials/depth/#blit-perspective 
 * - (2020.2/v10 +) Enabling generation of DepthNormals (_CameraNormalsTexture)
 * 		This will only include shaders who have a DepthNormals pass (mostly Lit Shaders / Graphs)
 		(workaround for Unlit Shaders / Graphs: https://gist.github.com/Cyanilux/be5a796cf6ddb20f20a586b94be93f2b)
 * ------------------------------------------------------------------------------------------------------------------------
 * @Cyanilux
*/

namespace Cyan {
    /*
    CreateAssetMenu here allows creating the ScriptableObject without being attached to a Renderer Asset
    Can then Enqueue the pass manually via https://gist.github.com/Cyanilux/8fb3353529887e4184159841b8cad208
    as a workaround for 2D Renderer not supporting features (prior to 2021.2). Uncomment if needed.
    */
    //	[CreateAssetMenu(menuName = "Cyan/Blit")] 
    public class Blit : ScriptableRendererFeature
    {
        /////////////////////////////////////////////////////////////////////////////////////////////
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

            private System.Action onSetup;

            public BlitPass(RenderPassEvent renderPassEvent, BlitSettings settings, string tag)
            {
                this.renderPassEvent = renderPassEvent;

                this.settings = settings;

                m_ProfilerTag = tag;

                if (settings.srcType == Target.RenderTextureObject && settings.srcTextureObject)
                    srcTextureObject = RTHandles.Alloc(settings.srcTextureObject);

                if (settings.dstType == Target.RenderTextureObject && settings.dstTextureObject)
                    dstTextureObject = RTHandles.Alloc(settings.dstTextureObject);

                onSetup = () => 
                {
                    if (s_TempRTHandle == null)
                    {
                        s_TempRTHandle = RTHandles.Alloc(Vector2.one, TextureXR.slices, dimension: TextureXR.dimension, colorFormat: settings.graphicsFormat, enableRandomWrite: true, useDynamicScale: true, name: "TempRT");
                    }

                    temp = s_TempRTHandle;

                    onSetup = () => { };
                };
            }

            public void Setup(ScriptableRenderer renderer)
            {
                if (settings.requireDepthNormals)
                    ConfigureInput(ScriptableRenderPassInput.Normal);

                onSetup();
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
                if (renderingData.cameraData.cameraType == CameraType.Preview || renderingData.cameraData.camera.gameObject.layer != settings.mask) 
                    return;

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
                temp?.Release(); //lo quite para que no se destruya el temporario
                dstTextureId?.Release();
            }
        }
        /////////////////////////////////////////////////////////////////////////////////////////////
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

        /////////////////////////////////////////////////////////////////////////////////////////////
        public enum Target
        {
            CameraColor,
            TextureID,
            RenderTextureObject
        }
        
        /////////////////////////////////////////////////////////////////////////////////////////////
        

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