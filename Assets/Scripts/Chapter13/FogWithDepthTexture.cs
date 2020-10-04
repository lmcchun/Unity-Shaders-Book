using UnityEngine;

public class FogWithDepthTexture : PostEffectsBase
{
#pragma warning disable 0649, IDE0044
    [SerializeField]
    private Shader fogShader;

    [SerializeField, Range(0.0f, 3.0f)]
    private float fogDensity = 1.0f;

    [SerializeField]
    private Color fogColor = Color.white;

    [SerializeField]
    private float fogStart = 0.0f;

    [SerializeField]
    private float fogEnd = 2.0f;
#pragma warning restore 0649, IDE0044

    private Material fogMaterial = null;

    public Material material
    {
        get
        {
            fogMaterial = CheckShaderAndCreateMaterial(fogShader, fogMaterial);
            return fogMaterial;
        }
    }

    private Camera myCamera;

    public Camera MyCamera
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

    void OnEnable()
    {
        MyCamera.depthTextureMode |= DepthTextureMode.Depth;
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (material != null)
        {
            var frustumCorners = Matrix4x4.identity;

            var fov = MyCamera.fieldOfView;
            var near = MyCamera.nearClipPlane;
            var aspect = MyCamera.aspect;

            var halfHeight = near * Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad);
            var myCameraTransform = MyCamera.transform;
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

            material.SetMatrix("_FrustumCornersRay", frustumCorners);

            material.SetFloat("_FogDensity", fogDensity);
            material.SetColor("_FogColor", fogColor);
            material.SetFloat("_FogStart", fogStart);
            material.SetFloat("_FogEnd", fogEnd);

            Graphics.Blit(src, dest, material);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}
