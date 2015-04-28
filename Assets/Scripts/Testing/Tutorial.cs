using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour {

	public enum TutorialMode
	{
		FirstPerson = 0,
		ThirdPerson = 1,
		Scared = 2,
	}

	public OVRCameraRig mThirdPersonCam;
	public OVRCameraRig mFirstPersonCam;
	public GameObject mThisGameObject;


	private TutorialMode mTutorialMode = TutorialMode.FirstPerson;

	// Use this for initialization
	void Start () {

		//Initialize Defaults
		mThirdPersonCam.gameObject.SetActive (false); //Set the third person camera to inactive.
		mFirstPersonCam.gameObject.SetActive (true); //Set the First person camera to active.
		mTutorialMode = TutorialMode.FirstPerson; //Set the tutorial mode to First Person

		//Initialize the touchPad for use
		OVRTouchpad.Create ();
		OVRTouchpad.TouchHandler += TouchHandlerCapture; //Bind a function here to the events in hte touchpad.

		//Testing Section
		mThirdPersonCam.gameObject.SetActive (true); //Set the third person camera to inactive.
		mFirstPersonCam.gameObject.SetActive (false); //Set the First person camera to active.
		mTutorialMode = TutorialMode.ThirdPerson; //Set the tutorial mode to Third Person

	}
	
	// Update is called once per frame
	void Update () {

	}

	private void TouchHandlerCapture( object pSender, System.EventArgs pE)
	{
		OVRTouchpad.TouchArgs fTouchArguments = (OVRTouchpad.TouchArgs)pE;

		//Store event type as a bool for reuse in function
		bool fTap = false;
		bool fUp = false;
		bool fRight = false;
		bool fDown = false;
		bool fLeft = false;

		switch (fTouchArguments.TouchType) 
		{
		case OVRTouchpad.TouchEvent.SingleTap:
			//mThisGameObject.renderer.material.color = new Color(Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f));
			fTap = true;

			break;
		case OVRTouchpad.TouchEvent.Up:
			//mThisGameObject.transform.Translate (Vector3.up);
			fUp = true;

			break;
		case OVRTouchpad.TouchEvent.Right:
			//mThisGameObject.transform.Translate (Vector3.right);
			fRight = true;

			break;
		case OVRTouchpad.TouchEvent.Down:
			//mThisGameObject.transform.Translate (Vector3.down);
			fDown = true;

			break;
		case OVRTouchpad.TouchEvent.Left:
			//mThisGameObject.transform.Translate (Vector3.left);
			fLeft = true;

			break;
		default:
			break;
		}


		//Check to see what mode the character is in
		switch (mTutorialMode) 
		{
		case TutorialMode.FirstPerson:
			//We need to listen to key presses at this point to determine what is going to happen next.
			if(fTap)
			{
				//swap the player mode.
				Debug.Log ("FirstPerson Tap");

				//Cursor on so we can see what we are staring at


				//mThirdPersonCam.gameObject.SetActive (true); //Set the third person camera to inactive.
				//mFirstPersonCam.gameObject.SetActive (false); //Set the First person camera to active.
				//mTutorialMode = TutorialMode.ThirdPerson; //Set the tutorial mode to Third Person
			}
			break;
		case TutorialMode.ThirdPerson:
			//We need to listen to key presses at this point to determine what is going to happen next.
				//swap the player mode.
			if( fTap )
			{
				Debug.Log ("ThirdPerson Tap");
				//Swap over to First Person
				mThirdPersonCam.gameObject.SetActive (false); //Set the third person camera to inactive.
				mFirstPersonCam.gameObject.SetActive (true); //Set the First person camera to active.
				mTutorialMode = TutorialMode.FirstPerson; //Set the tutorial mode to First Person
			}


			break;
		case TutorialMode.Scared:
			break;
		default:
			break;
		}



	}

}
