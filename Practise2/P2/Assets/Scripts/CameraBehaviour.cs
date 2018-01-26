using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour {

	// Private
	private Rigidbody _targetRigidbody;

	// Public
	public float minHeight = 4f;
	public float maxHeight = 5f;
	public float heightFactor = 1.5f;
	public float speedAdjust = 1.0f;
	public GameObject target;
	
	// Use this for initialization
	void Start () {
		 _targetRigidbody = target.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void LateUpdate () {
		Vector3 initPosition = transform.position;
		Vector3 endPosition = target.transform.position;

		float startY = initPosition.y;
		float endY = Mathf.Min(minHeight * (1 + _targetRigidbody.velocity.magnitude / heightFactor), maxHeight);

		endPosition.y = Mathf.Lerp(startY, endY, Time.deltaTime * speedAdjust);

        transform.position = endPosition;
	}
}
