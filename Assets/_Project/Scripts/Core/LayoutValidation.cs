using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NeonSiege {
 // Offscreen render-target checks avoid the desktop's window-size limitation.
 public sealed class LayoutValidation : MonoBehaviour {
  Bootstrap boot;Canvas canvas;Camera view;RenderTexture target;
  readonly List<string> results=new List<string>();
  readonly Vector3[] corners=new Vector3[4];bool passed=true;
  public void Initialize(Bootstrap owner){boot=owner;StartCoroutine(Run());}
  void Check(bool value,string detail){passed&=value;results.Add((value?"PASS ":"FAIL ")+detail);}
  void Click(string label){foreach(var b in canvas.GetComponentsInChildren<Button>()){var text=b.GetComponentInChildren<TMP_Text>();if(text&&text.text==label){b.onClick.Invoke();return;}}Check(false,"Button missing: "+label);}
  IEnumerator Capture(string name){
   yield return null;yield return null;Canvas.ForceUpdateCanvases();yield return new WaitForEndOfFrame();
   var size=((RectTransform)canvas.transform).rect.size*canvas.scaleFactor;
   Check(Mathf.Abs(size.x-target.width)<2&&Mathf.Abs(size.y-target.height)<2,$"{name}: canvas {size.x:F0}x{size.y:F0}");
   bool contained=true;
   foreach(var control in canvas.GetComponentsInChildren<Selectable>()){
    ((RectTransform)control.transform).GetWorldCorners(corners);
    foreach(var point in corners){var p=view.WorldToViewportPoint(point);if(p.x<-.001f||p.x>1.001f||p.y<-.001f||p.y>1.001f)contained=false;}
   }
   Check(contained,name+": interactive controls stay inside viewport");
   if(name.EndsWith("hud"))foreach(var text in canvas.GetComponentsInChildren<TMP_Text>())if(text.text=="+"){var r=text.rectTransform;var center=view.WorldToViewportPoint(r.TransformPoint(r.rect.center));Check(Vector2.Distance(new Vector2(center.x,center.y),new Vector2(.5f,.5f))<.001f,name+": crosshair centered");}
   var old=RenderTexture.active;RenderTexture.active=target;var pixels=new Texture2D(target.width,target.height,TextureFormat.RGB24,false);pixels.ReadPixels(new Rect(0,0,target.width,target.height),0,0);pixels.Apply();RenderTexture.active=old;
   File.WriteAllBytes(Path.Combine(Application.dataPath,"../layout-"+name+".png"),pixels.EncodeToPNG());Destroy(pixels);
  }
  IEnumerator Run(){
   yield return new WaitForSecondsRealtime(1);canvas=FindAnyObjectByType<GameUI>().GetComponent<Canvas>();view=boot.Player.View;
   canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=view;canvas.planeDistance=1;
   foreach(int width in new[]{1920,2560}){
    target=new RenderTexture(width,1080,24,RenderTextureFormat.ARGB32);target.Create();view.targetTexture=target;view.aspect=(float)width/1080;
    string prefix=width==1920?"16x9-":"21x9-";
    boot.Session.Menu();yield return Capture(prefix+"menu");
    Click("SETTINGS");yield return Capture(prefix+"settings");
    Click("REBIND CONTROLS");yield return Capture(prefix+"bindings");
    boot.Session.StartRun();boot.Waves.Stop();Time.timeScale=0;yield return Capture(prefix+"hud");
    boot.Session.RollUpgrades();boot.Session.SetState(RunState.Upgrade);yield return Capture(prefix+"upgrades");
    boot.Session.Pause();yield return Capture(prefix+"pause");
    boot.Session.SetState(RunState.Victory);yield return Capture(prefix+"victory");
    boot.Session.Menu();view.targetTexture=null;target.Release();Destroy(target);
   }
   canvas.renderMode=RenderMode.ScreenSpaceOverlay;canvas.worldCamera=null;view.ResetAspect();
   File.WriteAllLines(Path.Combine(Application.dataPath,"../layout-validation.txt"),results);Debug.Log(passed?"LAYOUT_QA_PASS":"LAYOUT_QA_FAIL");yield return new WaitForSecondsRealtime(.2f);Application.Quit(passed?0:1);
  }
 }
}
