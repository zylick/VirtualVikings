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
	public AudioSource mBackgroundMusic;
	public AudioClip   mAngrySpiritClip;
	public AudioClip   mBroomSweepingClip;

	//Quest Audio Files
	public AudioSource mDinningRoomSound;
	public AudioSource mLibraryRoomSound;
	public AudioSource mMainHallSound;

	//Import Position Transforms
	public Transform mDinningRoomLocation;
	public Transform mLibraryRoomLocation;
	public Transform mMainHallLocation;

	private static float mTimer;
	private float mTimeLeft;
	private float mAccumulation;
	private int mFrames;
	private float mUpdateInterval;
	private GameMode mGameMode = GameMode.FirstPerson;
    
	//Private Quest Stuff
	public bool mMainHallAccess = false;

	private uint mCrayonCount = 0;
	private uint mDustPileCount = 0;


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

		//Turn on background Music if available
		if(mBackgroundMusic.isPlaying)
			mBackgroundMusic.Play ();
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
					}
					else
					{
						//Display a message saying that the item in question is not interactive?
						//You don't know what to do with that yet.

						//Some quests have other things that happen here:
						//If you click on DustPiles they cause angryGhostSounds
						if( hit.collider.name.Contains("Dust"))
						{
							AudioSource.PlayClipAtPoint(mAngrySpiritClip, hit.collider.transform.position);
							//Allow play to hit the Main Hall Trigger now.
							mMainHallAccess = true;
							//Debug.Log ("Main Hall Access Enabled");
						}
						else if( hit.collider.name.Contains("BookWrong") && TriggerSystem.IsTriggerPresent( "LibraryBookOnGround" ))
						{
							AudioSource.PlayClipAtPoint(mAngrySpiritClip, hit.collider.transform.position);
						}

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

				//Figure out if we need to go upstairs
				float fDistance = hit.point.y + mPlayerObject.transform.position.y;

				//Did they click on the floor above or below?
				if( fDistance > 3 && fDistance < 5)
				{
					//I believe he wants to go up stairs or downstairs
					if( hit.point.y > mPlayerObject.transform.position.y )
					{
						//Then it was upstairs
						iTween.MoveTo (mPlayerObject, iTween.Hash ("path", iTweenPath.GetPath ("UpStairs"), "time", 5));
					}
					else
					{
						//Then it was downstairs
						iTween.MoveTo (mPlayerObject, iTween.Hash ("path", iTweenPath.GetPath ("DownStairs"), "time", 5));
					}

				}
				else
				{
					//Walk to the destination
					mTimeToWalk /= 3;
					iTween.MoveTo (mPlayerObject, iTween.Hash ("x", hit.point.x, "y", mPlayerObject.transform.position.y, "z", hit.point.z, "easeType", "linear", "time", mTimeToWalk));
				}

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


			if(!mBackgroundMusic.isPlaying)
				mBackgroundMusic.Play ();
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
		//Do we have any requirements that haven't been met?
		if (pLocation == "MainHall" && !mMainHallAccess)
			return;
		//Debug.Log ("We passed MainHall Requirement");

		//Pause the Background Music
		if ( mBackgroundMusic.isPlaying ) 
		{
			//pause the music
			mBackgroundMusic.Pause();
		}

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
		case "MainHall":

			//Swap the Prefabs from present to past
			//Move the player to the Designated location
			Vector3 fMainHallLocation = new Vector3(mMainHallLocation.position.x, 
			                                           mPlayerObject.transform.position.y,
			                                        mMainHallLocation.position.z);
			mPlayerObject.transform.position = fMainHallLocation;
			//Play Audio for Dinning Room
			mMainHallSound.Play ();

			//Set Trigger for Dinning Room
			TriggerSystem.AddTriggerToList ("Broom", false); //False because it hasn't been aquired yet.

			//EndTrigger for Main Hall
			//Clean up seven (7) piles of dust to clam the Butler Spirit
			//TriggerSystem.AddTriggerToList ("MainHallPastSoul", false); //False because it hasn't been aquired yet.

			break;
		case "LibraryRoom":
			//Swap the Prefabs from present to past

			//Present Messages //Make the writting on the wall appear upstairs.

			//Move the player to the Designated location
			Vector3 fLibraryRoomLocation = new Vector3(mLibraryRoomLocation.position.x, 
			                                        mPlayerObject.transform.position.y,
			                                           mLibraryRoomLocation.position.z);
			mPlayerObject.transform.position = fLibraryRoomLocation;
			//Play Audio for Dinning Room
			mLibraryRoomSound.Play ();

			//Set Trigger for Dinning Room
			TriggerSystem.AddTriggerToList ("LibraryBookOnGround", false); //False because it hasn't been aquired yet.

			//EndTrigger for Library Room
			//Find the one out of place book and put it back into place
			//TriggerSystem.AddTriggerToList ("LibraryRoomPastSoul", false); //False because it hasn't been aquired yet.

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
		case "DustOne":
			mDustPileCount++;
			break;
		case "DustTwo":
			mDustPileCount++;
			break;
		case "DustThree":
			mDustPileCount++;
			break;
		case "DustFour":
			mDustPileCount++;
			break;
		case "DustFive":
			mDustPileCount++;
			break;
		case "DustSix":
			mDustPileCount++;
			break;
		case "DustSeven":
			mDustPileCount++;
			break;
		case "Broom":
			//If we picked up the broom then we can clean up now
			TriggerSystem.AddTriggerToList ("DustOne", false); //False because it hasn't been aquired yet.
			TriggerSystem.AddTriggerToList ("DustTwo", false); //False because it hasn't been aquired yet.
			TriggerSystem.AddTriggerToList ("DustThree", false); //False because it hasn't been aquired yet.
			TriggerSystem.AddTriggerToList ("DustFour", false); //False because it hasn't been aquired yet.
			TriggerSystem.AddTriggerToList ("DustFive", false); //False because it hasn't been aquired yet.
			TriggerSystem.AddTriggerToList ("DustSix", false); //False because it hasn't been aquired yet.
			TriggerSystem.AddTriggerToList ("DustSeven", false); //False because it hasn't been aquired yet.
			break;
		case "MainHallPastSoul":
			//We completed a quest
			//Thank you for Cleaning the MainHall
			StartCoroutine(ShowMessage("Broom Quest", 5.0f));
                break;
           
		case "LibraryBookOnGround":
			TriggerSystem.AddTriggerToList ("LibraryShelfForMissingBook", false); //False because it hasn't been aquired yet.
			break;
		case "LibraryShelfForMissingBook":
			//We put the library book back on the shelf we are good to flag for final here
			TriggerSystem.AddTriggerToList ("LibraryRoomPastSoul", false); //False because it hasn't been aquired yet.

			break;
		case "LibraryRoomPastSoul":
			//We completed a quest
			//Thank you for finding my lost book
			StartCoroutine(ShowMessage("Book Quest", 5.0f));
			break;
		
		

		default:
			break;
		}

		//Remove the Object from play as it has already been used.
		fHitObject.transform.gameObject.SetActive (false);

		if( fHitObject.collider.name.Contains("Dust") )
		{
			//We must be cleaning up an spot on the floor
			//Play Sweeping Sound?
			AudioSource.PlayClipAtPoint(mBroomSweepingClip, fHitObject.collider.transform.position);
		}
		else
		//Play audio file for picking up an item
		if( !mPickupItemSound.isPlaying )
			mPickupItemSound.Play();
		else 
		{
			mPickupItemSound.Stop();
			mPickupItemSound.Play();
		}


		//Check for Crayon Quest Win Condition (Five Crayons, and not already completed)
		if (mCrayonCount == 5 && !TriggerSystem.GetTriggerState( "DinningRoomPastSoul" )) 
		{
			if( !TriggerSystem.IsTriggerPresent( "DinningRoomPastSoul" ))
				TriggerSystem.AddTriggerToList( "DinningRoomPastSoul", false ); //False because it hasn't been aquired yet.
		}

		//Check for the Main Hall Quest Win Condition (Seven (7) Dust Piles, and not already completed)
		if( mDustPileCount == 7 && !TriggerSystem.GetTriggerState( "MainHallPastSoul" ))
		{
			if( !TriggerSystem.IsTriggerPresent("MainHallPastSoul" ))
			   TriggerSystem.AddTriggerToList( "MainHallPastSoul", false ); //False because it hasn't been aquired yet.
		}

		//Check for Game Win Condition
		if (TriggerSystem.GetTriggerState ("LibraryRoomPastSoul") && TriggerSystem.GetTriggerState ("DinningRoomPastSoul") && TriggerSystem.GetTriggerState ("MainHallPastSoul") ) 
		{
			//Debug.Log("Game has been won!");
			StartCoroutine(DelayedSceneLoad());
		}
	}

	//Helper Walk Function
	IEnumerator WalkHelper( RaycastHit fHit, float fTime )
	{
		//Wait the requested time
		yield return new WaitForSeconds (fTime);



	}

	IEnumerator DelayedSceneLoad()
	{
		//Play You Win!

		yield return new WaitForSeconds(5.0f);
		AsyncOperation async = Application.LoadLevelAsync("Credits");
		yield return async;
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

	/*
	IEnumerator FinishPastPerson(string fRoom)
	{
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

		//Send them back to first person mode
		ThirdToFirstPerson();
	}
	*/

    //Audio Handlers
    IEnumerator PlayWalkSound (float fTime)
	{
		mWalkingSound.Play ();
		yield return new WaitForSeconds (fTime);
		if( mWalkingSound.isPlaying)
			mWalkingSound.Stop ();

	}
}