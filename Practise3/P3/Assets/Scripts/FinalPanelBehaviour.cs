using UnityEngine;
using UnityEngine.UI;

public class FinalPanelBehaviour : MonoBehaviour
{
    // Definir Hashes de:
    // Parametros (Attack, Dead, Distance)
    // Estados (Attack, Idle)
    private const int BaseLayerIndex = 0;
    private readonly int AnimateHash = Animator.StringToHash("Animate");
    private readonly int ResetHash = Animator.StringToHash("Reset");
    private readonly int BaseLayerIdleHash = Animator.StringToHash("Base Layer.Idle");
    private readonly int BaseLayerAnimateHash = Animator.StringToHash("Base Layer.Animate");

    public Text label;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void Show(string message) {
        label.text = message;
        _animator.ResetTrigger(ResetHash);
        _animator.SetTrigger(AnimateHash);
    }

    public void Hide() {
        _animator.SetTrigger(ResetHash);
    }

    public void Reset()
    {
        _animator.ResetTrigger(AnimateHash);
        _animator.ResetTrigger(ResetHash);
        _animator.Play(BaseLayerIdleHash, BaseLayerIndex);
    }

}
