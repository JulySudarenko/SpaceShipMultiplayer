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
        [SerializeField] private string playerName;

       // private Dictionary<int, ShipController> _players = new Dictionary<int, ShipController>();

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader message)
        {
            var spawnTransform = GetStartPosition();
            var player = Instantiate(playerPrefab, spawnTransform.position, spawnTransform.rotation);
            _shipController = player.GetComponent<ShipController>();
            playerName = _nameInputField.text;
            _shipController.PlayerName = $"Player{conn.connectionId}";;
            //_players.Add(conn.connectionId, _shipController);
            
            if (message != null)
            {
                playerName = message.ReadMessage<StringMessage>().value;
                if (!string.IsNullOrEmpty(playerName))
                {
                    _shipController.PlayerName = playerName;
                }
            }
            
            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        }

        // public override void OnStartServer()
        // {
        //     base.OnStartServer();
        //
        //     NetworkServer.RegisterHandler(100, ReceiveName);
        // }
        //
        //
        // public void ReceiveName(NetworkMessage networkMessage)
        // {
        //     //ShipController shipController = _players[networkMessage.conn.connectionId];
        //     var nameMessage = networkMessage.ReadMessage<StringMessage>();
        //     //shipController.PlayerName = nameMessage.value;
        //     //_shipController.gameObject.name = nameMessage.value;
        //     
        //     if (networkMessage != null)
        //     {
        //         var playerName = networkMessage.ReadMessage<StringMessage>().value;
        //         if (!string.IsNullOrEmpty(playerName))
        //         {
        //             _shipController.PlayerName = playerName;
        //         }
        //     }
        // }

        public override void OnClientConnect(NetworkConnection conn)
        {
            //base.OnClientConnect(conn);
            var message = new StringMessage();
            message.value = _nameInputField.text;
            //conn.Send(100, message);
 
            
            ClientScene.AddPlayer(conn, 0, message);
        }
        public override void OnClientSceneChanged(NetworkConnection conn)
        {
        }
    }
}
