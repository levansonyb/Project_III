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
        // Debug kiểm tra null
        if (pieceManager == null)
        {
            Debug.LogError("pieceManager chưa được gán trong GameManagerOnline!");
        }
        // Xác định màu quân cờ cho người chơi
        isWhite = PhotonNetwork.IsMasterClient; // MasterClient sẽ là trắng

        // Khởi tạo bàn cờ và quân cờ
        board.Create();
        // Kiểm tra board trước khi gọi Setup
        if (board == null)
        {
            Debug.LogError("Board chưa được khởi tạo trong GameManagerOnline!");
        }

        pieceManager.Setup(board);

        // Khởi tạo đồng hồ
        clockManagerOnline.Setup(60, 60, this);

        // Cài đặt bàn cờ cho người chơi dựa trên màu sắc
        SetupBoardForPlayer(isWhite);
    }

    /// <summary>
    /// Cài đặt bàn cờ cho người chơi dựa trên màu sắc
    /// </summary>
    /// <param name="isWhite">Người chơi là trắng hay đen</param>
    private void SetupBoardForPlayer(bool isWhite)
    {
        if (!isWhite)
        {
            // Xoay bàn cờ 180 độ cho người chơi màu đen
            board.transform.localRotation = Quaternion.Euler(0, 0, 180);

            // Đặt lại vị trí quân cờ sau khi xoay bàn cờ
            foreach (List<Cell> row in board.allCells)
            {
                foreach (Cell boardCell in row)
                {
                    if (boardCell.currentPiece != null)
                    {
                        boardCell.currentPiece.PlaceInit(boardCell);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Xử lý sự kiện khi đối thủ thoát khỏi phòng
    /// </summary>
    /// <param name="otherPlayer">Người chơi rời khỏi phòng</param>
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("Đối thủ đã thoát! Bạn thắng!");

        // Quay lại menu chính
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Kết thúc trò chơi và thông báo kết quả
    /// </summary>
    /// <param name="state">Trạng thái kết thúc trò chơi</param>
    public void EndGame(GameState state)
    {
        photonView.RPC("RPC_EndGame", RpcTarget.All, state);
    }

    [PunRPC]
    void RPC_EndGame(GameState state)
    {
        string resultMessage = "";

        // Hiển thị kết quả trò chơi dựa trên trạng thái
        switch (state)
        {
            case GameState.WHITE_WIN:
                resultMessage = "Trắng thắng!";
                break;
            case GameState.BLACK_WIN:
                resultMessage = "Đen thắng!";
                break;
            case GameState.PAT:
                resultMessage = "Hòa!";
                break;
        }

        Debug.Log(resultMessage);

        // Quay lại menu sau 5 giây
        Invoke("BackToMenu", 5f);
    }

    /// <summary>
    /// Quay lại menu chính
    /// </summary>
    void BackToMenu()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0);
    }
}
