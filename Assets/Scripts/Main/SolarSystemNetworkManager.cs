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
        private ShipController _shipController;
        private string _playerName;

        // private Dictionary<int, ShipController> _players = new Dictionary<int, ShipController>();

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
        {
            var spawnTransform = GetStartPosition();
            var player = Instantiate(playerPrefab, spawnTransform.position, spawnTransform.rotation);
            _shipController = player.GetComponent<ShipController>();
            _playerName = _nameInputField.text;
            _shipController.PlayerName = _playerName;
            //_players.Add(conn.connectionId, _shipController);

            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            
            NetworkServer.RegisterHandler(100, ReceiveName);
        }

        public void ReceiveName(NetworkMessage networkMessage)
        {
            Debug.Log("Got message, size=" + networkMessage.reader.Length);
            //var someValue = networkMessage.reader.ReadInt32();
            var someString = networkMessage.reader.ReadString();
            Debug.Log("Message value=" +  " Message string='" + someString + "'");
            //ShipController shipController = _players[networkMessage.conn.connectionId];
            //var nameMessage = networkMessage.ReadMessage<StringMessage>();
            //shipController.PlayerName = nameMessage.value;
            //_shipController.gameObject.name = nameMessage.value;
        }

        private void SendMessage(NetworkMessage netmsg)
        {
            NetworkWriter writer = new NetworkWriter();
            writer.StartMessage(100);
            writer.Write(42);
            writer.Write("What is the answer");
            writer.FinishMessage();
            client.SendWriter(writer, 0);
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
            var message = new StringMessage();
            message.value = _nameInputField.text;
            conn.Send(100, message);
            //ClientScene.AddPlayer(conn, 0, message);

            client.RegisterHandler(100, SendMessage);
        }

        public override void OnClientSceneChanged(NetworkConnection conn)
        {
        }
    }
}
