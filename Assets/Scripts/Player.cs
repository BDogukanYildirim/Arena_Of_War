using System;
using System.Collections;
using UnityEngine;

public class Player_sc : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] int speed = 5;
    [SerializeField] int runSpeed = 9;

    [Header("Rotate")]
    [Tooltip("Karakterin dönme hızı")]
    [SerializeField] float turnSpeed = 120f;
    [SerializeField] float sensitivity = 500f;

    [Header("Animasyon")]
    [SerializeField] bool walk;
    [SerializeField] bool run;
    [SerializeField] bool defence;
    [SerializeField] bool hurt;
    [SerializeField] bool attack;
    [SerializeField] int attackState;
    Animator anim;
    int enemyCount;

    Rigidbody rb;
    float xRotation = 0f;
    [SerializeField] int health = 100;
    [SerializeField] Transform cameraTransform;
    Vector3 moveInput;
    BoxCollider hitArea;

    // Script aktif olduğunda (Oyun başladığında) GameManager'ı dinlemeye başla
    private void OnEnable()
    {
        GameManager.OnPauseStateChanged += PauseIslemleri;
    }

    // Script pasif olduğunda (Obje kapanınca/sahne değişince) dinlemeyi bırak
    private void OnDisable()
    {
        GameManager.OnPauseStateChanged -= PauseIslemleri;
    }

    void Start()
    {
        hitArea = GetComponent<BoxCollider>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        // Başlangıç ayarları 
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (hitArea != null) hitArea.enabled = false;
    }

    void Update()
    {
        // Eğer oyun durmuşsa (TimeScale 0 ise) input almayı kesebiliriz
        if (Time.timeScale == 0) return;

        GetInput();
        AttackDefence();
        ChooseAnimasyon();
        CameraLook();
    }

    void FixedUpdate()
    {
        Move();
    }

    void PauseIslemleri(bool isPaused)
    {
        if (isPaused)
        {
            // OYUN DURDU: Fareyi serbest bırak, görünsün
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            // OYUN DEVAM EDİYOR: Fareyi kilitle ve gizle
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void AttackDefence()
    {
        attack = Input.GetKeyDown(KeyCode.Mouse0);
        defence = Input.GetKey(KeyCode.Mouse1);

        if (attack)
        {
            StopCoroutine("CoroutineHitBox");
            StartCoroutine("CoroutineHitBox");
            attackState++;
            attackState %= 2;
        }
    }

    public void Damage(int dmg)
    {
        if (defence) { defence = false; return; }

        health -= dmg;

        if (health <= 0)
        {
            health = 0;
            anim.SetBool("die", true);
            return;
        }

        StopCoroutine("CoroutineHurt");
        StartCoroutine("CoroutineHurt");
    }

    IEnumerator CoroutineHurt()
    {
        hurt = true;
        yield return new WaitForSeconds(0.5f);
        hurt = false;
    }

    private void GetInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        moveInput = new Vector3(h, 0, v).normalized;
    }

    private void Move()
    {
        float x = moveInput.x;
        float z = moveInput.z;

        if (moveInput.sqrMagnitude < 0.1f)
        {
            walk = false;
            run = false;
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            return;
        }

        walk = true;
        run = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = run ? runSpeed : speed;

        Vector3 moveDirection = (transform.right * x) + (transform.forward * z);

        if (moveDirection.magnitude > 1f)
            moveDirection.Normalize();

        Vector3 targetVelocity = moveDirection * currentSpeed;
        rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);
    }

    private void CameraLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up * mouseX);

        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -30f, 40f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0, 0f);
    }

    IEnumerator CoroutineHitBox()
    {
        hitArea.enabled = true;
        yield return new WaitForSeconds(0.5f);
        hitArea.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemyCount++;
            Debug.Log("Düşmana Vuruldu: " + other.name + enemyCount);
        }
    }

    private void ChooseAnimasyon()
    {
        anim.SetBool("walk", walk);
        anim.SetBool("run", run);
        anim.SetBool("defence", defence);
        anim.SetBool("hurt", hurt);
        anim.SetBool("attack", attack);
        anim.SetInteger("attackState", attackState);
    }
}