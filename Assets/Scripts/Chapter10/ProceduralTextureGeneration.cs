using UnityEngine;


[ExecuteInEditMode]
public class ProceduralTextureGeneration : MonoBehaviour
{
    public int TextureWidth
    {
        get { return m_textureWidth; }
        set
        {
            m_textureWidth = value;
            UpdateMaterial();
        }
    }

    public Color BackgroundColor
    {
        get { return m_backgroundColor; }
        set
        {
            m_backgroundColor = value;
            UpdateMaterial();
        }
    }

    public Color CircleColor
    {
        get { return m_circleColor; }
        set
        {
            m_circleColor = value;
            UpdateMaterial();
        }
    }

    public float BlurFactor
    {
        get { return m_blurFactor; }
        set
        {
            m_blurFactor = value;
            UpdateMaterial();
        }
    }

#pragma warning disable 0649
    [SerializeField, SetProperty("textureWidth")]
    private int m_textureWidth = 512;

    [SerializeField, SetProperty("backgroundColor")]
    private Color m_backgroundColor = Color.white;

    [SerializeField, SetProperty("circleColor")]
    private Color m_circleColor = Color.yellow;

    [SerializeField, SetProperty("blurFactor")]
    private float m_blurFactor = 2.0f;
#pragma warning restore 0649

    private Material material = null;

    private Texture2D m_generatedTexture = null;

#pragma warning disable IDE0051
    void Start()
#pragma warning restore IDE0051
    {
        if (material == null)
        {
            var renderer = gameObject.GetComponent<Renderer>();
            if (renderer == null)
            {
                Debug.LogWarning("Cannot find a renderer.");
                return;
            }
            material = renderer.sharedMaterial;
        }
        UpdateMaterial();
    }

    private void UpdateMaterial()
    {
        if (material != null)
        {
            m_generatedTexture = GenerateProceduralTexture();
            material.SetTexture("_MainTex", m_generatedTexture);
        }
    }

    private Texture2D GenerateProceduralTexture()
    {
        var proceduralTexture = new Texture2D(TextureWidth, TextureWidth);

        // 定义圆与圆之间的间距
        var circleInterval = TextureWidth / 4.0f;
        // 定义圆的半径
        var radius = TextureWidth / 10.0f;
        // 定义模糊系数
        var edgeBlur = 1.0f / BlurFactor;

        for (var w = 0; w < TextureWidth; ++w)
        {
            for (var h = 0; h < TextureWidth; ++h)
            {
                // 使用背景颜色进行初始化
                var pixel = BackgroundColor;

                // 依次画 9 个圆
                for (var i = 0; i < 3; ++i)
                {
                    for (var j = 0; j < 3; ++j)
                    {
                        // 计算当前所绘制的圆的圆心位置
                        var circleCenter = new Vector2(circleInterval * (i + 1), circleInterval * (j + 1));

                        // 计算当前像素与圆的距离
                        var distance = Vector2.Distance(new Vector2(w, h), circleCenter) - radius;

                        // 模糊圆的边界
                        var color = MixColor(CircleColor, new Color(pixel.r, pixel.g, pixel.b, 0.0f), Mathf.SmoothStep(0.0f, 1.0f, distance * edgeBlur));

                        // 与之前得到的颜色进行混合
                        pixel = MixColor(pixel, color, color.a);
                    }
                }

                proceduralTexture.SetPixel(w, h, pixel);
            }
        }

        proceduralTexture.Apply();

        return proceduralTexture;
    }

    private Color MixColor(Color color0, Color color1, float mixFactor)
    {
        var mixedColor = Color.white;
        mixedColor.r = Mathf.Lerp(color0.r, color1.r, mixFactor);
        mixedColor.g = Mathf.Lerp(color0.g, color1.g, mixFactor);
        mixedColor.b = Mathf.Lerp(color0.b, color1.b, mixFactor);
        mixedColor.a = Mathf.Lerp(color0.a, color1.a, mixFactor);
        return mixedColor;
    }
}
