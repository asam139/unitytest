using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	#region SINGLETON
	protected static UIManager _instance = null;
	public static UIManager instance { get { return _instance; } }
	void Awake() { _instance = this; }
	#endregion

	// Menu principal
	public GameObject mainMenu			= null;	// Panel del menu principal (Primera pantalla en mostrarse)

	// Sub-menus durante el juego
    public GameObject gamePanel         = null; 
	public FinalPanelBehaviour endPanel	= null;	// Panel de fin de juego (Dentro de la interfaz del juego)
	public Text scoreText				= null;	// Puntuacion del juego
    public GameObject gamePad = null;


    public void ShowMainMenu()
	{
        // Mostrar objeto mainMenu
        mainMenu.SetActive(true);
	}

	public void HideMainMenu()
	{
        // Ocultar objeto mainMenu
        mainMenu.SetActive(false);
        HideEndPanel();
	}

	public void ShowEndPanel(bool win)
	{
		// Mostrar panel fin de juego (ganar o perder)
        if (win) {
            endPanel.Show("Mission Completed");
        } else {
            endPanel.Show("Mission Failed");
        }
	}

	public void HideEndPanel()
	{
		// Ocultar el panel
        endPanel.Hide();
	}

    public void ResetEndPanel() 
    {
        endPanel.Reset();
    }

	public void UpdateScore(int score)
	{
        // Actualizar el 'UI text' con la puntuacion 
        scoreText.text = score.ToString();
	}

    public void ShowGamePad()
    {
        gamePad.SetActive(true);
    }

    public void HideGamePad()
    {
        // Ocultar el panel
        gamePad.SetActive(false);
    }

}
