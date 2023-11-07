using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitProjectiles : MonoBehaviour
{
    public float orbitRadius = 2.5f; 
    public float orbitSpeed = 6f;    
    private bool isOrbiting = false;
    private Rigidbody2D characterRigidbody;
    public static bool canOrbit = false;
    private float orbitDuration = 3.0f;
    private float currentOrbitTime = 0.0f;
    private float cooldownDuration = 2.0f; 
    private float lastCtrlPressTime = 0.0f;

    public AudioSource catSFX;
    public AudioClip orbitSFX;
    public AudioClip noOrbitSFX;

    private void Start()
    {
        characterRigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (canOrbit)
        {
            if ((Input.GetKeyDown(KeyCode.LeftControl)) && (Time.time - lastCtrlPressTime >= cooldownDuration))
            {
                isOrbiting = true;
                currentOrbitTime = 0.0f;
                lastCtrlPressTime = Time.time;
                catSFX.PlayOneShot(orbitSFX);
            }

            else if ((Input.GetKeyDown(KeyCode.LeftControl)) && (Time.time - lastCtrlPressTime < cooldownDuration))
            {
                catSFX.PlayOneShot(noOrbitSFX);
            }

            if (Input.GetKeyUp(KeyCode.LeftControl))
            {
                isOrbiting = false;
            }

            if (isOrbiting)
            {
                if (currentOrbitTime < orbitDuration)
                {
                    Vector2 orbitCenter = characterRigidbody.position;
                    GameObject[] projectiles = GameObject.FindGameObjectsWithTag("Projectile");

                    foreach (var projectile in projectiles)
                    {
                        float distanceToProjectile = Vector2.Distance(orbitCenter, projectile.transform.position);

                        if (distanceToProjectile <= orbitRadius)
                        {
                            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

                            if (rb != null)
                            {
                                Vector2 directionToCenter = (orbitCenter - rb.position).normalized;
                                Vector2 orbitalDirection = new Vector2(-directionToCenter.y, directionToCenter.x); // perpendicular direction 
                                rb.velocity = orbitalDirection * orbitSpeed; // force for circular motion 
                            }
                        }
                    }

                    currentOrbitTime += Time.deltaTime;
                }
                else
                {
                    isOrbiting = false;
                }
            }
        }
    }
}