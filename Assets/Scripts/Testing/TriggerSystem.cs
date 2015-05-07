using System.Collections.Generic;

public static class TriggerSystem {

	//Class Variables
	public static List<TriggerObject> mTriggerObjects = new List<TriggerObject>();

	static public void Create()
	{
		// Does nothing but call constructor to add game object into scene
	}

	//Add a trigger to the system for monitoring
	public static void AddTriggerToList( string pName, bool pIsTriggered )
	{
		TriggerObject fNewTriggerObject = new TriggerObject (pName, pIsTriggered);
		mTriggerObjects.Add (fNewTriggerObject);
	}

	public static void SetTriggerInList( string pName, bool pIsTriggered )
	{
		foreach (TriggerObject fTO in mTriggerObjects) 
		{
			if( pName == fTO.mName )
			{
				//We found the trigger object
				fTO.mIsTriggered = pIsTriggered;
			}
		}
	}

	public static bool IsTriggerActive ( string pName )
	{
		foreach (TriggerObject fTO in mTriggerObjects) 
		{
			if( pName == fTO.mName )
			{
				//We found the trigger object
				return fTO.mIsTriggered;
			}
		}
		return false;
	}

	public static void CleanTriggerList()
	{
		foreach (TriggerObject fTO in mTriggerObjects) 
		{
			fTO.mName = "";
			fTO.mIsTriggered = false;
		}
	}
}
