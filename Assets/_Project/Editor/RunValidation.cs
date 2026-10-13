using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace NeonSiege.Editor {
 public static class RunValidation {
  public static void FullRun(){EditorTestStartup.Prepare();SessionState.SetString("NeonProfileBeforeTest",JsonUtility.ToJson(SaveStore.Load()));SessionState.SetBool("NeonFullRun",true);EditorSceneManager.OpenScene("Assets/_Project/Scenes/NeonSiege.unity");EditorApplication.EnterPlaymode();}
  public static void Interactive(){EditorTestStartup.Prepare();EditorSceneManager.OpenScene("Assets/_Project/Scenes/NeonSiege.unity");EditorApplication.EnterPlaymode();}
  [InitializeOnLoadMethod]static void Register(){EditorApplication.playModeStateChanged+=state=>{if(state!=PlayModeStateChange.EnteredPlayMode||!SessionState.GetBool("NeonFullRun",false))return;Bootstrap boot=null;int bosses=0,upgrades=0;double start=EditorApplication.timeSinceStartup,next=0;EditorApplication.CallbackFunction tick=null;tick=()=>{if(EditorApplication.timeSinceStartup-start>90){Finish(false,"Timeout");return;}if(EditorApplication.timeSinceStartup<next)return;next=EditorApplication.timeSinceStartup+.08;
    if(!boot){boot=UnityEngine.Object.FindAnyObjectByType<Bootstrap>();if(!boot||!boot.Session)return;boot.Waves.Configuration=UnityEngine.Object.Instantiate(boot.Waves.Configuration);boot.Waves.Configuration.BaseEnemies=2;boot.Waves.Configuration.EnemiesPerWave=1;boot.Waves.Configuration.SpawnInterval=.01f;boot.Waves.Configuration.OpeningDelay=.01f;boot.Enemies.EnemyKilled+=e=>{if(e.IsBoss)bosses++;};boot.Session.StartRun();return;}
    if(boot.Session.State==RunState.Upgrade){boot.Session.Choose(upgrades%3);upgrades++;return;}
    if(boot.Session.State==RunState.Victory){bool passed=boot.Waves.Wave==12&&bosses==2&&upgrades==11&&boot.Session.Kills==102;Finish(passed,$"waves={boot.Waves.Wave}, bosses={bosses}, upgrades={upgrades}, kills={boot.Session.Kills}");return;}
    for(int i=0;i<boot.Enemies.Pool.Length;i++){var enemy=boot.Enemies.Pool[i];if(enemy.gameObject.activeSelf)enemy.TakeDamage(99999);}
   };void Finish(bool passed,string details){EditorApplication.update-=tick;SaveStore.Save(JsonUtility.FromJson<SaveData>(SessionState.GetString("NeonProfileBeforeTest","{}")));SessionState.SetBool("NeonFullRun",false);Directory.CreateDirectory("Docs");File.WriteAllText("Docs/full-run-validation.txt",(passed?"PASS ":"FAIL ")+details+"\nAccelerated progression test; direct damage, not a balance/playability test.");Debug.Log((passed?"FULL_RUN_PASS ":"FULL_RUN_FAIL ")+details);EditorApplication.Exit(passed?0:1);}EditorApplication.update+=tick;};}
 }
}

