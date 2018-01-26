using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour {

	// Private
	Rigidbody _rigidbody;

	// Public 
	public float speed = 10;
	public Vector3 direction;

	// Use this for initialization
	void Start () {
		_rigidbody = this.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	// Update is called once per frame
	void FixedUpdate () {
		_rigidbody.MovePosition(transform.position + speed * direction * Time.deltaTime);
	}

	// Trigger

	void OnTriggerEnter (Collider other) {
		if (other.gameObject.CompareTag(this.tag)) {
			return;
		}

		if (other.gameObject.CompareTag("Player")) {
			Debug.Log(other.gameObject.name);

			PlayerBehaviour pB = other.gameObject.GetComponent<PlayerBehaviour>();
			pB.Hit();

		} else if (other.gameObject.CompareTag("Turret")) {
			Debug.Log(other.gameObject.name);
			other.gameObject.SetActive(false);
		}

		this.gameObject.SetActive(false);


	}
}
