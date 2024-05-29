using CSE5912.PenguinProductions;
using CSE5912.PenguinProductions.Player;
using Unity.Netcode;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerStatsSystem))]
    public class FirstPersonController : NetworkBehaviour
	{
		[Header("Player")]
		[Tooltip("Move speed of the character in m/s")]
		public float MoveSpeed = 4.0f;
		[Tooltip("Sprint speed of the character in m/s")]
		public float SprintSpeed = 8.0f;
        [Tooltip("Crpuch speed of the character in m/s")]
        public float CrouchSpeed = 2.0f;
        [Tooltip("Rotation speed of the character")]
		public float RotationSpeed = 1.0f;
		[Tooltip("Acceleration and deceleration")]
		public float SpeedChangeRate = 10.0f;

		[Space(10)]
		[Tooltip("The height the player can jump")]
		public float JumpHeight = 1.2f;
		[Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
		public float Gravity = -15.0f;

		[Space(10)]
		[Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
		public float JumpTimeout = 0.1f;
		[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
		public float FallTimeout = 0.15f;

		[Header("Player Grounded")]
		[Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
		public bool Grounded = true;
		[Tooltip("Useful for rough ground")]
		public float GroundedOffset = -0.14f;
		[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
		public float GroundedRadius = 0.5f;
		[Tooltip("What layers the character uses as ground")]
		public LayerMask GroundLayers;

		[Header("Cinemachine")]
		[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
		public GameObject CinemachineCameraTarget;
		[Tooltip("How far in degrees can you move the camera up")]
		public float TopClamp = 90.0f;
		[Tooltip("How far in degrees can you move the camera down")]
		public float BottomClamp = -70.0f;
        [Tooltip("Camera's height while player is crouching")]
        private Vector3 crouchHeight;
        private Vector3 normalHeight;

        // cinemachine
        private float _cinemachineTargetPitch;

		// player
		private float _speed;
        private float _animationBlend;
        private float _rotationVelocity;
		private float _verticalVelocity;
		private readonly float _terminalVelocity = 53.0f;

		// timeout deltatime
		private float _jumpTimeoutDelta;
		private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;
        private int _animIDMotionVelocityX;
        private int _animIDMotionVelocityZ;
        private int _animIDCrouch;
		private int _animIDEmote;
		private int _animIDEmoteIndex;

		private PlayerNetworkAnimator _animator;
        private CharacterController _controller;
        private PlayerMovementInputHandling _playerInput;
		private PlayerStatsSystem _playerStats;
        private GameObject _mainCamera;

        private bool _hasAnimator;

        private const float _threshold = 0.01f;

        public override void OnNetworkSpawn()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
				//Find MainCamera in scene
				_mainCamera = GameObject.FindWithTag("MainCamera");
            }

            crouchHeight = new Vector3(0, CinemachineCameraTarget.transform.position.y - .5f, 0.5f);
            normalHeight = CinemachineCameraTarget.transform.position;

			// Rest is only set up if its local player.
            if (!IsOwner) { return; }

            _animator = GetComponentInChildren<PlayerNetworkAnimator>();
            if (_animator != null) _hasAnimator = true;
            _controller = GetComponent<CharacterController>();
			_playerInput = InputManager.Singleton.GetComponent<PlayerMovementInputHandling>();
            _playerStats = GetComponent<PlayerStatsSystem>();

            AssignAnimationIDs();

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
			_fallTimeoutDelta = FallTimeout;

			//VivoxPlayer vp = GameObject.Find("Vivox").GetComponent<VivoxPlayer>();
			//vp.JoinChannelAsync(vp.VoiceChannelName);
		}

        private void Awake()
        {
			DontDestroyOnLoad(gameObject);
        }

        private void Update()
		{
            if (_animator != null) _hasAnimator = true;
            if ( !IsOwner)
            {
				return;
            }
            GroundedCheck();
            JumpAndGravity();
			Move();
			Emote();
            _hasAnimator = false;
        }

		private void LateUpdate()
		{
			if (!IsOwner) return;
			CameraRotation();
		}

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
            _animIDMotionVelocityX = Animator.StringToHash("VelocityX");
            _animIDMotionVelocityZ = Animator.StringToHash("VelocityZ");
            _animIDEmote = Animator.StringToHash("Emote");
			_animIDEmoteIndex = Animator.StringToHash("EmoteIndex");
        }

        private void GroundedCheck()
		{
			// set sphere position, with offset
			Vector3 spherePosition = new(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
			Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }

		private void CameraRotation()
		{
			// if there is an input
			if (_playerInput.LookDirection.sqrMagnitude >= _threshold)
			{
				//Don't multiply mouse input by Time.deltaTime
				float deltaTimeMultiplier = _playerInput.CurrentDevice is Mouse ? 1.0f : Time.deltaTime;
				
				_cinemachineTargetPitch += _playerInput.LookDirection.y * RotationSpeed * UserOptions.Singleton.LookSensitivity.ValueDenormalized / 10 * deltaTimeMultiplier;
				_rotationVelocity = _playerInput.LookDirection.x * RotationSpeed * UserOptions.Singleton.LookSensitivity.ValueDenormalized / 10 * deltaTimeMultiplier;

				// clamp our pitch rotation
				_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

				// Update Cinemachine camera target pitch
				CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

				// rotate the player left and right
				transform.Rotate(Vector3.up * _rotationVelocity);
			}
		}

		private void Move()
		{
			// set target speed based on move speed, sprint speed and if sprint is pressed
			float targetSpeed;
			if (_playerInput.IsSprinting && _playerStats.CanSprint) targetSpeed = SprintSpeed;
			else targetSpeed = MoveSpeed;
			
            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_playerInput.MoveDirection == Vector2.zero) targetSpeed = 0.0f;


			// a reference to the players current horizontal velocity
			float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

			float speedOffset = 0.1f;
			float inputMagnitude = 1f;

			// accelerate or decelerate to target speed
			if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
			{
				// creates curved result rather than a linear one giving a more organic speed change
				// note T in Lerp is clamped, so we don't need to clamp our speed
				_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

				// round speed to 3 decimal places
				_speed = Mathf.Round(_speed * 1000f) / 1000f;
			}
			else
			{
				_speed = targetSpeed;
			}
            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;
            // normalise input direction
            Vector3 inputDirection = new Vector3(_playerInput.MoveDirection.x, 0.0f, _playerInput.MoveDirection.y).normalized;

			// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is a move input rotate player when the player is moving
			if (_playerInput.MoveDirection != Vector2.zero)
			{
				// move
				inputDirection = transform.right * _playerInput.MoveDirection.x + transform.forward * _playerInput.MoveDirection.y;
			}

			// move the player
			_controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionVelocityX, _playerInput.MoveDirection.x * _animationBlend);
                _animator.SetFloat(_animIDMotionVelocityZ, _playerInput.MoveDirection.y * _animationBlend);
            }
        }

        private void Emote()
        {
            if (Grounded && _playerInput.IsEmote)
            {
                _animator.SetBool(_animIDEmote, true);
				_animator.SetFloat(_animIDEmoteIndex, _playerInput.EmoteIndex);
                UIManager.Singleton.InventoryController.SetSelectedItemHeld(false);
            }
            else
            {
                _animator.SetBool(_animIDEmote, false);
            }
        }

        private void JumpAndGravity()
		{
			if (Grounded && _playerStats.CanJump)
			{
				// reset the fall timeout timer
				_fallTimeoutDelta = FallTimeout;

                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
				{
					_verticalVelocity = -2f;
				}
                // Jump
                if (_playerInput.IsJumping && _jumpTimeoutDelta <= 0.0f && _playerStats.CanJump)
				{
					// the square root of H * -2 * G = how much velocity needed to reach desired height
					_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }

                }

				// jump timeout
				if (_jumpTimeoutDelta >= 0.0f)
				{
					_jumpTimeoutDelta -= Time.deltaTime;
				}
			}
			else
			{
				// reset the jump timeout timer
				_jumpTimeoutDelta = JumpTimeout;

				// fall timeout
				if (_fallTimeoutDelta >= 0.0f)
				{
					_fallTimeoutDelta -= Time.deltaTime;
				}
				else
				{
                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }


                // if we are not grounded, do not jump
                _playerInput.IsJumping = false;
			}

			// apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
			if (_verticalVelocity < _terminalVelocity)
			{
				_verticalVelocity += Gravity * Time.deltaTime;
			}
		}

		private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
		{
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}

		private void OnDrawGizmosSelected()
		{
			Color transparentGreen = new(0.0f, 1.0f, 0.0f, 0.35f);
			Color transparentRed = new(1.0f, 0.0f, 0.0f, 0.35f);

			if (Grounded) Gizmos.color = transparentGreen;
			else Gizmos.color = transparentRed;

			// when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
			Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
		}
    }
}
