using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


// An entity that the player can ride on, that moves through the space.
public class TransportEntity : MonoBehaviour
{

    // All the players on this log. Set up in prep for replacing w/ a generalized class that includes the 
    // bonus frog.
    protected List<PlayerController> attaches;

    public float movementSpeed = 0.0f; // How fast this entity moves, in pixels per second.
    bool can_carry = true; // Whether this entity can carry the player.
    protected void SetCarry(bool to)
    {
        can_carry = to;
        if (!to) attaches.Clear(); // Can't carry anymore; clear any being carried currently.
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start() { attaches = new List<PlayerController>(); }

    // Update is called once per frame
    protected virtual void Update()
    {
       // Turn movement speed into velocity; multiply by deltaTime to get it in units per second,
        // divide by 16 for pixels per second.
        Vector3 velocity = new Vector3(movementSpeed * Time.deltaTime / 16.0f, 0, 0);

        // Apply that velocity to self + any carried players.

        transform.position += velocity;

        print("applying veloci");
        foreach (PlayerController player in attaches) player.transform.position += velocity;

    }

    // Attach any player that gets onto the entity, so they can be moved with it.
    void OnTriggerStay2D(Collider2D collision)
    {
        // Can't pick anything up in the first place, ignore.
        if (!can_carry) return;

        // When colliding with the player...
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();

            // If they player isn't on this entity, and the player is unmoving, attach them to the entity.
            if (!attaches.Contains(player) && !player.moving) 
                attaches.Add(player);
        }
    }

    // Detach any player that leaves the entity, so they're no longer moved with it.
    void OnTriggerExit2D(Collider2D collision)
    {

        // When no longer colliding with the player...
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            
            // If they player was attached, detach them.
            if (attaches.Contains(player)) 
                attaches.Remove(player);
        }
    }
}
