using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region SINGLETON
    protected static GameManager _instance = null;
    public static GameManager instance { get { return _instance; } }
    void Awake() { _instance = this; }
    #endregion

    // Punteros a player y a todos los enemigos (lista 'enemiesList')
    public PlayerBehaviour player = null;
    public List<SkeletonBehaviour> enemiesList = null;  // No requiere inicializacion, se rellena desde el Inspector

    // Lista con los enemigos que quedan vivos
    List<SkeletonBehaviour> currentEnemiesList = null;

    // Variables internas
    int _score = 0;
    bool _soundEnabled = true;

    void Start()
    {
        currentEnemiesList = new List<SkeletonBehaviour>();

        // Reiniciamos el juego
        Reset();
    }

    private void Reset()        // Funcion para reiniciar el juego
    {
        // Reiniciamos a Player
        player.Reset();

        // Rellenamos la lista de enemigos actual.
        currentEnemiesList.Clear();
        foreach (SkeletonBehaviour skeleton in enemiesList)
        {
            skeleton.setPlayer(player);
            skeleton.Reset();

            currentEnemiesList.Add(skeleton);
        }

        // Incializamos la puntuacion a cero
        _score = 0;
        UIManager.instance.UpdateScore(_score);
        UIManager.instance.ShowMainMenu();
    }

    #region UI EVENTS
    // Evento al pulsar boton 'Start'
    public void onStartGameButton()
    {
        // Ocultamos el menu principal (UIManager)
        UIManager.instance.HideMainMenu();

        // Actualizamos la puntuacion en el panel Score (UIManager)
        UIManager.instance.UpdateScore(_score);

        // Quitamos la pausa a Player
        player.paused = false;
    }

    // Evento al pulsar boton 'Exit'
    public void onExitGameButton()
    {
        // Mostramos el panel principal
        UIManager.instance.ShowMainMenu();

        // Reseteamos el juego
        Reset();
    }
    #endregion

    #region GAME EVENTS
    // Evento al ser notificado por un enemigo (cuando muere)
    public void notifyEnemyKilled(SkeletonBehaviour enemy)
    {
        // Eliminamos enemigo de la lista actual
        currentEnemiesList.Remove(enemy);

        // Subimos 10 puntos y actualizamos la puntuacion en la UI
        _score += 10;
        UIManager.instance.UpdateScore(_score);

        // Si no quedan enemmigos
        if (currentEnemiesList.Count == 0)  // KEEP
        {
            // Mostrar panel de 'Mision cumplida' y pausar a Player
            player.AnimatorReset();

            UIManager.instance.ShowEndPanel(true);
        }
    }

    // Evento al ser notificado por player (cuando muere)
    public void notifyPlayerDead()
    {
        // Mostrar panel de 'Mision fallida' y pausar a Player
        player.AnimatorReset();

        UIManager.instance.ShowEndPanel(false);
    }
    #endregion

    #region CONFIGURATION 

    public void SetSoundEnabled(bool soundEnabled) {
        _soundEnabled = soundEnabled;
        AudioListener.pause = !_soundEnabled;
        AudioListener.volume = _soundEnabled ? 1f : 0f;
    }

    public bool SoundEnabled() {
        return _soundEnabled;
    }

    #endregion
}
