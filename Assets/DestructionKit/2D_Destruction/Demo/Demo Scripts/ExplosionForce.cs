﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ExplosionForce : MonoBehaviour {
	public float force = 50;
	public float radius = 5;
	public float upliftModifer = 5;
	
    /// <summary>
    /// create an explosion force
    /// </summary>
    /// <param name="position">location of the explosion</param>
	public void doExplosion(Vector3 position){
		// Debug.Log($"Explosion triggered at position: {position}");
		transform.localPosition = position;
		StartCoroutine(waitAndExplode());
	}

    /// <summary>
    /// exerts an explosion force on all rigidbodies within the given radius
    /// </summary>
    /// <returns></returns>
	private IEnumerator waitAndExplode(){
		yield return new WaitForFixedUpdate();
		
		Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position,radius);
		// Debug.Log($"Found {colliders.Length} colliders in explosion radius");
     
		foreach(Collider2D coll in colliders){
			if(coll.GetComponent<Rigidbody2D>()&&coll.gameObject.name!="Player"){
				// Debug.Log($"Applying explosion force to: {coll.gameObject.name}");
                AddExplosionForce(coll.GetComponent<Rigidbody2D>(), force, transform.position, radius, upliftModifer);
			}
		}
	}

    /// <summary>
    /// adds explosion force to given rigidbody
    /// </summary>
    /// <param name="body">rigidbody to add force to</param>
    /// <param name="explosionForce">base force of explosion</param>
    /// <param name="explosionPosition">location of the explosion source</param>
    /// <param name="explosionRadius">radius of explosion effect</param>
    /// <param name="upliftModifier">factor of additional upward force</param>
    private void AddExplosionForce(Rigidbody2D body, float explosionForce, Vector3 explosionPosition, float explosionRadius, float upliftModifier = 0)
	{
		var dir = (body.transform.position - explosionPosition);	
		float wearoff = 1 - (dir.magnitude / explosionRadius);
        Vector3 baseForce = dir.normalized * explosionForce * wearoff;
        baseForce.z = 0;
		// Debug.Log($"Adding force {baseForce} to {body.gameObject.name}");
		body.AddForce(baseForce);

        if (upliftModifier != 0)
        {
            float upliftWearoff = 1 - upliftModifier / explosionRadius;
            Vector3 upliftForce = Vector2.up * explosionForce * upliftWearoff;
            upliftForce.z = 0;
            body.AddForce(upliftForce);
			// Debug.Log($"Adding uplift force {upliftForce} to {body.gameObject.name}");
        }
	}
}
