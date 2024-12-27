using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Board board;

    public PieceManager pieceManager;

    // Phương thức được gọi khi bắt đầu trò chơi
    void Start()
    {
        board.Create();

        pieceManager.Setup(board);
    }

    // Quay trở lại menu chính
    public void BackMenu()
    {
        if (PieceManager.IAmode)
            pieceManager.stockfish.Close();
        SceneManager.LoadScene(0); // Menu
    }

    // Tải lại trò chơi
    public void Reload()
    {
        pieceManager.ResetGame();
    }

    // Xoay bàn cờ
    public void ReverseBoard()
    {
        board.transform.localRotation *= Quaternion.Euler(180, 180, 0);
        foreach (List<Cell> row in board.allCells)
        {
            foreach (Cell boardCell in row)
            {
                if (boardCell.currentPiece != null)
                    boardCell.currentPiece.PlaceInit(boardCell);
            }
        }
    }
    
}