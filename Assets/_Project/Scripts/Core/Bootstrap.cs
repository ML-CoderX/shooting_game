using UnityEngine;
namespace NeonSiege {
 public sealed class Bootstrap : MonoBehaviour {
  public Arena SceneArena; public Atmosphere Atmosphere {get;private set;}public GameSession Session {get;private set;}public EnemyDirector Enemies {get;private set;}public WaveDirector Waves {get;private set;} public Operator Player {get;private set;}public WeaponSystem Weapons {get;private set;}public CombatEffects Effects {get;private set;}public SiegeAudio Audio {get;private set;}
  void Start(){Palette.Init();if(!SceneArena)new GameObject("Arena").AddComponent<Arena>().Build();Player=new GameObject("Operator").AddComponent<Operator>();Player.gameObject.layer=2;Player.Initialize();Effects=new GameObject("Effects").AddComponent<CombatEffects>();Effects.Initialize();Audio=new GameObject("Audio").AddComponent<SiegeAudio>();Audio.Initialize();Weapons=Player.gameObject.AddComponent<WeaponSystem>();Weapons.Initialize(Player,Effects,Audio);Enemies=new GameObject("Enemies").AddComponent<EnemyDirector>();Enemies.Initialize(Player,Effects);Waves=gameObject.AddComponent<WaveDirector>();Waves.Initialize(Enemies,Audio);Atmosphere=new GameObject("Atmosphere").AddComponent<Atmosphere>();Atmosphere.Initialize();Session=gameObject.AddComponent<GameSession>();Session.Initialize(this);new GameObject("Interface").AddComponent<GameUI>().Initialize(Session);new GameObject("Damage numbers").AddComponent<DamageNumbers>().Initialize(Weapons,Player.View);gameObject.AddComponent<KillFeedback>().Initialize(Session);gameObject.AddComponent<RuntimeDiagnostics>().Initialize(this);if(System.Environment.CommandLine.Contains("-siegeQA"))gameObject.AddComponent<CombatValidation>().Initialize(this);if(System.Environment.CommandLine.Contains("ProjectBuilder.PlaySmoke"))Session.StartRun();}
 }
}






