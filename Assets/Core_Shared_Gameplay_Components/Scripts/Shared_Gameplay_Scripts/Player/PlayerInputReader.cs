using UnityEngine;

public class PlayerInputReader : MonoBehaviour
{

    // input movement properties. { get; private set; } means that other scripts can read these values but only THIS script can change these values
    // properties are 'functions desguised as variables' which means you can do cool stuff to them inside this script if we wanted to. they get used like variables everywhere else, but this script can manipulate them if we want.
    // I'm ising properties here instead of variables becauase we want to ensure that only <this script> can change these variables
    // I'm also using PascalCase for the variable names (instead of camelCase) because that's the c# convention to indicate something is a 'property' instead of just a regular 'variable'.
    public float MoveInput {  get; private set; } // this property will hold the direciton (horizontal) and value of movement pressed
    public bool JumpHeld { get; private set; } // this property will hold the bool for 'is the jump button currently pressed'
    public bool JumpPressed { get; private set; } // this property will hold the boolean 'was the jump button pressed in this current frame?'
    public bool CastPressed { get; private set; } // this will hold the bool CastPressed - was the cast pressed this frame? this will work like the jump one with the "latch" to keep it as true till its dealt with
    public bool ReelHeld { get; private set; } // this will hold the bool ReelHeld - is the reel button held this frame? 

    private void Update() // grabbing the player's inputs in update
    {
        // horizontal and jump inputs
        
        MoveInput = Input.GetAxisRaw("Horizontal"); // using GetAxisRaw here cos regular GetAxis smooths input over time. but since I'm going to add acelleration/decelleration we want to do that ourselves.
        JumpHeld = Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow); // the input funciton here is returning a bool - 'is key pressed? yes/no'. So that's why its posisble to set the varible 'JumpHeld' with an = directly instead of having to do an 'if/else'.
                                                                                 // Input has already created a bool for us to copy into the variable.  
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow)) // here we're checknig if space or uparrow was pressed and setting the JumpPressed property to true. We do this so that the jump command PERSISTS until its used by the movement controller, so we never miss a jump.
        {
            JumpPressed = true;
        }



        // reeling and casting inputs

        ReelHeld = Input.GetMouseButton(1); // reeling on RMB

        if (Input.GetMouseButtonDown(0)) // casting on LMB
        {
            CastPressed = true; // this one gets the latch treatment, like the jump
        }





    } // end update

    public void ConsumeJumpPress() // this little function will reset the JumpPressed bool to 'false' once the jump press has been recognised and actioned (or not) by the movement system
                                   // this lives here instead of letting other scripts clear jumpPressed so that we keep input arcitecture in one spot - this script.
    {
        JumpPressed = false;
    }


    public void ConsumeCastPress() // same deal as with the jumppress above
    {
        CastPressed = false;
    }

} // end of class