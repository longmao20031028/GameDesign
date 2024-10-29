using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using dpGame;

public class UserGUI : MonoBehaviour {
    private Interaction interaction;
    public int status = 1;
    GUIStyle style1;
	GUIStyle style2;
    void Start() {
		interaction = Director.getInstance ().currentSceneController as Interaction;

		style1 = new GUIStyle();
		style1.fontSize = 150;
		style1.alignment = TextAnchor.MiddleCenter;

		style2 = new GUIStyle("button");
		style2.fontSize = 60;
	}
    void OnGUI() {
		if (status == 0) 
		{
			style1.normal.textColor = Color.red;
			GUI.Label(new Rect(Screen.width/2-85, Screen.height/2-200, 170, 50), "G a m e o v e r !", style1);
			if (GUI.Button(new Rect(Screen.width/2-280, Screen.height/2-70, 560, 180), "Try again!", style2)) 
			{
				status = 1;
				interaction.Restart ();
			}
		} else if(status == 2) {
			style1.normal.textColor = Color.white;
			GUI.Label(new Rect(Screen.width/2-85, Screen.height/2-200, 170, 50), "W i n !", style1);
			if (GUI.Button(new Rect(Screen.width/2-280, Screen.height/2-70, 560, 180), "Restart", style2)) 
			{
				status = 1;
				interaction.Restart ();
			}
		}
	}
}