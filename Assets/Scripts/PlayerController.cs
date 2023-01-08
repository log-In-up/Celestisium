using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

[DisallowMultipleComponent]
public class PlayerController : MonoBehaviourPunCallbacks, IDamageable
{
    [SerializeField] private Slider _healthBar = null;
    [SerializeField] private GameObject _cameraHolder = null, _ui = null;
    [SerializeField, Min(0.0f)] private float _mouseSensitivity = 3.0f, _smoothTime = 0.15f, _sprintSpeed = 6.0f, _walkSpeed = 3.0f, _jumpForce = 300.0f;
    [SerializeField] private Item[] _items = null;

    private bool _grounded;
    private float _verticalLookrotation, _currentHealth;
    private int _itemIndex, _previousItemIndex = -1;
    private Vector3 _smoothMoveVelocity, _moveAmount;

    private PlayerManager _playerManager = null;
    private Rigidbody _rigidbody = null;
    private PhotonView _photonView = null;

    private const string VERTICAL_MOUSE_AXIS_NAME = "Mouse Y", HORIZONTAL_MOUSE_AXIS_NAME = "Mouse X",
        VERTICAL_AXIS_RAW = "Vertical", HORIZONTAL_AXIS_RAW = "Horizontal",
        MOUSE_SCROLLWHEEL_NAME = "Mouse ScrollWheel", ITEM_INDEX_KEY = "_itemIndex";

    private const float VERTICAL_MIN = -90.0f, VERTICAL_MAX = 90.0f, MAX_HEALTH = 100.0f;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _photonView = GetComponent<PhotonView>();

        _playerManager = PhotonView.Find((int)_photonView.InstantiationData[0]).GetComponent<PlayerManager>();
    }

    private void Start()
    {
        if (_photonView.IsMine)
        {
            EquipItem(0);

            _currentHealth = MAX_HEALTH;
            _healthBar.value = _currentHealth / MAX_HEALTH;
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

        Look();
        Move();
        Jump();

        SwitchWeapon();
        UseItem();
    }

    private void FixedUpdate()
    {
        if (!_photonView.IsMine) return;

        _rigidbody.MovePosition(_rigidbody.position + (transform.TransformDirection(_moveAmount) * Time.fixedDeltaTime));
    }

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

        _items[index].ItemGameObject.SetActive(true);

        if (_previousItemIndex != -1)
        {
            _items[_previousItemIndex].ItemGameObject.SetActive(false);
        }

        _previousItemIndex = _itemIndex;

        if (_photonView.IsMine)
        {
            Hashtable hashtable = new Hashtable
            {
                { ITEM_INDEX_KEY, _itemIndex }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
        }
    }

    private void SwitchWeapon()
    {
        for (int index = 0; index < _items.Length; index++)
        {
            if (Input.GetKeyDown((index + 1).ToString()))
            {
                EquipItem(index);
                break;
            }
        }

        if (Input.GetAxisRaw(MOUSE_SCROLLWHEEL_NAME) > 0f)
        {
            if (_itemIndex >= _items.Length - 1)
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
                EquipItem(_items.Length - 1);
            }
            else
            {
                EquipItem(_itemIndex - 1);
            }
        }
    }

    private void UseItem()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _items[_itemIndex].Use();
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
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey(ITEM_INDEX_KEY) && !_photonView.IsMine && targetPlayer == _photonView.Owner)
        {
            EquipItem((int)changedProps[ITEM_INDEX_KEY]);
        }
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