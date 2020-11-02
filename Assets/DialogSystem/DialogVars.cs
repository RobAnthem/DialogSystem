using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogVars
{
	public static DialogVars Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new DialogVars();
				instance.Init();
			}
			return instance;
		}
	}
	private static DialogVars instance;
	[System.Serializable]
	public class DialogVar
	{
		public string Name;
		public string ID;
		public bool value;
	}
	private const string fileName = "dialogVars";
	public Dictionary<string, DialogVar> dialogVars;
	public void Init()
	{
		Load();
		if (dialogVars == null)
			dialogVars = new Dictionary<string, DialogVar>();
	}
	public bool GetValue(string id)
	{
		if (!dialogVars.ContainsKey(id))
			return false;
		return dialogVars[id].value;
	}
	public void SetVar(string id, bool value)
	{
		if (dialogVars.ContainsKey(id))
		{
			dialogVars[id].value = value;
		}
		else
		{
			dialogVars.Add(id, new DialogVar() { ID = id, value = value } );
		}
	}
	public void Save()
	{
		SaveManager.Instance.SaveData(this.dialogVars, fileName);
	}
	public void Load()
	{
		dialogVars = SaveManager.Instance.LoadData<Dictionary<string, DialogVar>>(fileName);
	}
}
