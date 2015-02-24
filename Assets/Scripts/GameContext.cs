using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class GameContext : MonoBehaviour
{
	private const float CreationDelay = 0.3f;
	private const float LevelupDelay = 20.0f;

	private readonly List<Bubble> _bubles = new List<Bubble>();
	private int _bubblesLimit = 5;

	private float _createNewBubbleDelay = CreationDelay;

	private GameCanvas _gameCanvas;
	private float _borders = 0.0f;

	private int _scores = 0;
	private float _levelupTimer = LevelupDelay;
	private float _timeScale;

	// Use this for initialization
	void Start()
	{
		_gameCanvas = new GameCanvas(GetComponent<Canvas>().gameObject);
		_borders = _gameCanvas.BaseSize * 0.5f / Screen.width;
		_bubles.Add(new Bubble(_borders));

		StartGame();
	}

	public void StartGame()
	{
		_scores = 0;
		_levelupTimer = LevelupDelay;
		_timeScale = 1.0f;
		UpdateGui();
	}

	private void CreateNewBubblesIfNeeded()
	{
		if (_bubles.Count < _bubblesLimit) {
			_createNewBubbleDelay -= Time.deltaTime * _timeScale;
			if (_createNewBubbleDelay < 0) {
				_createNewBubbleDelay = CreationDelay;
				_bubles.Add(new Bubble(_borders));
			}
		}
	}

	private void UpdateBubblesPos()
	{
		var bubbleBaseSize = _gameCanvas.BaseSize * 0.5f / Screen.height;
		var needRemove = new List<Bubble>();
		foreach (Bubble bubble in _bubles) {
			bubble.UpdatePos(Time.deltaTime * _timeScale);
			if (bubble.PositionY < -bubbleBaseSize)
				needRemove.Add(bubble);
		}
		foreach (var b in needRemove) {
			_bubles.Remove(b);
		}
		_gameCanvas.UpdateBubbles(_bubles);
	}

	private void CheckInput()
	{
		if (Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject()) {
			var pe = new PointerEventData(EventSystem.current) { position = Input.mousePosition };

			var hits = new List<RaycastResult>();
			EventSystem.current.RaycastAll(pe, hits);

			if (hits.Count > 0 && hits[0].gameObject.name.StartsWith("bubble")) {
				int idx = _gameCanvas.GetIndexOf(hits[0].gameObject);
				if (idx >= 0) {
					_scores += _bubles[idx].Scores;
					_bubles.RemoveAt(idx);
				}
			}
		}
	}

	private void UpdateLevelupTimer()
	{
		_levelupTimer -= Time.deltaTime;
		if (_levelupTimer < 0) {
			_levelupTimer += LevelupDelay;
			_timeScale *= 1.3f;
			_bubblesLimit += 5;
		}
	}

	private void UpdateGui()
	{
		_gameCanvas.SetScores(_scores);
		_gameCanvas.SetTimer(_levelupTimer, _levelupTimer / LevelupDelay);
	}

	// Update is called once per frame
	void Update()
	{
		CreateNewBubblesIfNeeded();
		CheckInput();
		UpdateBubblesPos();
		UpdateLevelupTimer();
		UpdateGui();
	}
}
