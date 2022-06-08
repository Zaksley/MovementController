
using UnityEngine;
using UnityEngine.Events;

public class MovementController : MonoBehaviour
{
	[SerializeField] private float m_JumpForce = 400f;							// Amount of force added when the player jumps.
	[Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;			// Amount of maxSpeed applied to crouching movement. 1 = 100%
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// How much to smooth out the movement
	[SerializeField] private bool m_AirControl = false;							// Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;							// A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;							// A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_CeilingCheck;							// A position marking where to check for ceilings
	[SerializeField] private Collider2D m_CrouchDisableCollider;				// A collider that will be disabled when crouching

	const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
	private bool _isGrounded;            // Whether or not the player is grounded.
	private bool _wasGrounded = true; 
	
	private bool _wasRunning = false;

	private Animator _animator;
	private Action _action; 
	private enum Action
	{
		IDLE = 0,
		CROUCH = 1, 
		JUMP = 2,
		MANDATORY_CROUNCH = 3,
	}; 
	
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 _Velocity = Vector3.zero;

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	public BoolEvent OnCrouchEvent;
	private bool _wasCrouching = false;

	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
		_animator = GetComponent<Animator>();
		_action = Action.IDLE; 

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();
	}

	private void FixedUpdate()
	{
		_wasGrounded = _isGrounded;
		_isGrounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				_isGrounded = true;
				if (!_wasGrounded)
					OnLandEvent.Invoke();
			}
		}
	}


	public void Move(float move, bool crouch, bool jump)
	{
		
		// If the character has a ceiling preventing them from standing up, keep them crouching
		if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
		{
			crouch = true;
			PriorityAction(Action.MANDATORY_CROUNCH);
		}


		if (jump && _action <= Action.JUMP)
			PriorityAction(Action.JUMP);
		
		
		if (_isGrounded || m_AirControl)
		{
			if (_action == Action.MANDATORY_CROUNCH || _action <= Action.CROUCH)
			{
				if (crouch)
				{
					if (!_wasCrouching)
					{
						_wasCrouching = true;
						_animator.SetBool("Crouch", true);
						//OnCrouchEvent.Invoke(true);
					}
				
					move *= m_CrouchSpeed;
					
					if (m_CrouchDisableCollider != null)
						m_CrouchDisableCollider.enabled = false;
				} 
				else
				{
					if (m_CrouchDisableCollider != null)
						m_CrouchDisableCollider.enabled = true;

					if (_wasCrouching)
					{
						_wasCrouching = false;
						_animator.SetBool("Crouch", false);
						OnCrouchEvent.Invoke(true);
					}
				}	
			}

			if (move == 0)
			{
				if (_wasRunning)
				{
					_animator.SetBool("Running", false);
					_wasRunning = false;
				}
			}
			else
			{
				if (!_wasRunning)
				{
					_animator.SetBool("Running", true);
					_wasRunning = true; 
				}
			}
		}
		
		Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
		m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref _Velocity, m_MovementSmoothing);
		_animator.SetFloat("JumpVelocity", m_Rigidbody2D.velocity.y); 
		

		
		
		if (_isGrounded && jump && _action <= Action.JUMP)
		{
			// Add a vertical force to the player.
			_isGrounded = false;
			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
			_animator.SetBool("Grounded", false);
		}
		
		if ( (move > 0 && !m_FacingRight) || (move < 0 && m_FacingRight))
		{
			Flip();
		}

	}


	private void Flip()
	{
		m_FacingRight = !m_FacingRight;
		
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	private void PriorityAction(Action checkPriority)
	{
		_action = (_action > checkPriority) ? _action : checkPriority; 
	}

	public void PriorityReset()
	{
		_action = Action.IDLE; 
	}

	public void Land()
	{
		_animator.SetBool("Grounded", true);
		PriorityReset();
	}
	
}