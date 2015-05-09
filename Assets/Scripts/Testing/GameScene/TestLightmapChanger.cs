using UnityEngine;
using System.Collections;

public class TestLightmapChanger : MonoBehaviour {

	public GameObject  RoomInQuestion;
	private MeshRenderer[] ListofMeshRenders;

	// Use this for initialization
	void Start () {
		ListofMeshRenders = RoomInQuestion.GetComponentsInChildren<MeshRenderer> ();
		foreach (MeshRenderer fMR in ListofMeshRenders) {
			fMR.renderer.lightmapIndex = 0;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) 
		{

			foreach (MeshRenderer fMR in ListofMeshRenders) 
			{
				fMR.renderer.lightmapIndex = 1;
			}
		}
		else if (Input.GetMouseButtonDown (1)) 
		{

			foreach (MeshRenderer fMR in ListofMeshRenders) 
			{
				fMR.renderer.lightmapIndex = 2;
			}
		}
		else if (Input.GetMouseButtonDown (2)) 
		{
			foreach (MeshRenderer fMR in ListofMeshRenders) 
			{
				fMR.renderer.lightmapIndex = 0;
			}
		}

	}
}
