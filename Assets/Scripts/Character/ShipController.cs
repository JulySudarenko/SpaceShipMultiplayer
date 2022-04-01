using System;
using System.Collections;
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

        [SyncVar] private string _playerName;
        //[SyncEvent] public event Action OnSomethingHappend;

        protected override float speed => _shipSpeed;

        public string PlayerName
        {
            get => _playerName;
            set => _playerName = value;
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

            gameObject.name = _playerName;
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
        }

        [Server]
        private void OnTriggerEnter(Collider other)
        {
            if (hasAuthority)
                OnObjectHit();
        }

        private void OnObjectHit()
        {
            DestroySpaceShip();
        }

        private IEnumerable DestroySpaceShip()
        {
            DeactivatePlayer();
            yield return new WaitForSeconds(5);
            ActivatePlayer();
        }

        [Server]
        private void DeactivatePlayer()
        {
            gameObject.SetActive(false);
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.isKinematic = true;
            RpcDeactivatePlayer();
        }

        [ClientRpc]
        private void RpcDeactivatePlayer()
        {
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.isKinematic = true;
            gameObject.SetActive(false);
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
        }

        protected override void FromServerUpdate()
        {
        }

        protected override void SendToServer()
        {
        }

        // [Command]
        // private void CmdCommandMethod()
        // {
        // }
        //
        // [ClientRpc]
        // private void RpcMethod(int value)
        // {
        //     _shipSpeed *= value;
        // }
        //
        // [Client]
        // private void ClientMethod()
        // {
        // }

        [ClientCallback]
        private void LateUpdate()
        {
            _cameraOrbit?.CameraMovement();
        }

        [Server]
        private void ServerMethod()
        {
        }

        [ServerCallback]
        private void ServerCalbackMethod()
        {
        }

        [TargetRpc]
        private void RpcTargetMethod()
        {
        }

        //[ServerCallback]
        // public void OnTriggerEnter(Collider other)
        // {
        //     if (hasAuthority)
        //     {
        //     }
        //     else
        //     {
        //     }
        // }
    }
}
