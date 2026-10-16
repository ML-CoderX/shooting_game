using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
namespace NeonSiege.Editor {
 public static class NavigationValidation {
  public static void Rebuild(){
   EditorTestStartup.Prepare();
   var scene=EditorSceneManager.OpenScene("Assets/_Project/Scenes/NeonSiege.unity");
   Physics.SyncTransforms();var surface=UnityEngine.Object.FindAnyObjectByType<NavMeshSurface>();
   foreach(var box in surface.GetComponentsInChildren<BoxCollider>())if(box.name=="Pillar"||box.name=="Cover")Arena.ExcludeObstacle(box);
   surface.BuildNavMesh();surface.navMeshData.name="ArenaNavMesh";
   const string path="Assets/_Project/Scenes/ArenaNavMesh.asset";
   var existing=AssetDatabase.LoadAssetAtPath<NavMeshData>(path);
   if(existing){EditorUtility.CopySerialized(surface.navMeshData,existing);surface.RemoveData();surface.navMeshData=existing;surface.AddData();EditorUtility.SetDirty(existing);}
   else AssetDatabase.CreateAsset(surface.navMeshData,path);
   var results=new List<string>();bool passed=true;var route=new NavMeshPath();
   foreach(var collider in UnityEngine.Object.FindObjectsByType<BoxCollider>()){
    if(collider.name!="Pillar"&&collider.name!="Cover")continue;
    var center=collider.bounds.center;center.y=.1f;
    bool blocked=!NavMesh.SamplePosition(center,out var hit,.3f,NavMesh.AllAreas);
    results.Add((blocked?"PASS":"FAIL")+" ground navmesh excluded from "+collider.name+" "+center);passed&=blocked;
   }
   for(int i=0;i<72;i++){float a=i*Mathf.PI/36;var point=new Vector3(Mathf.Sin(a)*27,0,Mathf.Cos(a)*27);
    if(!NavMesh.SamplePosition(point,out var hit,4,NavMesh.AllAreas))continue;
    if(Physics.CheckCapsule(hit.position+Vector3.up*.6f,hit.position+Vector3.up*1.5f,.5f,1))continue;
    bool connected=NavMesh.CalculatePath(Vector3.zero,hit.position,NavMesh.AllAreas,route)&&route.status==NavMeshPathStatus.PathComplete;
    passed&=connected;results.Add((connected?"PASS":"FAIL")+" perimeter route "+i);
   }
   EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();File.WriteAllLines("Docs/navigation-validation.txt",results);
   if(!passed)throw new Exception("Navigation validation failed");Debug.Log("NAVIGATION_QA_PASS");
  }
 }
}
