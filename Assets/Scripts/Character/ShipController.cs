using System.Threading.Tasks;
using Crystals;
using Main;
using Mechanics;
using Network;
using UI;
using UnityEngine;
using UnityEngine.Networking;

namespace Characters
{
    public class ShipController : NetworkMovableObject
    {
        [SerializeField] private Transform _cameraAttach;
        private CameraOrbit _cameraOrbit;
        private PlayerLabel _playerLabel;
        private float _shipSpeed;
        private Rigidbody _rigidbody;
        private bool _isDestroy;

        [SyncVar] private string _playerName;

        protected override float speed => _shipSpeed;

        public string PlayerName
        {
            get => _playerName;
            set => _playerName = value;
        }

        private void Start()
        {
            gameObject.name = _playerName;
        }

        private void OnGUI()
        {
            if (_cameraOrbit == null)
                return;

            _cameraOrbit.ShowPlayerLabels(_playerLabel);
        }

        public override void OnStartAuthority()
        {
            _rigidbody = GetComponent<Rigidbody>();
            if (_rigidbody == null)
                return;

            _cameraOrbit = FindObjectOfType<CameraOrbit>();
            _cameraOrbit.Initiate(_cameraAttach == null ? transform : _cameraAttach);
            _playerLabel = GetComponentInChildren<PlayerLabel>();

            base.OnStartAuthority();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            Debug.Log("OnStartClient");
        }

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();

            Debug.Log("OnStartLocalPlayer");
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            Debug.Log("OnStartServer");
        }

        protected override void HasAuthorityMovement()
        {
            var spaceShipSettings = SettingsContainer.Instance?.SpaceShipSettings;
            if (spaceShipSettings == null)
                return;

            var isFaster = Input.GetKey(KeyCode.LeftShift);
            var speed = spaceShipSettings.ShipSpeed;
            var faster = isFaster ? spaceShipSettings.Faster : 1.0f;

            _shipSpeed = Mathf.Lerp(_shipSpeed, speed * faster, spaceShipSettings.Acceleration);

            var currentFov = isFaster ? spaceShipSettings.FasterFov : spaceShipSettings.NormalFov;
            _cameraOrbit.SetFov(currentFov, spaceShipSettings.ChangeFovSpeed);

            var velocity = _cameraOrbit.transform.TransformDirection(Vector3.forward) * _shipSpeed;
            _rigidbody.velocity =
                velocity * (_updatePhase == UpdatePhase.FixedUpdate ? Time.fixedDeltaTime : Time.deltaTime);

            if (!Input.GetKey(KeyCode.C))
            {
                var targetRotation =
                    Quaternion.LookRotation(Quaternion.AngleAxis(_cameraOrbit.LookAngle, -transform.right) * velocity);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speed);
            }

            CmdCommandMethod();

        }

        [Server]
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(typeof(Crystal), out var crystal))
            {
                other.gameObject.SetActive(false);
                RpcDeactivateCrystal(other.gameObject);
            }
            else
            {
                if (!_isDestroy)
                {
                    Debug.Log("HIT");
                    HitHandle();
                }
            }
        }

        private async void HitHandle()
        {
            DeactivatePlayer();
            await Task.Delay(1000);
            ActivatePlayer();
        }

        [Server]
        private void DeactivatePlayer()
        {
            _isDestroy = true;
            gameObject.SetActive(false);
            RpcDeactivatePlayer();
        }

        [ClientRpc]
        private void RpcDeactivatePlayer()
        {
            gameObject.SetActive(false);
        }
        
        [Server]
        private void RpcDeactivateCrystal(GameObject crystal)
        {
            crystal.SetActive(false);
            RpcDeactivateCrystalPlayer(crystal);
        }
        
        [ClientRpc]
        private void RpcDeactivateCrystalPlayer(GameObject crystal)
        {
            crystal.SetActive(false);
        }

        [Server]
        private void ActivatePlayer()
        {
            var startPosition = NetworkManager.singleton.GetStartPosition();
            transform.position = startPosition.position;
            transform.rotation = startPosition.rotation;
            gameObject.SetActive(true);
            RpcActivatePlayer(startPosition.position, startPosition.rotation);
        }

        [ClientRpc]
        private void RpcActivatePlayer(Vector3 position, Quaternion rotation)
        {
            var objectTransform = transform;
            objectTransform.position = position;
            objectTransform.rotation = rotation;
            gameObject.SetActive(true);
            _isDestroy = false;
        }

        protected override void FromServerUpdate()
        {
        }

        protected override void SendToServer()
        {
        }

        [Command]
        private void CmdCommandMethod()
        {
            gameObject.name = _playerName;
            RpcMethod();
        }

        [ClientRpc]
        private void RpcMethod()
        {
            gameObject.name = _playerName;
        }

        [ClientCallback]
        private void LateUpdate()
        {
            _cameraOrbit?.CameraMovement();
        }
    }
}
