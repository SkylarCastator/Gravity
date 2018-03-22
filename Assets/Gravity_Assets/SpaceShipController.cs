using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShipController : MonoBehaviour {

	public GameObject GridMesh;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		RaycastHit hit;
		if (Physics.Raycast(transform.position, -transform.up, out hit))
		{

		}
	}
}
