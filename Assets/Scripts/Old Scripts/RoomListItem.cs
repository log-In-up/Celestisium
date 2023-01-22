using Photon.Realtime;
using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public class RoomListItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _roomName = null;

    private RoomInfo _roomInfo;

    public void SetUp(RoomInfo roomInfo)
    {
        _roomInfo = roomInfo;
        _roomName.text = _roomInfo.Name;
    }

    public void OnClick()
    {
        Launcher.Instance.JoinRoom(_roomInfo);
    }
}
