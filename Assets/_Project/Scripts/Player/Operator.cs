using UnityEngine;
using Unity.Cinemachine;
namespace NeonSiege {
 public sealed class Operator : MonoBehaviour {
  public PlayerInput Input {get;private set;} public CharacterController Controller {get;private set;}
  public Transform Visual {get;private set;} public Camera View {get;private set;}
  public float Health=100,MaxHealth=100,SpeedMultiplier=1,DashCooldown=3,Sensitivity=.12f,Fov=70;
  public bool GodMode;public bool Active=true; public float DashReady=>Mathf.Clamp01(1-dashTimer/DashCooldown); public bool Invulnerable=>dashTime>0;
  public float DamageFlash {get;private set;}
  CinemachineCamera rig; CinemachineBrain brain; float yaw,pitch=13,dashTimer,dashTime,vertical,shake;Vector3 dashDirection,cameraVelocity;
  public void Initialize(){Input=new PlayerInput();Controller=gameObject.AddComponent<CharacterController>();Controller.height=1.8f;Controller.radius=.4f;Controller.center=Vector3.up*.9f;
   Visual=new GameObject("Operator visual").transform;Visual.SetParent(transform,false);
   Palette.Box("Body",Vector3.up*1.1f,new Vector3(.65f,.8f,.4f),Palette.White,Visual,false);Palette.Box("Helmet",Vector3.up*1.72f,new Vector3(.48f,.4f,.42f),Palette.Metal,Visual,false);Palette.Box("Visor",new Vector3(0,1.76f,.23f),new Vector3(.4f,.08f,.04f),Palette.CyanMat,Visual,false);
   if(ArtLibrary.Load()){foreach(Transform child in Visual)child.gameObject.SetActive(false);Visual.gameObject.AddComponent<ActorPresentation>().Initialize(true);}View=new GameObject("Main Camera").AddComponent<Camera>();View.tag="MainCamera";View.nearClipPlane=.1f;View.farClipPlane=180;View.backgroundColor=new Color(.025f,.04f,.075f);View.clearFlags=CameraClearFlags.SolidColor;View.gameObject.AddComponent<AudioListener>();View.transform.position=transform.position+new Vector3(0,3,-5);brain=View.gameObject.AddComponent<CinemachineBrain>();brain.UpdateMethod=CinemachineBrain.UpdateMethods.ManualUpdate;rig=new GameObject("Shoulder camera").AddComponent<CinemachineCamera>();rig.transform.position=View.transform.position;
  }
  void Update(){if(Input==null)return; DamageFlash=Mathf.Max(0,DamageFlash-Time.unscaledDeltaTime*2);if(!Active)return;
   Vector2 look=Input.Look.ReadValue<Vector2>()*Sensitivity+Input.StickLook.ReadValue<Vector2>()*(130*Time.deltaTime);yaw+=look.x;pitch=Mathf.Clamp(pitch-look.y,-30,65);
   transform.rotation=Quaternion.Euler(0,yaw,0);var axis=Input.Move.ReadValue<Vector2>();Vector3 movement=transform.right*axis.x+transform.forward*axis.y;
   dashTimer=Mathf.Max(0,dashTimer-Time.deltaTime);dashTime-=Time.deltaTime;
   if(Input.Dash.WasPressedThisFrame()&&dashTimer<=0){dashTimer=DashCooldown;dashTime=.18f;dashDirection=movement.sqrMagnitude>.1f?movement.normalized:transform.forward;}
   vertical=Controller.isGrounded?-2:vertical-25*Time.deltaTime;
   Controller.Move(((dashTime>0?dashDirection*30:movement*(Input.Sprint.IsPressed()?9:6)*SpeedMultiplier)+Vector3.up*vertical)*Time.deltaTime);
  }
  void LateUpdate(){if(!View)return;var rotation=Quaternion.Euler(pitch,yaw,0);var pivot=transform.position+Vector3.up*1.65f+transform.right*.55f;var distance=Input.Aim.IsPressed()?2.4f:4.6f;
   if(Physics.SphereCast(pivot,.2f,-(rotation*Vector3.forward),out var hit,distance,1,QueryTriggerInteraction.Ignore))distance=Mathf.Max(.3f,hit.distance-.1f);
   var desired=pivot-rotation*Vector3.forward*distance;rig.transform.position=Vector3.SmoothDamp(rig.transform.position,desired,ref cameraVelocity,.035f,999,Time.unscaledDeltaTime);
   rig.transform.rotation=rotation*Quaternion.Euler(Mathf.Sin(Time.unscaledTime*83)*shake,Mathf.Cos(Time.unscaledTime*71)*shake,0);shake=Mathf.MoveTowards(shake,0,Time.unscaledDeltaTime*12);rig.Lens.FieldOfView=Mathf.Lerp(rig.Lens.FieldOfView,Input.Aim.IsPressed()?Fov*.72f:Fov,Time.unscaledDeltaTime*12);brain.ManualUpdate();
  }
  public void LookAtPoint(Vector3 point){var delta=point-(transform.position+Vector3.up*1.65f+transform.right*.55f);yaw=Mathf.Atan2(delta.x,delta.z)*Mathf.Rad2Deg;pitch=-Mathf.Asin(delta.normalized.y)*Mathf.Rad2Deg;} public void Recoil(float amount){pitch=Mathf.Max(-30,pitch-amount);shake=.3f;}
  public void Damage(float amount){if(!Active||Invulnerable||GodMode)return;Health=Mathf.Max(0,Health-amount);DamageFlash=1;shake=1.5f;}
  public void ResetRun(){Controller.enabled=false;transform.position=Vector3.zero;Controller.enabled=true;Health=MaxHealth=100;SpeedMultiplier=1;DashCooldown=3;dashTimer=0;}
  void OnDestroy(){Input?.Dispose();}
 }
}





