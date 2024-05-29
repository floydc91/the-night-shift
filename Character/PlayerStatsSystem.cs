using CSE5912.PenguinProductions.Networking;
using CSE5912.PenguinProductions.Player;
using CSE5912.PenguinProductions.Utility;
using StarterAssets;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CSE5912.PenguinProductions
{
    public class PlayerStatsSystem : ExtendedNetworkBehaviour, IDataPersistable
    {
        [Header("Player Stats")]
        public float MAX_HEALTH = 250;
        public float MAX_STAMINA = 100;
        [Tooltip("Amount of time player has before they become paranoid")]
        public float _paranoiaTimer;


        [Header("Multipliers")]
        [Tooltip("The sprint multipler increases how much the stamina decreases when the player sprints")]
        [SerializeField]
        private float _sprintMultiplier;
        [SerializeField]
        [Tooltip("The jump multipler increases how much the stamina decreases when the player jumps")]
        private float _jumpMultiplier;
        [SerializeField]
        [Tooltip("The stamina regeneration multipler increases the rate how much the stamina increases")]
        private float _StaminaRegen;

        [Header("Paranoia")]
        public AudioClip heartbeatAudio;
        private GameObject ParanoiaOverlay { get { return UIManager.Singleton.paranoiaOverlay.gameObject; }
                                             set { Debug.Log("don't set this. will change later -Hayden"); } }
        public AudioClip[] paranoiaSounds;
        public float _paranoiaDuration;
        public float _paranoiaFadespeed;
        public float _paranoiaDurationtimer;

        [Header("Misc")]
        public PlayerMovementInputHandling playerMovement;
        public StatusHubController statsSystemController;
        [SerializeField] private AudioClip _damageSound;

        public float _currentHealth;
        public float _currentStamina;
        public float _currentParanoia;

        private float _staminaTimer = 0.0f;
        private readonly float _regenerationDelay = 1.5f;
        private bool _monsterSoundPlaying = false;
        private Image paranoia_overlay;
        //private int _paranoiaAudioSourceId = -1;

        public bool CanSprint { get; private set; }
        public bool CanJump { get; private set; }
        public bool CanMove { get; private set; }
        public bool IsParanoid { get; private set; }
        public bool IsDead { get; private set; }
        public float PlayerMoney { get; set; }
        public bool IsEnticed { get; set; }

        public bool InMansion { get { return _inMansion; } set { _inMansion = value; ParanoiaOverlay.SetActive(value); } }
        private bool _inMansion = false;

        public override void OnNetworkSpawn()
        {
            //Sets booleans, damage overlay, current health and stamina
            CanJump= true;
            CanSprint = true;
            CanMove = true;
            IsParanoid = false;
            IsDead = false;

            this._currentHealth = MAX_HEALTH;
            this._currentStamina = MAX_STAMINA;
            this._currentParanoia = _paranoiaTimer;

            //connects these stats to the local player and gets the player movement
            if (IsLocalPlayer)
            {
                StatusHubController.LocalPlayer = gameObject;
                if (StatusHubController.PlayerStatSytem != null)
                {
                    StatusHubController.PlayerStatSytem.TryGetComponent(out StatusHubController controller);
                    controller.RegisterPlayer(gameObject);
                }
            }

            playerMovement = InputManager.Singleton.GetComponent<PlayerMovementInputHandling>();
        }

        /// <summary>
        /// Unsubscribe from events.
        /// </summary>
        public override void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnTruckRespawn;
            base.OnDestroy();
        }

        public void SetStatusSystemyController(StatusHubController controller)
        {
            if (statsSystemController == null)
            {
                statsSystemController = controller;
            }
        }

        /// <summary>
        /// Damages the player by the given amount.
        /// </summary>
        public void Damage(int damage)
        {
            if (IsDead) return; // Shouldn't damage a dead player
            if (IsLocalPlayer) DamageLogic(damage); // Local damage, no need to network
            else if (IsServer) DamageClientRpc(damage); // Server tells client to take damage.
            // There should never be a case where the client needs to tell another client to take damage.
        }

        /// <summary>
        /// Code to run on the clients to take damage.
        /// </summary>
        /// <param name="damage">Damage taken.</param>
        [ClientRpc]
        private void DamageClientRpc(int damage)
        {
            if (!IsLocalPlayer) return; // Damage is not meant for this client
            if (IsDead) return; // Shouldn't damage a dead player
            DamageLogic(damage);
        }

        /// <summary>
        /// Logic for taking damage.
        /// </summary>
        /// <param name="damage">Damage taken.</param>
        private void DamageLogic(int damage)
        {
            AudioManager.Singleton.PersistentPlayOneShot(_damageSound, AudioManager.AudioGroup.FX);
            this._currentHealth = Mathf.Clamp(this._currentHealth - damage, 0, MAX_HEALTH);
            DataPersistenceManager.Singleton.Save(SaveKeys.GetHealthKey(), _currentHealth);
            if (statsSystemController != null)
            {
                this.statsSystemController.UpdateGUI(this._currentHealth, this.MAX_HEALTH);
            }
            if (_currentHealth == 0)
            {
                Dead();
            }
            UIManager.Singleton.damageOverlay.OverlayFadeOut();
        }

        //Increases the approiate player stats resulting from drinking a potion
        public void Recover(int _recovery)
        {
            this._currentHealth = Mathf.Clamp(this._currentHealth + _recovery, 0, MAX_HEALTH);
            this.statsSystemController.UpdateGUI(this._currentHealth, this.MAX_HEALTH);
            DataPersistenceManager.Singleton.Save(SaveKeys.GetHealthKey(), _currentHealth);
        }
        public void IncreaseStamina(int _increaseEnergy)
        {
            this._currentStamina = Mathf.Clamp(this._currentStamina + _increaseEnergy, 0, MAX_STAMINA);
            this.statsSystemController.UpdateGUIStamina(this._currentStamina, MAX_STAMINA);
            DataPersistenceManager.Singleton.Save(SaveKeys.GetStaminaKey(), _currentStamina);
        }
        public void DecreaseParanoia(int value)
        {
            this._currentParanoia += value;
            this._currentParanoia = Mathf.Clamp(this._currentParanoia +(value* Time.deltaTime), 0.0f, this._paranoiaTimer);
            DataPersistenceManager.Singleton.Save(SaveKeys.GetSanityKey(), _currentParanoia);
        }

        /// <summary>
        /// controls the player's stamina when sprinting and jumping 
        /// </summary>
        public void Stamina()
        {
            if (statsSystemController != null)
            {
                this.statsSystemController.UpdateGUIStamina(this._currentStamina, MAX_STAMINA);
            }
            if (playerMovement.IsSprinting)
            {
                _currentStamina = Mathf.Clamp(_currentStamina - (_sprintMultiplier* Time.deltaTime), 0.0f, MAX_STAMINA);
                _staminaTimer = 0.0f;
            }
            else if (playerMovement.IsJumping)
            {
                _currentStamina = Mathf.Clamp(_currentStamina - (_jumpMultiplier* Time.deltaTime), 0.0f, MAX_STAMINA);
                _staminaTimer = 0.0f;
            }
            else if (_currentStamina < MAX_STAMINA)
            {
                if (_staminaTimer >= _regenerationDelay) _currentStamina = Mathf.Clamp(_currentStamina + (_StaminaRegen * Time.deltaTime), 0.0f, MAX_STAMINA);
                else _staminaTimer += Time.deltaTime;
            }
        }

        /// <summary>
        /// Decreases the timer until the player is paranoid
        /// </summary>
        public void Paranoia()
        {
            if (statsSystemController != null)
            {
                this.statsSystemController.StartParanoiaGUI(this._currentParanoia);
            }
            if (InMansion && !IsParanoid)
            {
                this._currentParanoia = Mathf.Clamp(this._currentParanoia - Time.deltaTime, 0.0f, this._paranoiaTimer);
                if (this._currentParanoia <= 0.0f)
                {
                    Debug.Log("Triggering Paranoia Event");
                    _paranoiaDurationtimer = 0.0f;
                    //paranoia_overlay.color = new Color(0, 0, 0, 0);
                    ParanoiaEvent();
                    IsParanoid = true;
                                      
                }
            }
            else if (!InMansion)
            {
                this._currentParanoia = Mathf.Clamp(this._currentParanoia + Time.deltaTime, 0.0f, this._paranoiaTimer);
                if (this._currentParanoia > 0.0f)
                {
                    if (IsParanoid)
                    {
                        //if (_paranoiaAudioSourceId != -1) AudioManager.Singleton.StopPersistentPlay(_paranoiaAudioSourceId, 0.5f);
                    }
                    IsParanoid = false;
                }
            }
            if (UIManager.Singleton != null)UIManager.Singleton.SanityEffects.UpdateSanityEffects(_currentParanoia, _paranoiaTimer);
            //if (IsParanoid && !_monsterSoundPlaying && InMansion) StartCoroutine(RandomSounds());
        }

        /// <summary>
        /// Events and effects when the player is paranoid
        /// </summary>
        private void ParanoiaEvent()
        {
            if (!_monsterSoundPlaying) AudioManager.Singleton.PersistentPlayOneShot(paranoiaSounds[0], AudioManager.AudioGroup.FX);
            _monsterSoundPlaying = true;
            if(_paranoiaDurationtimer>10f) AudioManager.Singleton.PersistentPlayOneShot(paranoiaSounds[2], AudioManager.AudioGroup.FX);
            StartCoroutine(SuddenDeath());
            _paranoiaDurationtimer += Time.deltaTime;

        }

        private IEnumerator SuddenDeath()
        {
           yield return new WaitForSeconds(Random.Range(15, 30));          
           UIManager.Singleton.SanityEffects.ToggleSanityDeath();
           AudioManager.Singleton.PersistentPlayOneShot(paranoiaSounds[1], AudioManager.AudioGroup.FX);
           StartCoroutine(ParanoiaDelayedDeath());
        }
        private IEnumerator ParanoiaDelayedDeath()
        {
            
            yield return new WaitForSeconds(3.5f);
            UIManager.Singleton.SanityEffects.ToggleSanityDeath();
            _currentHealth = 0;
            Dead();
            
        }
        //Set character states to see if movements are limited or if they are dead
        public void CheckPlayerstats()
        {
            if (this._currentHealth <= 0) IsDead = true;

            if (this._currentStamina > 0)
            {
                CanSprint = true;
                CanJump = true;
            }
            else
            {
                CanSprint = false;
                CanJump = false;
            }
            
            if (IsEnticed)
            {
                CanMove = false;
                CanSprint = false;
                CanJump = false;

            } else
            {
                CanMove = true;
            }
        }

/*        public void ParanoiaOverlayGUI()
        {
            if (paranoia_overlay == null) return;

            if (IsParanoid && paranoia_overlay.color.a < 1)
            {
                _paranoiaDurationtimer += Time.deltaTime;                            
                var tempalpha = _paranoiaDurationtimer * _paranoiaFadespeed;
                paranoia_overlay.color = new Color(paranoia_overlay.color.r, paranoia_overlay.color.g, paranoia_overlay.color.b, tempalpha);
            }
        }*/

        public void Dead()
        {
            IsDead = true;
            GetComponentInChildren<PlayerNetworkAnimator>().SetBool("isDead", true);
            gameObject.GetComponent<FirstPersonController>().enabled = false;
            NetworkSetLayer((int)LayerEnum.Water);
            SceneManager.sceneLoaded += OnTruckRespawn;
            GameManager.Singleton.KillPlayer();
        }

        /// <summary>
        /// Respawns the dead player when the truck scene is loaded
        /// </summary>
        private void OnTruckRespawn(Scene arg0, LoadSceneMode arg1)
        {
            if (arg0.name == SceneTransitionManager.ToString(SceneTransitionManager.SceneName.TRUCK))
            {
                // Only need to respawn once.
                SceneManager.sceneLoaded -= OnTruckRespawn;

                if (GameManager.Singleton.ActiveState != GameManager.GameState.DEAD) return;

                // Reverse death code
                IsDead = false;
                GetComponentInChildren<PlayerNetworkAnimator>().SetBool("isDead", false);
                gameObject.GetComponent<FirstPersonController>().enabled = true;
                NetworkSetLayer((int)LayerEnum.Player);

                // Reset stats
                _currentHealth = MAX_HEALTH;
                _currentParanoia = _paranoiaTimer;
                _currentStamina = MAX_STAMINA;
                PlayerMoney = 0f;
                statsSystemController.UpdateGUI(_currentHealth, MAX_HEALTH);
                statsSystemController.UpdateGUIStamina(_currentStamina, MAX_STAMINA);

                IsParanoid = false;
                InMansion = false;

                DataPersistenceManager.Singleton.Save((SaveKeys.GetHealthKey(), _currentHealth), (SaveKeys.GetStaminaKey(), _currentStamina), 
                    (SaveKeys.GetSanityKey(), _currentParanoia), (SaveKeys.GetMoneyKey(), PlayerMoney));
                GameManager.Singleton.RespawnPlayer();
            }
        }

        public void AddMoney(float bread)
        {
            PlayerMoney += bread;
            DataPersistenceManager.Singleton.Save(SaveKeys.GetMoneyKey(), PlayerMoney);
        }

        public void RemoveMoney(float bread)
        {
            PlayerMoney -= bread;
            if (PlayerMoney < 0) Debug.Log("Player money went below $0.");
            DataPersistenceManager.Singleton.Save(SaveKeys.GetMoneyKey(), PlayerMoney);
        }

        public void Update()
        {
            if (GameManager.Singleton.ActiveState == GameManager.GameState.PLAYING || GameManager.Singleton.ActiveState == GameManager.GameState.MESSAGE_BOX)
            {
                CheckPlayerstats();
                Stamina();
                //ParanoiaOverlayGUI();
                Paranoia();

                DataPersistenceManager.Singleton.Save((SaveKeys.GetStaminaKey(), _currentStamina), (SaveKeys.GetSanityKey(), _currentParanoia));
            }
        }

        public DataPersistenceManager.SaveType GetSaveType() => DataPersistenceManager.SaveType.PLAYERSTATS;

        public void LoadData(SaveData data)
        {
            if (data.Get(SaveKeys.GetHealthKey(), out float amount)) _currentHealth = amount;
            if (data.Get(SaveKeys.GetStaminaKey(), out amount)) _currentStamina = amount;
            if (data.Get(SaveKeys.GetSanityKey(), out amount)) _currentParanoia = amount;
            if (data.Get(SaveKeys.GetMoneyKey(), out amount)) PlayerMoney = amount;
        }
    }
}
