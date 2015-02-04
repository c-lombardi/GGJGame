using UnityEngine;
using System.Collections;

public class ButtonNextLevel : MonoBehaviour 
{	
	public void NextLevelButton(string level)
	{
		Application.LoadLevel(level);
	}
}