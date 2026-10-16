using System;
using System.IO;
using Unity.Profiling;
using UnityEngine;
namespace NeonSiege {
 public sealed class RuntimeDiagnostics : MonoBehaviour {
  ProfilerRecorder batches,triangles,drawCalls;readonly float[] frames=new float[64000];int count,maxBatches,maxDrawCalls;long maxTriangles;float elapsed;Bootstrap boot;bool testing,captured;
  public void Initialize(Bootstrap b){boot=b;testing=Environment.CommandLine.Contains("-siegePerf");if(!testing)return;Screen.SetResolution(1920,1080,FullScreenMode.Windowed);Application.targetFrameRate=-1;QualitySettings.vSyncCount=0;drawCalls=ProfilerRecorder.StartNew(ProfilerCategory.Render,"Draw Calls Count");batches=ProfilerRecorder.StartNew(ProfilerCategory.Render,"Batches Count");triangles=ProfilerRecorder.StartNew(ProfilerCategory.Render,"Triangles Count");boot.Player.GodMode=true;boot.Session.StartRun();boot.Waves.Stop();for(int i=0;i<24;i++){
   bool spawned=false;for(int attempt=0;attempt<72&&!spawned;attempt++){float a=i*Mathf.PI*2/24+attempt*.17f,radius=14+attempt%3*3;spawned=boot.Enemies.Spawn(i==0?EnemyKind.Boss:(EnemyKind)(i%4),10,new Vector3(Mathf.Sin(a)*radius,0,Mathf.Cos(a)*radius));}
   if(!spawned)throw new InvalidOperationException("Performance fixture could not spawn all 24 enemies");
  }Debug.Log("PERFORMANCE_FIXTURE_ALIVE="+boot.Enemies.Alive);}
  void Update(){if(!testing)return;elapsed+=Time.unscaledDeltaTime;boot.Player.Health=boot.Player.MaxHealth;if(elapsed>5&&elapsed<40){if(count<frames.Length)frames[count++]=Time.unscaledDeltaTime*1000;maxDrawCalls=Mathf.Max(maxDrawCalls,(int)drawCalls.LastValue);maxBatches=Mathf.Max(maxBatches,(int)batches.LastValue);maxTriangles=Math.Max(maxTriangles,triangles.LastValue);}
   if(elapsed>10&&!captured){captured=true;ScreenCapture.CaptureScreenshot(Path.Combine(Application.dataPath,"../performance-scene.png"));}
   if(elapsed<40||count==0)return;Array.Sort(frames,0,count);double total=0;for(int i=0;i<count;i++)total+=frames[i];string result=$"GPU: {SystemInfo.graphicsDeviceName}\nResolution: {Screen.width}x{Screen.height}\nQuality: {boot.Session.Save.Quality}\n24 enemies including one wave-10 boss; stationary invulnerable operator; 5-second warmup, 35-second sample.\nSamples: {count}\nAverage FPS: {count*1000/total:F1}\nMedian frame ms: {frames[count/2]:F2}\n95th percentile ms: {frames[Mathf.Min(count-1,(int)(count*.95f))]:F2}\nMax draw calls: {maxDrawCalls}\nMax batches: {maxBatches}\nMax triangles: {maxTriangles}\nCounters of zero mean unavailable.\n";File.WriteAllText(Path.Combine(Application.dataPath,"../performance.txt"),result);Debug.Log("PERFORMANCE_PASS\n"+result);testing=false;Application.Quit();}
  void OnDestroy(){drawCalls.Dispose();batches.Dispose();triangles.Dispose();}
 }
}

