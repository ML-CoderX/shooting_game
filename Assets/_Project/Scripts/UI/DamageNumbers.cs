using TMPro;
using UnityEngine;
namespace NeonSiege {
 public sealed class DamageNumbers : MonoBehaviour {
  struct Number {public TextMeshPro Text;public Vector3 Velocity;public float Life;}
  readonly Number[] pool=new Number[32];int index;Camera view;
  public void Initialize(WeaponSystem weapons,Camera camera){view=camera;for(int i=0;i<pool.Length;i++){var o=new GameObject("Pooled damage number");o.transform.SetParent(transform);var text=o.AddComponent<TextMeshPro>();text.font=TMP_Settings.defaultFontAsset;text.fontSize=4;text.alignment=TextAlignmentOptions.Center;text.rectTransform.sizeDelta=new Vector2(3,1);text.renderer.shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off;text.gameObject.SetActive(false);pool[i].Text=text;}weapons.Hit+=Show;}
  void Show(Vector3 pos,float damage,bool fatal){ref var n=ref pool[index++%pool.Length];n.Text.transform.position=pos+Vector3.up*.7f;n.Text.SetText("{0:0}",damage);n.Text.color=fatal?Palette.Orange:Color.white;n.Text.gameObject.SetActive(true);n.Velocity=new Vector3(Random.Range(-.3f,.3f),1,0);n.Life=.55f;}
  void Update(){for(int i=0;i<pool.Length;i++){ref var n=ref pool[i];if(n.Life<=0)continue;n.Life-=Time.deltaTime;n.Text.transform.position+=n.Velocity*Time.deltaTime;n.Text.transform.rotation=view.transform.rotation;n.Text.alpha=Mathf.Clamp01(n.Life*3);if(n.Life<=0)n.Text.gameObject.SetActive(false);}}
 }
}
