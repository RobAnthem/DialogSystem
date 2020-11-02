using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogCamera : MonoBehaviour
{
	public static DialogCamera Instance
	{
		get
		{
			if (m_instance == null)
				m_instance = FindObjectOfType<DialogCamera>();
			return m_instance;
		}
		private set
		{
			m_instance = value;
		}
	}

	private static DialogCamera m_instance;
	public UnityStandardAssets.Characters.FirstPerson.FirstPersonController controller;
	public KeyCode interactKey = KeyCode.E;
	public LayerMask interactLayers;
	public float interactDistance = 2.0f;
	private bool inDialog;
	private Transform dialogTarget;
	private void Awake()
	{
		Instance = this;
	}
	private void Update()
	{
		if (Input.GetKey(interactKey))
		{
			RaycastHit hit;
			Ray ray = new Ray(transform.position, transform.forward);
			//Debug.Log("found object = " +
			if (Physics.Raycast(ray, out hit, interactDistance, interactLayers))
			{
				Interactable i;
				if (i = hit.transform.GetComponent<Interactable>())
				{
					i.Interact();
				}
			}
		}
	}
	private void LateUpdate()
	{
		if (inDialog)
		{
			transform.LookAt(dialogTarget);
			return;
		}
	}
	public void SetDialogTarget(Transform t)
	{
		controller.enabled = false;
		dialogTarget = t;
		inDialog = true;
	}
	public void StopDialog()
	{
		controller.enabled = true;
		inDialog = false;
	}
}
