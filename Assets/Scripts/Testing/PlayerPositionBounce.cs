using UnityEngine;
using System.Collections;

public class PlayerPositionBounce : MonoBehaviour {

	public bool mMove = true;
	public Vector3 mMoveVector = Vector3.up;
	public float mMoveRange = 2.0f;
	public float mMoveSpeed = 0.5f;
	public Transform mCube;

	private Vector3 mStartPosition;
	private PlayerPositionBounce mBounceObject;


	// Use this for initialization
	void Start () {
		mBounceObject = this;
		mStartPosition = mBounceObject.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (mMove) 
		{
			//Transform the position based on the timeSinceLevelLoad
			mBounceObject.transform.position = new Vector3 (mCube.position.x, mCube.position.y + 1, mCube.position.z ) + mMoveVector *
				(mMoveRange * Mathf.Sin(Time.timeSinceLevelLoad * mMoveSpeed));
		}
	}
}
