using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCamera : MonoBehaviour
{
	public float speed = 25f;
	public float turnSpeed = 150f;

	protected float lastMousePos_x = 0;
	protected float lastMousePos_y = 0;

	private void Start()
	{
		lastMousePos_x = Input.mousePosition.x;
		lastMousePos_y = Input.mousePosition.y;
	}

	void Update()
	{
		if (Input.GetKey(KeyCode.UpArrow))
		{
			transform.Translate(Vector3.forward * speed * Time.deltaTime);
		}
		else if (Input.GetKey(KeyCode.DownArrow))
		{
			transform.Translate(-Vector3.forward * speed * Time.deltaTime);
		}

		if (Input.GetKey(KeyCode.RightArrow))
		{
			transform.Translate(Vector3.right * speed * Time.deltaTime);
		}
		else if (Input.GetKey(KeyCode.LeftArrow))
		{
			transform.Translate(-Vector3.right * speed * Time.deltaTime);
		}

		transform.Rotate(Vector3.up *     Time.deltaTime * Input.GetAxis("Mouse X") * turnSpeed);
		transform.Rotate(-Vector3.right * Time.deltaTime * Input.GetAxis("Mouse Y") * turnSpeed);
		transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
	}
}
