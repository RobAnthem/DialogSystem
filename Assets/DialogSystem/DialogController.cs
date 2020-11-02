using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
public class DialogController : MonoBehaviour
{
	public enum DialogType
	{
		Text,
		Speech
	}
	public Text NPCText;
	public Text NPCName;
	public Text[] Responses;
	public GameObject exitButton;
	private DialogTrigger dialogTrigger;
	private string fullNPCString;
	public int placeInString;
	private int placeInDialog;
	public float durationPerText = .25f;
	public float audioInterval = .5f;
	public AudioClip writingClip;
	private float lastAudio;
	private float duration, currentTime;
	private static DialogController instance;
	public AudioSource dialogSource;
	public DialogType dialogType;
	public static DialogController Instance
	{
		get
		{
			if (instance == null)
				instance = FindObjectOfType<DialogController>();
			return instance;
		}
		set
		{
			instance = value;
		}
	}
	public bool isDialogConditionsMet(DialogObject.Dialog d)
	{
		bool met = true;
		if (d.conditions.isOneTime && DialogVars.Instance.GetValue(d.conditions.oneTimeID))
		{
			met = false;
		}


		if (!string.IsNullOrEmpty(d.conditions.varID))
		{
			met = DialogVars.Instance.GetValue(d.conditions.varID) == d.conditions.varState;
		}

		return met;
	}
	public bool isDialogConditionsMet(DialogObject.DialogResponse d)
	{
		bool met = true;
		if (d.conditions.isOneTime)
		{
			met = DialogVars.Instance.GetValue(d.conditions.oneTimeID) == false;
		}
		if (!string.IsNullOrEmpty(d.conditions.varID))
		{
			met = DialogVars.Instance.GetValue(d.conditions.varID) == d.conditions.varState;
		}

		return met;
	}
	public GameObject canvasObject;
	public void Init(DialogTrigger dt)
	{
		canvasObject.SetActive(true);
		NPCName.text = dt.NPCName;
		DialogCamera.Instance.SetDialogTarget(dt.targetLookObj);
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		dialogTrigger = dt;
		if (dialogTrigger.dialogObject.canExit)
			exitButton.SetActive(true);
		else
			exitButton.SetActive(false);
		int start = GetStartingDialog();
		if (start < 0)
		{
			CloseDialog();
		}
		else
		{
			SetDialogSection(start);
		}


	}
	public int GetDialogIndex(int section)
	{
		int index = 0;
		for (; index < dialogTrigger.dialogObject.dialog.Length; index++)
		{
			if (dialogTrigger.dialogObject.dialog[index].ID == section)
			{
				break;
			}
		}
		return index;
	}
	public void SetDialogSection(int section)
	{
		placeInDialog = GetDialogIndex(section);
		//NPCText.text = dialogTrigger.dialogObject.dialog[placeInDialog].text;
		StartTextSection(dialogTrigger.dialogObject.dialog[placeInDialog].text);
		if (dialogTrigger.dialogObject.dialog[placeInDialog].conditions.isOneTime)
		{
			DialogVars.Instance.SetVar(dialogTrigger.dialogObject.dialog[placeInDialog].conditions.oneTimeID, true);
		}
		int i = 0;
		for (; i < dialogTrigger.dialogObject.dialog[placeInDialog].responses.Length; i++)
		{
			if (!isDialogConditionsMet(dialogTrigger.dialogObject.dialog[placeInDialog].responses[i]))
			{
				Responses[i].gameObject.SetActive(false);
			}
			else
				Responses[i].gameObject.SetActive(true);
			if (dialogTrigger.dialogObject.dialog[placeInDialog].responses[i].nextID == 0 && dialogTrigger.dialogObject.dialog[placeInDialog].responses[i].eventID == 0)
			{
				//actionTexts[i].text = "{END DIALOG}";
			}
			else if (dialogTrigger.dialogObject.dialog[placeInDialog].responses[i].eventID > 0
				&& dialogTrigger.dialogObject.GetEvent(dialogTrigger.dialogObject.dialog[placeInDialog].responses[i].eventID).type == DialogObject.DialogEventType.CloseDialog)
			{
				//actionTexts[i].text = "{END DIALOG}";
			}
			else if (DoesChainEnd(i))
			{
				//actionTexts[i].text = "{END DIALOG}";
			}
			
			Responses[i].text = dialogTrigger.dialogObject.dialog[placeInDialog].responses[i].response; Responses[i].gameObject.SetActive(true);
		}
		for (; i < Responses.Length; i++)
		{
			Responses[i].gameObject.SetActive(false);
		}
		if (dialogTrigger.dialogObject.dialog[placeInDialog].hasEvent)
		{
			dialogTrigger.RunDialogEvent(dialogTrigger.dialogObject.dialog[placeInDialog].eventID);
		}
		dialogTrigger.animator.CrossFadeInFixedTime("Talk" + dialogTrigger.dialogObject.dialog[placeInDialog].TalkID.ToString(), .2f);
		dialogTrigger.PlayDialogClip(dialogTrigger.dialogObject.dialog[placeInDialog].clip);
	}
	public bool DoesChainEnd(int i)
	{
		if (dialogTrigger.dialogObject.dialog[placeInDialog].responses[i].eventID > 0)
		{
			DialogObject.DialogEvent dEvent = dialogTrigger.dialogObject.GetEvent(dialogTrigger.dialogObject.dialog[placeInDialog].responses[i].eventID);
			if (dEvent.nextDialogID > 0)
			{
				return false;
			}
			if (dEvent.nextEventID > 0 && dEvent.type != DialogObject.DialogEventType.CloseDialog)
			{
				DialogObject.DialogEvent dEvent2 = dialogTrigger.dialogObject.GetEvent(dEvent.nextEventID);
				if (dEvent2.nextDialogID > 0)
				{
					return false;
				}
				if (dEvent2.nextEventID > 0 && dEvent2.type != DialogObject.DialogEventType.CloseDialog)
				{
					DialogObject.DialogEvent dEvent3 = dialogTrigger.dialogObject.GetEvent(dEvent.nextEventID);
					if (dEvent3.nextDialogID > 0)
					{
						return false;
					}
					if (dEvent3.nextEventID > 0 || dEvent3.nextDialogID > 0 && dEvent3.type != DialogObject.DialogEventType.CloseDialog)
					{
						DialogObject.DialogEvent dEvent4 = dialogTrigger.dialogObject.GetEvent(dEvent.nextEventID);
						if (dEvent4.nextDialogID > 0)
						{
							return false;
						}
						if (dEvent4.nextEventID > 0 || dEvent4.nextDialogID > 0 && dEvent4.type != DialogObject.DialogEventType.CloseDialog)
						{
							return false;
						}
						else
						{
							return true;
						}
					}
					else
					{
						return true;
					}
				}
				else
				{
					return true;
				}
			}
			else
			{
				return true;
			}

		}
		return false;
	}
	public void SelectResponse(int i)
	{
		if (dialogTrigger.dialogObject.dialog[placeInDialog].responses[i].conditions.isOneTime)
		{
			DialogVars.Instance.SetVar(dialogTrigger.dialogObject.dialog[placeInDialog].responses[i].conditions.oneTimeID, true);
		}
		if (dialogTrigger.dialogObject.dialog[placeInDialog].responses[i].nextID > 0)
		{
			SetDialogSection(dialogTrigger.dialogObject.dialog[placeInDialog].responses[i].nextID);
		}
		else if (dialogTrigger.dialogObject.dialog[placeInDialog].responses[i].nextEventID > 0)
		{
			PerformDialogEvent(dialogTrigger.dialogObject.GetEvent(dialogTrigger.dialogObject.dialog[placeInDialog].responses[i].nextEventID));
		}
		else
		{
			if (dialogTrigger.dialogObject.dialog[placeInDialog].responses[i].eventID > 0)
			{
				dialogTrigger.RunDialogEvent(dialogTrigger.dialogObject.dialog[placeInDialog].responses[i].eventID);
			}
			if (dialogTrigger.dialogObject.dialog[placeInDialog].responses[i].nextID == -1)
			{
				CloseDialog();

			}
			else
			{
				CloseDialog();
			}
		}
			
	}
	public bool CanExit()
	{
		if (dialogTrigger)
			return dialogTrigger.dialogObject.canExit;
		else
			return true;
	}
	public void Exit()
	{
		dialogTrigger.animator.speed = 1;
	}
	public int GetStartingDialog()
	{
		int startD = -1;
		if (!isDialogConditionsMet(dialogTrigger.dialogObject.dialog[dialogTrigger.dialogObject.startDialog]))
		{
			foreach (DialogObject.Dialog dod in dialogTrigger.dialogObject.dialog)
			{
				if ((dialogTrigger.dialogObject.fallBacks.Contains(dod.ID) || dod.isEntry) && isDialogConditionsMet(dod))
				{
					startD = dod.ID;
					break;
				}
			}
		}
		else
		{
			startD = dialogTrigger.dialogObject.startDialog;
		}
		return startD;
	}

	void Update()
	{
		if (dialogType == DialogType.Text && TextInAction)
		{
			UpdateDialogSection();
			if (Input.GetMouseButtonDown(0))
			{
				TextInAction = false;
				NPCText.text = fullNPCString;
				dialogTrigger.StopLipsyncText();
				if (dialogSource.isPlaying)
					dialogSource.Stop();
			}
		}


	}
	public void PerformDialogEvent(DialogObject.DialogEvent dEvent)
	{
		if (dEvent.type == DialogObject.DialogEventType.SetVar)
		{
			DialogVars.Instance.SetVar(dEvent.stringField, dEvent.boolField);
		}
		else if (dEvent.type == DialogObject.DialogEventType.CloseDialog)
		{
			
			CloseDialog();
			return;
		}


		if (dEvent.nextEventID > 0)
		{
			PerformDialogEvent(dialogTrigger.dialogObject.GetEvent(dEvent.nextEventID));
		}
		else if (dEvent.nextDialogID > 0)
		{
			SetDialogSection(dEvent.nextDialogID);
		}
	
	}
	public void CloseDialog()
	{
		canvasObject.SetActive(false);
		DialogCamera.Instance.StopDialog();
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		if (dialogSource.isPlaying)
			dialogSource.Stop();
		dialogTrigger.SetDialogState(false);
	}
	private bool TextInAction;
	private void Awake()
	{
		Instance = this;
	}
	public void StartTextSection(string text)
	{
		if (dialogType == DialogType.Text)
		{
			fullNPCString = text;
			currentTime = 0;
			placeInString = 0;
			duration = durationPerText * fullNPCString.Length;
			dialogTrigger.PlayStringLipsynced(text, duration);
			TextInAction = true;
			if (!dialogSource.isPlaying)
				dialogSource.Play();
			lastAudio = 0;
		}
		else
		{
			NPCText.text = text;
		}
	}
	void UpdateDialogSection()
	{
		int index = (int)Mathf.Lerp(0, fullNPCString.Length, Mathf.InverseLerp(0, duration, placeInString * durationPerText));
		string s = fullNPCString.Substring(0, index);
		currentTime += Time.deltaTime;
		if (currentTime >= durationPerText)
		{
			currentTime -= durationPerText;
			placeInString++;
		}
		lastAudio += Time.deltaTime;
		if (lastAudio >= audioInterval)
		{
			lastAudio = 0;
			dialogSource.PlayOneShot(writingClip);
		}
		if (placeInString * durationPerText >= duration)
		{
			s = fullNPCString;
			TextInAction = false;
			if (dialogSource.isPlaying)
				dialogSource.Stop();
		}
		NPCText.text = s;

	}
}
