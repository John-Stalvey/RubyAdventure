using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class RubyController : MonoBehaviour
{
    public float speed = 3.0f;

    public int maxHealth = 4;
    public float timeInvincible = 2.0f;

    public GameObject projectilePrefab;
    public int ammo { get { return currentAmmo; }}
    public int maxAmmo = 4;
    int currentAmmo;

    public int health { get { return currentHealth; }}
    int currentHealth;

    bool isInvincible;
    float invincibleTimer;
    
    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;

    Animator animator;
    Vector2 lookDirection = new Vector2(1,0);

    public TextMeshProUGUI ammoText;

    public TextMeshProUGUI fixedText;
    private int scoreFixed = 0;

    AudioSource audioSource;

    public AudioClip throwSound;
    public AudioClip hitSound;
    public AudioClip winSound;
    public AudioClip loseSound;

    public AudioClip backgroundSound;

    public ParticleSystem hitEffect;
    public ParticleSystem healthEffect;

    public GameObject WinTextObject;
    public GameObject LoseTextObject;
    public GameObject JambiTextObject;
    bool gameOver;
    public static int level;


    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();

        currentHealth = maxHealth;

        currentAmmo = maxAmmo;

        animator = GetComponent<Animator>();

        audioSource= GetComponent<AudioSource>();

        fixedText.text = "Fixed Robots: " + scoreFixed.ToString() + "/4";

        WinTextObject.SetActive(false);

        LoseTextObject.SetActive(false);

        JambiTextObject.SetActive(false);

        gameOver = false;

        level = 1;

        audioSource.clip = backgroundSound;
        audioSource.Play();
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);

        if(!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }

        if(Input.GetKeyDown(KeyCode.C))
        {
          Launch();

          if (currentAmmo > 0)
          {
            ChangeAmmo(-1);
            AmmoText();
          }
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    character.DisplayDialog();
                }              
            }
            if (scoreFixed >= 4)
            {
                JambiTextObject.SetActive(false);
                SceneManager.LoadScene("Level 2");
                level = 2;
                
                
            }
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (gameOver == true)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;
        
        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
                return;

                isInvincible = true;
                invincibleTimer = timeInvincible;
                Instantiate(hitEffect, transform.position + Vector3.up * 0.5f, Quaternion.identity);
                PlaySound(hitSound);

                animator.SetTrigger("Hit");
        }

        if (amount > 0)
        {
            healthEffect = Instantiate(healthEffect, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        }

        if (currentHealth <= 0)
        {
            LoseTextObject.SetActive(true);

            audioSource.clip = backgroundSound;
            audioSource.Stop();

            audioSource.clip = loseSound;
            audioSource.Play();

            transform.position = new Vector3(-5f, 0f, -100f);
            speed = 0;
            Destroy(gameObject.GetComponent<SpriteRenderer>());
            gameOver = true;
        }
  
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
    }

public void ChangeAmmo(int amount)
{
    currentAmmo = Mathf.Abs(currentAmmo + amount);
    Debug.Log("Ammo: " + currentAmmo);
}

public void AmmoText()
{
    ammoText.text = "Ammo: " + currentAmmo.ToString();
}
    void Launch()
    {
        if (currentAmmo > 0)
        {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");

        PlaySound(throwSound);
        }
    }

    public void FixedRobots(int amount)
    {
        scoreFixed += amount;
        fixedText.text = "Fixed Robots: " + scoreFixed.ToString() + "/4";

        Debug.Log("Fixed Robots: " + scoreFixed);

        if (level == 1)
        {
            if (scoreFixed >= 4)
            {
                audioSource.clip = backgroundSound;
                audioSource.Stop();

                audioSource.clip = winSound;
                audioSource.Play();

                WinTextObject.SetActive(true);

                transform.position = new Vector3(-5f, 0f, -100f);
                speed = 0;

                Destroy(gameObject.GetComponent<SpriteRenderer>());

                gameOver = true;
            }
        }
    }
}
