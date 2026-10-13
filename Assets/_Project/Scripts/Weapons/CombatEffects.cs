using UnityEngine;
namespace NeonSiege {
 public sealed class CombatEffects : MonoBehaviour {
  struct Tracer {public LineRenderer Line;public float Life;}
  readonly Tracer[] traces=new Tracer[48];int traceIndex;ParticleSystem particles;
  public void Initialize(){particles=gameObject.AddComponent<ParticleSystem>();particles.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);var main=particles.main;main.loop=true;main.playOnAwake=false;main.maxParticles=1200;main.simulationSpace=ParticleSystemSimulationSpace.World;main.startLifetime=.35f;main.startSize=.065f;main.gravityModifier=1;var emission=particles.emission;emission.enabled=false;var shape=particles.shape;shape.enabled=false;particles.GetComponent<ParticleSystemRenderer>().sharedMaterial=Palette.OrangeMat;particles.Play();
   var mat=Resources.Load<Material>("Materials/Tracer");if(!mat){mat=new Material(Shader.Find("Universal Render Pipeline/Unlit"));mat.SetColor("_BaseColor",Palette.Cyan*2);}
   for(int i=0;i<traces.Length;i++){var o=new GameObject("Pooled tracer");o.transform.SetParent(transform);var line=o.AddComponent<LineRenderer>();line.sharedMaterial=mat;line.positionCount=2;line.startWidth=.035f;line.endWidth=.008f;line.enabled=false;traces[i].Line=line;}}
  public void Trace(Vector3 from,Vector3 to){ref var t=ref traces[traceIndex++%traces.Length];t.Line.SetPosition(0,from);t.Line.SetPosition(1,to);t.Line.enabled=true;t.Life=.055f;}
  public void Burst(Vector3 pos,int count=8){for(int i=0;i<count;i++){var emit=new ParticleSystem.EmitParams{position=pos,velocity=Random.insideUnitSphere*5+Vector3.up*2,startLifetime=Random.Range(.15f,.45f),startSize=Random.Range(.035f,.09f)};particles.Emit(emit,1);}}
  public void Shell(Vector3 pos,Vector3 direction){var emit=new ParticleSystem.EmitParams{position=pos,velocity=direction*3+Vector3.up*2,startLifetime=.6f,startSize=.055f};particles.Emit(emit,1);}
  void Update(){float dt=Time.deltaTime;for(int i=0;i<traces.Length;i++){ref var t=ref traces[i];if(t.Life<=0)continue;t.Life-=dt;if(t.Life<=0)t.Line.enabled=false;}}
 }
}


