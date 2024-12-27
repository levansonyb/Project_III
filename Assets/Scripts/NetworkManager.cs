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
        PhotonNetwork.ConnectUsingSettings();
        waitingPanel.SetActive(true);
        waitingText.text = "Đang kết nối...";
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        waitingText.text = "Đang tìm trận...";
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        waitingText.text = "Không tìm thấy trận, tạo phòng mới...";
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 2 });
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            waitingText.text = "Đang chờ đối thủ...";
        }
        else
        {
            waitingText.text = "Đối thủ đã vào! Bắt đầu trò chơi...";
            Invoke("StartGame", 2f); // Chờ 2 giây rồi bắt đầu
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        waitingText.text = "Đối thủ đã vào! Bắt đầu trò chơi...";
        Invoke("StartGame", 2f); // Chờ 2 giây rồi bắt đầu
    }

    void StartGame()
    {
        PhotonNetwork.LoadLevel("Multiplayer");
    }
}
