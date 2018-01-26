using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonBehaviour : MonoBehaviour
{
	// Definir Hashes de:
	// Parametros (Attack, Dead, Distance)
	// Estados (Attack, Idle)
    private const int BaseLayerIndex = 0;
    private readonly int AttackHash = Animator.StringToHash("Attack");
    private readonly int DeadHash = Animator.StringToHash("Dead");
    private readonly int DistanceHash = Animator.StringToHash("Distance");
    private readonly int BaseLayerIdleHash = Animator.StringToHash("Base Layer.Idle");
    private readonly int BaseLayerAttackHash = Animator.StringToHash("Base Layer.Attack");

    // Public
    public AudioClip attackAudioClip;
    public AudioClip deadAudioClip;

	// Variables auxiliares 
	PlayerBehaviour _player		= null;     //Puntero a Player (establecido por método 'setPlayer')
	bool _dead					= false;	// Indica si ya he sido eliminado
    float _range                = 2f;       // Squared distances
	float _timeToAttack			= 1f;		// Periodo de ataque
    float _lastAttackTime;
    float _rotationSpeed        = 0.5f;

    BoxCollider _boxCollider;
    float _originalColliderZ = 0;           // Valora original de la posición 'z' del collider
    Animator _animator;
    AudioSource _audioSource;

	public void setPlayer(PlayerBehaviour player)
	{
		_player = player;
	}

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider>();
        _originalColliderZ = _boxCollider.center.z;

        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
    }

    void Start ()
	{
        
	}
	
	void FixedUpdate ()
	{
        // Si estoy muerto ==> No hago nada
        if (_dead) return;

        // Si Player esta a menos de 1m de mi y no estoy muerto:
        // - Le miro
        // - Si ha pasado 1s o más desde el ultimo ataque ==> attack()
        if (!_player.paused) {
            if (IsInRange()) {
                LookToTarget();
                if (CanAttack()) {
                    Attack();
                }
            }
        }


		// Desplazar el collider en 'z' un multiplo del parametro Distance
        Vector3 center = _boxCollider.center;
        center.z = _originalColliderZ + _animator.GetFloat(DistanceHash) * _boxCollider.size.z;
        _boxCollider.center = center;
	}


	public void Kill()
	{
        // Guardo que estoy muerto, disparo trigger "Dead" y desactivo el collider
        _dead = true;
        _boxCollider.enabled = false;

        _animator.SetTrigger(DeadHash);
        _audioSource.clip = deadAudioClip;
        _audioSource.PlayDelayed(1f);

        // Notifico al GameManager que he sido eliminado
        GameManager.instance.notifyEnemyKilled(this);
	}

	// Funcion para resetear el collider (activado por defecto), la variable donde almaceno si he muerto y forzar el estado "Idle" en Animator
	public void Reset()
    {
        _dead = false;

        _boxCollider.enabled = true;

        _animator.Play(BaseLayerIdleHash, BaseLayerIndex);

        // Reseteo todos los triggers (Attack y Dead)
        _animator.ResetTrigger(AttackHash);
        _animator.ResetTrigger(DeadHash);
	}

	private void OnCollisionEnter(Collision collision)
	{
        // Si el estado es 'Attack' matamos al enemigo (mirar etiqueta)
        if (collision.gameObject.CompareTag("Player"))
        {
            // Obtener estado actual de la capa Attack Layer
            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(BaseLayerIndex);
            if (stateInfo.fullPathHash == BaseLayerAttackHash && _animator.GetFloat(DistanceHash) > 0)
            {
                PlayerBehaviour pBehaviour = collision.gameObject.GetComponent<PlayerBehaviour>();
                pBehaviour.RecieveDamage();
            }
        }
	}

    // Auxiliaries methods
    bool CanAttack()
    {
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(BaseLayerIndex);
        return Time.time - _lastAttackTime > _timeToAttack && stateInfo.fullPathHash != BaseLayerAttackHash;
    }

    bool IsInRange()
    {
        Vector3 direction = _player.gameObject.transform.position - transform.position;
        return direction.magnitude < _range;
    }

    void LookToTarget()
    {
        Vector3 direction = _player.gameObject.transform.position - transform.position;

        // Rotate Canon
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, _rotationSpeed * Time.time);
    }

    void Attack()
    {
        // Activo el trigger "Attack"
        _lastAttackTime = Time.time;

        _animator.SetTrigger(AttackHash);
        _audioSource.clip = attackAudioClip;
        _audioSource.PlayDelayed(0.75f);
    }
}
