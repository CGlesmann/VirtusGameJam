/* Created by Luna.Ticode */

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

[UnityEngine.RequireComponent(typeof(Collider2D))]
public class InteractionTrigger2D : MonoBehaviour
{
	[SerializeField] private LayeredEventTriggerData[] _onTriggerEnterData;
	[SerializeField] private LayeredEventTriggerData[] _onTriggerStayData;
	[SerializeField] private LayeredEventTriggerData[] _onTriggerExitData;

	private Coroutine _onTriggerStayProcess;

	private IEnumerator OnTriggerStayProcess2D(Collider2D other)
	{
		while (true)
		{
			for (int i = 0; i < this._onTriggerStayData.Length; i++)
			{
				if (this._onTriggerStayData[i]._LayerMask.Contains(other.gameObject))
					this._onTriggerStayData[i]._UnityEvent.Invoke();
			}

			yield return null;
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		for (int i = 0; i < this._onTriggerEnterData.Length; i++)
		{
			if (this._onTriggerEnterData[i]._LayerMask.Contains(other.gameObject))
				this._onTriggerEnterData[i]._UnityEvent.Invoke();
		}

		this._onTriggerStayProcess = this.StartCoroutine(this.OnTriggerStayProcess2D(other));
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		for (int i = 0; i < this._onTriggerExitData.Length; i++)
		{
			if (this._onTriggerExitData[i]._LayerMask.Contains(other.gameObject))
				this._onTriggerExitData[i]._UnityEvent.Invoke();
		}

		this.StopCoroutine(this._onTriggerStayProcess);
	}

#if UNITY_EDITOR
	private const float GIZMO_SIZE_BIAS = 0.1f;

	private BoxCollider2D _defaultCollider;

	[ContextMenu("Reset Collision Data")]
	public void CheckForTriggerColliders()
	{
		foreach (Collider2D collider in this.GetComponents<Collider2D>())
		{
			if (collider.isTrigger)
			{
				this._defaultCollider = collider as BoxCollider2D;
				return;
			}
		}

		this._defaultCollider = this.gameObject.AddComponent<BoxCollider2D>();
		this._defaultCollider.isTrigger = true;
	}

	[Header("Unity Editor Only")]
	[SerializeField] private Vector2 _gizmoSize = Vector2.one;

	private bool _triedToFindDefaultCollider;

	protected virtual void OnDrawGizmos()
	{
		if (!this._triedToFindDefaultCollider)
		{
			this.CheckForTriggerColliders();

			this._triedToFindDefaultCollider = true;
		}

		Color color = Color.cyan;
		color.a = 0.2f;
		Gizmos.color = color;

		if (this._defaultCollider != null)
		{
			Gizmos.DrawCube(this.transform.position + Vector3.Scale(this._defaultCollider.offset, this.transform.localScale), Vector2.Scale(this._defaultCollider.size, this.transform.localScale) + Vector2.one * GIZMO_SIZE_BIAS);
		}
		else
		{
			Gizmos.DrawCube(this.transform.position, this._gizmoSize + Vector2.one * GIZMO_SIZE_BIAS);
		}
	}
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(InteractionTrigger2D))]
[CanEditMultipleObjects]
public class InteractionTrigger2DEditor : Editor
{
	private void OnEnable()
	{
		InteractionTrigger2D sInteractionTrigger2D = this.target as InteractionTrigger2D;

		sInteractionTrigger2D.CheckForTriggerColliders();
	}

	public override void OnInspectorGUI()
	{
		this.DrawDefaultInspector();

#pragma warning disable 0219
		InteractionTrigger2D sInteractionTrigger2D = this.target as InteractionTrigger2D;
#pragma warning restore 0219

		foreach (Collider2D collider in sInteractionTrigger2D.GetComponents<Collider2D>())
		{
			if (collider.isTrigger)
			{
				return;
			}
		}

		EditorGUILayout.HelpBox("There is no trigger collider attached. Events might not trigger", MessageType.Warning); //TODO: Search for colliders recursively
	}
}
#endif