using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class BasePiece : EventTrigger
{
    [HideInInspector]
    public bool isWhite; // Xác định quân Trắng hay Đen

    public bool hasMoved = false; // Quân cờ đã di chuyển hay chưa

    [HideInInspector]
    static int cellPadding = 10; // Khoảng cách giữa các ô cờ

    protected Cell originalCell = null;

    [HideInInspector]
    public Cell currentCell = null;

    protected RectTransform rt = null;
    protected PieceManager pieceManager; // Quản lý các quân cờ và trạng thái trò chơi.

    protected Vector3Int movement = Vector3Int.one;
    protected List<Cell> highlightedCells = new List<Cell>();
    protected List<Cell> attackedCells = new List<Cell>();

    private Cell targetCell = null; // Ô mục tiêu mà quân cờ di chuyển tới.

    public bool inDrag = false; // Trạng thái quân cờ đang được kéo.

    public PieceManager GetPieceManager()
    {
        return pieceManager;
    }

    public static int CellPadding { get => cellPadding; }
    public Cell TargetCell { get => targetCell; set => targetCell = value; }

    public virtual void Setup(bool newIsWhite, PieceManager newPM)
    {
        inDrag = false;
        pieceManager = newPM;
        isWhite = newIsWhite;
        hasMoved = false;

        rt = GetComponent<RectTransform>();

        if (pieceManager.theme == null)
        {
            if (isWhite)
                GetComponent<Image>().color = Color.white;
            else
                GetComponent<Image>().color = Color.grey;
        }
        else
        {
            if (isWhite)
                GetComponent<Image>().color = pieceManager.theme.whitePiece;
            else
                GetComponent<Image>().color = pieceManager.theme.blackPiece;
        }
    }

    // Đặt quân cờ vào ô ban đầu trên bàn cờ và cập nhật trạng thái của ô (currentPiece)
    public void PlaceInit(Cell newCell)
    {
        currentCell = newCell;
        originalCell = newCell;
        currentCell.currentPiece = this;

        transform.position = newCell.transform.position;
        gameObject.SetActive(true); 
    }

    private void CreateCellPath(int xDirection, int yDirection, int movement)
    {
        int currentX = currentCell.boardPosition.x;
        int currentY = currentCell.boardPosition.y;

        // Kiểm tra trạng thái ô (ô trống, ô có quân đồng đội, ô có quân đối phương)
        for (int i = 1; i <= movement; i++)
        {
            currentX += xDirection;
            currentY += yDirection;

            if (currentX < 0 || currentY < 0 || currentX > currentCell.board.Column - 1 || currentY > currentCell.board.Row - 1)
                continue;

            Cell targeted = currentCell.board.allCells[currentX][currentY];

            CellState state = targeted.GetState(this);
            if (state != CellState.FRIEND && state != CellState.CHECK && state != CellState.CHECK_ENEMY && state != CellState.CHECK_FRIEND)
            {
                if (!pieceManager.checkVerificationInProcess)
                {
                    // Tô màu cho ô trống và ô tấn công đối phương
                    if (state == CellState.ENEMY)
                    {
                        targeted.outlineImage.GetComponent<Image>().color = new Color(1, 0, 0, (float)0.5);
                    }
                    else
                    {
                        targeted.outlineImage.GetComponent<Image>().color = new Color(0, 1, 0, (float)0.5);
                    }
                    // highlight
                }

                addPossibleCell(targeted);
            }
            if (state == CellState.ENEMY || state == CellState.FRIEND || state == CellState.CHECK_ENEMY || state == CellState.CHECK_FRIEND)
                break;
        }
    }

    // Kiểm tra các nước đi hợp lệ cho quân cờ theo nhiều hướng (ngang, dọc, chéo).
    protected void addPossibleCell(Cell possibleCell)
    {
        if (pieceManager.checkVerificationInProcess)
            attackedCells.Add(possibleCell);
        else
            highlightedCells.Add(possibleCell);
    }

    // Kiểm tra các nước đi hợp lệ cho quân cờ theo nhiều hướng (ngang, dọc, chéo).
    protected virtual void CheckPathing()
    {
        // Ngang
        CreateCellPath(1, 0, movement.x);
        CreateCellPath(-1, 0, movement.x);

        // dọc 
        CreateCellPath(0, 1, movement.y);
        CreateCellPath(0, -1, movement.y);

        // Chéo xuôi
        CreateCellPath(1, 1, movement.z);
        CreateCellPath(-1, 1, movement.z);

        // Chéo ngước
        CreateCellPath(-1, -1, movement.z);
        CreateCellPath(1, -1, movement.z);

    }

    // Hiển thị các ô mà quân cờ có thể di chuyển đến bằng cách kích hoạt hình ảnh viền
    protected void ShowCellsHighlight()
    {
        foreach (Cell cell in highlightedCells)
            cell.outlineImage.enabled = true;
    }

    // Xóa tất cả các ô được đánh dấu và làm sạch danh sách
    protected void ClearCellsHighlight()
    {
        foreach (Cell cell in highlightedCells)
            cell.outlineImage.enabled = false;

        highlightedCells.Clear();
    }

    // Bắt đầu kéo quân cờ: kiểm tra nước đi hợp lệ và hiển thị các ô được đánh dấu
    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);

        inDrag = true;

        // Kiểm tra trạng thái ô cờ
        CheckPathing();

        // Hiển thị highlight ô cờ
        ShowCellsHighlight();

        transform.position = Input.mousePosition;
        transform.SetAsLastSibling();
    }

    // Di chuyển quân cờ theo con trỏ chuột.
    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        transform.position += (Vector3)eventData.delta;
    }

    // Kết thúc kéo
    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        inDrag = false;

        // Kiểm tra ô mục tiêu
        targetCell = null;
        foreach (Cell cell in highlightedCells)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(cell.rectTransform, Input.mousePosition))
            {
                targetCell = cell;
                break;
            }
        }

        // Nếu nước đi không hợp lệ, trả quân cờ về vị trí cũ, ngược lại thực hiện di chuyển quân cờ.
        if (!targetCell || pieceManager.gameState != GameState.INGAME)
        {
            transform.position = currentCell.transform.position; 
        }
        else
        {
            if (PieceManager.IAmode)
            {
                string move = "";
                move += pieceManager.posA[currentCell.boardPosition.x];
                move += pieceManager.posB[currentCell.boardPosition.y];
                move += pieceManager.posA[targetCell.boardPosition.x];
                move += pieceManager.posB[targetCell.boardPosition.y];
                // Nếu phong cấp
                if (this.GetType() == typeof(Pawn) && (TargetCell.boardPosition.y == 0 || TargetCell.boardPosition.y == 7))
                {
                    move += "q";
                }
                Debug.Log(move);
                pieceManager.stockfish.setIAmove(move);
                Move();
            }
            else
            {
                Move();
            }
        }
        // xóa Highlited
        ClearCellsHighlight();
    }

    // Trả về trạng thái ban đầu của trò chơi
    public void Reset()
    {
        Kill();
        PlaceInit(originalCell);
    }

    // Loại bỏ quân cờ khỏi bàn cờ và vô hiệu hóa đối tượng
    public virtual void Kill()
    {
        currentCell.currentPiece = null;
        gameObject.SetActive(false);
    }

    public virtual void Move()
    {
        // Thêm âm thanh khi di chuyển quân
        AudioClip clip = null;
        if (targetCell.currentPiece == null)
        {
            clip = (AudioClip)Resources.Load("Sounds/basic/move");
        }
        else
        {
            clip = (AudioClip)Resources.Load("Sounds/basic/pick");
        }

        // Xóa trạng thái bị chiếu nếu có
        if (pieceManager.getKing(isWhite).isCheck)
        {
            pieceManager.getKing(isWhite).setCheck(false);
        }

        // Xóa quân cờ trên ô đích nếu có
        targetCell.RemovePiece();

        bool castling = false;
        // Xử lý nhập thành
        if (currentCell.currentPiece.GetType() == typeof(King) && currentCell.currentPiece.hasMoved == false)
        {
            if (targetCell.boardPosition.x == 2)
            {
                BasePiece rook = currentCell.board.allCells[0][currentCell.boardPosition.y].currentPiece;
                rook.targetCell = currentCell.board.allCells[3][currentCell.boardPosition.y];
                rook.Move();
                castling = true;
            }
            else if (targetCell.boardPosition.x == 6)
            {
                BasePiece rook = currentCell.board.allCells[7][currentCell.boardPosition.y].currentPiece;
                rook.targetCell = currentCell.board.allCells[5][currentCell.boardPosition.y];
                rook.Move();
                castling = true;
            }
        }

        // Cập nhật trạng thái vị trí
        currentCell.currentPiece = null;

        currentCell = targetCell;
        currentCell.currentPiece = this;

        transform.position = currentCell.transform.position;
        targetCell = null;

        hasMoved = true;

        // Quản lý trạng thái bắt tốt qua đường
        if (pieceManager.enPassantCell != null)
        {
            pieceManager.enPassantCell.enPassant = null;
            pieceManager.enPassantCell = null;
        }

        // Kiểm tra trạng thái chiếu
        pieceManager.checkVerificationInProcess = true;
        if (isCheckVerif(isWhite))
        {
            pieceManager.getKing(!isWhite).setCheck(true);
            clip = (AudioClip)Resources.Load("Sounds/basic/check");
        }
        pieceManager.checkVerificationInProcess = false;

        CheckGameOver(!isWhite);

        // Âm thanh và đổi lượt
        if (pieceManager.gameState != GameState.INGAME)
            clip = null;
        if (clip != null)
            pieceManager.audio.PlayOneShot(clip);

        if (!pieceManager.IATurn && pieceManager.gameState == GameState.INGAME && !castling)
            pieceManager.SetTurn(!isWhite);
    }

    // Kiểm tra xem vua của đối phương có bị chiếu không bằng cách kiểm tra tất cả nước đi của quân cờ đang tấn công
    public bool isCheckVerif(bool AttakingSideIsWhite)
    {
        foreach (List<Cell> row in currentCell.board.allCells)
        {
            foreach (Cell boardCell in row)
            {
                BasePiece pieceBoard = boardCell.currentPiece;
                if (pieceBoard != null && pieceBoard.isWhite == AttakingSideIsWhite)
                {
                    King targetKing = pieceManager.getKing(!AttakingSideIsWhite);

                    pieceBoard.CheckPathing();
                    foreach (Cell cell in pieceBoard.attackedCells)
                    {
                        if (cell.boardPosition.x == targetKing.currentCell.boardPosition.x &&
                            cell.boardPosition.y == targetKing.currentCell.boardPosition.y)
                        {
                            pieceBoard.ClearAttackedCell();
                            return true;
                        }
                    }
                    pieceBoard.ClearAttackedCell();
                }
            }
        }
        return false;
    }

    // Xóa danh sách tấn công
    public void ClearAttackedCell()
    {
        attackedCells.Clear();
    }

    // Xác định xem phe trắng hoặc đen còn nước đi hợp lệ hay không
    public bool PossibleMove(bool isWhite)
    {
        foreach (List<Cell> row in currentCell.board.allCells)
        {
            foreach (Cell boardCell in row)
            {
                BasePiece piece = boardCell.currentPiece;
                if (piece != null && piece.isWhite == isWhite)
                {
                    piece.CheckPathing();
                    if (piece.highlightedCells.Count > 0)
                    {
                        piece.highlightedCells.Clear();
                        return true;
                    }
                    piece.highlightedCells.Clear();
                }
            }
        }
        return false;
    }

    // Kiểm tra trạng thái kết thúc trò chơi
    public void CheckGameOver(bool isWhite)
    {
        if (!PossibleMove(isWhite))
        {
            if (pieceManager.getKing(isWhite).isCheck)
            {
                if (isWhite)
                    pieceManager.gameState = GameState.BLACK_WIN;
                else
                    pieceManager.gameState = GameState.WHITE_WIN;
            }
            else
            {
                pieceManager.gameState = GameState.PAT;
            }
            Debug.Log(pieceManager.gameState);
            pieceManager.ShowResult();
        }
    }

    internal void Setup(bool v, GameManagerOnline gameManagerOnline)
    {
        throw new NotImplementedException();
    }

}
