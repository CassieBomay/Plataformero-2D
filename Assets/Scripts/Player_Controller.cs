using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Controller : MonoBehaviour
{
    [Header("Inputs")]
    public InputActionReference inputMove;
    public InputActionReference inputJump;
    public InputActionReference inputRewind;

    [Space(5)]
    [Header("Player")]
    private Rigidbody2D _rb;
    private bool isDead = false;

    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private float p_Speed;
    [SerializeField] private LayerMask layerGround;

    public GameObject[] HP;
    public int current_HP = 3;

    [Space(5)]
    [Header("Animation")]
    public Animator _animator;

    [Space(5)]
    [Header("Rewind")]
    [SerializeField] private int recordDuration = 60;
    private bool isRewinding;

    private Stack<Player_States> _rewind = new Stack<Player_States>();
    private Queue<Player_States> _states = new Queue<Player_States>();

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Rewind();
        if (isRewinding) return;

        Movement();
        Jump();

        Death();
    }

    //-----Player_Controller-----//

    void Movement()
    {
        //Movimiento
        float axisH = inputMove.action.ReadValue<Vector2>().x;
        _rb.linearVelocityX = axisH * p_Speed;

        //Rotación
        if (isDead == false)
        {
            if (axisH > 0.1f)
                _spriteRenderer.flipX = false;
            else if (axisH < -0.1f)
                _spriteRenderer.flipX = true;
        }

        //Volver el valor en absoluto para que no se vuelva negativo al cambiar la dirección del eje
        float currentSpeed = Mathf.Abs(_rb.linearVelocityX);
        //Pasar la información a Animator
        _animator.SetFloat("player_Speed", currentSpeed);
    }

    void Jump()
    {
        bool isGrounded = Physics2D.Raycast(transform.position, Vector3.down, 0.55f, layerGround);
        bool tryJumping = inputJump.action.triggered;

        if (isGrounded && tryJumping)
        {
            _animator.SetBool("isJumping", true);
            _rb.linearVelocityY = 5f;
        }
        else if (isGrounded && _rb.linearVelocityY <= 0.01f)
        {
            _animator.SetBool("isJumping", false);
        }
    }

    void Death()
    {
        if (current_HP == 0)
        {
            _animator.SetTrigger("isDead");
            _rb.simulated = false;
            isDead = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Tramp"))
        {
            HP[current_HP - 1].SetActive(false);
            current_HP--;
        }
    }

    //-----Rewind_Mechanic-----//

    void Rewind()
    {
        bool rewindAction = inputRewind.action.IsPressed();

        if (rewindAction && !isRewinding && _states.Count > 0)
        {
            isRewinding = true;

            foreach (var state in _states)
            {
                _rewind.Push(state);
            }

            _states.Clear();
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
            //Debug.Log("Hola" + _rewind.Peek());
            _states.Dequeue(); //Para borrar entradas viejas.
        }

        _states.Enqueue(new Player_States(transform.position, _rb.linearVelocity)); //Para guardar nuevas entradas.
        //Debug.Log("Recording state: " + transform.position + ", Velocity: " + _rb.linearVelocity);
        Debug.Log("States count: " + _states.Count);
    }

    void ReverseTime()
    {
        if (_rewind.Count > 0)
        {
            Player_States state = _rewind.Pop();

            transform.position = state.position;
            _rb.linearVelocity = state.velocity;
        }
        else
            isRewinding = false;
    }
}


[System.Serializable]
public class Player_States
{
    public Vector3 position;
    public Vector3 velocity;

    public Player_States(Vector3 pos, Vector3 vel)
    {
        position = pos;
        velocity = vel;
    }
}