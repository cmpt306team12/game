using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SubsystemsImplementation;
using UnityEngine.UIElements;

public class Projectile : MonoBehaviour
{

    public ProjectileProperties projProp;

    public Rigidbody2D rb; //projectiles rigidbody
    public GameObject deathEffect; // Effect on projectile death
    public GameObject explosion; // Explosion created if projectile is explosive
    public GameObject trail; //Trail child object

    // Local variables needed for applying perks over projectile lifetime
    private bool despawning = false;
    private int bouncesLeft;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    IEnumerator DespawnCoroutine(float waitTime)
    {
        // wait for specified time
        yield return new WaitForSeconds(waitTime);
        Despawn("Coroutine");
        yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate sprite to face direction of travel
        Vector2 rotation = rb.velocity.normalized;
        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotZ);
    }

    public void Despawn(string from)
    {
        despawning = true; // only allow one despawn for one projectile. Stops multiple triggers causing multiple effects being created
        //Debug.Log(from);
        GameObject effect = Instantiate(deathEffect,transform.position, transform.rotation);
        float scale = projProp.getScale();
        effect.transform.localScale = new Vector3(scale, scale, scale); // Scale deatheffect
        if (projProp.isExplosive())
        {
            Explode();
        }
        trail.transform.parent = null; // decouple trail to allow it to not instantly disappear
        trail.GetComponent<TrailRenderer>().autodestruct = true; // trail destroys when trail reaches end
        Destroy(gameObject); // destroy projectile
    }

    /** Fire()
     * Applies the impulse to fire the projectile, depends on the projectiles stats.
     * */
    public void Fire(ProjectileProperties prop)
    {
        this.projProp = prop;
        StartCoroutine(DespawnCoroutine(projProp.getLifetime())); // Start despawn coroutine
        ApplyScale(projProp.getScale()); // Give projectile proper scale
        bouncesLeft = projProp.getBounces(); // Set value for number of bounces remaining
        rb.velocity = transform.right * projProp.getSpeed(); // Give projectile speed
    }

    private void ApplyScale(float scale)
    {
        // Debug.Log("Scale: " + scale);
        gameObject.transform.localScale = new Vector3(scale, scale, scale); // Scale projectile
    }

    private void Explode()
    {
        float explosion_scale = projProp.getExplosionScale();
        GameObject expl = Instantiate(explosion, transform.position, transform.rotation); // create explosion
        expl.transform.localScale = new Vector3(explosion_scale, explosion_scale, explosion_scale); // scale explosion

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if ((collision.CompareTag("Projectile") || collision.CompareTag("Enemy")) && !despawning)
        {
            Despawn("Hit Proj/Enemy");
        } else if (collision.CompareTag("Wall") || collision.CompareTag("Obstacle"))
        {
            this.bouncesLeft = this.bouncesLeft - 1;
            if (this.bouncesLeft < 0)
            {
                Despawn("Hit Wall/Obs");
            } else if (projProp.isExplosive())
            {
                // Bounced and is explosive: Create explosion
                Explode();
            }
            
            
        }
    }
}
