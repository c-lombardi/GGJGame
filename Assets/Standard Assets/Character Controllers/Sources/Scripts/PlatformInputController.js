// This makes the character turn to face the current movement speed per default.
var autoRotate : boolean = true;
var maxRotationSpeed : float = 3600;


var JOYSTICK_STICKINESS : double = 0.24;

private var motor : CharacterMotor;

// Use this for initialization
function Awake () {
	motor = GetComponent(CharacterMotor);
}

// Update is called once per frame
function Update () {



	// Get the input vector from keyboard or analog stick

	var moveVector = new Vector3(Input.GetAxis("LeftHorizontal"), 0, Input.GetAxis("LeftVertical"));
	var directionVector = new Vector3(Input.GetAxis("RightHorizontal"), 0, Input.GetAxis("RightVertical"));
	
	if (Input.GetKey(KeyCode.DownArrow)){
		moveVector = new Vector3(0,0,-1);
		directionVector = new Vector3(0,0,-1);
		}
	if (Input.GetKey(KeyCode.UpArrow)){
		moveVector = new Vector3(0,0,1);
		directionVector = new Vector3(0,0,1);
		}
	if (Input.GetKey(KeyCode.LeftArrow)){
		moveVector = new Vector3(-1,0,0);
		directionVector = new Vector3(-1,0,0);
		}
	if (Input.GetKey(KeyCode.RightArrow)){
		moveVector = new Vector3(1,0,0);
		directionVector = new Vector3(1,0,0);
		}

	if (moveVector.magnitude > JOYSTICK_STICKINESS) {
		
		// Get the length of the directon vector and then normalize it
		// Dividing by the length is cheaper than normalizing when we already have the length anyway
		var directionLength = Mathf.Min(Mathf.Max(moveVector.magnitude - JOYSTICK_STICKINESS, 0.0) / (1.0 - JOYSTICK_STICKINESS), 1.0);

		// Multiply the normalized direction vector by the modified length
		moveVector = moveVector.normalized * Mathf.Pow(directionLength, 3);
		
		// Rotate input vector to be perpendicular to character's up vector
		var camToCharacterSpace = Quaternion.FromToRotation(-Camera.main.transform.forward, transform.up);
		moveVector = (camToCharacterSpace * moveVector);
	} else{
		moveVector = Vector3.zero;
	}
	
	// Apply the direction to the CharacterMotor
	motor.inputMoveDirection = moveVector;	

			
	// Set rotation to the move direction	
	if (autoRotate && directionVector.magnitude > 0.25) {
		
		var newForward : Vector3 = ConstantSlerp(
			transform.forward,
			directionVector,
			maxRotationSpeed * Time.deltaTime
		);
		newForward = ProjectOntoPlane(newForward, transform.up);
		
		transform.rotation = Quaternion.LookRotation(newForward, transform.up);
		
	}
}

function ProjectOntoPlane (v : Vector3, normal : Vector3) {
	return v - Vector3.Project(v, normal);
}

function ConstantSlerp (from : Vector3, to : Vector3, angle : float) {
	var value : float = Mathf.Min(1.0, angle / Vector3.Angle(from, to));
	return Vector3.Slerp(from, to, value);
}

// Require a character controller to be attached to the same game object
@script RequireComponent (CharacterMotor)
@script AddComponentMenu ("Character/Platform Input Controller")
