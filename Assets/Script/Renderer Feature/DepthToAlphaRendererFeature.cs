using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DepthToAlphaRendererFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        [System.Serializable]
        public class Data
        {
            public string name;

            public RTHandle rtTarget;

            /// <summary>
            /// destinado a donde va a ser copiada la textura procesada de las camaras
            /// </summary>
            public RenderTexture target;

            /// <summary>
            /// Destinado a donde van a renderizar las camaras
            /// </summary>
            public RenderTexture copyTarget;

            public bool Set => rtTarget != null;

            public void Configure(Settings settings, RenderTextureDescriptor desc, Material updateRef)
            {
                if (target != null)
                {
                    // Verifica si el tamaño actual es diferente al nuevo tamaño
                    if (settings.width != target.width || settings.height != target.height)
                    {
                        //rtTarget.Release();

                        target.Release();

                        target.width = settings.width;

                        target.height = settings.height;

                        target.Create();

                        Debug.Log(settings.width + " " + target.width + "-" + settings.height + " " + target.height);
                    }
                }
                else
                {
                    target = new RenderTexture(desc);

                    //copyTarget = new RenderTexture(desc);

                    rtTarget = RTHandles.Alloc(target);

                    updateRef.SetTexture(name, target);
                }
            }

            public Data(string name)
            {
                this.name = name;
            }
        }

        [Header("Camera to texture")]
        [Tooltip("Target layer pj")]
        public LayerMask targetLayer = 1; // Puedes ajustar el valor predeterminado según tus necesidades
        [Tooltip("Render texture to ui Player")]
        public RenderTexture renderTexturePlayer;

        [Header("Depth to alpha")]
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingSkybox;
        public Material materialBlitter;
        public int pass;
        public Material materialCombined;

        public int width = 1920, height = 1080;

        [HideInInspector]
        public Pictionarys<string, Data> cameraToRender = new Pictionarys<string, Data>();
    }
    class CustomRenderPass : ScriptableRenderPass
    {
        Settings settings;

        public CustomRenderPass(Settings settings)
        {
            this.settings = settings;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            foreach (var item in settings.cameraToRender)
            {
                item.value.Configure(settings, cameraTextureDescriptor, settings.materialCombined);
            }
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            Settings.Data data;

            string name = "_" + renderingData.cameraData.camera.name;

            if (!settings.cameraToRender.ContainsKey(name, out int index))
            {
                data = new Settings.Data(name);
                settings.cameraToRender.Add(name, data);
            }
            /*
            else
            {
                data = settings.cameraToRender[index];
            }
            */

            //Debug.Log("cameraData null = " + (renderingData.cameraData.camera == null) + "\ntargetTexture null =" + (renderingData.cameraData.camera.targetTexture == null));

            if (renderingData.cameraData.camera.targetTexture!=null && (renderingData.cameraData.camera.targetTexture.width != settings.width || renderingData.cameraData.camera.targetTexture.height != settings.height))
            {
                var copyTarget = renderingData.cameraData.camera.targetTexture;

                copyTarget.Release();

                copyTarget.width = settings.width;

                copyTarget.height = settings.height;

                copyTarget.Create();
            }
        }


        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            //aqui va la logica
            //No es necesario verificar la condición aquí, ya que se verifica en AddRenderPasses

            CommandBuffer cmd = CommandBufferPool.Get("DepthToAlphaRendererFeature");

            string name = "_" + renderingData.cameraData.camera.name;

            Settings.Data data;

            if (!settings.cameraToRender.ContainsKey(name, out int index))
            {
                return;
            }

            data = settings.cameraToRender[index];

            if (settings.materialBlitter != null && data.Set)
            {
                //Debug.Log(renderingData.cameraData.camera.name + " Se ejecuto el blit");

                //settings.materialBlitter.SetTexture("_MainTex", renderingData.cameraData.renderer.cameraColorTargetHandle);
                settings.materialBlitter.SetTexture("_MainDepth", renderingData.cameraData.renderer.cameraDepthTargetHandle);
                
                cmd.Blit(null, data.rtTarget, settings.materialBlitter, settings.pass);
            }

            context.ExecuteCommandBuffer(cmd);


            cmd.Clear();

            if (!renderingData.cameraData.camera.gameObject.CompareTag("MainCamera") || settings.renderTexturePlayer==null)
                return;

            cmd.SetRenderTarget(settings.renderTexturePlayer);
            cmd.ClearRenderTarget(true, true, Color.clear);

            DrawingSettings drawingSettings = CreateDrawingSettings(new ShaderTagId("UniversalForward"), ref renderingData, SortingCriteria.CommonOpaque);
            FilteringSettings filteringSettings = new(RenderQueueRange.all, settings.targetLayer);


            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);

            context.ExecuteCommandBuffer(cmd);

            CommandBufferPool.Release(cmd);
        }
    }

    public Settings settings = new Settings();

    CustomRenderPass m_ScriptablePass;

    public override void Create()
    {
        m_ScriptablePass = new CustomRenderPass(settings);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        // Añadir la pasada solo si el layer de la cámara coincide con el targetLayer
        if (renderingData.cameraData.cameraType == CameraType.Preview || renderingData.cameraData.cameraType == CameraType.Reflection || renderingData.cameraData.cameraType == CameraType.SceneView)
        {
            return;
        }

        if (renderingData.cameraData.camera.gameObject.layer == 13)
        {
            m_ScriptablePass.renderPassEvent = settings.renderPassEvent;
            renderer.EnqueuePass(m_ScriptablePass);
        }
    }

    [ContextMenu("Clear")]
    void Clear()
    {
        settings.cameraToRender.Clear();
    }
}


/*
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DepthToAlphaRendererFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public LayerMask targetLayer = 1; // Puedes ajustar el valor predeterminado según tus necesidades
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingSkybox;
        public Material mat;
        public Pictionarys<string, RenderTexture> cameraToRender = new Pictionarys<string, RenderTexture>();
    }
    class CustomRenderPass : ScriptableRenderPass
    {
        Settings settings;
        

        public CustomRenderPass(Settings settings)
        {
            this.settings = settings;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            // Aquí puedes poner tu lógica personalizada para renderizar
            // No es necesario verificar la condición aquí, ya que se verifica en AddRenderPasses

            CommandBuffer cmd = CommandBufferPool.Get("DepthToAlphaRendererFeature");

            if(settings.mat!=null)
            {
                cmd.Blit(renderingData.cameraData.camera.targetTexture, settings.cameraToRender[renderingData.cameraData.camera.name], settings.mat,0);
            }
                

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            if (!settings.cameraToRender.ContainsKey(renderingData.cameraData.camera.name))
            {
                settings.cameraToRender.Add(renderingData.cameraData.camera.name, renderingData.cameraData.camera.targetTexture);

                renderingData.cameraData.camera.targetTexture = new RenderTexture(renderingData.cameraData.camera.targetTexture);

                renderingData.cameraData.camera.targetTexture.name = "Copy";

                Debug.Log(renderingData.cameraData.camera.targetTexture.name + " " + settings.cameraToRender[renderingData.cameraData.camera.name]);
            }
        }
    }

    public Settings settings = new Settings();

    CustomRenderPass m_ScriptablePass;

    public override void Create()
    {
        m_ScriptablePass = new CustomRenderPass(settings);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        // Añadir la pasada solo si el layer de la cámara coincide con el targetLayer
        if ((renderingData.cameraData.camera.gameObject.layer == 13))
        {
            m_ScriptablePass.renderPassEvent = settings.renderPassEvent;
            renderer.EnqueuePass(m_ScriptablePass);
        }
    }
}
*/