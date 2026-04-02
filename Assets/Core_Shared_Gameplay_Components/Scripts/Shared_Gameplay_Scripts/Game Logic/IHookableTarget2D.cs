using UnityEngine;

// TS

// this is an INTERFACE script (see its definition where it would normally say "public void.." or similar), not a regular class. it acts as a DEFINITION.
// this does not go on any objects or anything in the scene, instead it lives in the project and is implemented through other scripts
// this is basically a sort of 'contract' - it means that anything that wants to be hookable needs to provide what is defined in this interface (an attach point, for example)

// NOTE: interface scripts have to be IMPLEMENTED through other scripts on the relevant object. this is done in that bit wher we normally say "monobehaviour", separated by a comma
// like this: "public class HookableFish2D : MonoBehaviour, IHookableTarget2D"
// so make sure that the main hookable script on a hookable object (ie. platforms and fish) have this implemented in that way next to monobehaviour.

// IMPLEMENTING this on another script just lets the Rod script ask questions of that script that align with the requirements of the interface script.
// so the Rod script can ask about the things we define here in the interface as necessary on those hookable objects - like "what is your attach point". 
// this can extend out to being things like "is this object currently hookable". that could also be somethng that our Fish script includes. 
// this interface doens't READ those things or anything, it just tells scripts that are using the interface what they should expect of the object they're trying to talk to

// using this approach means that the Rod script doesn't have to contain the instructions on what to do with every differnt TYPE of hookable object (platform, fish, etc.)
// instead we want the Rod script to just be like "does this thing use IHookableTarget2D and if so lemme ask about the contracted elements".
// the script on that object that implements this interface can do all the fancy stuff that object needs to do. the Rod just does its thing related to the contents of this contract. 

// this is an absurdly long comment but this is a bit of a complicated concept I needed a lot of notes to understand and remember.
// but really its not important for these purposes to know HOW its working, as long as the script on fish or on platforms IMPLEMENTS this interface, and includes the specified conditions below. 


public interface IHookableTarget2D 
{
    Transform AttachPoint { get; } // this means the thing must have an attachpoint transform

    void OnHooked(FishingRodController2D rod); // this means every hookable thing must also have this function with this name
    void OnUnhooked(FishingRodController2D rod); // this means every hookable thing must also have this function with this name
}