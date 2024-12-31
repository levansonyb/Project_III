using Photon.Pun;
using UnityEngine;

public class PlayerManager : MonoBehaviourPun
{
    private Board board;

    void Start()
    {
        board = FindObjectOfType<Board>();
        if (board == null)
        {
            Debug.LogError("Board not found in PlayerManager.");
        }
    }

    // Di chuyển quân cờ và đồng bộ qua mạng
    public void MovePiece(Cell targetCell)
    {
        if (targetCell == null || targetCell.currentPiece == null)
        {
            Debug.LogError("Target cell is invalid or has no piece.");
            return;
        }

        photonView.RPC("RPC_MovePiece", RpcTarget.All, targetCell.boardPosition.x, targetCell.boardPosition.y);
    }

    // RPC để di chuyển quân cờ trên tất cả các client
    [PunRPC]
    void RPC_MovePiece(int x, int y)
    {
        Cell targetCell = FindCell(x, y);
        if (targetCell == null || targetCell.currentPiece == null)
        {
            Debug.LogError($"Invalid move target: ({x}, {y})");
            return;
        }

        targetCell.currentPiece.Move();
    }

    // Tìm ô cờ theo vị trí
    private Cell FindCell(int x, int y)
    {
        if (board == null)
        {
            Debug.LogError("Board not initialized in PlayerManager.");
            return null;
        }

        if (x < 0 || x >= board.allCells.Count || y < 0 || y >= board.allCells[x].Count)
        {
            Debug.LogError($"Invalid cell position: ({x}, {y})");
            return null;
        }

        return board.allCells[x][y];
    }
}
