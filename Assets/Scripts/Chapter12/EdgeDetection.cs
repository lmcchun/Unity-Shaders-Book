using UnityEngine;

public class EdgeDetection : PostEffectsBase
{
#pragma warning disable 0649, IDE0044
    [SerializeField]
    private Shader edgeDetectShader;

    [SerializeField, Range(0.0f, 1.0f)]
    private float edgesOnly = 0.0f;

    [SerializeField]
    private Color edgeColor = Color.black;

    [SerializeField]
    private Color backgroundColor = Color.white;
#pragma warning restore 0649, IDE0044

    private Material edgeDetectMaterial = null;

    public Material Material
    {
        get
        {
            edgeDetectMaterial = CheckShaderAndCreateMaterial(edgeDetectShader, edgeDetectMaterial);
            return edgeDetectMaterial;
        }
    }

#pragma warning disable IDE0051
    void OnRenderImage(RenderTexture src, RenderTexture dest)
#pragma warning restore IDE0051
    {
        if (Material != null)
        {
            Material.SetFloat("_EdgeOnly", edgesOnly);
            Material.SetColor("_EdgeColor", edgeColor);
            Material.SetColor("_BackgroundColor", backgroundColor);
            Graphics.Blit(src, dest, Material);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}
