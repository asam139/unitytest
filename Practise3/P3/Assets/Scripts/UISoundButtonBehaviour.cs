using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISoundButtonBehaviour : MonoBehaviour
{
	// Sprites de imagen Activado y Desactivado.
	public Sprite SoundOn;
	public Sprite SoundOff;

	public Image buttonImage;	// Imagen mostrada en la interfaz
	
	public void toggleSound()	// Funcion que se llamara al pulsar encima
	{
        //Invertir el valor de GameManager.instance.soundEnabled
        bool soundEnabled = !GameManager.instance.SoundEnabled();
        GameManager.instance.SetSoundEnabled(soundEnabled);

        // Actualizar la imagen con el sprite correspondiente (buttonImage.sprite = SoundOn/SoundOff).
        buttonImage.sprite = soundEnabled ? SoundOn : SoundOff;
	}
}
