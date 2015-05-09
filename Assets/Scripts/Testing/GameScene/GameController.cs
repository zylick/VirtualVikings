using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

    public enum GameMode
    {
        FirstPerson = 0,
        ThirdPerson = 1,
        PastPerson = 2,
    }
    
    public OVRCameraRig mThirdPersonCam;
    public GameObject CenterPosition;
    public OVRCameraRig mFirstPersonCam;
    public TextMesh mTextTimer;
    public TextMesh mFPSCounter;
    public GameObject mPlayerObject;
    public GameObject mPlayerIcon;
    
    private static float mTimer;
    private float mTimeLeft;
    private float mAccumulation;
    private int mFrames;
    private float mUpdateInterval;
    private GameMode mGameMode = GameMode.FirstPerson;
    
    // Use this for initialization
    void Start () {
        
        //Initialize Defaults
        mThirdPersonCam.gameObject.SetActive (false); //Set the third person camera to inactive.
        mFirstPersonCam.gameObject.SetActive (true); //Set the First person camera to active.
        mGameMode = GameMode.FirstPerson; //Set the tutorial mode to First Person
        mPlayerObject.SetActive(false); // Turn the player off since we are going to First Person.
		Crosshair3D.mode = Crosshair3D.CrosshairMode.Dynamic;
		Crosshair3D.crosshairMaterial.color = Color.white;
		
		mPlayerObject.GetComponent<BoxCollider>().enabled = false;


        //Initialize the touchPad for use
        OVRTouchpad.Create ();
        OVRTouchpad.TouchHandler += TouchHandlerCapture; //Bind a function here to the events in hte touchpad.
        
        //Testing Section
        //mThirdPersonCam.gameObject.SetActive (true); //Set the third person camera to inactive.
        //mFirstPersonCam.gameObject.SetActive (false); //Set the First person camera to active.
        //mGameMode = GameMode.ThirdPerson; //Set the tutorial mode to Third Person
        //mPlayerObject.SetActive(true); // Turn the player on since we are going to Third Person.
        
        //Set the timer to the start position
        mTimer = 60.0f;
        mTimeLeft = mUpdateInterval = 0.5f;
        
        //Initialize my trigger System with the Trigger list
        TriggerSystem.Create ();
        //TriggerSystem.AddTriggerToList ("RedKey", false);
        //TriggerSystem.AddTriggerToList ("GreenKey", false);
        //TriggerSystem.AddTriggerToList ("BlueKey", false);
        //TriggerSystem.AddTriggerToList ("BrownKey", false);
        
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
        
        //Get the current FPS and display it here:
        mTimeLeft -= Time.deltaTime;
        mAccumulation += Time.timeScale / Time.deltaTime;
        ++mFrames;
        
        if (mTimeLeft <= 0.0f) 
        {
            //Display float to the second digit
            mFPSCounter.text = "FPS: " + (mAccumulation/mFrames).ToString("f2");
            mTimeLeft = mUpdateInterval;
            mAccumulation = 0.0f;
            mFrames = 0;
        }
        
        //If we touched our back button in first person we want to switch to third person
        if (Input.GetMouseButtonDown (1) && mGameMode == GameMode.FirstPerson ) 
        {
            mThirdPersonCam.gameObject.SetActive (true); //Set the third person camera to inactive.
            mFirstPersonCam.gameObject.SetActive (false); //Set the First person camera to active.
            mGameMode = GameMode.ThirdPerson; //Set the tutorial mode to Third Person
            mPlayerIcon.SetActive(true); //Turn the player on since we are going to 3rd person.
            Crosshair3D.mode = Crosshair3D.CrosshairMode.Dynamic;
            Crosshair3D.crosshairMaterial.color = Color.white;
            
            mPlayerObject.GetComponent<BoxCollider>().enabled = true;
        }
        
    }
    
    private void TouchHandlerCapture( object pSender, System.EventArgs pE)
    {
        switch (mGameMode) 
        {
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
    
    private void FirstPersonTouchHandler(object pSender, System.EventArgs pE)
    {
        OVRTouchpad.TouchArgs fTouchArguments = (OVRTouchpad.TouchArgs)pE;
        switch (fTouchArguments.TouchType) 
        {
            case OVRTouchpad.TouchEvent.SingleTap:
                //mThisGameObject.renderer.material.color = new Color(Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f));
                //swap the player mode.
                //Debug.Log ("FirstPerson Tap");
                //mFirstPersonCam.centerEyeAnchor.position;
                //mFirstPersonCam.centerEyeAnchor.forward;
                
                //Cursor on so we can see what we are staring at
                //Raycast to see what we are looking at.
                Ray ray;
                RaycastHit hit;
                Vector3 cameraPosition = mFirstPersonCam.centerEyeAnchor.position;
                Vector3 cameraForward = mFirstPersonCam.centerEyeAnchor.forward;
                
                ray = new Ray(cameraPosition, cameraForward);
                Physics.Raycast(ray, out hit);
                //Tap to either pick up an item, interact with a door or swap to see what we are looking at.
                //Debug.Log (hit.collider.gameObject.layer);
                if(hit.collider.gameObject.layer == 8 && Crosshair3D.mode == Crosshair3D.CrosshairMode.DynamicObjects) //8 is keys
                {
                    //Debug.Log(hit.collider.name);
                    //Which key was it?
                    //Debug.Log (TriggerSystem.IsTriggerPresent(hit.collider.name));
                    
                    if (TriggerSystem.IsTriggerPresent(hit.collider.name))
                    {
                        TriggerSystem.SetTriggerInList(hit.collider.name, true);
                        
                        //We touched a key pick it up.
                        hit.transform.gameObject.SetActive(false);
                    }
                    else
                    {
                        //Display a message saying that the item in question is not interactive?
                    }
                }
                else if( hit.collider.gameObject.layer == 9) //9 is doors
                {
                    //We touched a door see if we can open it
                    //If we have key for door open door else display message that the door is locked?
                    hit.collider.gameObject.SetActive(false);
                }
                else
                {
                    //We want to switch Cursor Mode from seeing where we are looking to check for interaction
                    switch (Crosshair3D.mode)
                    {
                        case Crosshair3D.CrosshairMode.DynamicObjects:
                            Crosshair3D.mode = Crosshair3D.CrosshairMode.Dynamic;
                            Crosshair3D.crosshairMaterial.color = Color.white;
                            break;
                        case Crosshair3D.CrosshairMode.Dynamic:
                            Crosshair3D.mode = Crosshair3D.CrosshairMode.DynamicObjects;
                            Crosshair3D.crosshairMaterial.color = Color.red;
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
                //Debug.Log ("ThirdPerson Tap");
                
                //If we do not click on the player move him to that location.
                Ray ray;
                RaycastHit hit;
                Vector3 cameraPosition = mThirdPersonCam.centerEyeAnchor.position;
                Vector3 cameraForward = mThirdPersonCam.centerEyeAnchor.forward;
                
                ray = new Ray(cameraPosition, cameraForward);
                Physics.Raycast(ray, out hit);
                
                //Debug.DrawLine(cameraPosition, hit.point, Color.blue, 30, true);
                //Debug.Log(hit.point);
                
                //Debug.Log ( hit.collider.name );
                if( hit.collider.name != "Player" )
                {
                    //Calculate the time it would take to move the character at a constant pace
                    float mTimeToWalk = Vector3.Distance(mPlayerObject.transform.position, new Vector3( hit.point.x, mPlayerObject.transform.position.y,hit.point.z));
                    //iTween.MoveBy(gameObject, iTween.Hash("x", 2, "easeType", "easeInOutExpo", "loopType", "pingPong", "delay", .1));
                    //Debug.Log (mTimeToWalk);
                    iTween.MoveTo (mPlayerObject, iTween.Hash("x",hit.point.x ,"y",mPlayerObject.transform.position.y ,"z",hit.point.z , "easeType", "linear", "time", mTimeToWalk / 3));
                    //iTween.MoveTo(mPlayerObject,new Vector3( hit.point.x, mPlayerObject.transform.position.y,hit.point.z),mTimeToWalk);
                    //mPlayerObject.transform.position = new Vector3( hit.point.x, mPlayerObject.transform.position.y,hit.point.z );
                    
                }
                //If we click on the player switch it to first person mode
                else
                {
                    //Swap over to First Person
                    mThirdPersonCam.gameObject.SetActive (false); //Set the third person camera to inactive.
                    mFirstPersonCam.gameObject.SetActive (true); //Set the First person camera to active.
                    mGameMode = GameMode.FirstPerson; //Set the tutorial mode to First Person
                    mPlayerIcon.SetActive(false); // Turn the player off since we are going to First Person.  
                    
                    //Make sure to disable the box collider so It doesn't interfere with the cursor
                    mPlayerObject.GetComponent<BoxCollider>().enabled = false;
                    
                    //If we have an iTween Running pause it
                    iTween.Stop(mPlayerObject);
                    
                    //Crosshair3D.mode = Crosshair3D.CrosshairMode.Dynamic;
                    //Crosshair3D.crosshairMaterial.color = Color.white;
                }
                
                break;
            case OVRTouchpad.TouchEvent.Up:
                //Means go Left (swipeing toward the top of the device)
                Vector3 left = new Vector3(-forward.z, 0.0f, forward.x);
                mPlayerObject.transform.position += left;
                break;
            case OVRTouchpad.TouchEvent.Right:
                //Means go forward (swipeing toward the front of the device)
                mPlayerObject.transform.position -= forward;
                break;
            case OVRTouchpad.TouchEvent.Down:
                //Means go right (swipeing toward the bottom of the device)
                Vector3 right = new Vector3(forward.z, 0.0f, -forward.x);
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

    private void PastModeTouchhandler(object pSender, System.EventArgs pE)
    {
        OVRTouchpad.TouchArgs fTouchArguments = (OVRTouchpad.TouchArgs)pE;

        switch (fTouchArguments.TouchType) 
        {
            case OVRTouchpad.TouchEvent.SingleTap:
                //Send them back to first person mode
                mThirdPersonCam.gameObject.SetActive (false); //Set the third person camera to inactive.
                mFirstPersonCam.gameObject.SetActive (true); //Set the First person camera to active.
                mGameMode = GameMode.FirstPerson; //Set the tutorial mode to First Person
                mPlayerIcon.SetActive(false); // Turn the player off since we are going to First Person.  
                
                //Make sure to disable the box collider so It doesn't interfere with the cursor
                mPlayerObject.GetComponent<BoxCollider>().enabled = false;
                
                //If we have an iTween Running pause it
                iTween.Stop(mPlayerObject);

                //Switch the scene back to regular mode

                //Turn off the old film shaders

                break;
        }
    }
}