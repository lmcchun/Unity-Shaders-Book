using UnityEngine;

public class BrightnessSaturationAndContrast : PostEffectsBase
{
#pragma warning disable 0649, IDE0044
    [SerializeField]
    private Shader briSatConShader;

    [SerializeField, Range(0.0f, 3.0f)]
    private float brightness = 1.0f;

    [SerializeField, Range(0.0f, 3.0f)]
    private float saturation = 1.0f;

    [SerializeField, Range(0.0f, 3.0f)]
    private float contrast = 1.0f;
#pragma warning restore 0649, IDE0044

    private Material briSatConMaterial;

    public Material Material
    {
        get
        {
            briSatConMaterial = CheckShaderAndCreateMaterial(briSatConShader, briSatConMaterial);
            return briSatConMaterial;
        }
    }

#pragma warning disable IDE0051
    void OnRenderImage(RenderTexture src, RenderTexture dest)
#pragma warning restore IDE0051
    {
        if (Material != null)
        {
            Material.SetFloat("_Brightness", brightness);
            Material.SetFloat("_Saturation", saturation);
            Material.SetFloat("_Contrast", contrast);
            Graphics.Blit(src, dest, Material);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}
