using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum RunButtonState { NONE, RUN }

public class RunButton : UIBehaviour
{
    #region STATIC
    static RunButtonState state = RunButtonState.NONE;
    static public bool IsRun()
    {
        return state == RunButtonState.RUN;
    }
    #endregion

    Image image;

    public void update(bool pushed)
    {
        changeColor(pushed);

        if (pushed)
        {
            state = RunButtonState.RUN;
        }
        else
        {
            state = RunButtonState.NONE;
        }
    }

    void changeColor(bool pushed)
    {
        image.color = pushed ? new Color(0, 0, 0, 1) : new Color(0, 0, 0, 0.3f);
    }

    protected override void Start()
    {
        image = GetComponent<Image>();
    }
}