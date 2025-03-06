using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Handles individual puzzle piece interactions including dragging, dropping, and placement validation.
/// </summary>
public class PuzzlePiece : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public static  Action OnPiecePlacedCorrectly;

    private RectTransform _rectTransform;
    private Vector3 _correctPosition;
    private bool _isPlacedCorrectly = false;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        ShowOutline(true);
    }

    /// <summary>
    /// Assigns the correct position for the piece.
    /// </summary>
    public void SetCorrectPosition(Vector3 position)
    {
        _correctPosition = position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isPlacedCorrectly)
        {
            _rectTransform.position += (Vector3)eventData.delta;
            AudioManager.OnPlayAudio?.Invoke(SoundType.Drag);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (Vector3.Distance(_rectTransform.position, _correctPosition) < 20f) // Snap range
        {
            AudioManager.OnPlayAudio?.Invoke(SoundType.DropCorrect);
            MarkAsCorrectlyPlaced();
            OnPiecePlacedCorrectly?.Invoke();
        }
        else
        {
            StartCoroutine(ShakePiece());
            AudioManager.OnPlayAudio?.Invoke(SoundType.DropIncorrect);
            ShowOutline(true);
        }
    }

    /// <summary>
    /// Returns whether the piece is correctly placed.
    /// </summary>
    public bool IsCorrectlyPlaced()
    {
        return _isPlacedCorrectly;
    }

    /// <summary>
    /// Displays or hides the outline around the piece.
    /// </summary>
    public void ShowOutline(bool show)
    {
        Outline outline = GetComponent<Outline>() ?? gameObject.AddComponent<Outline>();
        outline.effectColor = Color.red;
        outline.effectDistance = new Vector2(3, 3);
        outline.enabled = show;
    }

    /// <summary>
    /// Marks the piece as correctly placed and disables interaction.
    /// </summary>
    public void MarkAsCorrectlyPlaced()
    {
        _rectTransform.position = _correctPosition;
        _isPlacedCorrectly = true;
        ShowOutline(false);
        
    }

    /// <summary>
    /// Plays a shake animation when a piece is placed incorrectly.
    /// </summary>
    private IEnumerator ShakePiece()
    {
        Vector3 originalPosition = _rectTransform.position;
        float duration = 0.3f;
        float magnitude = 10f;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            _rectTransform.position = originalPosition + new Vector3(
                UnityEngine.Random.Range(-magnitude, magnitude),
                UnityEngine.Random.Range(-magnitude, magnitude),
                0
            );
            elapsed += Time.deltaTime;
            yield return null;
        }

        _rectTransform.position = originalPosition;
    }
}
