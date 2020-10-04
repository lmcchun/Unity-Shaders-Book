using UnityEngine;

public class EdgeDetectNormalAndDepth : PostEffectsBase
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

    [SerializeField]
    private float sampleDistance = 1.0f;

    [SerializeField]
    private float sensitivityDepth = 1.0f;

    [SerializeField]
    private float sensitivityNormal = 1.0f;
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
        MyCamera.depthTextureMode |= DepthTextureMode.DepthNormals;
    }

#pragma warning disable IDE0051
    [ImageEffectOpaque]
    void OnRenderImage(RenderTexture src, RenderTexture dest)
#pragma warning restore IDE0051
    {
        if (Material != null)
        {
            Material.SetFloat("_EdgeOnly", edgesOnly);
            Material.SetColor("_EdgeColor", edgeColor);
            Material.SetColor("_BackgroundColor", backgroundColor);
            Material.SetFloat("_SampleDistance", sampleDistance);
            Material.SetVector("_Sensitivity", new Vector4(sensitivityNormal, sensitivityDepth, 0.0f, 0.0f));
            Graphics.Blit(src, dest, Material);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}
