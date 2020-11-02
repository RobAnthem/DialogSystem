using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
[CreateAssetMenu]
public class DialogGraph : NodeGraph {
	public DialogObject dialogObject
	{
		get
		{
			if (m_dialog == null)
				m_dialog = Resources.Load<DialogObject>(name);
			return m_dialog;
		}
		set
		{
			m_dialog = value;
		}
	}
	private DialogObject m_dialog;
	public string Name;
	public override void RemoveNode(Node node)
	{
		base.RemoveNode(node);
	}
	public override Node AddNode(Type type)
	{

		Node.graphHotfix = this;
		Node node = ScriptableObject.CreateInstance(type) as Node;
		node.graph = this;
		nodes.Add(node);
		if (type == typeof(DialogNode))
		{
			DialogNode dNode = node as DialogNode;
			dNode.dialogID = AddDialog();
			dNode.dialog = dNode.GetObject();
			dNode.Output0 = 0;
			dNode.Output1 = 1;
			dNode.Output2 = 2;
			dNode.Output3 = 3;
			dNode.Output4 = 4;
		}
		else if (type == typeof(DialogEventNode))
		{
			DialogObject.DialogEvent dEvent = new DialogObject.DialogEvent();
			dEvent.id = dialogObject.events.Count + UnityEngine.Random.Range(0, 100000);
			dialogObject.events.Add(dEvent);
			DialogEventNode dEventNode = node as DialogEventNode;
			dEventNode.dialogObjectRef = dialogObject;
			dEventNode.dialogEventID = dEvent.id;
		}
		return node;
	}
	public int AddDialog()
	{
		if (dialogObject.dialog == null)
			dialogObject.dialog = new DialogObject.Dialog[0];
		int i = dialogObject.dialog.Length;
		List<DialogObject.Dialog> dialogs = new List<DialogObject.Dialog>(dialogObject.dialog);
		DialogObject.Dialog d = new DialogObject.Dialog();
		d.ID = i;
		dialogs.Add(d);
		dialogObject.dialog = dialogs.ToArray();
		return i;
	}
	public DialogObject.Dialog GetDialog(int id)
	{
		return dialogObject.dialog[id];
	}
}