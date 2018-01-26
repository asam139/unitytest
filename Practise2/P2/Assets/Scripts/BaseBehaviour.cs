using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBehaviour : MonoBehaviour {
	// Private
	bool _founded;
	Renderer _renderer;
	Light _pointLight;

	// Use this for initialization
	void Start () {
		_renderer = GetComponent<Renderer>();
		_pointLight = transform.Find("Point Light").GetComponent<Light>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Player") && !_founded) {
			Material mat = _renderer.material;
			mat.SetColor("_EmissionColor", Color.green);

			_pointLight.enabled = true;

			Debug.Log(other.gameObject.name);
			PlayerBehaviour pB = other.gameObject.GetComponent<PlayerBehaviour>();
			pB.BaseFounded();
			pB.Heal();

			_founded = true;
		}
	}
}
