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
	public GameObject CenterPosition;
	public OVRCameraRig mFirstPersonCam;
	public GameObject mThisGameObject;
	public TextMesh mTextTimer;

	private static float mTimer;
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

		//Set the timer to the start position
		mTimer = 60.0f;
	}
	
	// Update is called once per frame
	void Update () {

		//Time.deltaTime; //How much time has passed since the last frame.
		if( mTimer < 0.0f )
			mTextTimer.text = "GameOver";
		else
		{
			mTextTimer.text = "Time Remaining: " + Mathf.Floor(mTimer);
			mTimer -= Time.deltaTime;
		}
	}

	private void TouchHandlerCapture( object pSender, System.EventArgs pE)
	{
		switch (mTutorialMode) 
		{
		case TutorialMode.FirstPerson:
			FirstPersonTouchHandler (pSender, pE);
			break;
		case TutorialMode.ThirdPerson:
			ThirdPersonTouchHandler (pSender, pE);
			break;
		case TutorialMode.Scared:
			TutorialModeTouchhandler (pSender, pE);
			break;
		default:
			break;
        }
	}

	private void FirstPersonTouchHandler(object pSender, System.EventArgs pE)
	{
		OVRTouchpad.TouchArgs fTouchArguments = (OVRTouchpad.TouchArgs)pE;
		switch (fTouchArguments.TouchType) 
		{
		case OVRTouchpad.TouchEvent.SingleTap:
			//mThisGameObject.renderer.material.color = new Color(Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f));
			//swap the player mode.
			Debug.Log ("FirstPerson Tap");
			
			//Cursor on so we can see what we are staring at
			
			
			mThirdPersonCam.gameObject.SetActive (true); //Set the third person camera to inactive.
			mFirstPersonCam.gameObject.SetActive (false); //Set the First person camera to active.
            mTutorialMode = TutorialMode.ThirdPerson; //Set the tutorial mode to Third Person
			
			break;
		case OVRTouchpad.TouchEvent.Up:
			//mThisGameObject.transform.Translate (Vector3.up);
			
			
			break;
		case OVRTouchpad.TouchEvent.Right:
			//mThisGameObject.transform.Translate (Vector3.right);
			
			
			break;
		case OVRTouchpad.TouchEvent.Down:
                //mThisGameObject.transform.Translate (Vector3.down);
                
                break;
            case OVRTouchpad.TouchEvent.Left:
                //mThisGameObject.transform.Translate (Vector3.left);
                
                
                break;
		default:
			break;
		}
	}

	private void ThirdPersonTouchHandler(object pSender, System.EventArgs pE)
	{
		OVRTouchpad.TouchArgs fTouchArguments = (OVRTouchpad.TouchArgs)pE;

		//Variables that are reused in the switch statment
		//Movement Code normalize the camera looking direction and line it up with the floor (so people don't fly or sink)
		Vector3 forward = CenterPosition.transform.TransformDirection(Vector3.forward);
		forward.y = 0f;
		forward = forward.normalized; 

		//transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, Camera.main.transform.localEulerAngles.y, transform.localEulerAngles.z);

		switch (fTouchArguments.TouchType) 
		{
		case OVRTouchpad.TouchEvent.SingleTap:
			//mThisGameObject.renderer.material.color = new Color(Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f));
			Debug.Log ("ThirdPerson Tap");
			//Swap over to First Person
			mThirdPersonCam.gameObject.SetActive (false); //Set the third person camera to inactive.
			mFirstPersonCam.gameObject.SetActive (true); //Set the First person camera to active.
			mTutorialMode = TutorialMode.FirstPerson; //Set the tutorial mode to First Person
                
            break;
		case OVRTouchpad.TouchEvent.Up:
			//Means go Left (swipeing toward the top of the device)
			Vector3 left = new Vector3(-forward.z, 0.0f, forward.x);
			mThisGameObject.transform.position += left;
			break;
		case OVRTouchpad.TouchEvent.Right:
			//Means go forward (swipeing toward the front of the device)
			mThisGameObject.transform.position -= forward;
			break;
		case OVRTouchpad.TouchEvent.Down:
			//Means go right (swipeing toward the bottom of the device)
			Vector3 right = new Vector3(forward.z, 0.0f, -forward.x);
			mThisGameObject.transform.position += right;
            break;
        case OVRTouchpad.TouchEvent.Left:
			//Means go backward (swipeing toward the head)
			mThisGameObject.transform.position += forward;

            break;
		default:
			break;
		}
	}

	private void TutorialModeTouchhandler(object pSender, System.EventArgs pE)
	{
		OVRTouchpad.TouchArgs fTouchArguments = (OVRTouchpad.TouchArgs)pE;
		switch (fTouchArguments.TouchType) 
		{
		case OVRTouchpad.TouchEvent.SingleTap:
			//mThisGameObject.renderer.material.color = new Color(Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f));
			
			
			break;
		case OVRTouchpad.TouchEvent.Up:
			//mThisGameObject.transform.Translate (Vector3.up);
			
			
			break;
		case OVRTouchpad.TouchEvent.Right:
			//mThisGameObject.transform.Translate (Vector3.right);
			
			
			break;
		case OVRTouchpad.TouchEvent.Down:
            //mThisGameObject.transform.Translate (Vector3.down);
                
            break;
        case OVRTouchpad.TouchEvent.Left:
            //mThisGameObject.transform.Translate (Vector3.left);
                
                
            break;
		default:
			break;
		}
	}

}
