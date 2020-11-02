using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class DialogTriggerCreator : EditorWindow
{
	public static void OpenDialogCreator()
	{
		EditorWindow.GetWindow<DialogTriggerCreator>();
	}
	public GameObject dialogEntity;
	public DialogTrigger dialogTrigger;
	public bool requireVar;
	private void OnGUI()
	{
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Entity Object");
		dialogEntity = (GameObject)EditorGUILayout.ObjectField(dialogEntity, typeof(GameObject), true);
		EditorGUILayout.EndHorizontal();
		if (dialogEntity == null)
			return;
		if (dialogTrigger == null)
		{
			dialogTrigger = dialogEntity.GetComponent<DialogTrigger>();
		}
		if (dialogTrigger == null)
		{
			if (GUILayout.Button("Add Dialog Component"))
			{
				dialogTrigger = dialogEntity.AddComponent<DialogTrigger>();
			}
		}
		else
		{
			dialogTrigger.NPCName = EditorGUILayout.TextField("NPC Name", dialogTrigger.NPCName);
			#region Dialog Object
			if (dialogTrigger.dialogObject == null)
			{
				if (GUILayout.Button("Set Dialog"))
				{
					ObjectPicker.GetObject(ObjectPicker.ObjectType.Dialog, SetDialog);
				}
			}
			else
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(dialogTrigger.dialogObject.name);
				if (GUILayout.Button("X"))
				{
					dialogTrigger.dialogObject = null;
				}
				EditorGUILayout.EndHorizontal();
			}
			#endregion
			#region Audio Source
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Audio Source");
			dialogTrigger.source = (AudioSource)EditorGUILayout.ObjectField(dialogTrigger.source, typeof(AudioSource), true);
			if (dialogTrigger.source == null)
			{
				if (GUILayout.Button("Find Audio"))
				{
					if (dialogTrigger.GetComponent<LipsyncController>())
					{
						dialogTrigger.source = dialogTrigger.GetComponent<LipsyncController>().mouthSource;
					}
					else
						dialogTrigger.source = dialogTrigger.GetComponent<AudioSource>();
				}
			}
			EditorGUILayout.EndHorizontal();
			#endregion
			if (dialogTrigger.animator == null)
				dialogTrigger.animator = dialogTrigger.GetComponent<Animator>();
			if (requireVar = EditorGUILayout.Toggle("Require Variable", requireVar))
			{
				EditorGUILayout.BeginHorizontal();
				if (!string.IsNullOrEmpty(dialogTrigger.requiredVar))
				{
					EditorGUILayout.LabelField(dialogTrigger.requiredVarName);
					dialogTrigger.requiredValue = EditorGUILayout.Toggle(dialogTrigger.requiredValue);
					if (GUILayout.Button("X"))
					{
						dialogTrigger.requiredVar = "";
						dialogTrigger.requiredVarName = "";
					}
				}
				else
				{
					if (GUILayout.Button("Set Var"))
					{
						ObjectPicker.GetObject(ObjectPicker.ObjectType.FullVar, SetRequiredVar);
					}
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Head Transfer");
			dialogTrigger.targetLookObj = (Transform)EditorGUILayout.ObjectField(dialogTrigger.targetLookObj, typeof(Transform), true);
			EditorGUILayout.EndHorizontal();
			dialogTrigger.RotateToTarget = EditorGUILayout.Toggle("Rotate On Interact", dialogTrigger.RotateToTarget);
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Interact Angle");
			dialogTrigger.interactAngle = EditorGUILayout.Slider(dialogTrigger.interactAngle, 15.0f, 180.0f);
			EditorGUILayout.EndHorizontal();
			EditorUtility.SetDirty(dialogTrigger);
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			if (GUILayout.Button("Close"))
			{
				this.Close();
			}
		}
	}
	public void SetDialog(object obj)
	{
		dialogTrigger.dialogObject = obj as DialogObject;
	}
	public void SetRequiredVar(object obj)
	{
		DialogVars.DialogVar  dv = obj as DialogVars.DialogVar;
		dialogTrigger.requiredVar = dv.ID;
		dialogTrigger.requiredVarName = dv.Name;
	}
}
