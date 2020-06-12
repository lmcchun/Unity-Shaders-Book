using UnityEngine;

public class GaussianBlur : PostEffectsBase
{
#pragma warning disable 0649, IDE0044
    [SerializeField]
    private Shader gaussianBlurShader;

    // Blur iterations - larger number means more blur.
    [SerializeField, Range(0, 4)]
    private int iterations = 3;

    // Blur spread for each iteration - larger value means more blur.
    [SerializeField, Range(0.2f, 3.0f)]
    private float blurSpread = 0.6f;

    [SerializeField, Range(1, 8)]
    private int downSample = 2;
#pragma warning restore 0649, IDE0044

    private Material gaussianBlurMaterial = null;

    public Material Material
    {
        get
        {
            gaussianBlurMaterial = CheckShaderAndCreateMaterial(gaussianBlurShader, gaussianBlurMaterial);
            return gaussianBlurMaterial;
        }
    }

#pragma warning disable IDE0051
    void OnRenderImage(RenderTexture src, RenderTexture dest)
#pragma warning restore IDE0051
    {
        //OnRenderImage1(src, dest);
        //OnRenderImage2(src, dest);
        OnRenderImage3(src, dest);
    }

    // 1st edition: just apply blur
    void OnRenderImage1(RenderTexture src, RenderTexture dest)
    {
        if (Material != null)
        {
            var rtW = src.width;
            var rtH = src.height;
            var buffer = RenderTexture.GetTemporary(rtW, rtH, 0);

            // Render the vertical pass
            Graphics.Blit(src, buffer, Material, 0);
            // Render the horizontal pass
            Graphics.Blit(buffer, dest, Material, 1);

            RenderTexture.ReleaseTemporary(buffer);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }

    // 2nd edition: scale the render texture
    void OnRenderImage2(RenderTexture src, RenderTexture dest)
    {
        if (Material != null)
        {
            var rtW = src.width / downSample;
            var rtH = src.height / downSample;
            var buffer = RenderTexture.GetTemporary(rtW, rtH, 0);
            buffer.filterMode = FilterMode.Bilinear;

            // Render the vertical pass
            Graphics.Blit(src, buffer, Material, 0);
            // Render the horizontal pass
            Graphics.Blit(buffer, dest, Material, 1);

            RenderTexture.ReleaseTemporary(buffer);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }

    // 3rd edition: use iterations for larger blur
    void OnRenderImage3(RenderTexture src, RenderTexture dest)
    {
        if (Material != null)
        {
            var rtW = src.width / downSample;
            var rtH = src.height / downSample;

            var buffer0 = RenderTexture.GetTemporary(rtW, rtH, 0);
            buffer0.filterMode = FilterMode.Bilinear;

            Graphics.Blit(src, buffer0);

            for (var i = 0; i < iterations; ++i)
            {
                Material.SetFloat("_BlurSize", 1.0f + i * blurSpread);

                var buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);

                // Render the vertical pass
                Graphics.Blit(buffer0, buffer1, Material, 0);

                RenderTexture.ReleaseTemporary(buffer0);
                buffer0 = buffer1;
                buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);

                // Render the horizontal pass
                Graphics.Blit(buffer0, buffer1, Material, 1);

                RenderTexture.ReleaseTemporary(buffer0);
                buffer0 = buffer1;
            }

            Graphics.Blit(buffer0, dest);
            RenderTexture.ReleaseTemporary(buffer0);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}
