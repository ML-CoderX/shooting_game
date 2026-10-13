using UnityEngine;
namespace NeonSiege {
 public sealed class PickupDirector : MonoBehaviour {
  struct Drop {public Transform Visual;public float Life;public int Kind;}
  readonly Drop[] drops=new Drop[32];int index;GameSession session;
  public void Initialize(GameSession run){session=run;for(int i=0;i<drops.Length;i++){var o=Palette.Box("Supply",Vector3.zero,Vector3.one*.35f,Palette.CyanMat,transform,false);o.SetActive(false);drops[i].Visual=o.transform;}}
  public void Spawn(Vector3 position){ref var d=ref drops[index++%drops.Length];d.Kind=Random.Range(0,3);d.Life=22;d.Visual.position=position+Vector3.up*.6f;d.Visual.GetComponent<Renderer>().sharedMaterial=d.Kind==0?Palette.PinkMat:d.Kind==1?Palette.CyanMat:Palette.OrangeMat;d.Visual.gameObject.SetActive(true);}
  void Update(){if(session.State!=RunState.Playing)return;for(int i=0;i<drops.Length;i++){ref var d=ref drops[i];if(d.Life<=0)continue;d.Life-=Time.deltaTime;d.Visual.Rotate(40*Time.deltaTime,90*Time.deltaTime,0);var delta=session.Boot.Player.transform.position+Vector3.up*.7f-d.Visual.position;if(delta.sqrMagnitude<16)d.Visual.position+=delta.normalized*Time.deltaTime*6;
    if(delta.sqrMagnitude<1){if(d.Kind==0)session.Boot.Player.Health=Mathf.Min(session.Boot.Player.MaxHealth,session.Boot.Player.Health+25);else if(d.Kind==1)session.Boot.Weapons.Refill();else session.Boost();session.Boot.Audio.Click();d.Life=0;}if(d.Life<=0)d.Visual.gameObject.SetActive(false);}}
  public void Clear(){for(int i=0;i<drops.Length;i++){drops[i].Life=0;drops[i].Visual.gameObject.SetActive(false);}}
 }
}
