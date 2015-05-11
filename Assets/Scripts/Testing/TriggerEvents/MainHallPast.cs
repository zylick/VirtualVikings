using UnityEngine;
using System.Collections;

public class MainHallPast : MonoBehaviour {
	public GameObject mSceneController;
	private bool NotRan = true;
	
	void OnTriggerEnter( Collider other )
	{
		//Debug.Log ("Dinning Room Past: OnTriggerEnter");
		if( mSceneController.GetComponent<GameController>().mMainHallAccess )
			if (NotRan) 
			{
				//Bring the Player into First Person
				mSceneController.GetComponent<GameController>().ThirdToPastPerson ("MainHall");
				NotRan = false;
			}
	}
}
