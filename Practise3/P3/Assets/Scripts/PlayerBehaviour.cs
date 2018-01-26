using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    // Definir Hashes de:
    // Parametros (Speed, Attack, Damage, Dead)
    // Estados (Base Layer.Idle, Attack Layer.Idle, Attack Layer.Attack)
    private const int BaseLayerIndex = 0;
    private const int AttackLayerIndex = 1;
    private readonly int SpeedHash = Animator.StringToHash("Speed");
    private readonly int AttackHash = Animator.StringToHash("Attack");
    private readonly int DamageHash = Animator.StringToHash("Damage");
    private readonly int DeadHash = Animator.StringToHash("Dead");
    private readonly int DistanceHash = Animator.StringToHash("Distance");
    private readonly int BaseLayerIdleHash = Animator.StringToHash("Base Layer.Idle");
    private readonly int AttackLayerIdleHash = Animator.StringToHash("Attack Layer.Idle");
    private readonly int AttackLayerAttackHash = Animator.StringToHash("Attack Layer.Attack");

    // Public
    public int maxLives = 3;

	public float walkSpeed		= 1f;		// Parametro que define la velocidad de "caminar"
	public float runSpeed		= 2.0f;		// Parametro que define la velocidad de "correr"
	public float rotateSpeed	= 160f;      // Parametro que define la velocidad de "girar"

    public AudioClip attackAudioClip;
    public AudioClip damageAudioClip;
    public AudioClip deadAudioClip;

    // Variables auxiliares
    BoxCollider _boxCollider;
    float _originalColliderZ = 0;           // Valora original de la posición 'z' del collider
    Rigidbody _rigidbody;
	float _angularSpeed			= 0;		// Velocidad de giro actual
	float _speed				= 0;		// Velocidad de traslacion actual

    Animator _animator;

    AudioSource _audioSource;

	// Variables internas:
    int _lives = 0;			                // Vidas restantes
	public bool paused = false;				// Indica si el player esta pausado (congelado). Que no responde al Input

	void Start()
	{
		// Obtener los componentes Animator, Rigidbody y el valor original center.z del BoxCollider
        _boxCollider = GetComponent<BoxCollider>();
        _rigidbody = GetComponent<Rigidbody>();
        _originalColliderZ = _boxCollider.center.z;

        _animator = GetComponent<Animator>();

        _audioSource = GetComponent<AudioSource>();

        Reset();

#if UNITY_IOS || UNITY_ANDROID
        UIManager.instance.ShowGamePad();
#else
        UIManager.instance.HideGamePad();
#endif
	}

	// Aqui moveremos y giraremos la araña en funcion del Input
	void FixedUpdate()
	{
		// Si estoy en pausa no hacer nada (no moverme ni atacar)
		if (paused) return;

        /////////////////////////////////////////////////////////////////////////////////////
        // Si giro izquierda: _angularSpeed = -rotateSpeed;
        // Calculo de velocidad lineal (_speed) y angular (_angularSpeed) en función del Input
        // Si camino/corro hacia delante delante: _speed = walkSpeed   /  _speed = runSpeed
        // Si camino / corro hacia delante detras: _speed = -walkSpeed / _speed = -runSpeed
        // Si no me muevo: _speed = 0
        /////////////////////////////////////////////////////////////////////////////////////

        /////////////////////////////////////////////////////////////////////////////////////
        // Si giro izquierda: _angularSpeed = -rotateSpeed;
        // Si giro izquierda: _angularSpeed = -rotateSpeed;
        // Si giro derecha: _angularSpeed = rotateSpeed;
        // Si no giro : _angularSpeed = 0;
        /////////////////////////////////////////////////////////////////////////////////////
#if UNITY_IOS || UNITY_ANDROID
        float direction = 0f;
        if (CrossButton.GetInput(InputType.UP)) {
            direction = 1f; 
        } else if (CrossButton.GetInput(InputType.DOWN)) {
            direction = -1f;
        }
        
        if (RunButton.IsRun()) {
            _speed = direction * runSpeed;
        } else {
            _speed = direction * walkSpeed;
        }

        float torqueDirection = 0f;
        if (CrossButton.GetInput(InputType.RIGHT)) {
            torqueDirection = 1f;
        } else if (CrossButton.GetInput(InputType.LEFT)) {
            torqueDirection = -1f;
        }
        _angularSpeed = torqueDirection * rotateSpeed;
#else
        float verticalAxis = Input.GetAxis("Vertical");
        if (Input.GetButton("Run")) {
            _speed = verticalAxis * runSpeed;
        } else {
            _speed = verticalAxis * walkSpeed;
        }


        _angularSpeed = Input.GetAxis("Horizontal") * rotateSpeed;
#endif

        // Actualizamos el parámetro "Speed" en función de _speed. Para activar la anicación de caminar/correr
        _animator.SetFloat(SpeedHash, _speed);

        // Movemov y rotamos el rigidbody (MovePosition y MoveRotation) en función de "_speed" y "_angularSpeed"
        _rigidbody.MovePosition(transform.position + transform.forward * _speed * Time.deltaTime);
        _rigidbody.MoveRotation(transform.rotation * Quaternion.Euler(0, _angularSpeed * Time.deltaTime, 0));

		// Mover el collider en función del parámetro "Distance" (necesario cuando atacamos)
        Vector3 center = _boxCollider.center;
        center.z = _originalColliderZ + _animator.GetFloat(DistanceHash) * _boxCollider.size.z;
        _boxCollider.center = center;
	}

	// En este bucle solamente comprobaremos si el Input nos indica "atacar" y activaremos el trigger "Attack"
	private void Update()
	{
        // Si estoy en pausa no hacer nada (no moverme ni atacar)
        if (paused) return;

		// Si detecto Input tecla/boton ataque ==> Activo disparados 'Attack'
        if (Input.GetButton("Fire") || AttackButton.IsAttack()) {
            _animator.SetTrigger(AttackHash);
            _audioSource.clip = attackAudioClip;
            _audioSource.PlayDelayed(1.5f);
        } 
	}

	// Función para resetear el Player
	public void Reset()
	{
        AnimatorReset();

        // Posicionar el jugador en el (0,0,0) y rotación nula (Quaternion.identity)
        transform.position = new Vector3(0, 0, 0);
        transform.rotation = Quaternion.identity;

        //Reiniciar el numero de vidas
        _lives = maxLives;
	}

    public void AnimatorReset() 
    {
        // Pausamos a Player
        paused = true;

        //Reiniciar el numero de vidas
        _lives = maxLives;

        //Reset speed
        _speed = 0;
        _animator.SetFloat(SpeedHash, _speed);

        // Forzar estado Idle en las dos capas (Base Layer y Attack Layer): función Play() de Animator
        _animator.Play(BaseLayerIdleHash, BaseLayerIndex);
        _animator.Play(AttackLayerIdleHash, AttackLayerIndex);

        // Reseteo todos los triggers (Attack y Dead)
        _animator.ResetTrigger(AttackHash);
        _animator.ResetTrigger(DeadHash);
    }

    // Funcion recibir daño
    public void RecieveDamage()
	{
        if (_lives < 1) {
            return;
        }
        // Restar una vida
        // Si no me quedan vidas notificar al GameManager (notifyPlayerDead) y disparar trigger "Dead"
        if (--_lives == 0) {
            _animator.SetTrigger(DeadHash);
            _audioSource.clip = deadAudioClip;
            _audioSource.PlayDelayed(0.5f);

            GameManager.instance.notifyPlayerDead();
        } else {
            _animator.SetTrigger(DamageHash);
            _audioSource.clip = damageAudioClip;
            _audioSource.Play();
        }
	}

	private void OnCollisionEnter(Collision collision)
	{
		// Si el estado es 'Attack' matamos al enemigo (mirar etiqueta)
        if (collision.gameObject.CompareTag("Enemy")) {
            // Obtener estado actual de la capa Attack Layer
            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(AttackLayerIndex);
            if (stateInfo.fullPathHash == AttackLayerAttackHash && _animator.GetFloat(DistanceHash) > 0) {
                SkeletonBehaviour sBehaviour = collision.gameObject.GetComponent<SkeletonBehaviour>();
                sBehaviour.Kill();
            }
        } 
	}
}
