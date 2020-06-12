using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class PostEffectsBase : MonoBehaviour
{
    protected void Start()
    {
        CheckResources();
    }

    // Called when start
    protected void CheckResources()
    {
        if (!CheckSupport())
        {
            NotSupported();
        }
    }

    // Called when the platform doesn't support this effect
    protected void NotSupported()
    {
        enabled = false;
    }

    // Called in CheckResources to check support on this platform
    protected static bool CheckSupport()
    {
        if (!SystemInfo.supportsImageEffects/* || !SystemInfo.supportsRenderTextures*/)
        {
            Debug.LogWarning("This platform does not support image effects or render textures.");
            return false;
        }

        return true;
    }

    // Called when need to create the material used by this effect
    protected static Material CheckShaderAndCreateMaterial(Shader shader, Material material)
    {
        if (shader == null)
        {
            return null;
        }
        if (!shader.isSupported)
        {
            return null;
        }
        if (material && material.shader == shader)
        {
            return material;
        }
        material = new Material(shader)
        {
            hideFlags = HideFlags.DontSave
        };
        if (material)
        {
            return material;
        }
        else
        {
            return null;
        }
    }
}
