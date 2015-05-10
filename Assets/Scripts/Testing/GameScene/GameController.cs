using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{

	public enum GameMode
	{
		FirstPerson = 0,
		ThirdPerson = 1,
		PastPerson = 2,
	}
    
	public OVRCameraRig mFirstPersonCam;
	public Crosshair3D mFirstPersonCursor;
	public TextMesh mFirstPersonMessage;

	public OVRCameraRig mThirdPersonCam;
	public Crosshair3D mThirdPersonCursor;
	public TextMesh mThirdPersonMessage;

	public GameObject mPlayerObject;
	public GameObject mPlayerIcon;

	//Audio Files
	public AudioSource mWalkingSound;
	public AudioSource mOldFilmSound;
	public AudioSource mPickupItemSound;

	//Quest Audio Files
	public AudioSource mDinningRoomSound;

	//Import Position Transforms
	public Transform mDinningRoomLocation;



	private static float mTimer;
	private float mTimeLeft;
	private float mAccumulation;
	private int mFrames;
	private float mUpdateInterval;
	private GameMode mGameMode = GameMode.FirstPerson;
    
	//Private Quest Stuff

	private uint mCrayonCount = 0;


	// Use this for initialization
	void Start ()
	{
        
		//Initialize Defaults
		ThirdToFirstPerson ();

		//Initialize the touchPad for use
		OVRTouchpad.Create ();
		OVRTouchpad.TouchHandler += TouchHandlerCapture; //Bind a function here to the events in hte touchpad.
       
		//Set the timer to the start position
		mTimer = 60.0f;
		mTimeLeft = mUpdateInterval = 0.5f;
        
		//Initialize my trigger System with the Trigger list
		TriggerSystem.Create ();
		//TriggerSystem.AddTriggerToList ("GreenKey", false);
		//TriggerSystem.AddTriggerToList ("BlueKey", false);
		//TriggerSystem.AddTriggerToList ("BrownKey", false);
        
	}

	void FixedUpdate ()
	{

	}
    
	// Update is called once per frame
	void Update ()
	{
        /*
		//Time.deltaTime; //How much time has passed since the last frame.
		if (mTimer < 0.0f)
			mTextTimer.text = "GameOver";
		else {
			mTextTimer.text = "Time Remaining: " + Mathf.Floor (mTimer);
			mTimer -= Time.deltaTime;
		}
        
		//Get the current FPS and display it here:
		mTimeLeft -= Time.deltaTime;
		mAccumulation += Time.timeScale / Time.deltaTime;
		++mFrames;
        
		if (mTimeLeft <= 0.0f) {
			//Display float to the second digit
			mFPSCounter.text = "FPS: " + (mAccumulation / mFrames).ToString ("f2");
			mTimeLeft = mUpdateInterval;
			mAccumulation = 0.0f;
			mFrames = 0;
		}

		*/

		//If we touched our back button in first person we want to switch to third person
		if (Input.GetMouseButtonDown (1) && mGameMode == GameMode.FirstPerson) {
			FirstToThirdPerson();
		}
	}
    
	private void TouchHandlerCapture (object pSender, System.EventArgs pE)
	{
		switch (mGameMode) {
		case GameMode.FirstPerson:
			FirstPersonTouchHandler (pSender, pE);
			break;
		case GameMode.ThirdPerson:
			ThirdPersonTouchHandler (pSender, pE);
			break;
		case GameMode.PastPerson:
			PastModeTouchhandler (pSender, pE);
			break;
		default:
			break;
		}
	}
    
	private void FirstPersonTouchHandler (object pSender, System.EventArgs pE)
	{
		OVRTouchpad.TouchArgs fTouchArguments = (OVRTouchpad.TouchArgs)pE;
		switch (fTouchArguments.TouchType) {
		case OVRTouchpad.TouchEvent.SingleTap:               
            //Cursor on so we can see what we are staring at
            //Raycast to see what we are looking at.
			Ray ray;
			RaycastHit hit;
			Vector3 cameraPosition = mFirstPersonCam.centerEyeAnchor.position;
			Vector3 cameraForward = mFirstPersonCam.centerEyeAnchor.forward;
                
			ray = new Ray (cameraPosition, cameraForward);
			Physics.Raycast (ray, out hit);
                //Tap to either pick up an item, interact with a door or swap to see what we are looking at.
                //Debug.Log (hit.collider.gameObject.layer);
			if( hit.collider != null && mFirstPersonCursor.mode == Crosshair3D.CrosshairMode.DynamicObjects)
			{
				if (hit.collider.gameObject.layer == 8) 
				{ //8 is keys
					//Debug.Log(hit.collider.name);
					//Which key was it?
					//Debug.Log (TriggerSystem.IsTriggerPresent(hit.collider.name));
	                    
					if (TriggerSystem.IsTriggerPresent (hit.collider.name)) {
						TriggerSystem.SetTriggerInList (hit.collider.name, true);
	                        
						//We touched a something interactible pick it up/use it
						QuestChecker(hit);
					} else {
						//Display a message saying that the item in question is not interactive?
						//You don't know what to do with that yet.
					}
				}
				else if (hit.collider.gameObject.layer == 9) 
				{ //9 is doors
					//We touched a door see if we can open it
					//If we have key for door open door else display message that the door is locked?
					hit.collider.gameObject.SetActive (false);
				}
				else
				{
					//We didn't click anything interative we are just swaping the cursor back
					switch (mFirstPersonCursor.mode) {
					case Crosshair3D.CrosshairMode.DynamicObjects:
						mFirstPersonCursor.mode = Crosshair3D.CrosshairMode.Dynamic;
						mFirstPersonCursor.crosshairMaterial.color = Color.white;
						break;
					case Crosshair3D.CrosshairMode.Dynamic:
						mFirstPersonCursor.mode = Crosshair3D.CrosshairMode.DynamicObjects;
						mFirstPersonCursor.crosshairMaterial.color = Color.red;
						break;
					default:
						break;
					}
				}
			}
			else if( hit.collider != null )
			{
				//We want to switch Cursor Mode from seeing where we are looking to check for interaction
				switch (mFirstPersonCursor.mode) {
				case Crosshair3D.CrosshairMode.DynamicObjects:
					mFirstPersonCursor.mode = Crosshair3D.CrosshairMode.Dynamic;
					mFirstPersonCursor.crosshairMaterial.color = Color.white;
					break;
				case Crosshair3D.CrosshairMode.Dynamic:
					mFirstPersonCursor.mode = Crosshair3D.CrosshairMode.DynamicObjects;
					mFirstPersonCursor.crosshairMaterial.color = Color.red;
					break;
				default:
					break;
				}
			}
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
    

    
    private void ThirdPersonTouchHandler (object pSender, System.EventArgs pE)
	{
		OVRTouchpad.TouchArgs fTouchArguments = (OVRTouchpad.TouchArgs)pE;
        
		//Variables that are reused in the switch statment
		//Movement Code normalize the camera looking direction and line it up with the floor (so people don't fly or sink)
		Vector3 forward = mFirstPersonCam.centerEyeAnchor.transform.TransformDirection (Vector3.forward);
		//Vector3 forward = CenterPosition.transform.TransformDirection(Vector3.forward);
		forward.y = 0f;
		forward = forward.normalized; 
        
		//transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, Camera.main.transform.localEulerAngles.y, transform.localEulerAngles.z);
        
		switch (fTouchArguments.TouchType) {
		case OVRTouchpad.TouchEvent.SingleTap:
            //mThisGameObject.renderer.material.color = new Color(Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f));
            //Debug.Log ("ThirdPerson Tap");
            
            //If we do not click on the player move him to that location.
			Ray ray;
			RaycastHit hit;
			Vector3 cameraPosition = mThirdPersonCam.centerEyeAnchor.position;
			Vector3 cameraForward = mThirdPersonCam.centerEyeAnchor.forward;
                
			ray = new Ray (cameraPosition, cameraForward);
			Physics.Raycast (ray, out hit);
            
            //Debug.DrawLine(cameraPosition, hit.point, Color.blue, 30, true);
            //Debug.Log(hit.point);
            
            //Debug.Log ( hit.collider.name );
			if( hit.collider != null )
			if (hit.collider.name != "1stPersonPlayer") {
				//Calculate the time it would take to move the character at a constant pace
				float mTimeToWalk = Vector3.Distance (mPlayerObject.transform.position, new Vector3 (hit.point.x, mPlayerObject.transform.position.y, hit.point.z));
				//iTween.MoveBy(gameObject, iTween.Hash("x", 2, "easeType", "easeInOutExpo", "loopType", "pingPong", "delay", .1));
				//Debug.Log (mTimeToWalk);
				mTimeToWalk /= 3;
				iTween.MoveTo (mPlayerObject, iTween.Hash ("x", hit.point.x, "y", mPlayerObject.transform.position.y, "z", hit.point.z, "easeType", "linear", "time", mTimeToWalk));
					
				//Play the walking sound effect for the timeframe
				if (!mWalkingSound.isPlaying)
					StartCoroutine ("PlayWalkSound", mTimeToWalk);
				else 
				{
					StopCoroutine("PlayWalkSound");
                    mWalkingSound.Stop();
					StartCoroutine ("PlayWalkSound", mTimeToWalk);
                }


                    
				//iTween.MoveTo(mPlayerObject,new Vector3( hit.point.x, mPlayerObject.transform.position.y,hit.point.z),mTimeToWalk);
				//mPlayerObject.transform.position = new Vector3( hit.point.x, mPlayerObject.transform.position.y,hit.point.z );
                    
			}
            //If we click on the player switch it to first person mode
            else 
			{
				//Swap from Third Person to First Person
				ThirdToFirstPerson();
			}
                
			break;
		case OVRTouchpad.TouchEvent.Up:
                //Means go Left (swipeing toward the top of the device)
			Vector3 left = new Vector3 (-forward.z, 0.0f, forward.x);
			mPlayerObject.transform.position += left;
			break;
		case OVRTouchpad.TouchEvent.Right:
                //Means go forward (swipeing toward the front of the device)
			mPlayerObject.transform.position -= forward;
			break;
		case OVRTouchpad.TouchEvent.Down:
                //Means go right (swipeing toward the bottom of the device)
			Vector3 right = new Vector3 (forward.z, 0.0f, -forward.x);
			mPlayerObject.transform.position += right;
			break;
		case OVRTouchpad.TouchEvent.Left:
            //Means go backward (swipeing toward the head)
			mPlayerObject.transform.position += forward;
            
			break;
		default:
			break;
		}
	}

	private void PastModeTouchhandler (object pSender, System.EventArgs pE)
	{
		OVRTouchpad.TouchArgs fTouchArguments = (OVRTouchpad.TouchArgs)pE;

		switch (fTouchArguments.TouchType) {
		case OVRTouchpad.TouchEvent.SingleTap:
            //Send them back to first person mode
			ThirdToFirstPerson();

            //Switch the scene back to regular mode


            //Turn off the old film shaders
			//Enable the shaders on the eyes
			OldFilmEffect fThisOldFilmEffect = mFirstPersonCam.leftEyeAnchor.GetComponent<OldFilmEffect> ();
			fThisOldFilmEffect.enabled = false;
			
			fThisOldFilmEffect = mFirstPersonCam.rightEyeAnchor.GetComponent<OldFilmEffect> ();
            fThisOldFilmEffect.enabled = false;

			//Stop the Projector Audio sound effect if it is still playing
			if( mOldFilmSound.isPlaying )
                    mOldFilmSound.Stop ();

			break;
		}
	}
	
	//Person Switching
	public void FirstToThirdPerson()
	{
		mThirdPersonCam.gameObject.SetActive (true); //Set the third person camera to inactive.
		mFirstPersonCam.gameObject.SetActive (false); //Set the First person camera to active.
		mGameMode = GameMode.ThirdPerson; //Set the tutorial mode to Third Person
		mPlayerIcon.SetActive (true); //Turn the player on since we are going to 3rd person.
		mThirdPersonCursor.mode = Crosshair3D.CrosshairMode.Dynamic;
		mThirdPersonCursor.crosshairMaterial.color = Color.white;

		mPlayerObject.GetComponent<BoxCollider> ().enabled = true;
    }
    
	public void ThirdToFirstPerson()
	{
		//Swap over to First Person
		mThirdPersonCam.gameObject.SetActive (false); //Set the third person camera to inactive.
		mFirstPersonCam.gameObject.SetActive (true); //Set the First person camera to active.
		mGameMode = GameMode.FirstPerson; //Set the tutorial mode to First Person
		mPlayerIcon.SetActive (false); // Turn the player off since we are going to First Person.  
		mFirstPersonCursor.mode = Crosshair3D.CrosshairMode.Dynamic;
		mFirstPersonCursor.crosshairMaterial.color = Color.white;

		//Make sure to disable the box collider so It doesn't interfere with the cursor
		mPlayerObject.GetComponent<BoxCollider> ().enabled = false;
		
		//If we have an iTween Running stop it
		iTween.Stop (mPlayerObject);

		//If we have to stop walking then we need to stop our walk sound effect as well
		mWalkingSound.Stop ();
    }
    
	public void ThirdToPastPerson(string pLocation)
	{
		//Swap over to First Person
		mThirdPersonCam.gameObject.SetActive (false); //Set the third person camera to inactive.
		mFirstPersonCam.gameObject.SetActive (true); //Set the First person camera to active.
		mGameMode = GameMode.PastPerson; //Set the tutorial mode to First Person
		mPlayerIcon.SetActive (false); // Turn the player off since we are going to First Person.  
		
		//Make sure to disable the box collider so It doesn't interfere with the cursor
		mPlayerObject.GetComponent<BoxCollider> ().enabled = false;
		
		//If we have an iTween Running stop it
		iTween.Stop (mPlayerObject);
		//If we have to stop walking then we need to stop our walk sound effect as well
		mWalkingSound.Stop ();

		//In the past make the chrosshair transparent
		mFirstPersonCursor.mode = Crosshair3D.CrosshairMode.Dynamic;
		mFirstPersonCursor.crosshairMaterial.color = new Color(0, 0, 0, 0);

		//Enable the shaders on the eyes
		OldFilmEffect fThisOldFilmEffect = mFirstPersonCam.leftEyeAnchor.GetComponent<OldFilmEffect> ();
		fThisOldFilmEffect.enabled = true;

		fThisOldFilmEffect = mFirstPersonCam.rightEyeAnchor.GetComponent<OldFilmEffect> ();
		fThisOldFilmEffect.enabled = true;

		//Move the player to DinningRoom
		switch( pLocation )
		{
		case "DinningRoom":
			//Swap the Prefabs from present to past


			//Move the player to the Designated location
			Vector3 fDinningRoomLocation = new Vector3(mDinningRoomLocation.position.x, 
			                                           mPlayerObject.transform.position.y,
			                                           mDinningRoomLocation.position.z);
			mPlayerObject.transform.position = fDinningRoomLocation;

			//Play Audio for Dinning Room
			//There is a ghost in the dining room
			mDinningRoomSound.Play (); //For now we will force the player to listen no matter what.

			//Set Trigger for Dinning Room
			//StartTrigger for Dinning Room
			//Collect five (5) Crayons
			//Spawn five crayons in the Study to pick up and give to ghost
			TriggerSystem.AddTriggerToList ("CrayonOne", false); //False because it hasn't been aquired yet.
			TriggerSystem.AddTriggerToList ("CrayonTwo", false); //False because it hasn't been aquired yet.
			TriggerSystem.AddTriggerToList ("CrayonThree", false); //False because it hasn't been aquired yet.
			TriggerSystem.AddTriggerToList ("CrayonFour", false); //False because it hasn't been aquired yet.
			TriggerSystem.AddTriggerToList ("CrayonFive", false); //False because it hasn't been aquired yet.

			//EndTrigger for Dinning Room
			//Give five (5) crayons to ghost under table enable End Trigger
			//TriggerSystem.AddTriggerToList ("DinningRoomPastSoul", false); //False because it hasn't been aquired yet.


			break;
		case "MainHallRoom":
			//Swap the Prefabs from present to past

			//Move the player to the Designated location

			//Play Audio for Dinning Room
			
			//Set Trigger for Dinning Room

			break;
		case "LibraryRoom":
			//Swap the Prefabs from present to past
			
			//Move the player to the Designated location
			
			//Play Audio for Dinning Room
			
			//Set Trigger for Dinning Room

			break;
		case "LibraryHall":
			//Swap the Prefabs from present to past
			
			//Move the player to the Designated location
			
			//Play Audio for Dinning Room
			
			//Set Trigger for Dinning Room

			break;
		case "StudyRoom":
			//Swap the Prefabs from present to past
			
			//Move the player to the Designated location
			
			//Play Audio for Dinning Room
			
			//Set Trigger for Dinning Room

			break;
		default:
			break;
		}

		//Play the Projector Start Audio sound effect
		if( !mOldFilmSound.isPlaying )
			mOldFilmSound.Play ();
    }
    
	//Quest Checker based on name
	private void QuestChecker( RaycastHit fHitObject )
	{
		//Trigger has been set to true and was active so process the trigger
		switch (fHitObject.collider.name) 
		{
		case "CrayonOne":
			mCrayonCount++;
			break;
		case "CrayonTwo":
			mCrayonCount++;
			break;
		case "CrayonThree":
			mCrayonCount++;
			break;
		case "CrayonFour":
			mCrayonCount++;
			break;
		case "CrayonFive":
			mCrayonCount++;
			break;
		case "DinningRoomPastSoul":
			//We completed a quest
			//Thank you for finding the crayons
			StartCoroutine(ShowMessage("Crayon Quest", 5.0f));
			break;
		default:
			break;
		}

		//Remove the Object from play as it has already been used.
		fHitObject.transform.gameObject.SetActive (false);

		//Play audio file for picking up an item
		if( !mPickupItemSound.isPlaying )
			mPickupItemSound.Play();
		else 
		{
			mPickupItemSound.Stop();
			mPickupItemSound.Play();
		}

		//Check for Crayon Quest Win Condition (Five Crayons, and not already completed)
		if (mCrayonCount == 5 && !TriggerSystem.GetTriggerState("DinningRoomPastSoul")) 
		{
			if( !TriggerSystem.IsTriggerPresent("DinningRoomPastSoul" ) )
				TriggerSystem.AddTriggerToList ("DinningRoomPastSoul", false); //False because it hasn't been aquired yet.
		}


	}

	IEnumerator ShowMessage( string fMessage, float fTime)
	{
		//Turn on text to put the message
		mFirstPersonMessage.gameObject.SetActive (true);
		mThirdPersonMessage.gameObject.SetActive (true);

		//Put the message in the text
		mFirstPersonMessage.text = fMessage;
		mThirdPersonMessage.text = fMessage;

		//Wait the requested time
		yield return new WaitForSeconds (fTime);

		//Turn off text to hide the message
		mFirstPersonMessage.gameObject.SetActive (false);
		mThirdPersonMessage.gameObject.SetActive (false);

		//Clear the message
		mFirstPersonMessage.text = "";
		mThirdPersonMessage.text = "";

	}

    //Audio Handlers
    IEnumerator PlayWalkSound (float fTime)
	{
		mWalkingSound.Play ();
		yield return new WaitForSeconds (fTime);
		if( mWalkingSound.isPlaying)
			mWalkingSound.Stop ();

	}
}