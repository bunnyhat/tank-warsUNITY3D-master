using UnityEngine;
using System.Collections;

public class PlayerControllerScript : MonoBehaviour {
    
    public GameObject headlightR, headlightL;
    public float forwardSpeed;
	public float turnSpeed;		//Turn speed defined by degree per second.
	public float playerNumber = 1;

	private string movementAxisName;
	private string turnAxisName;
	private float movementInputValue;
	private float turnInputValue;
    private bool isHeadlightOn;
    
    Rigidbody m_rigidbody;

	void Awake() {
		m_rigidbody = GetComponent<Rigidbody>();
	}

	// Use this for initialization
	void Start () {
		movementAxisName = "Vertical" + playerNumber;
		turnAxisName = "Horizontal" + playerNumber;
	}
	
	// Update is called once per frame
	void Update () {      
		//Controls
		movementInputValue = Input.GetAxis(movementAxisName);
		turnInputValue = Input.GetAxis(turnAxisName);
        
        if(headlightL.activeInHierarchy && headlightR.activeInHierarchy) {
            isHeadlightOn = true;
        }
	    
        if(Input.GetKeyDown(KeyCode.Q) && isHeadlightOn == true) {
            headlightL.SetActive(false);
            headlightR.SetActive(false);
            isHeadlightOn = false;
        } else if(Input.GetKeyDown(KeyCode.Q) && isHeadlightOn == false) {
            headlightL.SetActive(true);
            headlightR.SetActive(true);
            isHeadlightOn = true;
        }
	}

	void FixedUpdate() {
		Move();
		
        if(Input.GetAxis(turnAxisName) != 0){
            Turn();
        }
        if(Input.GetAxis(turnAxisName) == 0){
            turnInputValue = 0;
            m_rigidbody.velocity = Vector3.zero;
            m_rigidbody.angularVelocity = Vector3.zero;
        }
	}

	private void Move() {
		//A Vector3 in the direction the tank is facing with a magnitude based on the input value.
		Vector3 movement = transform.forward * movementInputValue * forwardSpeed * Time.deltaTime;

		//Apply this movement to the rigidbody's position.
		m_rigidbody.MovePosition(m_rigidbody.position + movement);
	}

	private void Turn() {
		//Determine the number of degrees to be turned based on the input, speed and time between frames.
		float turn = turnInputValue * turnSpeed * Time.deltaTime;

		//Make this into a rotation in the Y axis.
		Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);

		//Apply this rotation to the rigidbody's rotation.
		m_rigidbody.MoveRotation(m_rigidbody.rotation * turnRotation);
	}

	private void OnEnable() {
		m_rigidbody.isKinematic = false;

		movementInputValue = 0f;
		turnInputValue = 0f;
	}

	private void OnDisable() {
		m_rigidbody.isKinematic = true;
        
	}
}
