using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum AttackButtonState { NONE, ATTACK }

public class AttackButton : UIBehaviour
{
    #region STATIC
    static AttackButtonState state = AttackButtonState.NONE;
    static public bool IsAttack()
    {
        return state == AttackButtonState.ATTACK;
    }
    #endregion

    Image image;

    public void update(bool pushed)
    {
        changeColor(pushed);

        if (pushed)
        {
            state = AttackButtonState.ATTACK;
        }
        else
        {
            state = AttackButtonState.NONE;
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