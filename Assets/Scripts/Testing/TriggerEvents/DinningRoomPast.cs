using UnityEngine;
using System.Collections;

public class DinningRoomPast : MonoBehaviour {

	void OnTriggerEnter( Collider fCollider )
	{
		Debug.Log ("Dinning Room Past: OnTriggerEnter");
	}

	void OnTriggerStay( Collider fCollider )
	{
		Debug.Log ("Dinning Room Past: OnTriggerStay");
	}

	void OnTriggerExit( Collider fCollider )
	{
		Debug.Log ("Dinning Room Past: OnTriggerExit");
	}
}
