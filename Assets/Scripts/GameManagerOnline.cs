using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerOnline : MonoBehaviourPunCallbacks
{
    public Board board;                  // Quản lý bàn cờ
    public PieceManager pieceManager;    // Quản lý quân cờ
    public ClockManagerOnline clockManagerOnline; // Quản lý đồng hồ đếm giờ
    public PlayerManager playerManager; // Quản lý người chơi

    private bool isWhite; // Người chơi là trắng hay đen

    void Start()
    {
        // Kiểm tra các thành phần cần thiết
        ValidateComponents();

        // Xác định màu của người chơi
        isWhite = PhotonNetwork.IsMasterClient;

        // Khởi tạo bàn cờ và quân cờ
        board.Create();
        pieceManager.Setup(board);

        // Khởi tạo đồng hồ đếm giờ
        clockManagerOnline.Setup(60, 60, isWhite, pieceManager);

        // Cài đặt bàn cờ cho người chơi
        SetupBoardForPlayer(isWhite);
    }

    // Kiểm tra các thành phần cần thiết
    private void ValidateComponents()
    {
        if (pieceManager == null)
            Debug.LogError("pieceManager chưa được gán trong GameManagerOnline!");

        if (board == null)
            Debug.LogError("Board chưa được khởi tạo trong GameManagerOnline!");

        if (clockManagerOnline == null)
            Debug.LogError("clockManagerOnline chưa được khởi tạo trong GameManagerOnline!");
    }

    // Cài đặt bàn cờ cho người chơi dựa trên màu sắc
    private void SetupBoardForPlayer(bool isWhite)
    {
        if (!isWhite)
        {
            // Xoay bàn cờ 180 độ cho người chơi đen
            board.transform.localRotation = Quaternion.Euler(0, 0, 180);

            // Cập nhật vị trí quân cờ sau khi xoay
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

    // Xử lý sự kiện khi đối thủ thoát khỏi phòng
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("Đối thủ đã thoát! Bạn thắng!");
        EndGame(isWhite ? GameState.WHITE_WIN : GameState.BLACK_WIN);
    }

    // Kết thúc trò chơi và thông báo kết quả
    public void EndGame(GameState state)
    {
        photonView.RPC("RPC_EndGame", RpcTarget.All, state);
    }

    [PunRPC]
    void RPC_EndGame(GameState state)
    {
        string resultMessage = state switch
        {
            GameState.WHITE_WIN => "Trắng thắng!",
            GameState.BLACK_WIN => "Đen thắng!",
            GameState.PAT => "Hòa!",
            _ => "Kết thúc trò chơi!"
        };

        Debug.Log(resultMessage);

        // Quay lại menu sau 5 giây
        Invoke("BackToMenu", 5f);
    }

    // Quay lại menu chính
    void BackToMenu()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0);
    }
}
