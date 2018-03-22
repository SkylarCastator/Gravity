using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Gravity_GameManager : MonoBehaviour {

	public int numberOfObsticals = 10;
	public GameObject[] obsticals;
	public float minXPosition;
	public float maxXPosition;
	public float minYPosition;
	public float maxYPosition;
	public GameObject map;

	// Use this for initialization
	void Start () {

		Instantiate(map);
		NetworkServer.Spawn(map);

		for (int i = 0; i < numberOfObsticals; i++)
		{
			int randSpawnNum = Random.RandomRange(0, obsticals.Length-1);
			float randSpawnX = Random.RandomRange(minXPosition, maxXPosition);
			float randSpawnY = Random.RandomRange(minXPosition, maxYPosition);

			Vector3 spawnLocation = new Vector3(randSpawnX,10,randSpawnY);
			GameObject obj = Instantiate(obsticals[randSpawnNum], spawnLocation, Quaternion.identity);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
