using Cinemachine;
using CSE5912.PenguinProductions.Player;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace CSE5912.PenguinProductions
{
    public class InitPlayer : NetworkBehaviour
    {
        [SerializeField] private GameObject playerModel;
        [SerializeField] private PlayerNetworkAnimator playerNetworkAnimator;
        [SerializeField] private CinemachineVirtualCamera _playerCamera;

        private PauseMenu _pauseMenu;

        private void Start()
        {
            if (GameManager.Singleton.IsCoop)
            {
                StartCoroutine(WaitForUI());
            }
        }

        private IEnumerator WaitForUI()
        {
            yield return new WaitForSeconds(0.5f);
            GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
            VivoxManager.Singleton.GetProximityChatController().SetPlayerCamera(camera);
        }


        public override void OnNetworkSpawn()
        {
            // Create player model
            var instance = Instantiate(playerModel, transform);
            Animator playerAnimator = instance.GetComponent<Animator>();
            _pauseMenu = instance.GetComponentInChildren<PauseMenu>();

            playerNetworkAnimator.SetAnimator(playerAnimator);
            _pauseMenu.PlayerNetworkAnimator = playerNetworkAnimator;

            if (IsOwner)
            {
                StartCoroutine(DistributePlayerObjectRoutine());
                gameObject.SetActive(false);
                gameObject.transform.position = new(-8, 0, 17);
                gameObject.SetActive(true);
                GameManager.Singleton.LocalPlayer = gameObject;
                GameManager.Singleton.LocalPlayerCamera = _playerCamera;
                _playerCamera.Priority = 0;
                _pauseMenu.IsLocalPauseMenu = true;
                gameObject.transform.Find(NameTagBillboard.TransformPath).gameObject.GetComponent<NameTagBillboard>().SetEnabled(false);

                if (NetworkManager.Singleton.IsHost) DataPersistenceManager.Singleton.LoadAsHost();
                StartCoroutine(FadeInRoutine());
            }
            else _playerCamera.Priority = -1;

            base.OnNetworkSpawn();
        }

        private IEnumerator FadeInRoutine() 
        {
            while (UIManager.Singleton == null) yield return null;
            UIManager.Singleton.PauseMenu = _pauseMenu;
            UIManager.Singleton.FadeInScene();
        }

        private IEnumerator DistributePlayerObjectRoutine() 
        {
            while (!ExtendedNetworkManager.Singleton.ConnectionOpen) yield return null;
            DistributePlayerObjectsServerRpc(NetworkManager.Singleton.LocalClientId, gameObject);
        }

        [ServerRpc]
        private void DistributePlayerObjectsServerRpc(ulong clientId, NetworkObjectReference playerObject) 
        {
            foreach ((ulong u, GameObject g) in ExtendedNetworkManager.Singleton.ClientObjects) 
            {
                InitializeClientObjectClientRpc(clientId, u, g);
            }
            DistributePlayerObjectClientRpc(clientId, playerObject);
        }

        [ClientRpc]
        private void DistributePlayerObjectClientRpc(ulong clientId, NetworkObjectReference playerObject) 
        {
            ExtendedNetworkManager.Singleton.ClientObjects.Add(clientId, playerObject);
        }

        [ClientRpc]
        private void InitializeClientObjectClientRpc(ulong targetClientId, ulong clientId, NetworkObjectReference clientObject) 
        {
            if (NetworkManager.Singleton.LocalClientId != targetClientId) return;
            ExtendedNetworkManager.Singleton.ClientObjects.Add(clientId, clientObject);
        }
    }
}
