using UnityEngine;
using System.Collections;

public class TestPhantomScare : MonoBehaviour {

	//Default Settings for Phantom
	public GameObject mPhantomFace;
	public AudioSource mPhantomScream;
	public GameObject mMessages;
	//public GameObject mBoxTest;


	private bool mStageOne 			= false; //Normaly set to true but done with testing for now.
	private bool mStageTwo 			= false;
	private bool mStageThree 		= false;

	private const float STAGE_TWO_TRIGGER = 0.005f;
	private const float STAGE_THREE_TRIGGER = 0.05f;

	// Use this for initialization
	void Start () {
		Color color = mPhantomFace.renderer.material.color;
		color.a = 0.0f;
		mPhantomFace.renderer.material.color = color;
	}

	// Update is called once per frame
	void FixedUpdate () {

		//How slow to make it appear (Really slow at first) and then suddenly
		//float timer += Time.deltaTime;

		//float fMinutes = Mathf.Floor(timer / 60).ToString("00");
		//float fSeconds = (timer % 60).ToString("00");

		//So make stages Stage One
		if( mStageOne ) 
		{
			//slowly increase the alpha
			Color color = mPhantomFace.renderer.material.color;
			color.a += 0.000005f;
			mPhantomFace.renderer.material.color = color;

			if( color.a > STAGE_TWO_TRIGGER )
			{
				//Switch to Stage Two
				mStageOne = false;
				mStageTwo = true;
				mStageThree = false;
			}
		}

		if( mStageTwo ) 
		{
			//slowly increase the alpha
			Color color = mPhantomFace.renderer.material.color;
			color.a += 0.005f;
			mPhantomFace.renderer.material.color = color;

			//Make some noises or start turning down audio
			if( color.a > STAGE_THREE_TRIGGER)
			{
				//Switch to Stage Three
				mStageOne = false;
				mStageTwo = false;
				mStageThree = true;
			}
		}

		if( mStageThree )
		{
			//Finish
			mStageOne = false;
			mStageTwo = false;
			mStageThree = false;

			//Play scream
			mPhantomScream.Play ();

			//Set the Alpha to Max
			Color color = mPhantomFace.renderer.material.color;
			color.a = 0.70f;
			mPhantomFace.renderer.material.color = color;

			//Start our countdown to clear and show message
			StartCoroutine(GameOver());
		}
	}

	IEnumerator GameOver()
	{
		//Shake the face
		iTween.ShakePosition (mPhantomFace, new Vector3(0.10f, 0.10f, 0.10f), 6.0f);
		//iTween.ShakePosition(mPhantomFace, iTween.Hash("amout", new Vector3(0,0.1f,0), "time", 3.0f));

		//Wait for two seconds so sound can finish playing.
		yield return new WaitForSeconds (7);
		//Stop scream
		mPhantomScream.Stop ();

		//Hide Face again
		Color color = mPhantomFace.renderer.material.color;
		color.a = 0.0f;
		mPhantomFace.renderer.material.color = color;

		//Update text message
		mMessages.SetActive (true);
		TextMesh fMessage = mMessages.GetComponent<TextMesh> ();
		fMessage.text = "You have failed...";
	}

}
