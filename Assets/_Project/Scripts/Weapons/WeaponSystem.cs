using System;
using UnityEngine;
using UnityEngine.InputSystem;
namespace NeonSiege {
 public sealed class WeaponSystem : MonoBehaviour {
  public WeaponDefinition[] Definitions;public WeaponDefinition Current=>Definitions[Slot];public int Slot {get;private set;}public int Ammo {get;private set;}public float ReloadRemaining {get;private set;}
  public float DamageMultiplier=1,RateMultiplier=1,MagazineMultiplier=1;public bool Explosive,Lifesteal,Ricochet;public float HitMarker {get;private set;}public int ShotsFired {get;private set;}
  public event Action<Vector3,float,bool> Hit;
  readonly int[] magazines=new int[5]; bool equipped; Operator player;CombatEffects effects;SiegeAudio audioBus;Transform mount; GameObject[] models;float cooldown;readonly RaycastHit[] hits=new RaycastHit[32];readonly Collider[] overlaps=new Collider[64];readonly Damageable[] damaged=new Damageable[64];
  struct Grenade {public Transform Visual;public Vector3 Velocity;public float Life,Damage;}
  readonly Grenade[] grenades=new Grenade[24];int grenadeIndex;
  public void Initialize(Operator owner,CombatEffects fx,SiegeAudio sound){player=owner;effects=fx;audioBus=sound;Definitions=Resources.LoadAll<WeaponDefinition>("Weapons");Array.Sort(Definitions,(a,b)=>a.Slot.CompareTo(b.Slot));
   mount=new GameObject("Weapon mount").transform;mount.SetParent(player.Visual,false);mount.localPosition=new Vector3(.4f,1.15f,.4f);Palette.Box("Rifle",new Vector3(0,0,.15f),new Vector3(.16f,.2f,.85f),Palette.Metal,mount,false);Palette.Box("Capacitor",new Vector3(0,.12f,.15f),new Vector3(.1f,.04f,.5f),Palette.CyanMat,mount,false);
   if(ArtLibrary.Load()){foreach(Transform child in mount)child.gameObject.SetActive(false);models=new GameObject[5];for(int i=0;i<5;i++){models[i]=ArtLibrary.Load().Spawn(ArtLibrary.Load().Weapons[i],mount);models[i].SetActive(false);}}for(int i=0;i<grenades.Length;i++){var o=GameObject.CreatePrimitive(PrimitiveType.Sphere);o.name="Pooled grenade";o.transform.localScale=Vector3.one*.2f;var c=o.GetComponent<Collider>();c.enabled=false;Destroy(c);o.GetComponent<Renderer>().sharedMaterial=Palette.OrangeMat;o.SetActive(false);grenades[i].Visual=o.transform;}for(int i=0;i<5;i++)magazines[i]=Definitions[i].Magazine;Switch(0);}
  public void Refill(){Ammo=Mathf.CeilToInt(Current.Magazine*MagazineMultiplier);ReloadRemaining=0;} public void Switch(int slot){if(equipped)magazines[Slot]=Ammo;Slot=(slot+Definitions.Length)%Definitions.Length;Ammo=magazines[Slot];if(models!=null)for(int i=0;i<5;i++)models[i].SetActive(i==Slot);equipped=true;ReloadRemaining=0;cooldown=.15f;}
  public void Reload(){if(ReloadRemaining>0||Ammo>=Mathf.CeilToInt(Current.Magazine*MagazineMultiplier))return;player.Visual.GetComponent<ActorPresentation>()?.Trigger("Reload");ReloadRemaining=Current.ReloadTime;audioBus.Reload();}
  void Update(){HitMarker=Mathf.Max(0,HitMarker-Time.unscaledDeltaTime);if(!player.Active)return;float dt=Time.deltaTime;cooldown-=dt;
   if(ReloadRemaining>0){ReloadRemaining-=dt;if(ReloadRemaining<=0)Ammo=Mathf.CeilToInt(Current.Magazine*MagazineMultiplier);}
   if(player.Input.Next.WasPressedThisFrame()||(Mouse.current!=null&&Mouse.current.scroll.ReadValue().y<0))Switch(Slot+1);
   if(Mouse.current!=null&&Mouse.current.scroll.ReadValue().y>0)Switch(Slot-1);
   if(Keyboard.current!=null)for(int i=0;i<5;i++)if(Keyboard.current[(Key)((int)Key.Digit1+i)].wasPressedThisFrame)Switch(i);
   if(player.Input.Reload.WasPressedThisFrame())Reload();if(player.Input.Fire.IsPressed())TryFire();
   for(int i=0;i<grenades.Length;i++){ref var g=ref grenades[i];if(g.Life<=0)continue;g.Life-=dt;g.Velocity+=Vector3.down*9*dt;Vector3 step=g.Velocity*dt;
    if(Physics.Raycast(g.Visual.position,step.normalized,out var h,step.magnitude,~(1<<2),QueryTriggerInteraction.Ignore)){g.Visual.position=h.point;g.Life=0;}else g.Visual.position+=step;
    if(g.Life<=0){Explode(g.Visual.position,g.Damage,4.5f);g.Visual.gameObject.SetActive(false);}}
  }
  public bool TryFire(){if(cooldown>0||ReloadRemaining>0)return false;if(Ammo<=0){Reload();return false;}Ammo--;ShotsFired++;cooldown=1/(Current.FireRate*RateMultiplier);player.Recoil(Current.Recoil);audioBus.Shot(Current.Slot==3?1.3f:1);Vector3 origin=player.View.transform.position;Vector3 forward=player.View.transform.forward;Vector3 muzzle=mount.position+player.transform.forward*.85f;effects.Burst(muzzle,2);effects.Shell(mount.position,player.transform.right);
   if(Current.Projectile){ref var g=ref grenades[grenadeIndex++%grenades.Length];g.Visual.position=muzzle;g.Velocity=forward*24+Vector3.up*3;g.Life=2;g.Damage=Current.Damage*DamageMultiplier;g.Visual.gameObject.SetActive(true);return true;}
   for(int pellet=0;pellet<Current.Pellets;pellet++){var spread=UnityEngine.Random.insideUnitCircle*Current.Spread*(player.Input.Aim.IsPressed()?.35f:1);var direction=Quaternion.Euler(spread.y,spread.x,0)*forward;Vector3 target=origin+direction*Current.Range;if(Physics.Raycast(origin,direction,out var aimHit,Current.Range,~(1<<2),QueryTriggerInteraction.Ignore))target=aimHit.point;
    direction=(target-muzzle).normalized;int count=Physics.RaycastNonAlloc(muzzle,direction,hits,Current.Range,~(1<<2),QueryTriggerInteraction.Ignore);SortHits(count);Vector3 end=muzzle+direction*Current.Range;
    for(int n=0;n<count;n++){var h=hits[n];end=h.point;var victim=h.collider.GetComponentInParent<Damageable>();if(victim){Apply(victim,Current.Damage*DamageMultiplier,h.point);if(Explosive)Explode(h.point,Current.Damage*.3f*DamageMultiplier,2);if(Ricochet)Bounce(victim,h.point,Current.Damage*.4f*DamageMultiplier);}effects.Burst(h.point,4);if(!Current.Pierce||!victim)break;}
    effects.Trace(muzzle,end);
   }return true;
  }
  void SortHits(int count){for(int i=1;i<count;i++){var value=hits[i];int j=i-1;while(j>=0&&hits[j].distance>value.distance){hits[j+1]=hits[j];j--;}hits[j+1]=value;}}
  void Apply(Damageable victim,float amount,Vector3 point){if(!victim.gameObject.activeInHierarchy||victim.Health<=0)return;bool fatal=victim.TakeDamage(amount);HitMarker=.12f;audioBus.Hit(fatal);Hit?.Invoke(point,amount,fatal);if(Lifesteal)player.Health=Mathf.Min(player.MaxHealth,player.Health+amount*.035f);}
  public void Explode(Vector3 point,float damage,float radius){effects.Burst(point,24);int count=Physics.OverlapSphereNonAlloc(point,radius,overlaps,~(1<<2),QueryTriggerInteraction.Ignore),unique=0;for(int i=0;i<count;i++){var victim=overlaps[i].GetComponentInParent<Damageable>();if(!victim)continue;bool seen=false;for(int j=0;j<unique;j++)if(damaged[j]==victim){seen=true;break;}if(seen)continue;damaged[unique++]=victim;Vector3 to=victim.transform.position+Vector3.up-point;if(!Physics.Raycast(point+to.normalized*.1f,to.normalized,out var block,to.magnitude,1,QueryTriggerInteraction.Ignore)||block.collider.GetComponentInParent<Damageable>()==victim)Apply(victim,damage,point);}}
  void Bounce(Damageable first,Vector3 point,float damage){int count=Physics.OverlapSphereNonAlloc(point,6,overlaps,~(1<<2));for(int i=0;i<count;i++){var other=overlaps[i].GetComponentInParent<Damageable>();if(other&&other!=first&&other.Health>0){var end=other.transform.position+Vector3.up;if(!Physics.Linecast(point,end,out var block,1)||block.collider.GetComponentInParent<Damageable>()==other){effects.Trace(point,end);Apply(other,damage,end);}break;}}}
  public void ResetRun(){DamageMultiplier=RateMultiplier=MagazineMultiplier=1;Explosive=Lifesteal=Ricochet=false;equipped=false;for(int i=0;i<5;i++)magazines[i]=Definitions[i].Magazine;Switch(0);for(int i=0;i<grenades.Length;i++){grenades[i].Life=0;grenades[i].Visual.gameObject.SetActive(false);}}
 }
}



