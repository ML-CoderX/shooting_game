using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
namespace NeonSiege {
 public enum RunState { Menu,Playing,Upgrade,Paused,Defeat,Victory }
 public sealed class GameSession : MonoBehaviour {
  public Bootstrap Boot {get;private set;}public RunState State {get;private set;}public SaveData Save {get;private set;}public PickupDirector Pickups {get;private set;}
  public bool CaptureInput;public int Score {get;private set;}public int Kills {get;private set;}public int Combo {get;private set;}public float RunTime {get;private set;}public float BoostTime {get;private set;}
  public UpgradeDefinition[] Choices=new UpgradeDefinition[3];UpgradeDefinition[] upgrades;float comboTimer;public event Action StateChanged;
  public void Initialize(Bootstrap boot){Boot=boot;Save=SaveStore.Load();upgrades=Resources.LoadAll<UpgradeDefinition>("Upgrades");if(!string.IsNullOrEmpty(Save.Bindings))try{Boot.Player.Input.Asset.LoadBindingOverridesFromJson(Save.Bindings);}catch(ArgumentException){Save.Bindings="";}
   Pickups=gameObject.AddComponent<PickupDirector>();Pickups.Initialize(this);Boot.Enemies.EnemyKilled+=OnKill;Boot.Waves.WaveCleared+=OnWaveCleared;ApplySettings();ApplyDisplay();SetState(RunState.Menu);}
  void Update(){if(!CaptureInput&&Boot.Player.Input.Pause.WasPressedThisFrame()){if(State==RunState.Playing)SetState(RunState.Paused);else if(State==RunState.Paused)SetState(RunState.Playing);}if(State!=RunState.Playing)return;
   RunTime+=Time.deltaTime;comboTimer-=Time.deltaTime;if(comboTimer<=0)Combo=0;
   if(BoostTime>0){BoostTime-=Time.deltaTime;if(BoostTime<=0)Boot.Weapons.RateMultiplier-=.5f;}
   if(Boot.Player.Health<=0)End(false);}
  public void StartRun(){Boot.Enemies.Clear();Pickups.Clear();Boot.Player.ResetRun();Boot.Weapons.ResetRun();Score=Kills=Combo=0;RunTime=BoostTime=0;SetState(RunState.Playing);Boot.Waves.Begin();}
  public void Resume(){SetState(RunState.Playing);}public void Pause(){SetState(RunState.Paused);}
  public void Menu(){Boot.Waves.Stop();Boot.Enemies.Clear();Pickups.Clear();SetState(RunState.Menu);}
  void OnKill(Enemy enemy){Kills++;Combo=Mathf.Min(8,Combo+1);comboTimer=4;Score+=enemy.Definition.Points*(1+Combo/3);if(UnityEngine.Random.value<.28f||enemy.IsBoss)Pickups.Spawn(enemy.transform.position);}
  void OnWaveCleared(){if(Boot.Waves.Wave>=Boot.Waves.Configuration.TotalWaves){End(true);return;}Boot.Enemies.Projectiles.Clear();RollUpgrades();SetState(RunState.Upgrade);}
  public void RollUpgrades(){for(int i=0;i<3;i++){for(int attempt=0;attempt<100;attempt++){var candidate=upgrades[UnityEngine.Random.Range(0,upgrades.Length)];bool unique=true;for(int j=0;j<i;j++)if(Choices[j]==candidate)unique=false;if(unique&&candidate.Available(Boot.Weapons)){Choices[i]=candidate;break;}}}}
  public void Choose(int index){if(State!=RunState.Upgrade||index<0||index>2)return;Choices[index].Apply(Boot.Player,Boot.Weapons);Boot.Player.Health=Mathf.Min(Boot.Player.MaxHealth,Boot.Player.Health+15);Boot.Weapons.Refill();SetState(RunState.Playing);Boot.Waves.Next();}
  public void Boost(){if(BoostTime<=0)Boot.Weapons.RateMultiplier+=.5f;BoostTime=10;}
  public void End(bool victory){Boot.Waves.Stop();if(Score>Save.HighScore){Save.HighScore=Score;SaveStore.Save(Save);}SetState(victory?RunState.Victory:RunState.Defeat);}
  public void SetState(RunState state){State=state;Boot.Player.Active=state==RunState.Playing;Time.timeScale=Boot.Player.Active?1:0;Cursor.lockState=Boot.Player.Active?CursorLockMode.Locked:CursorLockMode.None;Cursor.visible=!Boot.Player.Active;StateChanged?.Invoke();}
  public void ApplySettings(){var s=Save;s.Quality=Mathf.Clamp(s.Quality,0,2);QualitySettings.SetQualityLevel(Mathf.Min(s.Quality,QualitySettings.names.Length-1),true);var pipeline=QualitySettings.renderPipeline as UniversalRenderPipelineAsset;if(pipeline){pipeline.renderScale=s.Quality==0?.75f:s.Quality==2?1.15f:1;pipeline.shadowDistance=s.Shadows==0?0:s.Shadows==2?60:40;pipeline.shadowCascadeCount=s.Shadows==2?2:1;pipeline.mainLightShadowmapResolution=s.Shadows==2?2048:1024;}QualitySettings.vSyncCount=s.VSync?1:0;Application.targetFrameRate=s.Fps;
   QualitySettings.shadows=s.Shadows==0?UnityEngine.ShadowQuality.Disable:UnityEngine.ShadowQuality.All;QualitySettings.shadowResolution=s.Shadows==2?UnityEngine.ShadowResolution.High:UnityEngine.ShadowResolution.Medium;Boot.Player.Sensitivity=Mathf.Clamp(s.Sensitivity,.03f,.4f);Boot.Player.Fov=Mathf.Clamp(s.Fov,55,100);
   Boot.Audio.Master=s.Master;Boot.Audio.MusicVolume=s.Music;Boot.Audio.SfxVolume=s.Sfx;if(Boot.Atmosphere)Boot.Atmosphere.Blur.active=s.MotionBlur;var camera=Boot.Player.View.GetUniversalAdditionalCameraData();camera.renderPostProcessing=s.Quality>0;camera.antialiasing=s.AntiAliasing==0?AntialiasingMode.None:s.AntiAliasing==1?AntialiasingMode.FastApproximateAntialiasing:AntialiasingMode.SubpixelMorphologicalAntiAliasing;
  }
  public void ApplyDisplay(){int[] widths={1280,1600,1920,2560},heights={720,900,1080,1080};int r=Mathf.Clamp(Save.Resolution,0,3);Screen.SetResolution(widths[r],heights[r],Save.Fullscreen==0?FullScreenMode.FullScreenWindow:FullScreenMode.Windowed);}
  public void Persist(){Save.Bindings=Boot.Player.Input.Asset.SaveBindingOverridesAsJson();SaveStore.Save(Save);}
  void OnDestroy(){Time.timeScale=1;Cursor.lockState=CursorLockMode.None;Cursor.visible=true;}
 }
}





