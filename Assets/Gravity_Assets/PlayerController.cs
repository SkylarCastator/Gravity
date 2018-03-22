using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {
	public GameObject model;
	public VectorGridDemoFireObject fireComponent;

	// Use this for initialization
	void Start () {
		 if (!isLocalPlayer)
         {
			model.SetActive(true);
         	return;
         }

		 fireComponent.m_VectorGrid = GameObject.Find("VectorGrid(Clone)").GetComponent<VectorGrid>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
