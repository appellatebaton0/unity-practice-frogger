using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    // -- Movement Variables -- //

    // Input & transformation
    InputAction moveAction; 
    Vector2 lastMoveDirection; // Used to detect when to move, since moveAction.WasPressedThisFrame won't detect the vector change.

    // Animation & interpolation
    public bool moving; // Whether the player is currently in a movement animation.
    float moveTimer = 0.0f; // Used to detect how far along in the movement animation the player is.
    float moveAnimationDuration = 0.33f; // How long each movement takes, in seconds.

    Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        // Initialize the variables that need to be.
        moveAction = InputSystem.actions.FindAction("Move");
        animator = GetComponent<Animator>();

        moveAnimationDuration = findAnimation(animator, "PlayerMove").length;

        setMoving(false);
    }

    // Update is called once per frame
    void Update()
    {
        /// Get all the changing movement variables.
        // Locks the directions to 0 or 1, since we'll never be doing diagonals.
        Vector2 moveDirection = Vector2Int.RoundToInt(moveAction.ReadValue<Vector2>());
        bool movePressed = moveAction.IsPressed();

        // If trying to move and not moving already, start moving.
        if (movePressed && moveDirection != lastMoveDirection && moveDirection != Vector2.zero && !moving)
        {
            setMoving(true);
            lastMoveDirection = moveDirection;

            transform.eulerAngles = new Vector3(0, 0, -Vector2.SignedAngle(moveDirection, Vector2.up));

            // print(new Vector3(0, 0, Vector2.SignedAngle(moveDirection, Vector2.up)));
            // print(moveDirection + " vs " + Vector2.up);
        }

        // If currently in an animation, do the relevant stuff for it.
        if (moving)
        {
            moveTimer += Time.deltaTime;

            // Transform by the amount that should be moved this frame to reach the destination by the end.
            stepTransform(lastMoveDirection * (Time.deltaTime / moveAnimationDuration));

            if (moveTimer >= moveAnimationDuration)
            {

                setMoving(false);
                lastMoveDirection = Vector2Int.zero;

                // Using deltaTime for the transformation causes little inaccuracies that build up.
                // Round the Y value to a whole number. X can be a float bc of moving platforms.

                transform.position = new Vector3(transform.position.x, MathF.Round(transform.position.y), transform.position.z);

                // print("Player's position is now " + transform.position);
            }
        }
    }

    void setMoving(bool value)
    {
        moving = value;

        // Update the animator depending on the move state.
        if (animator && value)
        {
            animator.Play("PlayerMove", 0, 0.0f);
        }

        moveTimer = 0.0f;
    }

    // Transforms the position by a certain x OR y amount. Disallows diagonals (prefers x).
    void stepTransform(Vector2 transformation)
    {
         if (transformation.x != 0)
        {
            transform.position += new Vector3(transformation.x, 0, 0);
        } 

        else if (transformation.y != 0)
        {
            transform.position += new Vector3(0, transformation.y, 0);
        }
    }

    static public AnimationClip findAnimation (Animator animator, string name) 
{
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == name)
            {
                return clip;
            }
        }

        return null;
    }
}

