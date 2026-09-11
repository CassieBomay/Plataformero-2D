using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class Player_Controller : MonoBehaviour
{
    #region Valores
    [Header("Inputs")]
    [SerializeField] private InputActionReference inputMove;
    [SerializeField] private InputActionReference inputJump;
    [SerializeField] private InputActionReference inputRewind;

    [Space(5)]
    [Header("Player")]
    private Rigidbody2D _rb;
    private bool isDead;

    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private float p_Speed;
    [SerializeField] private LayerMask layerGround;

    //Health
    [SerializeField] private GameObject[] HP;
    private int current_HP = 3;
    private float hitTimer = 0f;

    [Space(5)]
    [Header("Animation")]
    [SerializeField] private Animator _animator;

    [Space(5)]
    [Header("Rewind")]
    [SerializeField] private int recordDuration = 60;
    private bool isRewinding;

    private Stack<Player_States> _rewind = new Stack<Player_States>();
    private Queue<Player_States> _states = new Queue<Player_States>();

    [SerializeField] private float rewindCooldown = 2f;
    private float rewindTimer = 0f;

    [Space(5)]
    [Header("UI")]
    private int Coins = 0;
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private Image rewindImage;

    [Space(5)]
    [Header("Checkpoint")]
    public GameObject player;
    public GameObject Notification;
    public float Timer = 0f;
    #endregion

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Rewind();
        if (isRewinding) return; //Mientras se rebobina, no se puede mover ni saltar.

        Movement();
        Jump();

        Hit_Color();
        Death();
    }

    #region Player_Movement
    void Movement()
    {
        float axisH = inputMove.action.ReadValue<Vector2>().x;

        _rb.linearVelocityX = axisH * p_Speed; //Movimiento del jugador en el eje X.
        float currentSpeed = Mathf.Abs(_rb.linearVelocityX); //Volver el valor en absoluto para que no se vuelva negativo al cambiar la dirección del eje.
        _animator.SetFloat("player_Speed", currentSpeed); //Pasar la información a Animator.

        if (!isDead) //Se asegura de que el sprite no se voltee cuando el jugador muere.
        {
            if (axisH > 0.1f)
                _spriteRenderer.flipX = false;
            else if (axisH < -0.1f)
                _spriteRenderer.flipX = true;
        } //Flipeo del sprite.
    }

    void Jump()
    {
        bool tryJumping = inputJump.action.triggered;
        bool isGrounded = Physics2D.Raycast(transform.position, Vector3.down, 0.55f, layerGround); //Detectar el "suelo".

        if (isGrounded && tryJumping)
        {
            _animator.SetBool("isJumping", true);
            _rb.linearVelocityY = 5f;
        }
        else if (isGrounded && _rb.linearVelocityY <= 0.01f)
            _animator.SetBool("isJumping", false);
    }
    #endregion

    #region Death
    void UpdateHP()
    {
        for (int i = 0; i < HP.Length; i++)
        {
            HP[i].SetActive(i < current_HP);
        } //Si el índice es menor que la vida actual, se activa el objeto de vida correspondiente; de lo contrario, se desactiva.
    }

    void Hit_Color()
    {
        if (hitTimer > 0f)
        {
            hitTimer -= Time.deltaTime;
            _spriteRenderer.color = Color.red;
        }
        else
            _spriteRenderer.color = Color.white;
    } //Cambio del color a recibir el daño.

    void Death()
    {
        if (current_HP == 0)
        {
            _animator.SetTrigger("isDead");
            _rb.simulated = false;
            isDead = true;
        }
    }
    #endregion

    #region Rewind
    void Rewind()
    {
        if (rewindTimer > 0f)
        {
            rewindTimer -= Time.deltaTime;
            rewindImage.fillAmount = rewindTimer / rewindCooldown; //Actualizar la barra de rebobinado.
        }
        else
            rewindImage.fillAmount = 0f;

        bool rewindAction = inputRewind.action.IsPressed();

        if (rewindAction && !isRewinding && _states.Count > 0 && rewindTimer <= 0f)
        {
            isRewinding = true;

            foreach (var state in _states)
            {
                _rewind.Push(state); //Se pasan los estados guardados en el Queue a Stack para poder reproducirlos en orden inverso.
            }

            _states.Clear(); //Se limpia Queue de estados para que no se acumulen mientras se rebobina.
        }

        if (isRewinding)
        {
            ReverseTime();
        }
        else
            RecordState();
    }

    void RecordState()
    {
        if (_states.Count >= recordDuration)
        {
            _states.Dequeue(); //Borrar últimas entradas para no superar 60 frames.
        }

        AnimatorStateInfo currentFrame = _animator.GetCurrentAnimatorStateInfo(0); //Obtener el estado actual de la animación para poder reproducirlo al rebobinar.

        _states.Enqueue(new Player_States(
            transform.position,
            _rb.linearVelocity,
            currentFrame.fullPathHash,
            currentFrame.normalizedTime,
            current_HP)); //Guardar nuevas entradas.
    }

    void ReverseTime()
    {
        if (_rewind.Count > 0)
        {
            Player_States state = _rewind.Pop();

            //rewind_movement
            transform.position = state.position;
            _rb.linearVelocity = state.velocity;

            //revind_animation
            _animator.Play(state.animationFrame, 0, state.animationTime);

            //rewind_HP
            current_HP = state.currentHP;
            UpdateHP();
        }
        else
        {
            isRewinding = false;
            rewindTimer = rewindCooldown;
        }
            
    }
    #endregion

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Tramp") && hitTimer <= 0f)
        {
            HP[current_HP - 1].SetActive(false);
            current_HP--;

            float hitDuration = 0.3f;
            hitTimer = hitDuration;
        }

        if (collision.CompareTag("Coin"))
        {
            Coins += 1;
            coinText.text = Coins.ToString();
        }

        if (collision.CompareTag("Death"))
        {
            current_HP = 0;
            UpdateHP();

            Death();
        }
    } //Colisiones.
}



[System.Serializable]
public class Player_States
{
    public Vector3 position;
    public Vector3 velocity;
    public int animationFrame;
    public float animationTime;
    public int currentHP;

    public Player_States(Vector3 pos, Vector3 vel, int frame, float time, int hp)
    {
        position = pos;
        velocity = vel;
        animationFrame = frame;
        animationTime = time;
        currentHP = hp;
    }
} //Capsulación de los estados del player para el rebobinado.