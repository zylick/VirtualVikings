/************************************************************************************

Filename    :   StartupSample.cs
Content     :   An example of how to set up your game for fast loading with a
			:	black splash screen, and a small logo screen that triggers an
			:	async main scene load
Created     :   June 27, 2014
Authors     :   Andrew Welch

Copyright   :   Copyright 2014 Oculus VR, LLC. All Rights reserved.


************************************************************************************/

using UnityEngine;
using System.Collections;				// required for Coroutines

public class Credits : MonoBehaviour
{
	public float			delayBeforeLoad = 0.0f;
	public string			sceneToLoad = string.Empty;
	public GameObject		mSlide0;
	public GameObject		mSlide1;
	public GameObject		mSlide2;
	public GameObject		mSlide3;
	public GameObject		mSlide4;
	public GameObject		mSlide5;
	public GameObject		mSlide6;
	public GameObject		mSlide7;
	public GameObject		mSlide8;
	public GameObject		mSlide9;

	public AudioSource 		mSlamAudioSource;
	public AudioSource 		mCreditsMusicSource;
	private float 			mSlideTime = 0.5f;
	
	/// <summary>
	/// Start a delayed scene load
	/// </summary>
	void Start()
	{
		// start the main scene load
		StartCoroutine(DelayedSceneLoad());
		mSlide0.SetActive (false);
		mSlide1.SetActive (false);
		mSlide2.SetActive (false);
		mSlide3.SetActive (false);
		mSlide4.SetActive (false);
		mSlide5.SetActive (false);
		mSlide6.SetActive (false);
		mSlide7.SetActive (false);
		mSlide8.SetActive (false);
		mSlide9.SetActive (false);

	}
	
	/// <summary>
	/// Asynchronously start the main scene load
	/// </summary>
	IEnumerator DelayedSceneLoad()
	{
		// delay one frame to make sure everything has initialized
		yield return 0;
		
		yield return new WaitForSeconds (5);
		//At this point we can time the crossing the Threshold slide down
		mSlide0.SetActive (true);
		iTween.MoveTo (mSlide0, iTween.Hash("x", 0.0f, "y", 3.47f, "z", 9.0f, "easeType", "EaseOutQuart", "time", mSlideTime));
		mSlamAudioSource.Play ();
		
		yield return new WaitForSeconds (5);
		mSlamAudioSource.Stop ();
		
		mSlide1.SetActive (true);
		mSlamAudioSource.Play ();
		iTween.MoveTo (mSlide1, iTween.Hash("x", 0.0f, "y", 3.47f, "z", 8.9f, "easeType", "EaseOutQuart", "time", mSlideTime));

		yield return new WaitForSeconds (5);
		mSlamAudioSource.Stop ();
		
		mSlide2.SetActive (true);
		mSlamAudioSource.Play ();
		iTween.MoveTo (mSlide2, iTween.Hash("x", 0.0f, "y", 3.47f, "z", 8.8f, "easeType", "EaseOutQuart", "time", mSlideTime));

		yield return new WaitForSeconds (5);
		mSlamAudioSource.Stop ();
		
		mSlide3.SetActive (true);
		mSlamAudioSource.Play ();
		iTween.MoveTo (mSlide3, iTween.Hash("x", 0.0f, "y", 3.47f, "z", 8.7f, "easeType", "EaseOutQuart", "time", mSlideTime));

		yield return new WaitForSeconds (5);
		mSlamAudioSource.Stop ();
		
		mSlide4.SetActive (true);
		mSlamAudioSource.Play ();
		iTween.MoveTo (mSlide4, iTween.Hash("x", 0.0f, "y", 3.47f, "z", 8.6f, "easeType", "EaseOutQuart", "time", mSlideTime));

		yield return new WaitForSeconds (5);
		mSlamAudioSource.Stop ();
		
		mSlide5.SetActive (true);
		mSlamAudioSource.Play ();
		iTween.MoveTo (mSlide5, iTween.Hash("x", 0.0f, "y", 3.47f, "z", 8.5f, "easeType", "EaseOutQuart", "time", mSlideTime));

		yield return new WaitForSeconds (5);
		mSlamAudioSource.Stop ();
		
		mSlide6.SetActive (true);
		mSlamAudioSource.Play ();
		iTween.MoveTo (mSlide6, iTween.Hash("x", 0.0f, "y", 3.47f, "z", 8.4f, "easeType", "EaseOutQuart", "time", mSlideTime));

		yield return new WaitForSeconds (5);
		mSlamAudioSource.Stop ();
		
		mSlide7.SetActive (true);
		mSlamAudioSource.Play ();
		iTween.MoveTo (mSlide7, iTween.Hash("x", 0.0f, "y", 3.47f, "z", 8.3f, "easeType", "EaseOutQuart", "time", mSlideTime));

		yield return new WaitForSeconds (5);
		mSlamAudioSource.Stop ();
		
		mSlide8.SetActive (true);
		mSlamAudioSource.Play ();
		iTween.MoveTo (mSlide8, iTween.Hash("x", 0.0f, "y", 3.47f, "z", 8.2f, "easeType", "EaseOutQuart", "time", mSlideTime));

		yield return new WaitForSeconds (5);
		mSlamAudioSource.Stop ();
		
		mSlide9.SetActive (true);
		mSlamAudioSource.Play ();
		iTween.MoveTo (mSlide9, iTween.Hash("x", 0.0f, "y", 3.47f, "z", 8.1f, "easeType", "EaseOutQuart", "time", mSlideTime));

		//iTween.MoveTo (mPlayerObject, iTween.Hash("x",hit.point.x ,"y",mPlayerObject.transform.position.y ,"z",hit.point.z , "easeType", "linear", "time", mTimeToWalk / 3));
		// this is *ONLY* here for example as our 'main scene' will load too fast
		// remove this for production builds or set the time to 0.0f
		yield return new WaitForSeconds(delayBeforeLoad);
		mSlamAudioSource.Stop ();
		Debug.Log( "[LoadLevel] " + sceneToLoad + " Time: " + Time.time );
		
		float startTime = Time.realtimeSinceStartup;
		#if !UNITY_PRO_LICENSE
		Application.LoadLevel(sceneToLoad);
		#else
		//*************************
		// load the scene asynchronously.
		// this will allow the player to 
		// continue looking around in your loading screen
		//*************************
		AsyncOperation async = Application.LoadLevelAsync(sceneToLoad);
		yield return async;
		#endif
		Debug.Log( "[SceneLoad] Completed: " + (Time.realtimeSinceStartup - startTime).ToString("F2") + " sec(s)");
	}
	
}
