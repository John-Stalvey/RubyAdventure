using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardEnemyController : MonoBehaviour
{
    public float speed = 3.0f;
    public bool vertical; 
    public float changeTime = 3.0f;

    Rigidbody2D rigidbody2D;
    float timer;
    int direction = 1;

    
    public ParticleSystem smokeEffect;

    
    bool broken = true;

    
    Animator animator;

    AudioSource audioSource;
    public AudioClip fixedSound;
    public AudioClip brokenSound;

    
    private RubyController rubyController;


    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        timer = changeTime;

        // Animation
        animator = GetComponent<Animator>();

        // Audio Component
        audioSource = GetComponent<AudioSource>();

        // Broken Sound Plays on Loop
        audioSource.clip = brokenSound;
        audioSource.loop = true;
        audioSource.Play();

        //Ruby
        GameObject rubyControllerObject = GameObject.FindWithTag("Player");
        rubyController = rubyControllerObject.GetComponent<RubyController>();
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;

        if (timer < 0)
        {
            direction = -direction;
            timer = changeTime;
        }

        if(!broken)
        {
            return;
        }
    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2D.position;

        if (vertical)
        {
            position.y = position.y + Time.deltaTime * speed * direction;;
            
            // Animation (Vertical)
            animator.SetFloat("Move X", 0);
            animator.SetFloat("Move Y", direction);
        }
        
        else 
        {
            position.x = position.x + Time.deltaTime * speed * direction;;

            // Animation (Horizontal)
            animator.SetFloat("Move X", direction);
            animator.SetFloat("Move Y", 0);
        }

        // Fixing robot code
        if(!broken)
        {
            return;
        }
        
        rigidbody2D.MovePosition(position);
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        RubyController player = other.gameObject.GetComponent<RubyController>();

        if (player != null)
        {
            player.ChangeHealth(-2); // Changes health of player
        }
    }
    public void Fix()
    {
        broken = false;
        rigidbody2D.simulated = false;

        //optional if you added the fixed animation
        animator.SetTrigger("Fixed");

        // Particle effect set to false
        smokeEffect.Stop();

        // Broken sound effect stops and plays fixed sound
        audioSource.clip = fixedSound;
        audioSource.loop = false;
        audioSource.Play();

        if (rubyController != null)
        {
            rubyController.FixedRobots(1);
        }
    }
    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
