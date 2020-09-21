using UnityEngine;

public class MotionBlurWithDepthTexture : PostEffectsBase
{
#pragma warning disable 0649, IDE0044
    [SerializeField]
    private Shader motionBlurShader;

    [SerializeField, Range(0.0f, 1.0f)]
    private float blurSize = 0.5f;
#pragma warning restore 0649, IDE0044

    private Material motionBlurMaterial = null;

    public Material material
    {
        get
        {
            motionBlurMaterial = CheckShaderAndCreateMaterial(motionBlurShader, motionBlurMaterial);
            return motionBlurMaterial;
        }
    }

    private RenderTexture accumulationTexture;

    public Camera Camera
    {
        get
        {
            if (myCamera == null)
            {
                myCamera = GetComponent<Camera>();
            }
            return myCamera;
        }
    }

    private Camera myCamera;

    private Matrix4x4 previousViewProjectionMatrix;

    void OnEnable()
    {
        Camera.depthTextureMode |= DepthTextureMode.Depth;
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (material != null)
        {
            material.SetFloat("_BlurSize", blurSize);
            material.SetMatrix("_PreviousViewProjectionMatrix", previousViewProjectionMatrix);
            var currentViewProjectionMatrix = Camera.projectionMatrix * Camera.worldToCameraMatrix;
            var currentViewProjectionInverseMatrix = currentViewProjectionMatrix.inverse;
            material.SetMatrix("_CurrentViewProjectionInverseMatrix", currentViewProjectionInverseMatrix);
            previousViewProjectionMatrix = currentViewProjectionMatrix;

            Graphics.Blit(src, dest, material);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}
