using UnityEngine;
using UnityEngine.AI;
namespace NeonSiege {
 public sealed class Enemy : Damageable {
  public EnemyDefinition Definition {get;private set;}public NavMeshAgent Agent {get;private set;}public Transform Visual {get;private set;}
  ActorPresentation presentation;EnemyDirector director;Operator player;EnemyProjectiles projectiles;CombatEffects effects;float attack,think,telegraph;float strafePhase;Vector3 strike;LineRenderer warning;int wave;
  public bool IsBoss=>Definition&&Definition.Kind==EnemyKind.Boss;
  public void Initialize(EnemyDirector owner,Operator target,EnemyProjectiles shots,CombatEffects fx){strafePhase=Random.Range(0,6.28f);director=owner;player=target;projectiles=shots;effects=fx;gameObject.layer=6;
   Agent=gameObject.AddComponent<NavMeshAgent>();Agent.radius=.45f;Agent.height=2;Agent.acceleration=20;Agent.angularSpeed=720;Agent.enabled=false;Agent.obstacleAvoidanceType=ObstacleAvoidanceType.LowQualityObstacleAvoidance;
   var collider=gameObject.AddComponent<CapsuleCollider>();collider.height=2;collider.radius=.5f;collider.center=Vector3.up;
   Visual=new GameObject("Enemy visual").transform;Visual.SetParent(transform,false);Palette.Box("Armor",Vector3.up,new Vector3(.75f,1.2f,.6f),Palette.Metal,Visual,false);Palette.Box("Core",new Vector3(0,1.2f,.33f),new Vector3(.4f,.4f,.08f),Palette.OrangeMat,Visual,false);Palette.Box("Head",Vector3.up*1.8f,new Vector3(.55f,.4f,.5f),Palette.OrangeMat,Visual,false);
   if(ArtLibrary.Load()){foreach(Transform child in Visual)child.gameObject.SetActive(false);presentation=Visual.gameObject.AddComponent<ActorPresentation>();presentation.Initialize(false);}var ring=new GameObject("Attack telegraph");warning=ring.AddComponent<LineRenderer>();warning.sharedMaterial=Palette.OrangeMat;warning.positionCount=49;warning.startWidth=warning.endWidth=.1f;warning.enabled=false;}
  public void Spawn(EnemyDefinition definition,Vector3 pos,int waveNumber){Definition=definition;if(presentation)presentation.Select((int)definition.Kind);wave=waveNumber;MaxHealth=Health=definition.Health*(1+(wave-1)*.12f);attack=1.5f;think=0;telegraph=0;warning.enabled=false;transform.position=pos;transform.localScale=Vector3.one*definition.Scale;gameObject.SetActive(true);Agent.enabled=true;Agent.Warp(pos);Agent.speed=definition.Speed;Agent.stoppingDistance=IsBoss?5:definition.Kind==EnemyKind.Gunner?10:1.2f;Agent.isStopped=false;}
  void Update(){if(!Agent.enabled||!Agent.isOnNavMesh||!player.Active)return;float dt=Time.deltaTime;attack-=dt;think-=dt;Vector3 delta=player.transform.position-transform.position;float distance=delta.magnitude;
   if(telegraph>0){telegraph-=dt;if(telegraph<=0){warning.enabled=false;effects.Burst(strike+Vector3.up*.2f,30);if(Vector3.Distance(player.transform.position,strike)<4)player.Damage(Definition.Damage*1.7f);Agent.isStopped=false;for(int i=0;i<12;i++){float a=i*Mathf.PI/6;projectiles.Fire(transform.position+Vector3.up*1.3f,new Vector3(Mathf.Sin(a),0,Mathf.Cos(a)),12,10);}}return;}
   if(think<=0){think=.2f;Vector3 target=player.transform.position;
    if(Definition.Kind==EnemyKind.Gunner&&distance<15)target+=Vector3.Cross(delta.normalized,Vector3.up)*Mathf.Sin(Time.time*.7f+strafePhase)*6;
    Agent.SetDestination(target);}
   if(IsBoss){Agent.speed=Definition.Speed*(Health<MaxHealth*.35f?1.5f:1);if(attack<=0){attack=Health<MaxHealth*.5f?3:4.5f;strike=player.transform.position;telegraph=1.35f;Agent.isStopped=true;for(int i=0;i<49;i++){float a=i*Mathf.PI*2/48;warning.SetPosition(i,strike+new Vector3(Mathf.Cos(a)*4,.08f,Mathf.Sin(a)*4));}warning.enabled=true;}return;}
   if(Definition.Kind==EnemyKind.Gunner&&distance<22&&attack<=0){var origin=transform.position+Vector3.up*1.4f;var aim=player.transform.position+Vector3.up;if(!Physics.Linecast(origin,aim,1)){attack=Definition.AttackInterval;projectiles.Fire(origin,(aim-origin).normalized,Definition.Damage*(1+wave*.04f));}}
   else if(Definition.Kind!=EnemyKind.Gunner&&distance<1.8f*Definition.Scale&&attack<=0&&!Physics.Linecast(transform.position+Vector3.up,player.transform.position+Vector3.up,1,QueryTriggerInteraction.Ignore)){attack=Definition.AttackInterval;player.Damage(Definition.Damage*(1+wave*.04f));effects.Burst(player.transform.position+Vector3.up,4);}
  }
  public override bool TakeDamage(float amount){if(Health<=0)return false;float armor=Definition.Kind==EnemyKind.Tank?.7f:1;Health-=amount*armor;if(Health>0)return false;Health=0;effects.Burst(transform.position+Vector3.up,IsBoss?50:12);director.Killed(this);Despawn();return true;}
  public void Despawn(){warning.enabled=false;if(Agent.enabled){if(Agent.isOnNavMesh)Agent.ResetPath();Agent.enabled=false;}gameObject.SetActive(false);}
 }
}


