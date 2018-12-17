using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.IMGUI;
#endif

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AdvancedButton : Button
{
	[SerializeField] private UnityEvent _onPointerEnter;
	public UnityEvent _OnPointerEnter { get { return this._onPointerEnter; } }

	[SerializeField] private UnityEvent _onPointerExit;
	public UnityEvent _OnPointerExit { get { return this._onPointerExit; } }

	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);

		this._onPointerEnter.Invoke();
	}

	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);

		this._onPointerExit.Invoke();
	}

	[SerializeField] private UnityEvent _onPointerDown;
	public UnityEvent _OnPointerDown { get { return this._onPointerDown; } }

	[SerializeField] private UnityEvent _onPointerUp;
	public UnityEvent _OnPointerUp { get { return this._onPointerUp; } }

	public override void OnPointerDown(PointerEventData eventData)
	{
		base.OnPointerDown(eventData);

		this._onPointerDown.Invoke();
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		base.OnPointerUp(eventData);

		this._onPointerUp.Invoke();
	}
}