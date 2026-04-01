using DG.Tweening;
using System;
using UnityEngine;

public static class ScreenFader
{
	public static void OnFadeIn(CanvasGroup group, float duration, Action callback = null)
	{
		group.DOFade(1f, duration).OnComplete(() => callback?.Invoke());
	}

	public static void OnFadeOut(CanvasGroup group, float duration, Action callback = null)
	{
		group.DOFade(0f, duration).OnComplete(() => callback?.Invoke());
	}
}