using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum GameState
{
    INGAME,
    WHITE_WIN,
    BLACK_WIN,
    PAT,
    NULLE
}

public class PieceManager : MonoBehaviour
{
    [HideInInspector]
    public bool isKingAlive; // Biến kiểm tra xem vua có còn sống không

    private Board chessBoard; // Tham chiếu đến bàn cờ

    [HideInInspector]
    public GameState gameState; // Trạng thái hiện tại của trò chơi

    [HideInInspector]
    public Theme theme = null; // Giao diện của trò chơi

    [HideInInspector]
    public new AudioSource audio; // Điều khiển âm thanh

    public ClockManager clockManager; // Quản lý thời gian của trò chơi
    public ClockManagerOnline clockManagerOnline; // Quản lý thời gian của trò chơi online

    public GameObject piecePrefab; // Prefab của các quân cờ
    public TMP_Text result; // Hiển thị kết quả trò chơi

    private List<BasePiece> whitePieces = null; // Danh sách các quân cờ trắng 
    private List<BasePiece> blackPieces = null; // Danh sách các quân cờ đen

    public static float blackTime = 60;
    public static float whiteTime = 60;

    public static bool IAmode = true; // Biến kiểm tra xem có đang chơi với AI hay không
    public static bool isAIWhite = false;
    public static bool Online = true;
    public IA stockfish = null;
    [HideInInspector]
    public bool IATurn = false;

    [HideInInspector]
    public Cell enPassantCell = null;


    [HideInInspector]
    public King whiteKing = null;
    [HideInInspector]
    public King blackKing = null;
    [HideInInspector]
    public bool checkVerificationInProcess = false;

    // Các ký hiệu quân cờ
    private string[] pieceOrder = { "P", "P", "P", "P", "P", "P", "P", "P",
        "R", "KN", "B", "Q", "K", "B", "KN", "R" };

    // Xác định các ký hiệu quân cờ
    private Dictionary<string, Type> pieceDico = new Dictionary<string, Type>()
    {
        {"P", typeof(Pawn)},
        {"R", typeof(Rook)},
        {"KN", typeof(Knight)},
        {"B", typeof(Bishop)},
        {"K", typeof(King)},
        {"Q", typeof(Queen)}
    };

    // Chỉ số cột chuyển đổi từ ký tự -> số
    public Dictionary<string, int> coordA = new Dictionary<string, int>()
    {
        {"a", 0},
        {"b", 1},
        {"c", 2},
        {"d", 3},
        {"e", 4},
        {"f", 5},
        {"g", 6},
        {"h", 7}
    };

    // Chỉ số hàng chuyển đổi từ ký tự -> số
    public Dictionary<string, int> coordB = new Dictionary<string, int>()
    {
        {"1", 0},
        {"2", 1},
        {"3", 2},
        {"4", 3},
        {"5", 4},
        {"6", 5},
        {"7", 6},
        {"8", 7}
    };

    // Chỉ số cột chuyển đổi từ số -> ký tự
    public Dictionary<int, string> posA = new Dictionary<int, string>()
    {
        {0, "a"},
        {1, "b"},
        {2, "c"},
        {3, "d"},
        {4, "e"},
        {5, "f"},
        {6, "g"},
        {7, "h"},
    };

    // Chỉ số hàng chuyển đổi từ số -> ký tự
    public Dictionary<int, string> posB = new Dictionary<int, string>()
    {
        {0, "1"},
        {1, "2"},
        {2, "3"},
        {3, "4"},
        {4, "5"},
        {5, "6"},
        {6, "7"},
        {7, "8"},
    };

    // Khởi tạo trò chơi
    public void Setup(Board board)
    {
        audio = gameObject.AddComponent<AudioSource>();

        chessBoard = board;
        gameState = GameState.INGAME;

        result.text = "";

        isKingAlive = true;

        whitePieces = CreatePieces(true, chessBoard);
        blackPieces = CreatePieces(false, chessBoard);

        PlacePieces("2", "1", whitePieces, board);
        PlacePieces("7", "8", blackPieces, board);

        SetInteractive(whitePieces, false);
        SetInteractive(blackPieces, false);

        enPassantCell = null;
        checkVerificationInProcess = false;
        clockManager.Setup(whiteTime, blackTime, this);

        if (IAmode)
        {
            stockfish.Setup();
            if (isAIWhite)
            {
                StartCoroutine(showIAMoveCoroutine());
                clockManager.displayBlack.text = "Player";
                clockManager.displayWhite.text = "Computer " + IA.IA_Game_Level[IA.level];
            }
            else
            {
                SetInteractive(whitePieces, true);
                clockManager.displayWhite.text = "Player";
                clockManager.displayBlack.text = "Computer " + IA.IA_Game_Level[IA.level];
            }
        }
        else
        {
            SetInteractive(whitePieces, true);
        }
    }

    // Đặt lại trò chơi
    public void ResetGame()
    {
        if (IATurn)
            return;
        gameState = GameState.INGAME;

        result.text = "";

        foreach (List<Cell> row in chessBoard.allCells)
        {
            foreach (Cell boardCell in row)
            {
                boardCell.outlineImage.enabled = false;
                if (boardCell.currentPiece != null)
                {
                    boardCell.currentPiece.Kill();
                }
                boardCell.enPassant = null;
            }
        }

        whitePieces.Clear();
        blackPieces.Clear();

        whitePieces = CreatePieces(true, chessBoard);
        blackPieces = CreatePieces(false, chessBoard);

        PlacePieces("2", "1", whitePieces, chessBoard);
        PlacePieces("7", "8", blackPieces, chessBoard);

        SetInteractive(whitePieces, false);
        SetInteractive(blackPieces, false);

        enPassantCell = null;
        isKingAlive = true;

        clockManager.Setup(whiteTime, blackTime, this);

        checkVerificationInProcess = false;

        if (IAmode)
        {
            stockfish.Close();
            stockfish.Setup();
            if (isAIWhite)
            {
                StartCoroutine(showIAMoveCoroutine());
                clockManager.displayBlack.text = "Player";
                clockManager.displayWhite.text = "Computer " + IA.IA_Game_Level[IA.level];
            }
            else
            {
                SetInteractive(whitePieces, true);
                clockManager.displayWhite.text = "Player";
                clockManager.displayBlack.text = "Computer " + IA.IA_Game_Level[IA.level];
            }
        }
        else if (Online)
        {
            clockManager.displayBlack.text = "Player 1";
            clockManager.displayWhite.text = "Player 2";
        }
        else
        {
            SetInteractive(whitePieces, true);
        }
    }

    // Tạo danh sách quân cờ dựa trên màu sắc và sắp xếp chúng theo thứ tự định trước pieceOrder
    // Gắn các thuộc tính cơ bản (kích thước, vị trí) và tham chiếu đến bàn cờ
    private List<BasePiece> CreatePieces(bool isWhite, Board board)
    {
        List<BasePiece> pieceList = new List<BasePiece>();

        float board_width = board.GetComponent<RectTransform>().rect.width;
        float board_height = board.GetComponent<RectTransform>().rect.height;

        for (int i = 0; i < pieceOrder.Length; i++)
        {
            GameObject newPieceObject = Instantiate(piecePrefab);
            newPieceObject.transform.SetParent(transform);

            newPieceObject.transform.localScale = new Vector3(1, 1, 1);
            newPieceObject.transform.localRotation = Quaternion.identity;


            float piece_width = board_width / board.Column - BasePiece.CellPadding;
            float piece_height = board_height / board.Row - BasePiece.CellPadding;
            newPieceObject.GetComponent<RectTransform>().sizeDelta = new Vector2(piece_width, piece_height);


            string key = pieceOrder[i];
            Type pieceType = pieceDico[key];

            BasePiece newPiece = (BasePiece)newPieceObject.AddComponent(pieceType);
            pieceList.Add(newPiece);

            if (pieceDico[key] == typeof(King))
            {
                if (isWhite)
                    whiteKing = (King)newPiece;
                else
                    blackKing = (King)newPiece;
            }
            newPiece.Setup(isWhite, this);
        }
        return pieceList;
    }

    // Đặt các quân cờ vào vị trí ban đầu
    private void PlacePieces(string pawnRow, string royaltyRow, List<BasePiece> pieces, Board board)
    {
        for (int i = 0; i < board.Column; i++)
        {
            pieces[i].PlaceInit(board.allCells[i][coordB[pawnRow]]);
            pieces[i + 8].PlaceInit(board.allCells[i][coordB[royaltyRow]]);
        }
    }

    // Bật/tắt khả năng tương tác của quân cờ (kéo thả)
    private void SetInteractive(List<BasePiece> pieces, bool state)
    {
        foreach (BasePiece piece in pieces)
        {
            if (piece.inDrag)
                piece.OnEndDrag(null);
            piece.enabled = state;
        }
    }

    // Đổi lượt chơi
    public void SetTurn(bool isWhiteTurn)
    {
        if (IAmode)
        {
            clockManager.setTurn(isWhiteTurn);
            SetInteractive(whitePieces, false);
            SetInteractive(blackPieces, false);
            StartCoroutine(showIAMoveCoroutine());
        }
        else
        {
            if (isKingAlive == false)
                return;

            SetInteractive(whitePieces, isWhiteTurn);
            SetInteractive(blackPieces, !isWhiteTurn);

            if (clockManager.launched == false)
            {
                clockManager.StartClocks();
            }
            clockManager.setTurn(isWhiteTurn);
        }
    }

    // Dùng Coroutine để hiển thị nước đi của AI (gọi API)
    private IEnumerator showIAMoveCoroutine()
    {

        IATurn = true;
        string best = stockfish.GetBestMove();
        yield return new WaitForSeconds((float)2);

        string depA = best.Substring(0, 1);
        string depB = best.Substring(1, 1);
        string arrA = best.Substring(2, 1);
        string arrB = best.Substring(3, 1);

        Cell dep = chessBoard.allCells[coordA[depA]][coordB[depB]];
        Cell targ = chessBoard.allCells[coordA[arrA]][coordB[arrB]];

        // Nếu phong cấp
        if (dep.currentPiece.GetType() == typeof(Pawn) && (coordB[arrB] == 0 || coordB[arrB] == 7))
        {
            best += "q";
        }

        Debug.Log(best);

        stockfish.setIAmove(best);

        dep.currentPiece.TargetCell = targ;
        dep.currentPiece.Move();
        IATurn = false;

        if (GameState.INGAME == gameState)
        {
            if (isAIWhite)
                SetInteractive(blackPieces, true);
            else{
                SetInteractive(whitePieces, true);
            }          
            clockManager.setTurn(!isAIWhite);
        }
    }

    // Xử lý việc phong cấp
    public void PawnPromotion(Pawn pawn, Cell BeforeCell)
    {
        pawn.currentCell.RemovePiece();
        GameObject newPieceObject = Instantiate(piecePrefab);
        newPieceObject.transform.SetParent(transform);

        newPieceObject.transform.localScale = new Vector3(1, 1, 1);
        newPieceObject.transform.localRotation = Quaternion.identity;

        float board_width = BeforeCell.board.GetComponent<RectTransform>().rect.width;
        float board_height = BeforeCell.board.GetComponent<RectTransform>().rect.height;

        float piece_width = board_width / BeforeCell.board.Column - BasePiece.CellPadding;
        float piece_height = board_height / BeforeCell.board.Row - BasePiece.CellPadding;
        newPieceObject.GetComponent<RectTransform>().sizeDelta = new Vector2(piece_width, piece_height);

        Queen queen = (Queen)newPieceObject.AddComponent(typeof(Queen));

        queen.Setup(pawn.isWhite, this);
        queen.TargetCell = pawn.currentCell;
        queen.currentCell = BeforeCell;
        queen.currentCell.currentPiece = queen;
        queen.Move();
        if (pawn.isWhite)
        {
            whitePieces.Remove(pawn);
            whitePieces.Add(queen);
        }
        else
        {
            blackPieces.Remove(pawn);
            blackPieces.Add(queen);
        }
        queen.gameObject.SetActive(true);
    }


    public King getKing(bool isWhite)
    {
        if (isWhite)
            return whiteKing;
        else
            return blackKing;
    }

    // Hiển thị kết quả khi trò chơi kết thúc
    public void ShowResult()
    {
        audio.PlayOneShot((AudioClip)Resources.Load("Sounds/basic/end"));

        SetInteractive(whitePieces, false);
        SetInteractive(blackPieces, false);

        clockManager.StopClocks();

        clockManager.highlightClockW.SetActive(false);
        clockManager.highlightClockB.SetActive(false);

        result.enabled = false;

        StartCoroutine(showResultCoroutine());
    }

    private IEnumerator showResultCoroutine()
    {
        yield return new WaitForSeconds((float)2.1);
        if (gameState == GameState.BLACK_WIN)
        {
            result.text = "Đen thắng!";
            clockManager.highlightClockB.SetActive(true);
            clockManager.highlightClockB.GetComponent<Image>().color = new Color(1, (float)0.6816, 0, 1);
        }
        if (gameState == GameState.WHITE_WIN)
        {
            result.text = "Trắng thắng!";
            clockManager.highlightClockW.SetActive(true);
            clockManager.highlightClockW.GetComponent<Image>().color = new Color(1, (float)0.6816, 0, 1);

        }
        if (gameState == GameState.PAT)
        {
            result.text = "Hòa!";
        }
        result.enabled = true;
    }

    // Theme
    public void ApplyTheme(Theme newTheme)
    {
        theme = newTheme;
        for (int x = 0; x < chessBoard.Column; x++)
        {
            for (int y = 0; y < chessBoard.Row; y++)
            {
                Cell cell = chessBoard.allCells[x][y];
                cell.GetComponent<Image>().color = theme.blackCell; // Màu của ô đen
                cell.GetComponent<Image>().sprite = theme.textureSprite; // Họa tiết của ô

                if (cell.currentPiece != null) // Nếu ô có quân cờ
                {
                    // Áp dụng hình ảnh (sprite) dựa trên loại quân cờ
                    if (cell.currentPiece.isWhite)
                        cell.currentPiece.GetComponent<Image>().color = theme.whitePiece;
                    else
                        cell.currentPiece.GetComponent<Image>().color = theme.blackPiece;
                    if (cell.currentPiece.GetType() == typeof(King))
                        cell.currentPiece.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/" + theme.spriteFolder + "/king");
                    if (cell.currentPiece.GetType() == typeof(Pawn))
                        cell.currentPiece.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/" + theme.spriteFolder + "/pawn");
                    if (cell.currentPiece.GetType() == typeof(Queen))
                        cell.currentPiece.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/" + theme.spriteFolder + "/queen");
                    if (cell.currentPiece.GetType() == typeof(Bishop))
                        cell.currentPiece.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/" + theme.spriteFolder + "/bishop");
                    if (cell.currentPiece.GetType() == typeof(Knight))
                        cell.currentPiece.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/" + theme.spriteFolder + "/knight");
                    if (cell.currentPiece.GetType() == typeof(Rook))
                        cell.currentPiece.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/" + theme.spriteFolder + "/rook");
                }
            }
        }

        // Màu ô trắng
        for (int x = 0; x < chessBoard.Column; x += 2)
        {
            for (int y = 0; y < chessBoard.Row; y++)
            {
                // So le cho mỗi dòng
                int offset = (y % 2 != 0) ? 0 : 1;
                int finalX = x + offset;

                Image im = chessBoard.allCells[finalX][y].GetComponent<Image>();
                im.color = theme.whiteCell;
            }
        }
    }
}