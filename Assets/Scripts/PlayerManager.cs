using Photon.Pun;
using UnityEngine;

public class PlayerManager : MonoBehaviourPun
{
    private Board board;

    void Start()
    {
        board = FindObjectOfType<Board>();
    }

    public void MovePiece(Cell targetCell)
    {
        if (targetCell != null && targetCell.currentPiece != null)
        {
            photonView.RPC("RPC_MovePiece", RpcTarget.All, targetCell.boardPosition.x, targetCell.boardPosition.y);
        }
    }

    [PunRPC]
    void RPC_MovePiece(int x, int y)
    {
        Cell targetCell = FindCell(x, y);
        if (targetCell != null && targetCell.currentPiece != null)
        {
            targetCell.currentPiece.Move();
        }
        else
        {
            Debug.LogError($"Invalid move target: ({x}, {y})");
        }
    }

    private Cell FindCell(int x, int y)
    {
        if (board == null)
        {
            Debug.LogError("Board not found.");
            return null;
        }

        if (x >= 0 && x < board.allCells.Count && y >= 0 && y < board.allCells[x].Count)
        {
            return board.allCells[x][y];
        }

        Debug.LogError($"Invalid cell position: ({x}, {y})");
        return null;
    }
}
