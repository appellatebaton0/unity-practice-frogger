using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


// An entity that the player can ride on, that moves through the space.
public class Turtle : TransportEntity
{

    public float bobInterval = 5.0f; // How long this turtle remains up, before bobbing down again.
    public float bobLength   = 3.0f; // How long this turtle stays down, before bobbing back up.

    Animator animator;

    enum STATES {ABOVE, SINKING, BELOW, RISING}; // The possible states for the Turtle.
    string[] stateStrings = {"Above", "Sink", "Below", "Rise"}; // States as their strings (Animation names).

    STATES state; // The turtle's current state.
    double stateTimer; // The timer between state changes.

    protected override void Start()
    {
        // Same as Godot's super, if less easy to use.
        base.Start();

        // Init for the variables that need it.

        animator = GetComponent<Animator>();

        // The animator starts in 'Above', so we need to set the timer up
        // with how long it should wait before changing that, otherwise
        // we'll bob immediately.
        stateTimer = bobInterval;
    }


    // Update is called once per frame
    protected override void Update()
    {

        base.Update(); // Do the velocity stuff still.

        // Something something, run a loop concurrent with the animations to make 
        // the turtle bob up and down. 

        UpdateState(); // Make sure the state is right.

        stateTimer -= Time.deltaTime;

        if (stateTimer <= 0 && !(state == STATES.SINKING || state == STATES.RISING))
            // New state time!

            // Above & Below are the only options, so we can just ternary this.
            animator.Play(state == STATES.ABOVE ? "Sink" : "Rise", 0, 0.0f);
    }

    void SetState(STATES to)
    {

        if (state == to) return;
        
        state = to;

        SetCarry(state != STATES.BELOW); // The turtle can carry the player whenever it's not fully submerged.

        // Restart the timer depending on the new state.
        switch (to)
        {
            case STATES.ABOVE:
                stateTimer = bobInterval;
                break;
            case STATES.BELOW:
                stateTimer = bobLength;
                break;
        }

    }


    // The State is, functionally, a mirror of the Animator's animation state.
    // This maintains that mirror.
    STATES UpdateState()
    {

        // Make sure the two are out-of-sync before doing all this extra work.
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(stateStrings[(int) state])) return state;

        // Search all the state options for which is the current state.
        for (int i = 0; i < stateStrings.Length; i++)
        {
            
            if ((int) state == i) continue; // This is the current state, which we've already tested.

            string stateName = stateStrings[i];

            // If this state is the current state, update & break.
            if (animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
            {
                SetState((STATES) i);
                break;
            }
        }

        return state;
    }
}
