using System.IO;
using UnityEditor;
using UnityEngine;
namespace NeonSiege.Editor {
 public static class BuildPreparation {
  public static void Prepare(){Directory.CreateDirectory("Assets/_Project/Resources/Materials");const string path="Assets/_Project/Resources/Materials/Tracer.mat";if(!AssetDatabase.LoadAssetAtPath<Material>(path)){var m=new Material(Shader.Find("Universal Render Pipeline/Unlit"));m.SetColor("_BaseColor",Palette.Cyan*2);AssetDatabase.CreateAsset(m,path);}
   PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.StandaloneWindows64,false);PlayerSettings.SetGraphicsAPIs(BuildTarget.StandaloneWindows64,new[]{UnityEngine.Rendering.GraphicsDeviceType.Direct3D11});PlayerSettings.companyName="Siege Studio";PlayerSettings.productName="NEON SIEGE";PlayerSettings.defaultScreenWidth=1600;PlayerSettings.defaultScreenHeight=900;PlayerSettings.fullScreenMode=FullScreenMode.Windowed;PlayerSettings.runInBackground=true;PlayerSettings.SetScriptingBackend(UnityEditor.Build.NamedBuildTarget.Standalone,ScriptingImplementation.Mono2x);PlayerSettings.SetApiCompatibilityLevel(UnityEditor.Build.NamedBuildTarget.Standalone,ApiCompatibilityLevel.NET_Standard);PlayerSettings.SetManagedStrippingLevel(UnityEditor.Build.NamedBuildTarget.Standalone,ManagedStrippingLevel.Low);PlayerSettings.SetStaticBatchingForPlatform(BuildTarget.StandaloneWindows64,true);AssetDatabase.SaveAssets();}
 }
}

