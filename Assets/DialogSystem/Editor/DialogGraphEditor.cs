using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using XNodeEditor;

[CustomNodeGraphEditor(typeof(DialogGraph))]
public class DialogGraphEditor : NodeGraphEditor
{
	public DialogObject dialogObj;
	public bool options;
	public bool fallbacks;
	public DialogGraph graph;
	Vector2 box = new Vector2(300, 500);
	public override void OnGUI()
	{
		//GUILayout.BeginArea(new Rect(new Vector2(0, 0), box));
		if (graph == null)
			graph = target as DialogGraph;
		if (dialogObj == null)
			dialogObj = (target as DialogGraph).dialogObject;
		if (dialogObj == null)
			return;
		EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.MinHeight(20), GUILayout.MaxHeight(20));
		options = EditorGUILayout.Toggle("Options", options, GUILayout.MaxWidth(100));
		if (GUILayout.Button("Save", EditorStyles.toolbarButton))
		{
			if (dialogObj.name != graph.name)
			{
				AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(dialogObj), graph.name);
				AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(graph), dialogObj.name);
			}
			window.Save();
			AssetDatabase.SaveAssets();
		}
		if (GUILayout.Button("Add Node", EditorStyles.toolbarButton))
		{
			graph.AddNode(typeof(DialogNode));
		}
		if (GUILayout.Button("Add Event", EditorStyles.toolbarButton))
		{
			graph.AddNode(typeof(DialogEventNode));
		}
		graph.name = EditorGUILayout.TextField(graph.name, EditorStyles.toolbarTextField);

		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.MaxWidth(150));
		if (options)
		{
			Debug.Log("options enabled");
			EditorGUILayout.BeginVertical(EditorStyles.helpBox);
			//dialogObj.name = EditorGUILayout.TextField("Name", dialogObj.name);
			SerializedObject so = new SerializedObject(dialogObj);
			dialogObj.canExit = EditorGUILayout.Toggle("Can Exit", dialogObj.canExit);
			dialogObj.startDialog = EditorGUILayout.IntField("Entry", dialogObj.startDialog);
			if (dialogObj.fallBacks == null)
				dialogObj.fallBacks = new int[0];
			if (fallbacks = EditorGUILayout.Foldout(options, "Fallbacks"))
			{
				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button("+"))
				{
					List<int>  fb = new List<int>(dialogObj.fallBacks);
					fb.Add(0);
					dialogObj.fallBacks = fb.ToArray();
				}
				if (GUILayout.Button("-"))
				{
					List<int> fb = new List<int>(dialogObj.fallBacks);
					if (fb.Count > 0)
						fb.Remove(fb[fb.Count - 1]);
					dialogObj.fallBacks = fb.ToArray();
				}
				EditorGUILayout.EndHorizontal();
				for (int i = 0; i < dialogObj.fallBacks.Length; i++)
				{
					dialogObj.fallBacks[i] = EditorGUILayout.IntField("ID" + i.ToString(), dialogObj.fallBacks[i]);
				}

			}
			EditorGUILayout.EndVertical();

		}
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.EndVertical();
		EditorUtility.SetDirty(dialogObj);
		base.OnGUI();
	}
}
