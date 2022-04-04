using System.Collections.Generic;
using Characters;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;

namespace Main
{
    public class SolarSystemNetworkManager : NetworkManager
    {
        [SerializeField] private InputField _nameInputField;
        private string _playerName;

        private Dictionary<int, ShipController> _players = new Dictionary<int, ShipController>();

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
        {
            var spawnTransform = GetStartPosition();
            var player = Instantiate(playerPrefab, spawnTransform.position, spawnTransform.rotation);
            var shipController = player.GetComponent<ShipController>();
            _playerName = _nameInputField.text;
            shipController.PlayerName = _playerName;

            if (!_players.ContainsKey(conn.connectionId))
            {
                Debug.Log($"New connection = {conn.connectionId}");
                _players.Add(conn.connectionId, shipController);
            }
            else
            {
                Debug.Log($"Old connection = {conn.connectionId}");
            }

            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            NetworkServer.RegisterHandler(100, ReceiveName);
        }

        public void ReceiveName(NetworkMessage networkMessage)
        {
            var nameMessage = networkMessage.reader.ReadString();
            Debug.Log(" Message string='" + nameMessage + "'");
            ShipController shipController = _players[networkMessage.conn.connectionId];
            shipController.PlayerName = nameMessage;
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
            var message = new StringMessage();
            message.value = _nameInputField.text;
            conn.Send(100, message);
        }

        public override void OnClientSceneChanged(NetworkConnection conn)
        {
        }
    }
}
