using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Animator))]
public class Door : MonoBehaviour
{
	private Animator _animator;

	[SerializeField] private bool _locked = true;
	private bool _opened;

	[SerializeField] private Collider2D _collider;

	public void Open()
	{
		this._opened = true;

		this._animator.SetTrigger("Open");
	}

	public void DisableCollider()
	{
		this._collider.enabled = false;
	}

	public void EnableCollider()
	{
		this._collider.enabled = true;
	}

	public void Close()
	{
		this._opened = false;

		this._animator.SetTrigger("Close");
	}

	public void Toggle()
	{
		if (this._opened)
			this.Close();
		else
			this.Open();
	}

	[SerializeField] private InventoryItemSharedData _unlockingItemSharedData;

	public void CheckForInput()
	{
		if (Input.GetKeyDown(KeyCode.E))
		{
			if (!this._locked)
			{
				this.Toggle();
			}
			else if (GameManager.Instance.PlayerInventory_.Remove(this._unlockingItemSharedData.Id))
			{
				this._locked = false;

				this.Toggle();
			}
		}
	}

	private void Awake()
	{
		this._animator = this.GetComponent<Animator>();
	}

#if UNITY_EDITOR
	private void OnDrawGizmosSelected()
	{
		if (this._animator == null)
		{
			this._animator = this.GetComponent<Animator>();
		}

		this._animator.Update(Time.deltaTime);
	}
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(Door))]
[CanEditMultipleObjects]
public class DoorEditor : Editor
{
#pragma warning disable 0219, 414
	private Door _sDoor;
#pragma warning restore 0219, 414

	private Animator _sAnimator;

	private void OnEnable()
	{
		this._sDoor = this.target as Door;

		this._sAnimator = this._sDoor.GetComponent<Animator>();
	}

	public override void OnInspectorGUI()
	{
		this.DrawDefaultInspector();

		if (GUILayout.Button("Open"))
		{
			this._sAnimator.SetTrigger("Open");
		}

		if (GUILayout.Button("Close"))
		{
			this._sAnimator.SetTrigger("Close");
		}
	}
}
#endif