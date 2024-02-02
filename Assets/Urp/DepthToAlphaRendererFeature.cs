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
        public Material materialBlitter;
        public int pass;
        public Pictionarys<string, Data> cameraToRender = new Pictionarys<string, Data>();

        [System.Serializable]
        public class Data
        {
            //public int indexCameraRender;

            public RTHandle rtTarget;
            
            public RenderTexture target;

            public bool Set => rtTarget != null;
        }
    }
    class CustomRenderPass : ScriptableRenderPass
    {
        Settings settings;

        public CustomRenderPass(Settings settings)
        {
            this.settings = settings;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            Settings.Data data;

            if (!settings.cameraToRender.ContainsKey(renderingData.cameraData.camera.name, out int index))
            {
                data = new Settings.Data();
                settings.cameraToRender.Add(renderingData.cameraData.camera.name, data);
            }
            else
                data = settings.cameraToRender[index];


            if(data.target == null)
            {
                Debug.LogWarning(renderingData.cameraData.camera.name + " No posee una target render texture");
            }
            else if (data.rtTarget == null)
                data.rtTarget = RTHandles.Alloc(data.target);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            //aqui va la logica
            //No es necesario verificar la condición aquí, ya que se verifica en AddRenderPasses

            CommandBuffer cmd = CommandBufferPool.Get("DepthToAlphaRendererFeature");

            var data = settings.cameraToRender[renderingData.cameraData.camera.name];

            //Debug.Log(renderingData.cameraData.camera.name + $" Entro a ejecucion: {settings.mat != null} {data.Set}");

            if (settings.materialBlitter != null && data.Set)
            {
                //Debug.Log(renderingData.cameraData.camera.name + " Se ejecuto el blit");

                settings.materialBlitter.SetTexture("_MainTex", renderingData.cameraData.renderer.cameraColorTargetHandle);
                settings.materialBlitter.SetTexture("_MainDepth", renderingData.cameraData.renderer.cameraDepthTargetHandle);
                
                cmd.Blit(null, data.rtTarget, settings.materialBlitter, settings.pass);
            }

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
        if (renderingData.cameraData.cameraType != CameraType.Preview && renderingData.cameraData.cameraType != CameraType.SceneView && (renderingData.cameraData.camera.gameObject.layer == 13))
        {
            m_ScriptablePass.renderPassEvent = settings.renderPassEvent;
            renderer.EnqueuePass(m_ScriptablePass);
        }
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