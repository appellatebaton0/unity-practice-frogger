using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{

    InputAction moveAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        print("hello, world!");

        moveAction = InputSystem.actions.FindAction("Move");    
    }

    // Update is called once per frame
    void Update()
    {
        // Locks the directions to 0 or 1, since we'll never be doing diagonals.
        Vector2Int moveDirection = Vector2Int.RoundToInt(moveAction.ReadValue<Vector2>());
        bool movePressed = moveAction.WasPressedThisFrame();


        if (movePressed)
        {


            print(moveDirection);

            // Prioritizes X over Y movement. No gettin' round it.
            if (moveDirection.x != 0)
            {
                transform.position += new Vector3(moveDirection.x, 0, 0);
                print("transformed x.");
            } 

            else if (moveDirection.y != 0)
            {
                transform.position += new Vector3(0, moveDirection.y, 0);
                print("transformed y.");
            }
        }
    }
}
