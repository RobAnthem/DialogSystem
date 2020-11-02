#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class ObjectPicker : EditorWindow
{
	public enum ObjectType
	{
		Variable,
		FullVar,
		Dialog,
		Other
	}

	private System.Action<object> objectCallback;
	private ObjectType currentType;
	private DialogVars vars;
	private List<DialogObject> dialogObjects;
	/// <summary>
	/// Starts the object picker.
	/// </summary>
	/// <param name="typ"></param>
	/// <param name="callback"></param>
	public static void GetObject(ObjectType typ, System.Action<object> callback)
	{
		ObjectPicker objPick = GetWindow<ObjectPicker>();
		objPick.Init(typ, callback);
	}
	/// <summary>
	/// Initialization of object picker, add functionality here for more picker options.
	/// </summary>
	/// <param name="typ"></param>
	/// <param name="callback"></param>
	public void Init(ObjectType typ, System.Action<object> callback)
	{
		objectCallback = callback;
		currentType = typ;
		if (typ == ObjectType.Variable || typ == ObjectType.FullVar)
		{
			vars = DialogVars.Instance;
		}
		else if (typ == ObjectType.Dialog)
		{
			dialogObjects = new List<DialogObject>(Resources.LoadAll<DialogObject>(""));
		}
		else if (typ == ObjectType.Other)
		{
			//Empty init option for adding additional picker options
		}

	}
	private Vector2 scrollRect;
	void OnGUI()
	{
		scrollRect = EditorGUILayout.BeginScrollView(scrollRect);
		if (currentType == ObjectType.Variable)
		{
			ShowVars();
		}
		else if (currentType == ObjectType.FullVar)
		{
			ShowFullVars();
		}
		else if (currentType == ObjectType.Dialog)
		{
			ShowDialogs();
		}
		else if (currentType == ObjectType.Other)
		{
			ShowOther();
		}

		EditorGUILayout.EndScrollView();
	}

	/// <summary>
	/// Display variables as selection options.
	/// </summary>
	void ShowVars()
	{
		foreach (DialogVars.DialogVar dVar in vars.dialogVars.Values)
		{
			if (GUILayout.Button(dVar.Name))
			{
				SelectObject(dVar.ID);
			}
		}
	}
	void ShowFullVars()
	{
		foreach (DialogVars.DialogVar dVar in vars.dialogVars.Values)
		{
			if (GUILayout.Button(dVar.Name))
			{
				SelectObject(dVar);
			}
		}
	}
	void ShowDialogs()
	{
		foreach (DialogObject dObj in dialogObjects)
		{
			if (GUILayout.Button(dObj.name))
			{
				SelectObject(dObj);
			}
		}
	}
	/// <summary>
	/// Empty example function for adding moree picker options
	/// </summary>
	void ShowOther()
	{
		//Empty function example for adding more picker options
	}

	/// <summary>
	/// Finalize selection, sending callback to original caller
	/// </summary>
	/// <param name="obj"></param>
	public void SelectObject(object obj)
	{
		objectCallback.Invoke(obj);
		Close();
	}
}
#endif