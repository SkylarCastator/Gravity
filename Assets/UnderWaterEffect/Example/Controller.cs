using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {

	public GameObject[] screen;
	public float speed = 4;
	int index = 0;
	void Start () {
		SetActiveScreen (0);
	}

	void Update(){
		for (int x = 0; x < screen.Length; x++) {
			if (screen [x].transform.position.y > 10) {
				screen [x].transform.position = new Vector3 (screen [x].transform.position.x, 10, screen [x].transform.position.z);
			}
			if (screen [x].transform.position.y < -1.5f) {
				screen [x].transform.position = new Vector3 (screen [x].transform.position.x, -1.5f, screen [x].transform.position.z);
			}
		}
	}

	void OnGUI () {
		for (int x = 0; x < screen.Length; x++) {
			if (GUI.Button (new Rect (15, 15, 100, 30), "next")) {
				index ++;
				if (index >= screen.Length) {
					index = 0;
				}
				SetActiveScreen (index);
			}
			if (GUI.RepeatButton (new Rect (15, 70, 50, 30), "Up")) {
				screen[index].transform.Translate (0, Time.deltaTime*speed, 0);
			}
			if (GUI.RepeatButton (new Rect (15, 100, 50, 30), "Down")) {
				screen[index].transform.Translate (0, -Time.deltaTime*speed, 0);
			}
		}
	}

	void SetActiveScreen(int i){
		for (int x = 0; x < screen.Length; x++) {
			screen [x].SetActive (false);
		}
		screen [i].SetActive (true);
	}
}
