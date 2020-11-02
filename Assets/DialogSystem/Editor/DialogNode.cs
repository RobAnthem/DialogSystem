using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class DialogNode : Node {
	[HideInInspector]
	public int dialogID;
	[HideInInspector]
	public DialogObject.Dialog dialog;
	[Input(ShowBackingValue.Never)] public float Input;
	[Output(ShowBackingValue.Never), InspectorName("Out")] public int Output0 = 0;
	[Output(ShowBackingValue.Never), InspectorName("Out")] public int Output1 = 1;
	[Output(ShowBackingValue.Never), InspectorName("Out")] public int Output2 = 2;
	[Output(ShowBackingValue.Never), InspectorName("Out")] public int Output3 = 3;
	[Output(ShowBackingValue.Never), InspectorName("Out")] public int Output4 = 4;
	[HideInInspector]
	public List<bool> showOutputs {
		get
		{
			if (outputsShown == null || outputsShown.Count < 5)
			{
				outputsShown = new List<bool>();
				for (int i = 0; i < 5; i++)
					outputsShown.Add(false);
			}
			return outputsShown;
		}
		set
		{
			outputsShown = value;
		}
	}
	private List<bool> outputsShown;
	[HideInInspector]
	public int selectedResponse
	{
		get
		{
			return m_selected;
		}
		set
		{
			if (value != m_selected)
			{
				responseOptions = false;
				conditionOptions = false;
				showVar = false;
			}
			m_selected = value;
		}
	}
	private int m_selected;
	[HideInInspector]
	public bool Options;
	[HideInInspector]
	public bool responseOptions, conditionOptions, showVar, mainDialogEvent, showDialogText, showDialogCondition,
		showDialogVar;
	public DialogObject.Dialog GetObject()
	{
		return (graph as DialogGraph).GetDialog(dialogID);
	}
	public void SetObject()
	{
		(graph as DialogGraph).dialogObject.dialog[dialogID] = dialog;
	}
	public void AddResponse()
	{
		if (GetObject().responses.Length > 4)
			return;
		List<DialogObject.DialogResponse> responses = new List<DialogObject.DialogResponse>(dialog.responses);
		responses.Add(new DialogObject.DialogResponse());
		dialog.responses = responses.ToArray();
		//GetObject().responses = dialog.responses;
		SetObject();
	}
	public void RemoveResponse()
	{
		if (GetObject().responses.Length < 1)
			return;
		List<DialogObject.DialogResponse> responses = new List<DialogObject.DialogResponse>(dialog.responses);
		responses.Remove(responses[responses.Count - 1]);
		dialog.responses = responses.ToArray();
		SetObject();
	}
	// Use this for initialization
	protected override void Init() {
		base.Init();
		
	}

	public void SetVar(object vObj)
	{
		dialog.responses[selectedResponse].conditions.varID = vObj as string;
		SetObject();
	}
	public void SetDialogVar(object vObj)
	{
		dialog.conditions.varID = vObj as string;
		SetObject();
	}
	public override List<string> GetExclusions()
	{
		List<string> exclusions = new List<string>();
		if (dialog.responses == null)
			dialog.responses = new DialogObject.DialogResponse[0];
		exclusions.Add("Output4");
		exclusions.Add("Output3");
		exclusions.Add("Output2");
		exclusions.Add("Output1");
		exclusions.Add("Output0");
		return exclusions;
	}
	public List<string> GetExclusions(int id)
	{
		List<string> exclusions = new List<string>();
		if (dialog.responses == null)
			dialog.responses = new DialogObject.DialogResponse[0];
		if (id != 4)
		{
			exclusions.Add("Output4");
			//exclusions.Add("Response4");
		}
		if (id != 3)
		{
			exclusions.Add("Output3");
			//exclusions.Add("Response3");
		}
		if (id != 2)
		{
			exclusions.Add("Output2");
			//exclusions.Add("Response2");
		}
		if (id != 1)
		{
			exclusions.Add("Output1");
			//exclusions.Add("Response1");
		}
		if (id != 0)
		{
			exclusions.Add("Output0");
			//exclusions.Add("Response0");
		}
		return exclusions;
	}
	public override void onUpdate()
	{
		SetObject();
		base.onUpdate();
	}
	// Return the correct value of an output port when requested
	public override object GetValue(NodePort port) {
		if (port.IsOutput)
		{
			//Debug.Log(port.fieldName);
			switch (port.fieldName)
			{
				case "Output0":
					return 0;
				case "Output1":
					return 1;
				case "Output2":
					return 2;
				case "Output3":
					return 3;
				case "Output4":
					return 4;
				default:
					return 0;
			}
		}
		else
			return port.fieldName;
	}
	public override void OnCreateConnection(NodePort from, NodePort to)
	{
		DialogNode dN = from.node as DialogNode;
		DialogNode dN2 = to.node as DialogNode;
		System.Type portType = from.ValueType;
		//string tooltip = "";
		//tooltip = portType.PrettyName();
		if (from.IsOutput)
		{
			object obj = from.node.GetValue(from);
			Debug.Log(obj);
			if (from.node is DialogNode && to.node is DialogNode)
			{
				(from.node as DialogNode).SetResponse((int)obj, (to.node as DialogNode).dialogID);
			}
			else if (from.node is DialogEventNode && to.node is DialogNode)
			{
				(from.node as DialogEventNode).dialogEvent.nextDialogID = (to.node as DialogNode).dialogID;
			}
			else if (to.node is DialogEventNode && from.node is DialogNode)
			{
				(from.node as DialogNode).SetEvent((int)obj, (to.node as DialogEventNode).dialogEventID);
			}
		}
		base.OnCreateConnection(from, to);
	}
	public void SetResponse(int res, int d)
	{
		dialog.responses[res].nextID = d;
		SetObject();
	}
	public void SetEvent(int res, int d)
	{
		dialog.responses[res].nextEventID = d;
		SetObject();
	}
	public override void OnRemoveConnection(NodePort port)
	{
		if (port.IsOutput)
		{
			if (port.node is DialogNode)
			{
				object obj = port.node.GetValue(port);
				(port.node as DialogNode).dialog.responses[(int)obj].nextID = 0;
				(port.node as DialogNode).dialog.responses[(int)obj].nextEventID = 0;
			}
			else if (port.node is DialogEventNode)
			{
				(port.node as DialogEventNode).dialogEvent.nextDialogID = 0;
				(port.node as DialogEventNode).dialogEvent.nextEventID = 0;
			}

		}
		base.OnRemoveConnection(port);
	}
}