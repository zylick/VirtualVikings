using UnityEngine;
using System.Collections;

public class DoorBehaviour : MonoBehaviour 
{
	void Update () 
	{
		if (Input.GetMouseButton (0)) {
			Ray Ray1 = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit Raycasthit1;

		
		

		if ( Physics.Raycast(Ray1.origin , Ray1.direction, out Raycasthit1, Mathf.Infinity))
	    {

				Door Door1 = Raycasthit1.transform.GetComponent<Door>();

			if (Door1)
			{
				Door1.playanmation();

			}
		}

	}
}
}


	

