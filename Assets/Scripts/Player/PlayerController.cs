using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Players
{
    [DisallowMultipleComponent]
    public class PlayerController : MonoBehaviourPunCallbacks, IDamageable
    {
        #region Editor fields
        [SerializeField] private Slider _healthBar = null;
        [SerializeField] private GameObject _cameraHolder = null, _ui = null;
        [SerializeField, Min(0.0f)] private float _mouseSensitivity = 3.0f, _smoothTime = 0.15f, _sprintSpeed = 6.0f, _walkSpeed = 3.0f, _jumpForce = 300.0f;
        [SerializeField] private Gun[] _weapons = null;
        #endregion

        #region Fields
        private bool _grounded, _isInMenu;
        private float _verticalLookrotation, _currentHealth;
        private int _itemIndex, _previousItemIndex = -1;
        private Vector3 _smoothMoveVelocity, _moveAmount;

        private PlayerManager _playerManager = null;
        private Rigidbody _rigidbody = null;
        private PhotonView _photonView = null;
        private PhotonTeamsManager _teamsManager = null;
        private Player[] _playerTeammates = null;

        private const string VERTICAL_MOUSE_AXIS_NAME = "Mouse Y", HORIZONTAL_MOUSE_AXIS_NAME = "Mouse X",
            VERTICAL_AXIS_RAW = "Vertical", HORIZONTAL_AXIS_RAW = "Horizontal",
            MOUSE_SCROLLWHEEL_NAME = "Mouse ScrollWheel", ITEM_INDEX_KEY = "_itemIndex";

        private const float VERTICAL_MIN = -90.0f, VERTICAL_MAX = 90.0f, MAX_HEALTH = 100.0f;
        #endregion

        #region Properties
        public bool IsInMenu { get => _isInMenu; set => _isInMenu = value; }
        #endregion

        #region MonoBehaviour API
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _photonView = GetComponent<PhotonView>();

            _isInMenu = false;

            int viewID = (int)_photonView.InstantiationData[0];
            _playerManager = PhotonView.Find(viewID).GetComponent<PlayerManager>();

            _teamsManager = PhotonTeamsManager.Instance;
        }

        private void Start()
        {
            if (_photonView.IsMine)
            {
                ResetPlayerStats();

                if (_teamsManager.TryGetTeamMatesOfPlayer(PhotonNetwork.LocalPlayer, out Player[] teammates))
                {
                    _playerTeammates = teammates;
                }
            }
            else
            {
                Destroy(GetComponentInChildren<Camera>().gameObject);
                Destroy(_ui);
                Destroy(_rigidbody);
            }
        }

        private void Update()
        {
            if (!_photonView.IsMine) return;
            if (_isInMenu) return;

            Look();
            Move();
            Jump();

            SwitchWeapon();
            UseWeapon();
        }

        private void FixedUpdate()
        {
            if (!_photonView.IsMine) return;
            if (_isInMenu) return;

            _rigidbody.MovePosition(_rigidbody.position + (transform.TransformDirection(_moveAmount) * Time.fixedDeltaTime));
        }
        #endregion

        #region Methods
        private void Look()
        {
            Vector3 eulers = _mouseSensitivity * Input.GetAxisRaw(HORIZONTAL_MOUSE_AXIS_NAME) * Vector3.up;
            transform.Rotate(eulers);

            _verticalLookrotation += Input.GetAxisRaw(VERTICAL_MOUSE_AXIS_NAME) * _mouseSensitivity;
            _verticalLookrotation = Mathf.Clamp(_verticalLookrotation, VERTICAL_MIN, VERTICAL_MAX);

            _cameraHolder.transform.localEulerAngles = Vector3.left * _verticalLookrotation;
        }

        private void Move()
        {
            float x = Input.GetAxisRaw(HORIZONTAL_AXIS_RAW);
            float z = Input.GetAxisRaw(VERTICAL_AXIS_RAW);
            Vector3 moveDirection = new Vector3(x, 0.0f, z).normalized;

            float movementSpeed = Input.GetKey(KeyCode.LeftShift) ? _sprintSpeed : _walkSpeed;
            _moveAmount = Vector3.SmoothDamp(_moveAmount, moveDirection * movementSpeed, ref _smoothMoveVelocity, _smoothTime);
        }

        private void Jump()
        {
            if (Input.GetKeyDown(KeyCode.Space) && _grounded)
            {
                _rigidbody.AddForce(Vector3.up * _jumpForce);
            }
        }

        private void EquipItem(int index)
        {
            if (index == _previousItemIndex) return;

            _itemIndex = index;

            _weapons[index].ItemGameObject.SetActive(true);

            if (_previousItemIndex != -1)
            {
                _weapons[_previousItemIndex].ItemGameObject.SetActive(false);
            }

            _previousItemIndex = _itemIndex;

            if (_photonView.IsMine)
            {
                Hashtable hashtable = new Hashtable { { ITEM_INDEX_KEY, _itemIndex } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
            }
        }

        private void SwitchWeapon()
        {
            for (int index = 0; index < _weapons.Length; index++)
            {
                if (Input.GetKeyDown((index + 1).ToString()))
                {
                    EquipItem(index);
                    break;
                }
            }

            if (Input.GetAxisRaw(MOUSE_SCROLLWHEEL_NAME) > 0f)
            {
                if (_itemIndex >= _weapons.Length - 1)
                {
                    EquipItem(0);
                }
                else
                {
                    EquipItem(_itemIndex + 1);
                }
            }
            else if (Input.GetAxisRaw(MOUSE_SCROLLWHEEL_NAME) < 0f)
            {
                if (_itemIndex <= 0)
                {
                    EquipItem(_weapons.Length - 1);
                }
                else
                {
                    EquipItem(_itemIndex - 1);
                }
            }
        }

        private void UseWeapon()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _weapons[_itemIndex].Use(_playerTeammates);
            }
        }

        private void Die()
        {
            _playerManager.Die();
        }

        [PunRPC]
        private void RPC_TakeDamage(float damage, PhotonMessageInfo photonMessageInfo)
        {
            _currentHealth -= damage;

            _healthBar.value = _currentHealth / MAX_HEALTH;

            if (_currentHealth <= 0.0f)
            {
                Die();
                PlayerManager.Find(photonMessageInfo.Sender).GetKill();
            }
        }
        #endregion

        #region Pun Callbacks
        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (changedProps.ContainsKey(ITEM_INDEX_KEY) && !_photonView.IsMine && targetPlayer == _photonView.Owner)
            {
                EquipItem((int)changedProps[ITEM_INDEX_KEY]);
            }
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            if (_teamsManager.TryGetTeamMatesOfPlayer(PhotonNetwork.LocalPlayer, out Player[] teammates))
            {
                _playerTeammates = teammates;
            }
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (_teamsManager.TryGetTeamMatesOfPlayer(PhotonNetwork.LocalPlayer, out Player[] teammates))
            {
                _playerTeammates = teammates;
            }
        }
        #endregion

        public void ResetPlayerStats()
        {
            EquipItem(0);

            _currentHealth = MAX_HEALTH;
            _healthBar.value = _currentHealth / MAX_HEALTH;
        }

        public void SetGroundedState(bool grounded)
        {
            _grounded = grounded;
        }

        public void TakeDamage(float damage)
        {
            _photonView.RPC(nameof(RPC_TakeDamage), _photonView.Owner, damage);
        }
    }
}