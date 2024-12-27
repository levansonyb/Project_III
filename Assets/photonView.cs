using Photon.Pun;
using UnityEngine;

public class PhotonViewManager : MonoBehaviourPunCallbacks
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Khởi tạo các biến hoặc thực hiện các hành động cần thiết
    }

    // Update is called once per frame
    void Update()
    {
        // Thực hiện các hành động trong mỗi frame
    }

    // Thêm các phương thức để quản lý các hành động mạng
    public void SomeNetworkAction()
    {
        // Ví dụ: gọi một RPC hoặc thực hiện một hành động mạng khác
        photonView.RPC("SomeRPCMethod", RpcTarget.All);
    }

    [PunRPC]
    public void SomeRPCMethod()
    {
        // Thực hiện hành động khi RPC được gọi
    }
}
