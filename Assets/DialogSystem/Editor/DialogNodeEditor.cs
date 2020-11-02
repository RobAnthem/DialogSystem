using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using XNode;
[CustomEditor(typeof(DialogNode))]
public class DialogNodeEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DialogNode dn = target as DialogNode;
		SerializedObject so = new SerializedObject(dn);
		so.FindProperty("Input");
	}
}
