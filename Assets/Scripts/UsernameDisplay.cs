using Photon.Pun;
using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(PhotonView))]
public class UsernameDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshPro _username = null;

    private PhotonView _photonView = null;

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (_photonView.IsMine)
        {
            _username.gameObject.SetActive(false);
        }
        _username.text = _photonView.Owner.NickName;
    }
}