using UnityEngine;

public class FogWithNoise : PostEffectsBase
{
#pragma warning disable 0649, IDE0044
    [SerializeField]
    private Shader fogShader;

    [SerializeField, Range(0.1f, 3.0f)]
    private float fogDensity = 1.0f;

    [SerializeField]
    private Color fogColor = Color.white;

    [SerializeField]
    private float fogStart = 0.0f;

    [SerializeField]
    private float fogEnd = 2.0f;

    [SerializeField]
    private Texture noiseTexture;

    [SerializeField, Range(-0.5f, 0.5f)]
    private float fogXSpeed = 0.1f;

    [SerializeField, Range(-0.5f, 0.5f)]
    private float fogYSpeed = 0.1f;

    [SerializeField, Range(0.0f, 3.0f)]
    private float noiseAmount = 1.0f;
#pragma warning restore 0649, IDE0044

    private Material fogMaterial = null;

    private Camera myCamera = null;

    void Start()
    {
        fogMaterial = CheckShaderAndCreateMaterial(fogShader, fogMaterial);
    }

    void OnEnable()
    {
        if (myCamera == null)
        {
            myCamera = GetComponent<Camera>();
        }
        myCamera.depthTextureMode |= DepthTextureMode.Depth;
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (fogMaterial != null)
        {
            var frustumCorners = Matrix4x4.identity;

            var fov = myCamera.fieldOfView;
            var near = myCamera.nearClipPlane;
            var aspect = myCamera.aspect;

            var halfHeight = near * Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad);
            var myCameraTransform = myCamera.transform;
            var toRight = myCameraTransform.right * halfHeight * aspect;
            var toTop = myCameraTransform.up * halfHeight;

            var center = myCameraTransform.forward * near;
            var topLeft = center + toTop - toRight;
            var scale = topLeft.magnitude / near;

            topLeft.Normalize();
            topLeft *= scale;

            var topRight = center + toTop + toRight;
            topRight.Normalize();
            topRight *= scale;

            var bottomLeft = center - toTop - toRight;
            bottomLeft.Normalize();
            bottomLeft *= scale;

            var bottomRight = center - toTop + toRight;
            bottomRight.Normalize();
            bottomRight *= scale;

            frustumCorners.SetRow(0, bottomLeft);
            frustumCorners.SetRow(1, bottomRight);
            frustumCorners.SetRow(2, topRight);
            frustumCorners.SetRow(3, topLeft);

            fogMaterial.SetMatrix("_FrustumCornersRay", frustumCorners);

            fogMaterial.SetFloat("_FogDensity", fogDensity);
            fogMaterial.SetColor("_FogColor", fogColor);
            fogMaterial.SetFloat("_FogStart", fogStart);
            fogMaterial.SetFloat("_FogEnd", fogEnd);

            fogMaterial.SetTexture("_NoiseTex", noiseTexture);
            fogMaterial.SetFloat("_FogXSpeed", fogXSpeed);
            fogMaterial.SetFloat("_FogYSpeed", fogYSpeed);
            fogMaterial.SetFloat("_NoiseAmount", noiseAmount);

            Graphics.Blit(src, dest, fogMaterial);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}
