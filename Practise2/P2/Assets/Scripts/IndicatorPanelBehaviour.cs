using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IndicatorPanelBehaviour : MonoBehaviour {

    // Private

    List<GameObject> _bases;
    List<GameObject> _baseIndicatorPool = new List<GameObject>();
    int _baseIndicatorPoolCursor = 0;

    List<GameObject> _turret; 
    List<GameObject> _turretIndicatorPool = new List<GameObject>();
    int _turretIndicatorPoolCursor = 0;

    List<GameObject> _objects = new List<GameObject>();

    // Public 
    public GameObject baseIndicatorPrefab;
    public GameObject turretIndicatorPrefab;

	// Use this for initialization
	void Start () {
        _bases = new List<GameObject>(GameObject.FindGameObjectsWithTag("Base"));
        _turret = new List<GameObject>(GameObject.FindGameObjectsWithTag("Turret"));

        _objects.Clear();
        _objects.AddRange(_bases);
        _objects.AddRange(_turret);
	}
	
	// Update is called once per frame
	void Update () {
        paint();
	}

    void paint() {
        resetPool();

        foreach (GameObject obj in _objects) {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(obj.transform.position);

            // Onscreen
            if (screenPos.z > 0 && 
               screenPos.x > 0 && screenPos.x < Screen.width &&
                screenPos.y > 0 && screenPos.y < Screen.height) {

            } else { // Offscreen
                if (screenPos.z < 0) {
                    screenPos *= -1;
                }

                Vector3 screenCenter = new Vector3(Screen.width, Screen.height, 0) * 0.5f;

                //Coordinates Translated
                // Make (0,0) the center of the screen instead of bottom left
                screenPos -= screenCenter;

                // Find angle from center of screen to position
                float angle = Mathf.Atan2(screenPos.y, screenPos.x);

                angle -= 90 * Mathf.Deg2Rad;

                float cos = Mathf.Cos(angle);
                float sin = -Mathf.Sin(angle);

                screenPos = screenCenter + new Vector3(150 * sin, 150 * cos, 0);

                //Slope–intercept form linear-equation: y = mx+b
                float m = cos / sin;

                Vector3 screenBounds = screenCenter * 0.9f;

                // Check up and down
                if (cos > 0) {
                    screenPos = new Vector3(screenBounds.y / m, screenBounds.y, 0);
                } else {
                    screenPos = new Vector3(-screenBounds.y / m, -screenBounds.y, 0);
                }

                // If out of bounds, get point on appropiate side
                if (screenPos.x > screenBounds.x) {
                    screenPos = new Vector3(screenBounds.x, screenBounds.x * m, 0);
                } else if (screenPos.x < -screenBounds.x) {
                    screenPos = new Vector3(-screenBounds.x, -screenBounds.x * m, 0);
                } //Else in bounds


                // Remove coordinate translation
                screenPos += screenCenter;


                GameObject arrow;
                if (_bases.Contains(obj)) {
                    arrow = getBaseArrow();
                    Image arrowSprite = arrow.GetComponent<Image>();
                    arrowSprite.color = Color.yellow;
                } else {
                     arrow = getTurretArrow();
                    Image arrowSprite = arrow.GetComponent<Image>();
                    arrowSprite.color = Color.red;
                }

                arrow.transform.localPosition = screenPos;
                arrow.transform.localRotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);

            }
        
        }

        cleanPool();

    }

    void resetPool () {
        _baseIndicatorPoolCursor = 0;
        _turretIndicatorPoolCursor = 0;
    }

    GameObject getBaseArrow() {
        GameObject output;
        if (_baseIndicatorPoolCursor < _baseIndicatorPool.Count) {
            output = _baseIndicatorPool[_baseIndicatorPoolCursor];
        } else {
            output = Instantiate(baseIndicatorPrefab);
            output.transform.SetParent(transform);
            _baseIndicatorPool.Add(output);
            _baseIndicatorPoolCursor++;
        }

        return output;
    }

    GameObject getTurretArrow() {
        GameObject output;
        if (_turretIndicatorPoolCursor < _turretIndicatorPool.Count) {
            output = _turretIndicatorPool[_turretIndicatorPoolCursor];
        } else {
            output = Instantiate(turretIndicatorPrefab);
            output.transform.SetParent(transform);
            _turretIndicatorPool.Add(output);
            _turretIndicatorPoolCursor++;
        }

        return output;
    }

    void cleanPool () {
        while (_baseIndicatorPool.Count > _baseIndicatorPoolCursor) {
            GameObject arrow = _baseIndicatorPool[_baseIndicatorPool.Count - 1];
            _baseIndicatorPool.Remove(arrow);
            Destroy(arrow);
        }

        while (_turretIndicatorPool.Count > _turretIndicatorPoolCursor) {
            GameObject arrow = _turretIndicatorPool[_turretIndicatorPool.Count - 1];
            _turretIndicatorPool.Remove(arrow);
            Destroy(arrow);
        }
    }
                    
}
