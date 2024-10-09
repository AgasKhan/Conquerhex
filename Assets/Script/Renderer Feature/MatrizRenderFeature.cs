using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MatrizRenderFeature : ScriptableRendererFeature
{
    public class MatrizDeCamaraPruebas : ScriptableRenderPass
    {
        Data data;

        Vector3 deltaPosition;

        public MatrizDeCamaraPruebas(Data data)
        {
            this.data = data;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("CameraMatrixViews");

            //Camera cam = renderingData.cameraData.camera;
            // _view = Matrix4x4.LookAt(cam.transform.position, Character, el up de la camara);
            //_proj = Matrix4x4.Perspective(cam.fieldOfView, cam.aspect, cam.nearClipPlane, cam.farClipPlane);

            data.originalView = renderingData.cameraData.camera.worldToCameraMatrix;
            data.originalProj = renderingData.cameraData.camera.projectionMatrix;



            if (!renderingData.cameraData.camera.TryGetCullingParameters(out var originalCullingParams))
            {
                Debug.LogWarning("No se puedo obtener el cullingParameter");
            }

            //Debug.Log(data._view);
            //Debug.Log(renderingData.cameraData.camera.cullingMatrix);
            //Debug.Log("///////////////////////////");

            //for (int i = 0; i < _renderTextures.Length; i++)
            {
                deltaPosition = data.position - renderingData.cameraData.camera.transform.position;

                deltaPosition *= -1;

                var cullingParams = originalCullingParams;

                for (int i = 0; i < cullingParams.cullingPlaneCount; i++)
                {
                    var plane = cullingParams.GetCullingPlane(i);
                    plane.Translate(deltaPosition);
                    cullingParams.SetCullingPlane(i, plane);
                }

                data.view = data.originalView * Matrix4x4.TRS(deltaPosition, data.rotation, data.scale);

                data.proj = data.originalProj;

                var cullResults = context.Cull(ref cullingParams);//Esto es lo que lo cuelga

                cmd.SetViewProjectionMatrices(data.view, data.proj);
                cmd.SetRenderTarget(data._renderTextures[0]);
                cmd.ClearRenderTarget(true, true, Color.clear);

                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                DrawingSettings drawingSettings = CreateDrawingSettings(new ShaderTagId("UniversalForward"), ref renderingData, SortingCriteria.CommonOpaque);
                FilteringSettings filteringSettings = new(RenderQueueRange.all, renderingData.cameraData.camera.cullingMask);

                context.DrawRenderers(cullResults, ref drawingSettings, ref filteringSettings);


                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
            }

            CommandBufferPool.Release(cmd);
        }
    }

    MatrizDeCamaraPruebas _pass;
    public Data data;

    [System.Serializable]
    public class Data
    {
        public RenderTexture[] _renderTextures;

        public Vector3 position;
        
        public Quaternion rotation = Quaternion.identity;

        public Vector3 scale;

        public Matrix4x4 originalView;
        public Matrix4x4 originalProj;

        public Matrix4x4 view;
        public Matrix4x4 proj;
    }


    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {

        if (!renderingData.cameraData.camera.CompareTag("MainCamera"))
            return;

        renderer.EnqueuePass(_pass);
    }
    public override void Create()
    {
        _pass = new MatrizDeCamaraPruebas(data)
        {
            renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing
        };
    }
}
