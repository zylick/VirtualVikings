using UnityEngine;
using System.Collections;

public class LibraryPast : MonoBehaviour {

	public GameObject mSceneController;
	private bool NotRan = true;
	
	void OnTriggerEnter( Collider other )
	{
		//Debug.Log ("Dinning Room Past: OnTriggerEnter");
		if (NotRan) 
		{
			//Bring the Player into First Person
			mSceneController.GetComponent<GameController>().ThirdToPastPerson ("LibraryRoom");
			NotRan = false;
		}
	}
}
