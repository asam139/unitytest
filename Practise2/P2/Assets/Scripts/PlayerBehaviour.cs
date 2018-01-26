using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour {

	// Private
	int _lifes = 3;
	int _visitedBases = 0;

	Rigidbody _rigidbody;
	float _lastFireTime = 0f;

	// Public
	public int maxLifes = 3;
	public int maxBases = 4;
	public float force = 10f;
	public float torque = 0.5f;
	public float firingRate = 5; // Bullets per seconds 

	public GameObject bulletPrefab;


	// Use this for initialization
	void Start () {
		_lifes = maxLifes;

		_rigidbody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		float h = Input.GetAxis("Horizontal") * torque * Time.deltaTime;
        float v = Input.GetAxis("Vertical") * force * Time.deltaTime;
        
        _rigidbody.AddForce(transform.forward * v, ForceMode.Impulse);
        _rigidbody.AddTorque(transform.up * h, ForceMode.Impulse);

        if(Input.GetButton("Fire") && CanFire()) {
     		Shoot();
 		}
	}

	// Actions

	bool CanFire () {
		return Time.time - _lastFireTime > 1f / firingRate;
	}

	public void Shoot () {
		_lastFireTime = Time.time;
		
		GameObject newBullet = Instantiate(bulletPrefab);
		newBullet.tag = this.tag;
		newBullet.transform.position = transform.position;

        //Set initial velocity
        newBullet.GetComponent<Rigidbody>().velocity = transform.forward * _rigidbody.velocity.magnitude;

        // Set direction
		BulletBehaviour bulletBehaviour = newBullet.GetComponent<BulletBehaviour>();
		bulletBehaviour.direction = transform.forward;

	}

	public void BaseFounded () {
		_visitedBases += 1;
		if (_visitedBases == maxBases) {
			Debug.Log("End of Game!!!");	
		}
	}

	public void Hit () {
		_lifes -= 1;

		if (_lifes > 0) {
			Debug.Log("Player: Damaged");

			Vector3 explosionPos = transform.position;
			float radius = 5.0f;
    		float power = 500.0f;
			_rigidbody.AddExplosionForce(power, explosionPos, radius, 0.0f, ForceMode.Impulse);

		} else {
			Debug.Log("Player: Dead");
			Debug.Log("Player: Game Over!!!");

			gameObject.SetActive(false);
		}
	}

	public void Heal () {
		_lifes = maxLifes;
	}

 
}
