using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace NeonSiege {
 // Opt-in integration playtest. Drives the same Input Actions as a controller.
 // Never changes health, damage, positions, wave tuning, or the game time scale.
 public sealed class NormalRunValidation : MonoBehaviour {
  Bootstrap boot;
  Gamepad pad;
  SaveData original;
  ProfilerRecorder calls,triangles;
  readonly float[] frameTimes=new float[200000];
  readonly List<string> events=new List<string>();
  readonly NavMeshPath route=new NavMeshPath();
  readonly Vector3[] corners=new Vector3[32];
  readonly Vector3[] patrol={new Vector3(0,0,18),new Vector3(18,0,0),new Vector3(0,0,-18),new Vector3(-18,0,0)};
  RunState previous=(RunState)(-1);
  int waypoint,cornerCount,frames,maxCalls,bosses,upgrades,lastWave,choice,errors;
  long maxTriangles;
  float launch,start,stateSince,nextRoute,nextReport,minHealth=100,nextDiagnostic;
  Vector3 steering;
  bool finished;
  string Output(string name)=>Path.Combine(Application.dataPath,"../"+name);

  public void Initialize(Bootstrap owner){
   boot=owner;original=JsonUtility.FromJson<SaveData>(JsonUtility.ToJson(boot.Session.Save));
   pad=InputSystem.AddDevice<Gamepad>();launch=Time.realtimeSinceStartup;
   Screen.SetResolution(1920,1080,FullScreenMode.Windowed);
   QualitySettings.vSyncCount=0;Application.targetFrameRate=144;
   calls=ProfilerRecorder.StartNew(ProfilerCategory.Render,"Draw Calls Count");
   triangles=ProfilerRecorder.StartNew(ProfilerCategory.Render,"Triangles Count");
   boot.Enemies.EnemyKilled+=e=>{if(e.IsBoss)bosses++;};
   Application.logMessageReceived+=LogMessage;
   Record("Normal-speed controller playtest; original wave/weapon tuning; no invulnerability, direct damage, healing, or teleports. 1080p, 144 FPS cap.");
  }
  void LogMessage(string message,string stack,LogType type){if(type==LogType.Exception||type==LogType.Error||type==LogType.Assert){errors++;events.Add("RUNTIME ERROR: "+message);}}
  void Record(string value){events.Add(value);File.WriteAllLines(Output("normal-run-progress.txt"),events);Debug.Log("NORMAL_RUN "+value);}
  int Rank(UpgradeDefinition u){switch(u.Kind){case UpgradeKind.Lifesteal:return 100;case UpgradeKind.Health:return boot.Player.Health<60?95:35;case UpgradeKind.Damage:return 85;case UpgradeKind.FireRate:return 80;case UpgradeKind.Ricochet:return 75;case UpgradeKind.Explosive:return 70;case UpgradeKind.Magazine:return 55;case UpgradeKind.Speed:return 45;default:return 25;}}

  void LateUpdate(){
   if(finished||!pad.added)return;
   float now=Time.realtimeSinceStartup;
   if(now-launch>1200){Finish(false,"20-minute timeout");return;}
   var session=boot.Session;var input=new GamepadState();
   if(session.State!=previous){
    previous=session.State;stateSince=now;
    if(previous==RunState.Playing&&start==0)start=now;
    if(previous==RunState.Upgrade){choice=0;for(int i=1;i<3;i++)if(Rank(session.Choices[i])>Rank(session.Choices[choice]))choice=i;upgrades++;Record($"Wave {boot.Waves.Wave} cleared at {session.RunTime:F1}s; HP {boot.Player.Health:F1}; choose {session.Choices[choice].Title}");}
   }
   float stateAge=now-stateSince;
   if(session.State==RunState.Menu){if(stateAge>1.5f&&stateAge<1.7f)input=input.WithButton(GamepadButton.South);}
   else if(session.State==RunState.Upgrade){
    if(choice>=1&&stateAge>1&&stateAge<1.15f||choice>=2&&stateAge>1.4f&&stateAge<1.55f)input=input.WithButton(GamepadButton.DpadRight);
    if(stateAge>2.2f&&stateAge<2.4f)input=input.WithButton(GamepadButton.South);
   }
   else if(session.State==RunState.Victory){Finish(true,"Victory");return;}
   else if(session.State==RunState.Defeat){Finish(false,"Defeat");return;}
   else if(session.State==RunState.Playing){
    if(boot.Player.GodMode){Finish(false,"Invulnerability unexpectedly enabled");return;}
    minHealth=Mathf.Min(minHealth,boot.Player.Health);
    if(now-start>5&&frames<frameTimes.Length){frameTimes[frames++]=Time.unscaledDeltaTime*1000;maxCalls=Mathf.Max(maxCalls,(int)calls.LastValue);maxTriangles=Math.Max(maxTriangles,triangles.LastValue);}
    if(lastWave!=boot.Waves.Wave){lastWave=boot.Waves.Wave;Record($"Wave {lastWave} started; kills={session.Kills}; HP={boot.Player.Health:F1}");if(lastWave==5||lastWave==10)ScreenCapture.CaptureScreenshot(Output($"normal-wave-{lastWave}.png"));}
    if(now>nextReport){nextReport=now+30;Record($"Progress: wave={lastWave}, time={session.RunTime:F1}s, remaining={boot.Waves.Remaining}, HP={boot.Player.Health:F1}, kills={session.Kills}, shots={boot.Weapons.ShotsFired}");}
    // Briefly hold the opening position to exercise incoming melee damage.
    if(now-start<12&&boot.Player.Health>=100){InputSystem.QueueStateEvent(pad,input);return;}
    Vector3 position=boot.Player.transform.position;
    Vector3 patrolDelta=position-patrol[waypoint];patrolDelta.y=0;
    if(patrolDelta.magnitude<2.5f)waypoint=(waypoint+1)%patrol.Length;
    Enemy pursuit=null;float pursuitDistance=float.MaxValue;
    foreach(var e in boot.Enemies.Pool)if(e.gameObject.activeSelf){float d=(e.transform.position-position).sqrMagnitude;if(d<pursuitDistance){pursuitDistance=d;pursuit=e;}}
    Vector3 destination=patrol[waypoint];
    if(pursuit&&boot.Waves.Remaining<=3&&(pursuitDistance>144||Physics.Linecast(position+Vector3.up,pursuit.transform.position+Vector3.up,1)))destination=pursuit.transform.position;
    if(now>nextRoute){
     nextRoute=now+.25f;Vector3 routeStart=position;
     if(NavMesh.SamplePosition(position,out var onMesh,2,NavMesh.AllAreas))routeStart=onMesh.position;
     NavMesh.CalculatePath(routeStart,destination,NavMesh.AllAreas,route);cornerCount=route.GetCornersNonAlloc(corners);
     steering=destination;
     // Advance past reached corners instead of oscillating across the same point.
     for(int i=1;i<cornerCount;i++){var delta=corners[i]-position;delta.y=0;if(delta.sqrMagnitude>.8f*.8f){steering=corners[i];break;}}
    }
    Vector3 movement=steering-position;movement.y=0;movement.Normalize();
    Vector3 local=boot.Player.transform.InverseTransformDirection(movement);input.leftStick=new Vector2(local.x,local.z);input=input.WithButton(GamepadButton.LeftStick);
    Enemy target=null;float nearest=float.MaxValue,closest=float.MaxValue;
    foreach(var enemy in boot.Enemies.Pool){if(!enemy.gameObject.activeSelf)continue;float distance=(enemy.transform.position-position).sqrMagnitude;closest=Mathf.Min(closest,distance);if(distance<nearest&&!Physics.Linecast(boot.Player.View.transform.position,enemy.transform.position+Vector3.up*1.1f,1)){nearest=distance;target=enemy;}}
    if(closest<12&&boot.Player.DashReady>.99f)input=input.WithButton(GamepadButton.South);
    if(target){
     var direction=target.transform.position+Vector3.up*1.1f-boot.Player.View.transform.position;
     var aim=Quaternion.LookRotation(direction).eulerAngles;var current=boot.Player.View.transform.eulerAngles;
     float yaw=Mathf.DeltaAngle(current.y,aim.y),pitch=Mathf.DeltaAngle(current.x,aim.x);
     float turn=130*Mathf.Max(Time.deltaTime,.001f);
     input.rightStick=new Vector2(Mathf.Clamp(yaw/turn,-1,1),-Mathf.Clamp(pitch/turn,-1,1));
     input.leftTrigger=1;if(Mathf.Abs(yaw)<4&&Mathf.Abs(pitch)<4)input.rightTrigger=1;
     if(now>nextDiagnostic){nextDiagnostic=now+15;Record($"Aim: player={position}, target={target.transform.position}, yaw={yaw:F1}, pitch={pitch:F1}, path={route.status}, steering={steering}, stick={input.leftStick}");}
    }else if(boot.Weapons.Ammo<boot.Weapons.Current.Magazine/2)input=input.WithButton(GamepadButton.West);
    if(!target&&now>nextDiagnostic){nextDiagnostic=now+15;Record($"No visible target: player={position}, nearest={(pursuit?pursuit.transform.position:Vector3.zero)}, path={route.status}, steering={steering}, stick={input.leftStick}");}
   }
   InputSystem.QueueStateEvent(pad,input);
  }
  void Finish(bool victory,string reason){
   finished=true;InputSystem.QueueStateEvent(pad,new GamepadState());
   float wallTime=start>0?Time.realtimeSinceStartup-start:0;
   Array.Sort(frameTimes,0,frames);double sum=0;int overBudget=0;for(int i=0;i<frames;i++){sum+=frameTimes[i];if(frameTimes[i]>16.67f)overBudget++;}
   bool passed=victory&&boot.Waves.Wave==12&&bosses==2&&upgrades==11&&errors==0;
   string report=$"{(passed?"PASS":"FAIL")} normal-speed run: {reason}\nWaves: {boot.Waves.Wave}\nBoss kills: {bosses}\nUpgrades: {upgrades}\nKills: {boot.Session.Kills}\nShots fired: {boot.Weapons.ShotsFired}\nGameplay seconds: {boot.Session.RunTime:F1}\nWall seconds including upgrades: {wallTime:F1}\nWithin 10–15 minute target: {wallTime>=600&&wallTime<=900}\nMinimum health: {minHealth:F1}\nGod mode: {boot.Player.GodMode}\nRuntime errors: {errors}\nResolution: {Screen.width}x{Screen.height}\nSampled gameplay frames: {frames}\nAverage FPS: {(sum>0?frames*1000/sum:0):F1}\n95th percentile ms: {(frames>0?frameTimes[Mathf.Min(frames-1,(int)(frames*.95f))]:0):F2}\nFrames over 16.67 ms: {overBudget}\nPeak draw calls: {maxCalls}\nPeak triangles: {maxTriangles}\nAutomated controller with scene-state targeting; this verifies actual combat/progression, not human difficulty or subjective enjoyment.\n";
   File.WriteAllText(Output("normal-run-validation.txt"),report);Record(report);SaveStore.Save(original);
   ScreenCapture.CaptureScreenshot(Output(victory?"normal-victory.png":"normal-defeat.png"));StartCoroutine(Quit(passed));
  }
  IEnumerator Quit(bool passed){yield return new WaitForSecondsRealtime(1);Application.Quit(passed?0:1);}
  void OnDestroy(){Application.logMessageReceived-=LogMessage;if(pad!=null&&pad.added)InputSystem.RemoveDevice(pad);calls.Dispose();triangles.Dispose();}
 }
}
