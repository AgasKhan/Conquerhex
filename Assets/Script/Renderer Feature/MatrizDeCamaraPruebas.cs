
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MatrizRenderFeature : ScriptableRendererFeature
{
    public class MatrizDeCamaraPruebas : ScriptableRenderPass
    {
        RenderTargetIdentifier[] _renderTextures;

        Matrix4x4 _view;
        Matrix4x4 _proj;

        public MatrizDeCamaraPruebas(RenderTargetIdentifier[] render)
        {
            _renderTextures = render;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("CameraMatrixViews");

            //Camera cam = renderingData.cameraData.camera;
            // _view = Matrix4x4.LookAt(cam.transform.position, Character, el up de la camara);
            //_proj = Matrix4x4.Perspective(cam.fieldOfView, cam.aspect, cam.nearClipPlane, cam.farClipPlane);

            _view = renderingData.cameraData.camera.worldToCameraMatrix;
            _proj = renderingData.cameraData.camera.projectionMatrix;


            for (int i = 0; i < _renderTextures.Length; i++)
            {
                cmd.SetRenderTarget(_renderTextures[i]);

                Matrix4x4 viewMatrix = GetModifiedViewMatrix(i);

                cmd.SetViewProjectionMatrices(viewMatrix, _proj);
                cmd.ClearRenderTarget(true, true, Color.black);


                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
            }



            CommandBufferPool.Release(cmd);
        }

        Matrix4x4 GetModifiedViewMatrix(int index)
        {
            //Ver de cambiar la posicion de la camara o cambiar el codigo... Temporal
            Vector3 positionOffset = new Vector3(index * 2, index * 2, 0);

            //Hay que cambiar la posicion de translate de camara con la posicion deseada
            return Matrix4x4.Translate(positionOffset) * _view;
        }
    }

    MatrizDeCamaraPruebas _pass;
    public RenderTexture[] ren;



    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(_pass);

    }

    public override void Create()
    {

        RenderTargetIdentifier[] ids = new RenderTargetIdentifier[ren.Length];

        for (int i = 0; i < ren.Length; i++)
        {
            ids[i] = new RenderTargetIdentifier(ren[i]);
        }

        _pass = new MatrizDeCamaraPruebas(ids);
    }
}
