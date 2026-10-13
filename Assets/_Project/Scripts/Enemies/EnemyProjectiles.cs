using UnityEngine;
namespace NeonSiege {
 public sealed class EnemyProjectiles : MonoBehaviour {
  struct Bolt {public Transform Visual;public Vector3 Velocity;public float Life,Damage;}
  readonly Bolt[] bolts=new Bolt[160];int index;Operator player;CombatEffects effects;
  public void Initialize(Operator target,CombatEffects fx){player=target;effects=fx;for(int i=0;i<bolts.Length;i++){var o=Palette.Box("Enemy bolt",Vector3.zero,new Vector3(.16f,.16f,.5f),Palette.OrangeMat,transform,false);o.SetActive(false);bolts[i].Visual=o.transform;}}
  public void Fire(Vector3 origin,Vector3 direction,float damage,float speed=13){ref var b=ref bolts[index++%bolts.Length];b.Visual.position=origin;b.Visual.rotation=Quaternion.LookRotation(direction);b.Velocity=direction*speed;b.Damage=damage;b.Life=5;b.Visual.gameObject.SetActive(true);}
  void Update(){float dt=Time.deltaTime;for(int i=0;i<bolts.Length;i++){ref var b=ref bolts[i];if(b.Life<=0)continue;var from=b.Visual.position;var step=b.Velocity*dt;b.Life-=dt;
   if(Physics.SphereCast(from,.14f,step.normalized,out var hit,step.magnitude,1,QueryTriggerInteraction.Ignore))b.Life=0;
   Vector3 center=player.transform.position+Vector3.up;float u=Mathf.Clamp01(Vector3.Dot(center-from,step)/Mathf.Max(.0001f,step.sqrMagnitude));if(Vector3.Distance(center,from+step*u)<.65f){player.Damage(b.Damage);effects.Burst(center,3);b.Life=0;}
   b.Visual.position+=step;if(b.Life<=0)b.Visual.gameObject.SetActive(false);}}
  public void Clear(){for(int i=0;i<bolts.Length;i++){bolts[i].Life=0;bolts[i].Visual.gameObject.SetActive(false);}}
 }
}
