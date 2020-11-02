using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class DialogEventNode : Node {

	[Input(ShowBackingValue.Never)] public int Input;
	[Output(ShowBackingValue.Never), InspectorName("Out")] public int Output = 0;
	// Use this for initialization
	protected override void Init() {
		base.Init();
		
	}
	[HideInInspector]
	public DialogObject dialogObjectRef;
	public DialogObject.DialogEvent dialogEvent
	{
		get
		{
			return (dialogObjectRef.GetEvent(dialogEventID));
		}
	}
	public int dialogEventID;
	// Return the correct value of an output port when requested
	public override object GetValue(NodePort port) {
		return dialogEventID;
	}
	public override void OnCreateConnection(NodePort from, NodePort to)
	{
		if (from.IsOutput)
		{
			if (from.node is DialogEventNode && to.node is DialogEventNode)
			{
				object obj = to.node.GetValue(to);
				(from.node as DialogEventNode).dialogEvent.nextEventID = (int)obj;
			}

		}
		base.OnCreateConnection(from, to);
	}
	public override void OnRemoveConnection(NodePort port)
	{
		if (port.IsOutput)
		{
			if (port.node is DialogEventNode)
			{
				(port.node as DialogEventNode).dialogEvent.nextEventID = 0;
			}

		}
		base.OnRemoveConnection(port);
	}
	public void SetStringField(object obj)
	{
		dialogEvent.stringField = (string)obj;
	}
	public void SetIntField(object obj)
	{
		dialogEvent.intField = (int)obj;
	}
	public void SetItemField(object iObj)
	{
		//MLSpace.PhysicsInventoryItem item = (MLSpace.PhysicsInventoryItem)iObj;
		//SetStringField(item.name);
		Debug.Log("set item field");
	}
}