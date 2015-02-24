using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

class GameCanvas
{
	private readonly GameObject _bubbleSample;
	private readonly List<GameObject> _bubbles = new List<GameObject>();
	private readonly List<GameObject> _pool = new List<GameObject>();

	private readonly Image _timerProgress;
	private readonly Text _scores;
	private readonly Text _timer;

	public float BaseSize { get; private set; }


	public GameCanvas(GameObject canvas)
	{
		var bubble = GetChild<Image>(canvas, "bubble");
		if (bubble != null) {
			BaseSize = bubble.GetComponent<RectTransform>().sizeDelta.x;
			_bubbleSample = bubble.gameObject;
			_bubbleSample.SetActive(false);
		}

		_timerProgress = GetChild<Image>(canvas, "pb_timer");
		_scores = GetChild<Text>(canvas, "text_scores");
		_timer = GetChild<Text>(canvas, "text_timer");
	}

	public static T GetChild<T>(GameObject parent, string n) where T : Component
	{
		var children = parent.GetComponentsInChildren<T>();
		return children.FirstOrDefault(c => c.name == n);
	}

	private void CreateBubble()
	{
		if (_pool.Count > 0) { // create bubble from pool
			_bubbles.Add(_pool[0]);
			_pool[0].SetActive(true);
			_pool.RemoveAt(0);
		}
		else { // create new bubble gameObject
			_bubbleSample.SetActive(true);
			var clone = (UnityEngine.Object.Instantiate(_bubbleSample) as GameObject);
			if (clone != null) {
				clone.transform.SetParent(_bubbleSample.transform.parent, false);
				clone.transform.localScale = _bubbleSample.transform.localScale;
				clone.transform.position = _bubbleSample.transform.position;
			}
			_bubbles.Add(clone);
			_bubbleSample.SetActive(false);
		}
	}

	public void UpdateBubbles(IList<Bubble> bubbles)
	{
		int count = Math.Max(bubbles.Count, _bubbles.Count);
		for (int i = 0; i < count; ++i) {
			if (i >= bubbles.Count) { //hide unused bubble gameObjects and move it to pool
				_pool.Add(_bubbles[i]);
				_bubbles[i].SetActive(false);
				continue;
			}
			if (i == _bubbles.Count)
				CreateBubble();

			UpdateBubble(_bubbles[i], bubbles[i]);
		}
		if (_bubbles.Count > bubbles.Count) // remove unused bubbles
			_bubbles.RemoveRange(bubbles.Count, _bubbles.Count - bubbles.Count);
	}

	private void UpdateBubble(GameObject image, Bubble bubble)
	{
		var pos = image.transform.position;
		image.transform.position = new Vector3(Screen.width * bubble.PositionX, Screen.height * bubble.PositionY, pos.z);
		image.transform.localScale = new Vector3(1, 1, 1) * bubble.Size;
		image.GetComponent<Image>().color = bubble.Color;
	}

	public int GetIndexOf(GameObject obj)
	{
		return _bubbles.IndexOf(obj);
	}

	public void SetScores(int scores)
	{
		if (_scores != null)
			_scores.text = scores.ToString();
	}

	public void SetTimer(float tmVal, float pbVal)
	{
		if (_timer != null)
			_timer.text = ((int) (tmVal + 0.5f)).ToString();
		if (_timerProgress != null)
			_timerProgress.fillAmount = pbVal;
	}
}
