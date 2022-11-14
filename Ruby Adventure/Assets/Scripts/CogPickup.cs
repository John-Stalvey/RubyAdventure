using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CogPickup : MonoBehaviour
{

    public AudioClip collectedClip;

    // Start is called before the first frame update
    void OnTriggerEnter2D(Collider2D other)
    {
        RubyController controller = other.GetComponent<RubyController>();

        if (controller != null)
        {
            if(controller.ammo < controller.maxAmmo)
            {     
            controller.ChangeAmmo(4);
            Destroy(gameObject);

            controller.PlaySound(collectedClip);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
