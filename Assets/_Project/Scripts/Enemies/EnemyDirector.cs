using System;
using UnityEngine;
using UnityEngine.AI;
namespace NeonSiege {
 public sealed class EnemyDirector : MonoBehaviour {
  public readonly Enemy[] Pool=new Enemy[32];public EnemyDefinition[] Definitions;public int Alive {get;private set;}public Enemy Boss {get;private set;}public event Action<Enemy> EnemyKilled;
  public EnemyProjectiles Projectiles {get;private set;}readonly NavMeshPath spawnPath=new NavMeshPath();
  public void Initialize(Operator player,CombatEffects effects){Definitions=Resources.LoadAll<EnemyDefinition>("Enemies");Array.Sort(Definitions,(a,b)=>a.Kind.CompareTo(b.Kind));Projectiles=gameObject.AddComponent<EnemyProjectiles>();Projectiles.Initialize(player,effects);
   for(int i=0;i<Pool.Length;i++){var o=new GameObject("Pooled enemy "+i);o.transform.SetParent(transform);var enemy=o.AddComponent<Enemy>();enemy.Initialize(this,player,Projectiles,effects);o.SetActive(false);Pool[i]=enemy;}}
  public bool Spawn(EnemyKind kind,int wave,Vector3? position=null){Enemy enemy=null;for(int i=0;i<Pool.Length;i++)if(!Pool[i].gameObject.activeSelf){enemy=Pool[i];break;}if(!enemy)return false;
   float a=UnityEngine.Random.value*Mathf.PI*2;var desired=position??new Vector3(Mathf.Sin(a)*27,0,Mathf.Cos(a)*27);if(!NavMesh.SamplePosition(desired,out var hit,4,NavMesh.AllAreas))return false;
   if(Physics.CheckCapsule(hit.position+Vector3.up*.6f,hit.position+Vector3.up*1.5f,.5f,1,QueryTriggerInteraction.Ignore)||!NavMesh.CalculatePath(hit.position,Vector3.zero,NavMesh.AllAreas,spawnPath)||spawnPath.status!=NavMeshPathStatus.PathComplete)return false;
   enemy.Spawn(Definitions[(int)kind],hit.position,wave);Alive++;if(kind==EnemyKind.Boss)Boss=enemy;return true;}
  public void Killed(Enemy enemy){Alive=Mathf.Max(0,Alive-1);if(Boss==enemy)Boss=null;EnemyKilled?.Invoke(enemy);}
  public void Clear(){for(int i=0;i<Pool.Length;i++)if(Pool[i].gameObject.activeSelf)Pool[i].Despawn();Alive=0;Boss=null;Projectiles.Clear();}
 }
}
