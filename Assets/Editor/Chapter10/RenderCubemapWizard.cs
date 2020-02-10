using UnityEditor;
using UnityEngine;

public class RenderCubemapWizard : ScriptableWizard
{
#pragma warning disable 0649
	[SerializeField]
	private Transform renderFromPosition;

	[SerializeField]
	private Cubemap cubemap;
#pragma warning restore 0649

	[MenuItem("GameObject/Render into Cubemap")]
	private static void RenderCubemap () {
		ScriptableWizard.DisplayWizard<RenderCubemapWizard>("Render cubemap", "Render!");
	}

	private void OnWizardCreate()
	{
		// create temporary camera for rendering
		var go = new GameObject("CubemapCamera");
		go.AddComponent<Camera>();
		// place it on the object
		go.transform.position = renderFromPosition.position;
		// render into cubemap
		go.GetComponent<Camera>().RenderToCubemap(cubemap);

		// destroy temporary camera
		DestroyImmediate(go);
	}

	private void OnWizardUpdate()
	{
		helpString = "Select transform to render from and cubemap to render into";
		isValid = (renderFromPosition != null) && (cubemap != null);
	}
}
