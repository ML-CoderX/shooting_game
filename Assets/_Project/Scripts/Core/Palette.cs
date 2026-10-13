using UnityEngine;
namespace NeonSiege {
 public static class Palette {
  public static readonly Color Cyan=new Color(.05f,.9f,1), Orange=new Color(1,.27f,.07f), Pink=new Color(1,.08f,.4f);
  public static Material Metal,Floor,CyanMat,OrangeMat,PinkMat,White;
  public static void Init(){ if(Metal) return; Metal=Mat("Obsidian",new Color(.065f,.09f,.13f)); Floor=Mat("Basalt",new Color(.12f,.16f,.2f)); CyanMat=Mat("Ion",Cyan,3); OrangeMat=Mat("Ember",Orange,3); PinkMat=Mat("Corrupt",Pink,3); White=Mat("Ceramic",new Color(.55f,.66f,.7f)); }
  static Material Mat(string name,Color color,float glow=0){var saved=Resources.Load<Material>("Materials/"+name);if(saved)return saved;var m=new Material(Shader.Find("Universal Render Pipeline/Lit")){name=name,enableInstancing=true};m.SetColor("_BaseColor",color);m.SetFloat("_Metallic",.45f);m.SetFloat("_Smoothness",.5f);if(glow>0){m.EnableKeyword("_EMISSION");m.SetColor("_EmissionColor",color*glow);}return m;}
  public static GameObject Box(string name,Vector3 pos,Vector3 scale,Material mat,Transform parent=null,bool collider=true){var o=GameObject.CreatePrimitive(PrimitiveType.Cube);o.name=name;o.transform.SetParent(parent,false);o.transform.localPosition=pos;o.transform.localScale=scale;var renderer=o.GetComponent<Renderer>();renderer.sharedMaterial=mat;if(!collider&&mat.IsKeywordEnabled("_EMISSION")){renderer.shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off;renderer.receiveShadows=false;}if(!collider) {o.GetComponent<Collider>().enabled=false;if(Application.isPlaying)Object.Destroy(o.GetComponent<Collider>());else Object.DestroyImmediate(o.GetComponent<Collider>());}return o;}
 }
}


