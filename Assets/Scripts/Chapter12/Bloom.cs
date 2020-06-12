using UnityEngine;

public class Bloom : PostEffectsBase
{
#pragma warning disable 0649, IDE0044
    [SerializeField]
    private Shader bloomShader;

    // Blur iterations - larger number means more blur.
    [SerializeField, Range(0, 4)]
    private int iterations = 3;

    // Blur spread for each iteration - larger value means more blur.
    [SerializeField, Range(0.2f, 3.0f)]
    private float blurSpread = 0.6f;

    [SerializeField, Range(1, 8)]
    private int downSample = 2;

    [SerializeField, Range(0.0f, 4.0f)]
    private float luminanceThreshold = 0.6f;
#pragma warning restore 0649, IDE0044

    private Material bloomMaterial = null;

    public Material Material
    {
        get
        {
            bloomMaterial = CheckShaderAndCreateMaterial(bloomShader, bloomMaterial);
            return bloomMaterial;
        }
    }

#pragma warning disable IDE0051
    void OnRenderImage(RenderTexture src, RenderTexture dest)
#pragma warning restore IDE0051
    {
        if (Material != null)
        {
            Material.SetFloat("_LuminanceThreshold", luminanceThreshold);

            var rtW = src.width / downSample;
            var rtH = src.height / downSample;

            var buffer0 = RenderTexture.GetTemporary(rtW, rtH, 0);
            buffer0.filterMode = FilterMode.Bilinear;

            Graphics.Blit(src, buffer0, Material, 0);

            for (var i = 0; i < iterations; ++i)
            {
                Material.SetFloat("_BlurSize", 1.0f + i * blurSpread);

                var buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);

                // Render the vertical pass
                Graphics.Blit(buffer0, buffer1, Material, 1);

                RenderTexture.ReleaseTemporary(buffer0);
                buffer0 = buffer1;
                buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);

                // Render the horizontal pass
                Graphics.Blit(buffer0, buffer1, Material, 2);

                RenderTexture.ReleaseTemporary(buffer0);
                buffer0 = buffer1;
            }

            Material.SetTexture("_Bloom", buffer0);
            Graphics.Blit(src, dest, Material, 3);

            RenderTexture.ReleaseTemporary(buffer0);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}
