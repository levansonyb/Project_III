using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Knight : BasePiece
{
    public override void Setup(bool newIsWhite, PieceManager newPM)
    {
        base.Setup(newIsWhite, newPM);

        movement = new Vector3Int(1, 1, 1);
        if (pieceManager.theme == null)
            GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/basic/knight");
        else GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/" + pieceManager.theme.spriteFolder + "/knight");
    }

    // Cách đi của quân Mã
    private void CreateCellPath(int yDirection)
    {
        int currentX = currentCell.boardPosition.x;
        int currentY = currentCell.boardPosition.y;
        Cell targetCell;

        // max left 
        try
        {
            targetCell = currentCell.board.allCells[currentX - 2][currentY + 1 * yDirection];
            MatchesState(targetCell);
        }
        catch (Exception e) { e.ToString(); }

        // left 
        try
        {
            targetCell = currentCell.board.allCells[currentX - 1][currentY + 2 * yDirection];
            MatchesState(targetCell);
        }
        catch (Exception e) { e.ToString(); }

        // max right 
        try
        {
            targetCell = currentCell.board.allCells[currentX + 2][currentY + 1 * yDirection];
            MatchesState(targetCell);
        }
        catch (Exception e) { e.ToString(); }

        // right 
        try
        {
            targetCell = currentCell.board.allCells[currentX + 1][currentY + 2 * yDirection];
            MatchesState(targetCell);
        }
        catch (Exception e) { e.ToString(); }
    }

    // Kiểm tra các nước đi hợp lệ
    protected override void CheckPathing()
    {
        // Kiểm tra các nước đi về phía trên
        CreateCellPath(1);
        // Kiểm tra các nước đi về phía dưới
        CreateCellPath(-1);
    }

    // Xác định trạng thái ô mục tiêu
    private void MatchesState(Cell target)
    {
        CellState targetState = target.GetState(this);

        // Friend: Ô mục tiêu chứa quân đồng minh, không hợp lệ.
        // ENEMY: Ô mục tiêu chứa quân địch, hợp lệ, được đánh dấu đỏ.
        // CHECK, CHECK_ENEMY, CHECK_FRIEND: Các trạng thái đặc biệt (kiểm tra Vua), không di chuyển được
        if (targetState != CellState.FRIEND && targetState != CellState.CHECK && targetState != CellState.CHECK_ENEMY && targetState != CellState.CHECK_FRIEND)
        {
            if (!pieceManager.checkVerificationInProcess)
            {
                // Ô mục tiêu chứa quân địch, đánh dấu đỏ
                if (targetState == CellState.ENEMY)
                {
                    target.outlineImage.GetComponent<Image>().color = new Color(1, 0, 0, (float)0.5);
                }
                else
                {
                    // Đánh dấu ô hợp lệ bằng màu xanh lá.
                    target.outlineImage.GetComponent<Image>().color = new Color(0, 1, 0, (float)0.5);
                }
            }
            // highlight 
            addPossibleCell(target);
        }
    }
}
