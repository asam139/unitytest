using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBehaviour : MonoBehaviour {
	// Private
	float _lastFireTime = 0;

	// Public
	public Transform canonTransform;
	public float rotationSpeed = 0.5f;

	public GameObject bulletPrefab;
	public float range = 1f; // Squared distances
	public float firingRate = 50; // Bullets per seconds 

	public GameObject target;

	// Use this for initialization
	void Start () {
		//_rigidbody = this.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        if (!target.activeSelf) {
            return;
        }

        if (IsInRange()) {
			LookToTarget();
			if (CanFire()) {
				Shoot();	
			}
		}
	}

	bool CanFire () {
		return Time.time - _lastFireTime > 1f / firingRate;
	}

	bool IsInRange () {
        Vector3 direction = target.transform.position - canonTransform.position;
		return direction.magnitude < range;
	}

	void LookToTarget () {
        Vector3 direction = target.transform.position - canonTransform.position;

		// Rotate Canon
		Quaternion rotation = Quaternion.LookRotation(direction);
		canonTransform.rotation = Quaternion.Slerp(canonTransform.rotation, rotation, rotationSpeed * Time.time);
        canonTransform.rotation = rotation;
	}

	void Shoot () {
		_lastFireTime = Time.time;
		
        Vector3 direction = target.transform.position - canonTransform.position;
		direction.Normalize();

		// Create Bullet
		GameObject newBullet = Instantiate(bulletPrefab);
		newBullet.tag = this.tag;
		newBullet.transform.position = canonTransform.position;

		BulletBehaviour bulletBehaviour = newBullet.GetComponent<BulletBehaviour>();
		bulletBehaviour.direction = direction;
	}
}
