using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace NeonSiege.Editor {
 public static class ProjectBuilder {
  [MenuItem("Neon Siege/Generate project")]
  public static void Generate(){ContentBuilder.Create();Directory.CreateDirectory("Assets/_Project/Scenes");var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);new GameObject("NEON SIEGE").AddComponent<Bootstrap>();EditorSceneManager.SaveScene(scene,"Assets/_Project/Scenes/NeonSiege.unity");EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(scene.path,true)};PlayerSettings.companyName="Siege Studio";PlayerSettings.productName="NEON SIEGE";PlayerSettings.defaultScreenWidth=1600;PlayerSettings.defaultScreenHeight=900;PlayerSettings.fullScreenMode=FullScreenMode.Windowed;PlayerSettings.runInBackground=true;AssetDatabase.SaveAssets();Debug.Log("M1_GENERATED");}
  public static void PlaySmoke(){EditorTestStartup.Prepare();ContentBuilder.Create();EditorSceneManager.OpenScene("Assets/_Project/Scenes/NeonSiege.unity");SessionState.SetBool("NeonSmoke",true);EditorApplication.EnterPlaymode();}
  [InitializeOnLoadMethod]static void ResumeSmoke(){EditorApplication.playModeStateChanged+=state=>{if(state==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool("NeonSmoke",false)){double end=EditorApplication.timeSinceStartup+5;EditorApplication.CallbackFunction check=null;check=()=>{if(EditorApplication.timeSinceStartup<end)return;EditorApplication.update-=check;var boot=Object.FindAnyObjectByType<Bootstrap>();bool okay=boot&&boot.Player&&boot.Player.View&&boot.Player.Controller.isGrounded; if(okay&&boot.Weapons){int ammo=boot.Weapons.Ammo;okay=boot.Weapons.Definitions.Length==5&&boot.Weapons.TryFire()&&boot.Weapons.Ammo==ammo-1;boot.Weapons.Reload();okay &= boot.Weapons.ReloadRemaining>0;}if(okay&&boot.Enemies){okay=boot.Enemies.Alive>0;boot.Waves.Stop();boot.Enemies.Clear();okay &= boot.Enemies.Spawn(EnemyKind.Boss,5);var boss=boot.Enemies.Boss;okay &= boss&&boss.Agent.isOnNavMesh;boss.TakeDamage(99999);okay &= boot.Enemies.Alive==0;okay &= boot.Enemies.Spawn(EnemyKind.Grunt,1);}if(okay&&boot.Session){var run=boot.Session;run.Pause();okay &= Time.timeScale==0&&!boot.Player.Active;run.Resume();okay &= Time.timeScale==1&&boot.Player.Active;run.RollUpgrades();okay &= run.Choices[0]!=run.Choices[1]&&run.Choices[1]!=run.Choices[2];run.SetState(RunState.Upgrade);int wave=boot.Waves.Wave;run.Choose(0);okay &= boot.Waves.Wave==wave+1&&run.State==RunState.Playing;run.End(false);okay &= run.State==RunState.Defeat;run.StartRun();okay &= run.Score==0&&boot.Player.Health==100&&boot.Waves.Wave==1;var saved=JsonUtility.FromJson<SaveData>(JsonUtility.ToJson(run.Save));okay &= saved.Fps==run.Save.Fps;run.Menu();okay &= run.State==RunState.Menu&&boot.Enemies.Alive==0;}Debug.Log(okay?"PLAY_SMOKE_PASS":"PLAY_SMOKE_FAIL");SessionState.SetBool("NeonSmoke",false);EditorApplication.Exit(okay?0:1);};EditorApplication.update+=check;}};}
  public static void BakeAndBuild(){BakedArenaBuilder.Create();Build();}
  public static void Build(){BuildPreparation.Prepare();var report=BuildPipeline.BuildPlayer(EditorBuildSettings.scenes,"Builds/Windows/NeonSiege.exe",BuildTarget.StandaloneWindows64,BuildOptions.None);if(report.summary.result!=UnityEditor.Build.Reporting.BuildResult.Succeeded)throw new System.Exception("Windows build failed");Debug.Log("WINDOWS_BUILD_PASS "+report.summary.totalSize);}
 }
}






