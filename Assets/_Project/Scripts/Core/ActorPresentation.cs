using UnityEngine;
namespace NeonSiege {
 public sealed class ActorPresentation : MonoBehaviour {
  GameObject[] variants;Animator[] animators;int active;ArtLibrary art;Vector3 previous;float smoothSpeed;
  public void Initialize(bool hero){art=ArtLibrary.Load();if(!art)return;int count=hero?1:5;variants=new GameObject[count];animators=new Animator[count];for(int i=0;i<count;i++){variants[i]=art.Spawn(hero?art.Operator:art.Enemies[i],transform,!hero);animators[i]=variants[i].GetComponent<Animator>();if(!animators[i])animators[i]=variants[i].AddComponent<Animator>();animators[i].runtimeAnimatorController=art.Controllers[hero?0:i+1];animators[i].applyRootMotion=false;variants[i].SetActive(i==0);}previous=transform.position;}
  public void Select(int index){if(variants==null)return;active=index;for(int i=0;i<variants.Length;i++)variants[i].SetActive(i==index);previous=transform.position;}
  void Update(){if(animators==null)return;float speed=Time.deltaTime>0?(transform.position-previous).magnitude/Time.deltaTime:0;previous=transform.position;smoothSpeed=Mathf.Lerp(smoothSpeed,speed,Time.deltaTime*12);animators[active].SetFloat("Speed",smoothSpeed);}
  public void Trigger(string name){if(animators!=null)animators[active].SetTrigger(name);}
 }
}
