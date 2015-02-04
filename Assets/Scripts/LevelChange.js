#pragma strict

function Start () {

}

function Update(){

	if(Input.GetKeyUp(KeyCode.Return)){
		Application.LoadLevel ("GameOver");
	}	

}