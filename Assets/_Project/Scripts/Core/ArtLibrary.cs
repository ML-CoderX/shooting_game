using UnityEngine;
namespace NeonSiege {
 public sealed class ArtLibrary : ScriptableObject {
  public GameObject Operator;public GameObject[] Enemies,Weapons;public GameObject Cover,Pillar,Crate;public RuntimeAnimatorController[] Controllers;public Material Armor,PlayerGlow,EnemyGlow;
  static ArtLibrary cached;public static ArtLibrary Load()=>cached?cached:cached=Resources.Load<ArtLibrary>("ArtLibrary");
  public GameObject Spawn(GameObject source,Transform parent,bool enemy=false){var obj=Instantiate(source,parent,false);foreach(var renderer in obj.GetComponentsInChildren<Renderer>()){var materials=renderer.sharedMaterials;for(int i=0;i<materials.Length;i++)materials[i]=materials[i].name.Contains("Glow")?(enemy?EnemyGlow:PlayerGlow):Armor;renderer.sharedMaterials=materials;}return obj;}
 }
}
