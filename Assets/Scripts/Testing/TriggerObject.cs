using System.Collections.Generic;

public class TriggerObject {
	public bool mIsTriggered = false;
	public string mName = "";

	public TriggerObject(string pName , bool pIsTriggered )
	{
		mIsTriggered = pIsTriggered;
		mName = pName;
	}

	void SetTriggerObjectName(string pName)
	{
		mName = pName;
	}

	string GetTriggerObjectName()
	{
		return mName;
	}

	void SetIsTriggered( bool pIsTriggered)
	{
		mIsTriggered = pIsTriggered;
	}

	bool GetIsTriggered()
	{
		return mIsTriggered;
	}
}
