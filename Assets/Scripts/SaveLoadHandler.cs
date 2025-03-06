using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Handles saving and loading the puzzle game state.
/// </summary>
public class SaveLoadHandler : MonoBehaviour
{
    private string _saveFilePath;
    private PuzzleManager _puzzleManager;
    private List<PuzzlePiece> _puzzlePieces;

    private void Awake()
    {
        PuzzlePiece.OnPiecePlacedCorrectly += SaveGame;
        PuzzleManager.OnGameCompleted += DeleteSaveFile;
    }

    public void Initialize()
    {
        _saveFilePath = Path.Combine(Application.persistentDataPath, "puzzle_save.json");
        Debug.Log($"Save file path: {_saveFilePath}");  // 🔹 Debug: Print save file path

        _puzzleManager = GetComponent<PuzzleManager>();
        _puzzlePieces = _puzzleManager.GetPuzzlePieces();

        // 🔹 Debug: Ensure we can read the file
        if (File.Exists(_saveFilePath))
        {
            Debug.Log("Save file found! Attempting to load...");
        }
        else
        {
            Debug.LogWarning("Save file NOT found! Skipping load.");
        }

    }

    /// <summary>
    /// Deletes the save file when the game is completed.
    /// </summary>
    private void DeleteSaveFile()
    {
        if (File.Exists(_saveFilePath))
        {
            File.Delete(_saveFilePath);
            Debug.Log("Save file deleted.");
        }
    }

    /// <summary>
    /// Saves the current game state.
    /// </summary>
    public void SaveGame()
    {
        if (_puzzleManager.GetGameOverState()) return;
        
        if (_puzzlePieces == null || _puzzlePieces.Count == 0) return;

        PuzzleSaveData saveData = new PuzzleSaveData();
        foreach (var piece in _puzzlePieces)
        {
            saveData.PlacedPositions.Add(piece.transform.position);
            saveData.PiecesPlacedCorrectly.Add(piece.IsCorrectlyPlaced());
        }

        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(_saveFilePath, json);

        // 🔹 Debug: Confirm file is written
        if (File.Exists(_saveFilePath))
        {
            Debug.Log($"✅ Game Saved Successfully: {_saveFilePath}");
        }
        else
        {
            Debug.LogError("❌ ERROR: Save file was not created!");
        }
    }

    /// <summary>
    /// Loads the saved game state, ensuring pieces are placed correctly.
    /// </summary>
    public void LoadGame()
    {
        if (!File.Exists(_saveFilePath))
        {
            Debug.LogError("❌ ERROR: LoadGame() failed because save file does not exist!");
            return;
        }

        string json = File.ReadAllText(_saveFilePath);
        PuzzleSaveData saveData = JsonUtility.FromJson<PuzzleSaveData>(json);

        for (int i = 0; i < _puzzlePieces.Count && i < saveData.PlacedPositions.Count; i++)
        {
            _puzzlePieces[i].transform.position = saveData.PlacedPositions[i];

            if (saveData.PiecesPlacedCorrectly[i])
            {
                _puzzlePieces[i].MarkAsCorrectlyPlaced();
            }
        }

        Debug.Log("✅ Game Loaded Successfully.");
    }

    private void OnDisable()
    {
        PuzzlePiece.OnPiecePlacedCorrectly -= SaveGame;
        PuzzleManager.OnGameCompleted -= DeleteSaveFile;
    }
}

/// <summary>
/// Structure for storing puzzle save data.
/// </summary>
[Serializable]
public class PuzzleSaveData
{
    public List<Vector3> PlacedPositions = new List<Vector3>();
    public List<bool> PiecesPlacedCorrectly = new List<bool>();
}
