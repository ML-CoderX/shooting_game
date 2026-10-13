using UnityEngine;
using UnityEngine.InputSystem;
namespace NeonSiege {
 public sealed class PlayerInput : System.IDisposable {
  public readonly InputActionAsset Asset;
  public readonly InputAction Move,Look,StickLook,Fire,Aim,Sprint,Dash,Reload,Pause,Next;
  public PlayerInput(){var template=Resources.Load<InputActionAsset>("Controls");if(template){Asset=Object.Instantiate(template);var actions=Asset.FindActionMap("Operator",true);Move=actions.FindAction("Move",true);Look=actions.FindAction("Mouse look",true);StickLook=actions.FindAction("Stick look",true);Fire=actions.FindAction("Fire",true);Aim=actions.FindAction("Aim",true);Sprint=actions.FindAction("Sprint",true);Dash=actions.FindAction("Dash",true);Reload=actions.FindAction("Reload",true);Pause=actions.FindAction("Pause",true);Next=actions.FindAction("Next weapon",true);Asset.Enable();return;}Asset=ScriptableObject.CreateInstance<InputActionAsset>();var map=new InputActionMap("Operator");Asset.AddActionMap(map);
   Move=map.AddAction("Move",InputActionType.Value);Move.AddCompositeBinding("2DVector").With("Up","<Keyboard>/w").With("Down","<Keyboard>/s").With("Left","<Keyboard>/a").With("Right","<Keyboard>/d");Move.AddBinding("<Gamepad>/leftStick");
   Look=map.AddAction("Mouse look",InputActionType.Value,"<Mouse>/delta");StickLook=map.AddAction("Stick look",InputActionType.Value,"<Gamepad>/rightStick");
   Fire=Button(map,"Fire","<Mouse>/leftButton","<Gamepad>/rightTrigger");Aim=Button(map,"Aim","<Mouse>/rightButton","<Gamepad>/leftTrigger");Sprint=Button(map,"Sprint","<Keyboard>/leftShift","<Gamepad>/leftStickPress");Dash=Button(map,"Dash","<Keyboard>/space","<Gamepad>/buttonSouth");Reload=Button(map,"Reload","<Keyboard>/r","<Gamepad>/buttonWest");Pause=Button(map,"Pause","<Keyboard>/escape","<Gamepad>/start");Next=Button(map,"Next weapon","<Keyboard>/q","<Gamepad>/rightShoulder");Asset.Enable();}
  static InputAction Button(InputActionMap map,string name,string key,string pad){var a=map.AddAction(name,InputActionType.Button,key);a.AddBinding(pad);return a;}
  public void Dispose(){Asset.Disable();Object.Destroy(Asset);}
 }
}

