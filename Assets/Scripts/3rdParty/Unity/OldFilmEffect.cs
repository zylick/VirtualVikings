/*
using UnityEngine;
using System.Collections;

public class OldFilmEffect : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
*/
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Color Adjustments/Grayscale")]
public class OldFilmEffect : ImageEffectBase {

	private float _timeLapse;
	private float _vignettingValue;
	void Start()
	{
		_timeLapse = 0.0f;
		_vignettingValue = 0.6f;
		//CreateMaterials ();
	}
	// Called by camera to apply image effect
	void OnRenderImage (RenderTexture source, RenderTexture destination) {
		//Set shader uniform values
		material.SetFloat("SepiaValue", 0.3f);
		material.SetFloat("NoiseValue", 0.3f);
		material.SetFloat("ScratchValue", 0.3f);
		
		material.SetFloat("InnerVignetting", 1.0f - _vignettingValue);
		material.SetFloat("OuterVignetting", 1.4f - _vignettingValue);
		
		var rnd = Random.value;
		material.SetFloat("RandomValue", rnd);
		
		_timeLapse += 1.0f;
		material.SetFloat("TimeLapse", _timeLapse);

		Graphics.Blit (source, destination, material);
	}
}