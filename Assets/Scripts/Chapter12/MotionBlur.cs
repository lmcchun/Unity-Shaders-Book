using UnityEngine;

public class MotionBlur : PostEffectsBase
{
#pragma warning disable 0649, IDE0044
    [SerializeField]
    private Shader motionBlurShader;

    [SerializeField, Range(0.0f, 0.9f)]
    private float blurAmount = 0.5f;
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

    void OnDisable()
    {
        DestroyImmediate(accumulationTexture);
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (material != null)
        {
            // Create the accumulation texture
            if (accumulationTexture == null || accumulationTexture.width != src.width || accumulationTexture.height != src.height)
            {
                DestroyImmediate(accumulationTexture);
                accumulationTexture = new RenderTexture(src.width, src.height, 0);
                accumulationTexture.hideFlags = HideFlags.HideAndDontSave;
                Graphics.Blit(src, accumulationTexture);
            }

            // We are accumulating motion over frames without clear/discard
            // by design, so silence any performance warnings from Unity
            accumulationTexture.MarkRestoreExpected();

            // 在 Shader 中 _BlurAmount 实际上表示当前渲染结果的权重, 所以需要用 1 减去 blurAmount
            material.SetFloat("_BlurAmount", 1.0f - blurAmount);

            // 混合时 src 是当前渲染的结果, dest 是 accumulationTexture
            Graphics.Blit(src, accumulationTexture, material);
            Graphics.Blit(accumulationTexture, dest);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}
