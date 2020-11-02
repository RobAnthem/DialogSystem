using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTrigger : Interactable
{
	public enum DialogState
	{
		Idle,
		RotatingToTarget,
		Talking,
		RotatingToIdle
	}
	private DialogState cState;
	public DialogObject dialogObject;
	public AudioSource source;
	public string NPCName;
	public Animator animator;
	public List<UnityEngine.Events.UnityEvent> dialogEvents;
	private bool dialogComplete;
	public string requiredVar;
	public string requiredVarName;
	public bool requiredValue;
	public GameObject[] enableAbles;
	public Transform targetLookObj;
	public LipsyncController lipsycController;
	private Vector3 originalForward;
	public float rotationSpeed = 30;
	public bool RotateToTarget;
	public float interactAngle = 180;
	void Awake()
	{
		lipsycController = GetComponent<LipsyncController>();
		originalForward = transform.forward;
	}
	public void PlayDialogClip(AudioClip clip)
	{
		if (lipsycController)
			lipsycController.PlaySound(clip);
		else
		{
			source.Stop();
			source.clip = clip;
			source.Play();
		}
	}

	public void RunDialogEvent(int i)
	{
		dialogEvents[i].Invoke();
	}
	public void CompleteDialog()
	{
		dialogComplete = true;
	}
	public override void Interact()
	{
		if (dialogComplete || (!string.IsNullOrEmpty(requiredVar)))//requiredValue != MissionController.Instance.GetVariableValue(requiredVar)))
			return;
		if (Vector3.Angle(transform.forward, DialogCamera.Instance.transform.position - transform.position) > interactAngle)
			return;
		DialogController.Instance.Init(this);
		SetDialogState(true);
	}
	public void PlayStringLipsynced(string s, float duration)
	{
		if (lipsycController)
			lipsycController.GetTextFreq(s, duration);
	}
	public void StopLipsyncText()
	{
		if (lipsycController)
			lipsycController.StopText();
	}
	public void PlayClipWithAnimation(int id)
	{
		PlayDialogClip(dialogObject.dialog[id].clip);
		animator.CrossFadeInFixedTime("Talk" + dialogObject.dialog[id].TalkID.ToString(), .2f);
	}

	public void SetVarTrue(string var)
	{
		DialogVars.Instance.SetVar(var, true);
	}
	public void SetVarFalse(string var)
	{
		DialogVars.Instance.SetVar(var, false);
	}
	public void EnableObject(int id)
	{
		enableAbles[id].SetActive(true);
	}
	private bool inDIalog;
	public void SetDialogState(bool pState)
	{
		inDIalog = pState;
		if (!RotateToTarget)
			return;
		if (pState)
			cState = DialogState.RotatingToTarget;
		else
			cState = DialogState.RotatingToIdle;
	}
	public void DisableObject(int id)
	{
		enableAbles[id].SetActive(false);
	}
	private void Update()
	{
		if (inDIalog)
		{
			if (cState == DialogState.RotatingToTarget)
			{
				Vector3 playerPos = DialogCamera.Instance.transform.position;
				playerPos.y = transform.position.y;
				Vector3 direction = playerPos - transform.position;
				if (Vector3.Angle(transform.forward, playerPos - transform.position) > 3.0f)
				{
					direction = direction.normalized;
					transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
				
				}
				else
				{
					cState = DialogState.Talking;
				
				}
			}
		}
		else if (cState == DialogState.Idle)
			return;
		else if (cState == DialogState.RotatingToIdle)
		{
			Vector3 playerPos = DialogCamera.Instance.transform.position;
			playerPos.y = transform.position.y;
			Vector3 direction = playerPos - transform.position;
			if (Vector3.Angle(transform.forward, originalForward) > 3.0f)
			{
				direction = originalForward - transform.forward;
				transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(originalForward), rotationSpeed * Time.deltaTime);
			
			}
			else
			{
				cState = DialogState.Idle;

			}
		}
	}
}
