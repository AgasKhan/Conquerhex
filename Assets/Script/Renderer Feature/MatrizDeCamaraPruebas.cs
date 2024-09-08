using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MatrizRenderFeature : ScriptableRendererFeature
{
    public class MatrizDeCamaraPruebas : ScriptableRenderPass
    {
        Data data;

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

            data._view = renderingData.cameraData.camera.worldToCameraMatrix;
            data._proj = renderingData.cameraData.camera.projectionMatrix;

            Debug.Log(data._view);
            Debug.Log(renderingData.cameraData.camera.cullingMatrix);
            Debug.Log("///////////////////////////");

            //for (int i = 0; i < _renderTextures.Length; i++)
            {
                cmd.SetRenderTarget(data._renderTextures[0]);
                cmd.ClearRenderTarget(true, true, Color.clear);
                data._view = GetModifiedViewMatrix(0, renderingData.cameraData.camera.transform.position);


                Debug.Log(data._view);

                cmd.SetViewProjectionMatrices(data._view, data._proj);

                renderingData.cameraData.camera.TryGetCullingParameters(out var scriptableCullingParameters);

                scriptableCullingParameters.origin = data._position;

                var cullResults = context.Cull(ref scriptableCullingParameters);

                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                //Debug.Log(data._view);

               

                DrawingSettings drawingSettings = CreateDrawingSettings(new ShaderTagId("UniversalForward"), ref renderingData, SortingCriteria.CommonOpaque);
                FilteringSettings filteringSettings = new(RenderQueueRange.all, renderingData.cameraData.camera.cullingMask);

                context.DrawRenderers(cullResults, ref drawingSettings, ref filteringSettings);


                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
            }

            


            CommandBufferPool.Release(cmd);
        }

        //projection: perspective or orthogonal
        //

        Matrix4x4 GetModifiedViewMatrix(int index, Vector3 pos)
        {
            //Ver de cambiar la posicion de la camara o cambiar el codigo... Temporal
            //Vector3 positionOffset = new Vector3(index * 2, index * 2, 0);

            //Matrix4x4.TRS(_position, _rotation, Vector3.one);

            //Hay que cambiar la posicion de translate de camara con la posicion deseada

            Vector3 deltaPosition = (pos - data._position);

            deltaPosition.x *= -1;
            deltaPosition.y *= -1;

            Matrix4x4 m = data._view * Matrix4x4.TRS(deltaPosition, data._rotation, data._scale);

            return m;
        }
    }

    MatrizDeCamaraPruebas _pass;
    public Data data;

    [System.Serializable]
    public class Data
    {
        public RenderTexture[] _renderTextures;

        
        public Quaternion _rotation = Quaternion.identity;

        public Vector3 _scale;
        
        public Vector3 _position;

        public Matrix4x4 _view;
        public Matrix4x4 _proj;
    }


    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {

        if (!renderingData.cameraData.camera.CompareTag("MainCamera"))
            return;

        renderer.EnqueuePass(_pass);
    }
    public override void Create()
    {
        _pass = new MatrizDeCamaraPruebas(data);
    }

}
