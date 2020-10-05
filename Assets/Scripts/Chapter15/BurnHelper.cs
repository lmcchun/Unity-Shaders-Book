using UnityEngine;

public class BurnHelper : MonoBehaviour
{
#pragma warning disable 0649, IDE0044
	[SerializeField]
	private Material material;

	[SerializeField, Range(0.01f, 1.0f)]
	private float burnSpeed = 0.3f;
#pragma warning restore 0649, IDE0044

	private float burnAmount = 0.0f;

	void Start ()
	{
		if (material == null)
		{
			Renderer renderer = gameObject.GetComponentInChildren<Renderer>();
			if (renderer != null)
			{
				material = renderer.material;
			}
		}

		if (material == null)
		{
			this.enabled = false;
		}
		else
		{
			material.SetFloat("_BurnAmount", 0.0f);
		}
	}
	
	void Update ()
	{
		burnAmount = Mathf.Repeat(Time.time * burnSpeed, 1.0f);
		material.SetFloat("_BurnAmount", burnAmount);
	}
}
