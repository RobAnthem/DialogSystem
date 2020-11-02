using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogObject : ScriptableObject
{
	public enum DialogEventType
	{
		SetVar,
		CloseDialog,
	}
	[System.Serializable]
	public class DialogCondition
	{
		public string varID;
		public bool varState;
		public bool isOneTime;
		public string oneTimeID;
	}
	[System.Serializable]
	public class Dialog
	{
		public string text;
		public int ID;
		public AudioClip clip;
		public GameObject sourceObject;
		public GameObject entity;
		public int TalkID;
		public DialogResponse[] responses;
		public bool hasEvent;
		public int eventID;
		public DialogCondition conditions;
		public bool isEntry;
		//public int entryID;
	}
	[System.Serializable]
	public class DialogResponse
	{
		public string response;
		public int nextID;
		public int nextEventID = -1;
		public int eventID;
		public DialogCondition conditions;
	}
	[System.Serializable]
	public class DialogEvent
	{
		public int id;
		public DialogEventType type;
		public string stringField;
		public int intField;
		public bool boolField;
		public int nextDialogID;
		public int nextEventID;
	}
	public int startDialog;
	public int[] fallBacks;
	public bool canExit;
	public Dialog[] dialog;
	public int entries;
	public List<DialogEvent> events = new List<DialogEvent>();
	public DialogEvent GetEvent(int id)
	{
		foreach (DialogEvent dEvent in events)
		{
			if (dEvent.id == id)
				return dEvent;
		}
		return null;
	}
}
