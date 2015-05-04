using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {

	private int A_index;
	public int door_number;
	private bool lock1;

	void Start ()
	{

		lock1 = false;
		A_index=0;
	}
	
	// Update is called once per frame
	public void playanmation ()
	{

	if (lock1 != true) {
			if (!animation.isPlaying) {
				if (A_index == 0) {
					animation.Play ("door_open");
					A_index = 1;
				} else {
					animation.Play ("door_close");
					A_index = 0;
				}
				if (lock1 == true) {
					//	audio.PlayOneShot(); //need an audio source
					Debug.Log ("Door is closed");
				}

			}
		}
	}
}
