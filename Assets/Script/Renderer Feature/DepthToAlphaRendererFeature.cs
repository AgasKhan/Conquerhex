using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DepthToAlphaRendererFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class DataPerCamera
    {
        public string name;
        public RTHandle rtTargetColor { get; set; }
        /// <summary>
        /// destinado a donde va a ser copiada la textura procesada de las camaras
        /// </summary>
        public RenderTexture targetColor;
        public Transform offset { get; set; }

        public ScriptableCullingParameters cullingParameters;

        public Matrix4x4 view;
        public Matrix4x4 proj;

        public Pictionarys<string,Material> materialsToApply = new();

        public bool Set => rtTargetColor != null;

        public void Configure(Settings settings, RenderTextureDescriptor desc, Material updateRef)
        {
            if (materialsToApply.Count==0)
            {
                materialsToApply.Add("DepthToAlpha", settings.materialBlitter);
            }
            
            if (Set)
            {
                // Verifica si el tamaño actual es diferente al nuevo tamaño
                if (Screen.width != targetColor.width || Screen.height != targetColor.height)
                {
                    targetColor.Release();

                    targetColor.width = Screen.width;
                    targetColor.height = Screen.height;

                    targetColor.Create();

                    Debug.Log(Screen.width + " " + targetColor.width + "-" + Screen.height + " " + targetColor.height);
                }
            }
            else
            {
                if (targetColor == null)
                    return;
                
                rtTargetColor = RTHandles.Alloc(targetColor);
                
                updateRef.SetTexture(name, targetColor);
            }
        }

        public void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData, Settings settings)
        {
            if (!renderingData.cameraData.camera.TryGetCullingParameters(out var originalCullingParams))
            {
                Debug.LogWarning("No se puedo obtener el cullingParameter");
            }

            var deltaPosition = (offset.position - renderingData.cameraData.camera.transform.position);
            
            deltaPosition *= -1;
            
            view = settings.view  * Matrix4x4.TRS(deltaPosition, Quaternion.identity, Vector3.one);;
            proj = settings.proj;

            //Debug.Log(offset.name + "\n" +view);
            
            cullingParameters = originalCullingParams;
            
            for (int i = 0; i < cullingParameters.cullingPlaneCount; i++)
            {
                var plane = cullingParameters.GetCullingPlane(i);
                plane.Translate(deltaPosition);
                cullingParameters.SetCullingPlane(i, plane);
            }

            if(settings!=null)
                cullingParameters.cullingMask = (uint)(int)settings.cullingMask;

            //Debug.Log($"{offset.position}\n{deltaPosition}\n{view}");
        }

        public DataPerCamera(string name)
        {
            this.name = name;
        }
    }

    [System.Serializable]
    public class Settings
    {
        [Header("Camera to texture")]
        [Tooltip("Target layer pj")]
        public LayerMask targetLayer = 1; // Puedes ajustar el valor predeterminado según tus necesidades
        [Tooltip("Render texture to ui Player")]
        public RenderTexture renderTexturePlayer;

        [Header("offset renderer")]
        public LayerMask cullingMask;

        [Header("Depth to alpha")]
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingSkybox;
        public Material materialBlitter;
        public int pass;
        public Material materialCombined;
        public Material combinedMaterial;
        
        public Matrix4x4 view;
        public Matrix4x4 proj;

        public List<DataPerCamera> cameraToRender = new();

        public DataPerCamera Search(string name)
        {
            foreach (var dataPerCamera in cameraToRender)
            {
                if (dataPerCamera.name == name)
                    return dataPerCamera;
            }

            return null;
        }
    }
    class CustomRenderPass : ScriptableRenderPass
    {
        Settings settings;
        
        private ProfilingSampler _profilingSampler;
        
        private List<ShaderTagId> _shaderTagIds = new List<ShaderTagId>();

        private RTHandle colorBuffer;
        private RTHandle depthBuffer;
        
        private RenderTexture auxiliarTexture;

        public CustomRenderPass(Settings settings, string name)
        {
            this.settings = settings;
            
            _shaderTagIds.Add(new ShaderTagId("SRPDefaultUnlit"));
            _shaderTagIds.Add(new ShaderTagId("UniversalForward"));
            _shaderTagIds.Add(new ShaderTagId("UniversalForwardOnly"));
            
            _profilingSampler = new ProfilingSampler(name);
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            foreach (var item in settings.cameraToRender)
            {
                item.Configure(settings,cameraTextureDescriptor, settings.materialCombined);
            }
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            depthBuffer = renderingData.cameraData.renderer.cameraDepthTargetHandle;
            colorBuffer = renderingData.cameraData.renderer.cameraColorTargetHandle;
            
            if (auxiliarTexture == null)
            {
                auxiliarTexture = new(colorBuffer);
            }
            else
            {
                auxiliarTexture.Release();
                auxiliarTexture.descriptor = colorBuffer.rt.descriptor;
                auxiliarTexture.Create();
            }
            
            settings.view = renderingData.cameraData.camera.worldToCameraMatrix;
            settings.proj = renderingData.cameraData.camera.projectionMatrix;
            
            if (MainCamera.instance == null)
                return;

            DataPerCamera data;

            foreach (var camera in MainCamera.instance.rendersOverlay.camerasTr)
            {
                if (camera.gameObject.layer != 13)
                    continue;

                string name = "_" + camera.name;

                if ((data = settings.Search(name)) == null)
                {
                    data = new DataPerCamera(name);
                    data.offset = camera;

                    data.OnCameraSetup(cmd, ref renderingData, settings);
                    
                    settings.cameraToRender.Add(data);
                }
                else
                {
                    data.offset = camera;
                }
            }
        
        }


        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("DepthToAlphaRendererFeature");
            //aqui va la logica
            //No es necesario verificar la condición aquí, ya que se verifica en AddRenderPasses

            using (new ProfilingScope(cmd, _profilingSampler))
            {           
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                
                foreach (var data in settings.cameraToRender)
                {
                    if (data?.offset==null || !data.offset.gameObject.activeInHierarchy)
                        continue;
                    
                    //Debug.Log("Render " + data.name);
                    if(data.name != "_MainCamera")
                    {
                        //if (data.offset.gameObject.activeInHierarchy)
                        data.OnCameraSetup(cmd, ref renderingData, settings);
                        
                        //var cullResults = context.Cull(ref data.cullingParameters);
                        
                        var cullResults = renderingData.cullResults;
                        
                        cmd.SetRenderTarget(colorBuffer, depthBuffer);
                        cmd.ClearRenderTarget(RTClearFlags.All, Color.clear,  1, 0);
                        cmd.SetViewProjectionMatrices(data.view, data.proj);
                        
                        context.ExecuteCommandBuffer(cmd);
                        cmd.Clear();
                        
                        DrawingSettings drawingSettings = CreateDrawingSettings(_shaderTagIds, ref renderingData, SortingCriteria.CommonOpaque);
                        FilteringSettings filteringSettings = new(RenderQueueRange.all, Physics.AllLayers);
                        context.DrawRenderers(cullResults, ref drawingSettings, ref filteringSettings);

                        //Shader.SetGlobalTexture("_CameraDepthTexture", depthBuffer);
                        
                        context.ExecuteCommandBuffer(cmd);
                        cmd.Clear();
                    }
                    
                    ///////////////////////////////////////////////////////////////////////////////////
                    ///DepthToAlpha
                    ///////////////////////////////////////////////////////////////////////////////////
                    if (data.Set)
                    {
                        cmd.Blit(colorBuffer, auxiliarTexture);
                        context.ExecuteCommandBuffer(cmd);
                        cmd.Clear();
                        
                        for (int i = 0; i < data.materialsToApply.Count; i++)
                        {
                            settings.materialBlitter.SetTexture("_MainTex", auxiliarTexture);
                            settings.materialBlitter.SetTexture("_MainDepth", depthBuffer);

                            cmd.Blit(null, data.targetColor, data.materialsToApply[i], -1);

                            context.ExecuteCommandBuffer(cmd);
                            cmd.Clear();

                            if (i < data.materialsToApply.Count - 1)
                            {
                                cmd.Blit(data.targetColor, auxiliarTexture);
                            
                                context.ExecuteCommandBuffer(cmd);
                                cmd.Clear();    
                            }
                        }
                    }
                }
                
                //Separar en otro rendererPass luego
                if (settings.renderTexturePlayer != null)
                {
                    cmd.SetRenderTarget(settings.renderTexturePlayer);
                    cmd.ClearRenderTarget(RTClearFlags.All, Color.clear,  1, 0);
                    cmd.SetViewProjectionMatrices(settings.view, settings.proj);

                    //Al final si era necesario
                    context.ExecuteCommandBuffer(cmd);
                    cmd.Clear();

                    DrawingSettings drawingSettings = CreateDrawingSettings(_shaderTagIds, ref renderingData, SortingCriteria.CommonOpaque);
                    FilteringSettings filteringSettings = new(RenderQueueRange.all, settings.targetLayer);
                            
                    context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);

                    context.ExecuteCommandBuffer(cmd);
                    cmd.Clear();
                }
                
                /*
                //COMBINADO
                cmd.Blit(settings.cameraToRender[0].targetColor, auxiliarTexture);
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                
                for (int i = 1; i < settings.cameraToRender.Count; i++)
                {
                    var data = settings.cameraToRender[i];
                    
                    if (data?.offset == null || !data.offset.gameObject.activeInHierarchy)
                        continue;
                    
                    settings.combinedMaterial.SetTexture("_Texture1", auxiliarTexture);
                    settings.combinedMaterial.SetTexture("_Texture2", data.targetColor);
                    
                    cmd.Blit(null, colorBuffer, settings.combinedMaterial);
                    
                    context.ExecuteCommandBuffer(cmd);
                    cmd.Clear();
                    
                    cmd.Blit(colorBuffer, auxiliarTexture);
                    
                    context.ExecuteCommandBuffer(cmd);
                    cmd.Clear();        
                }
                */
            }
            
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            auxiliarTexture.Release();
        }
    }

    public Settings settings = new Settings();

    CustomRenderPass m_ScriptablePass;

    public override void Create()
    {
        m_ScriptablePass = new CustomRenderPass(settings, name);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        // Añadir la pasada solo si el layer de la cámara coincide con el targetLayer
        //if (renderingData.cameraData.cameraType == CameraType.Preview || renderingData.cameraData.cameraType == CameraType.Reflection || renderingData.cameraData.cameraType == CameraType.SceneView)
        //{
          //  return;
        //}

        if (renderingData.cameraData.camera.CompareTag("MainCamera"))
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