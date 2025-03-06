using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Manages the overall puzzle mechanics, including game completion checks and piece interactions.
/// </summary>
public class PuzzleManager : MonoBehaviour
{
    public static  Action OnGameCompleted;

    [SerializeField] private List<GameObject> puzzlePieces;
    [SerializeField] private Transform puzzleContainer;
    [SerializeField] private Transform basePositionsContainer;
    [SerializeField] private GameObject winPanel;

    private List<PuzzlePiece> _pieces = new List<PuzzlePiece>();
    private bool _isGameOver = false;
    private void Awake()
    {
        PuzzlePiece.OnPiecePlacedCorrectly += CheckCompletion;
    }
    
    public bool GetGameOverState()
    {
        return _isGameOver;
    }

    public List<PuzzlePiece> GetPuzzlePieces()
    {
        return _pieces;
    }
    private void Start()
    {
        InitializePuzzle();
    }

    /// <summary>
    /// Initializes the puzzle pieces and their correct positions.
    /// </summary>
    private void InitializePuzzle()
    {
        List<Vector3> correctPositions = new List<Vector3>();

        for (int i = 0; i < basePositionsContainer.childCount; i++)
        {
            correctPositions.Add(basePositionsContainer.GetChild(i).position);
        }

        foreach (GameObject piecePrefab in puzzlePieces)
        {
            GameObject pieceInstance = Instantiate(piecePrefab, puzzleContainer);
            PuzzlePiece piece = pieceInstance.GetComponent<PuzzlePiece>();

            // Randomize placement within puzzle area
            pieceInstance.transform.position = GetRandomPositionWithinContainer();

            piece.SetCorrectPosition(correctPositions[_pieces.Count]);
            _pieces.Add(piece);
        }

        SaveLoadHandler slh = GetComponent<SaveLoadHandler>();
        slh.Initialize();
        slh.LoadGame();
    }

    /// <summary>
    /// Checks if all pieces are correctly placed and triggers the win state if true.
    /// </summary>
    private void CheckCompletion()
    {
        bool isComplete = _pieces.TrueForAll(piece => piece.IsCorrectlyPlaced());

        if (isComplete)
        {
            _isGameOver = true;
            AudioManager.OnPlayAudio?.Invoke(SoundType.Win);
            OnGameCompleted?.Invoke();
            winPanel.SetActive(true);
        }
    }

    /// <summary>
    /// Generates a random position within the puzzle container's boundaries.
    /// </summary>
    private Vector3 GetRandomPositionWithinContainer()
    {
        RectTransform rectTransform = puzzleContainer.GetComponent<RectTransform>();
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        return new Vector3(
            UnityEngine.Random.Range(corners[0].x + 5, corners[2].x - 5),
            UnityEngine.Random.Range(corners[0].y + 5, corners[2].y - 5),
            0
        );
    }

    private void OnDisable()
    {
        PuzzlePiece.OnPiecePlacedCorrectly -= CheckCompletion;
    }

    public void ReloadGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
