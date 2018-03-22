using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

 // Use this for initialization
 void Start () {
     GameObject[] Objects = GameObject.FindGameObjectsWithTag ("Planet");
     
     //the gravity between each couple of object is calculated
     foreach (GameObject ObjectA in Objects) 
     {
         ObjectA.GetComponent<Rigidbody>().AddForce(new Vector3(100,0,0));
     }
 }
 void ApplyGravity(Rigidbody A, Rigidbody B)
 {
     //This is how to get the distance vector between two objects.
     Vector3 dist = B.transform.position - A.transform.position; 
     float r = dist.magnitude;
     dist /= r;
     
     //This is the Newton's equation
     //G = 6.67 * 10^-11 N.m².kg^-2
     double G =  6.674f * (10 ^ 11);
     float force = ((float)G * A.mass * B.mass) / (r * r);
     
     //Then, just apply the forces
     A.AddForce (dist * force);
     B.AddForce (-dist * force);
 }
 void FixedUpdate () 
 {
     //Get every object 
     GameObject[] Objects = GameObject.FindGameObjectsWithTag ("Planet");
     
     //the gravity between each couple of object is calculated
     foreach (GameObject ObjectA in Objects) 
     {
         foreach (GameObject ObjectB in Objects)
         {
             //Objects must not self interact 
             if(ObjectA == ObjectB)
                 continue;
             
             ApplyGravity(ObjectA.GetComponent<Rigidbody>(), ObjectB.GetComponent<Rigidbody>());
         }
         
     }
 
 }
}