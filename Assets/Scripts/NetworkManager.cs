using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public GameObject waitingPanel; // UI hiển thị phòng chờ
    public TMP_Text waitingText;    // Thông báo trạng thái

    void Start()
    {
        ConnectToServer();
    }

    // Kết nối đến máy chủ Photon
    private void ConnectToServer()
    {
        waitingPanel.SetActive(true);
        waitingText.text = "Đang kết nối...";
        PhotonNetwork.ConnectUsingSettings();
    }

    // Khi kết nối đến máy chủ thành công
    public override void OnConnectedToMaster()
    {
        waitingText.text = "Đã kết nối đến máy chủ. Đang tham gia sảnh chờ...";
        PhotonNetwork.JoinLobby();
    }

    // Khi tham gia sảnh chờ thành công
    public override void OnJoinedLobby()
    {
        waitingText.text = "Đang tìm trận...";
        PhotonNetwork.JoinRandomRoom();
    }

    // Nếu không tìm thấy phòng, tạo phòng mới
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        waitingText.text = "Không tìm thấy trận, tạo phòng mới...";
        RoomOptions roomOptions = new RoomOptions { MaxPlayers = 2 };
        PhotonNetwork.CreateRoom(null, roomOptions);
    }

    // Khi đã tham gia vào phòng thành công
    public override void OnJoinedRoom()
    {
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        if (playerCount == 1)
        {
            waitingText.text = "Đang chờ đối thủ...";
        }
        else
        {
            waitingText.text = "Đối thủ đã vào! Bắt đầu trò chơi...";
            Invoke("StartGame", 2f); // Bắt đầu trò chơi sau 2 giây
        }
    }

    // Khi một người chơi khác vào phòng
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        waitingText.text = "Đối thủ đã vào! Bắt đầu trò chơi...";
        Invoke("StartGame", 2f); // Bắt đầu trò chơi sau 2 giây
    }

    // Khi một người chơi rời khỏi phòng
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        waitingText.text = "Đối thủ đã rời phòng. Đang chờ đối thủ mới...";
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.JoinRandomRoom();
    }

    // Bắt đầu trò chơi bằng cách chuyển đến scene Multiplayer
    private void StartGame()
    {
        PhotonNetwork.LoadLevel("Multiplayer");
    }

    // Xử lý khi ngắt kết nối
    public override void OnDisconnected(DisconnectCause cause)
    {
        waitingText.text = $"Mất kết nối: {cause}. Đang thử lại...";
        ConnectToServer();
    }
}
