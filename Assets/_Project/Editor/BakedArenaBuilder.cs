using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace NeonSiege.Editor {
 public static class BakedArenaBuilder {
  [MenuItem("Neon Siege/Rebuild baked arena")] public static void Create(){ContentBuilder.Create();ArtBuilder.Create();MixerBuilder.Create();Directory.CreateDirectory("Assets/_Project/Resources/Materials");Palette.Init();foreach(var mat in new[]{Palette.Metal,Palette.Floor,Palette.CyanMat,Palette.OrangeMat,Palette.PinkMat,Palette.White})if(!AssetDatabase.Contains(mat)){if(mat.IsKeywordEnabled("_EMISSION"))mat.globalIlluminationFlags=MaterialGlobalIlluminationFlags.BakedEmissive;AssetDatabase.CreateAsset(mat,"Assets/_Project/Resources/Materials/"+mat.name+".mat");}
   var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);var arena=new GameObject("OUTPOST 07").AddComponent<Arena>();arena.Build();var boot=new GameObject("NEON SIEGE").AddComponent<Bootstrap>();boot.SceneArena=arena;
   foreach(var renderer in arena.GetComponentsInChildren<MeshRenderer>()){if(renderer.enabled)GameObjectUtility.SetStaticEditorFlags(renderer.gameObject,StaticEditorFlags.ContributeGI|StaticEditorFlags.BatchingStatic|StaticEditorFlags.ReflectionProbeStatic);}
   foreach(var light in UnityEngine.Object.FindObjectsByType<Light>())light.lightmapBakeType=LightmapBakeType.Mixed;
   var probes=new GameObject("Light probes").AddComponent<LightProbeGroup>();var positions=new List<Vector3>();for(int x=-24;x<=24;x+=12)for(int z=-24;z<=24;z+=12){positions.Add(new Vector3(x,1.2f,z));positions.Add(new Vector3(x,4,z));}probes.probePositions=positions.ToArray();
   var lighting=new LightingSettings();lighting.bakedGI=true;lighting.realtimeGI=false;lighting.lightmapper=LightingSettings.Lightmapper.UnityComputeGPU;lighting.lightmapResolution=4;lighting.lightmapMaxSize=1024;lighting.directSampleCount=32;lighting.indirectSampleCount=64;lighting.environmentSampleCount=32;Lightmapping.lightingSettings=lighting;
   string lightPath="Assets/_Project/Scenes/LightingSettings.lighting";if(AssetDatabase.LoadAssetAtPath<LightingSettings>(lightPath))AssetDatabase.DeleteAsset(lightPath);AssetDatabase.CreateAsset(lighting,lightPath);
   string navPath="Assets/_Project/Scenes/ArenaNavMesh.asset";if(AssetDatabase.LoadAssetAtPath<UnityEngine.AI.NavMeshData>(navPath))AssetDatabase.DeleteAsset(navPath);AssetDatabase.CreateAsset(arena.Surface.navMeshData,navPath);
   EditorSceneManager.SaveScene(scene,"Assets/_Project/Scenes/NeonSiege.unity");EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(scene.path,true)};AssetDatabase.SaveAssets();bool result=Lightmapping.Bake();EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();Debug.Log(result?"BAKED_ARENA_PASS":"BAKED_ARENA_FAILED");if(!result)throw new System.Exception("Lightmap bake failed");}
 }
}

