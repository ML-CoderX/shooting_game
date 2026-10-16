using UnityEngine;
using Unity.AI.Navigation;
namespace NeonSiege {
 public sealed class Arena : MonoBehaviour {
  public NavMeshSurface Surface {get;private set;}
  public void Build(){
   Palette.Init();
   Palette.Box("Arena floor",new Vector3(0,-.5f,0),new Vector3(64,1,64),Palette.Floor,transform);
   for(int i=-30;i<=30;i+=6){Palette.Box("Floor seam",new Vector3(i,.006f,0),new Vector3(.035f,.012f,62),Palette.CyanMat,transform,false);Palette.Box("Floor seam",new Vector3(0,.006f,i),new Vector3(62,.012f,.035f),Palette.CyanMat,transform,false);}
   for(int side=0;side<4;side++){
    var wall=new GameObject("Perimeter").transform;wall.SetParent(transform);wall.rotation=Quaternion.Euler(0,side*90,0);
    Palette.Box("Wall",new Vector3(0,2,32),new Vector3(65,4,1),Palette.Metal,wall);
    Palette.Box("Boundary light",new Vector3(0,3.8f,31.4f),new Vector3(64,.12f,.12f),Palette.OrangeMat,wall,false);
    for(int x=-24;x<=24;x+=12){Palette.Box("Buttress",new Vector3(x,4,33),new Vector3(2,8,3),Palette.Metal,wall);Palette.Box("Beacon",new Vector3(x,7.6f,31.4f),new Vector3(.25f,3,.25f),Palette.OrangeMat,wall,false);}
   }
   for(int x=-1;x<=1;x+=2)for(int z=-1;z<=1;z+=2){
    Palette.Box("Cover",new Vector3(x*12,1.2f,z*10),new Vector3(6,2.4f,2),Palette.Metal,transform);
    Palette.Box("Cover trim",new Vector3(x*12,2.42f,z*10),new Vector3(6,.06f,2),Palette.CyanMat,transform,false);
    Palette.Box("Pillar",new Vector3(x*21,3,z*19),new Vector3(3,6,3),Palette.Metal,transform);
    Palette.Box("Pillar strip",new Vector3(x*21,3,z*19-1.52f),new Vector3(.15f,5,.08f),Palette.CyanMat,transform,false);
   }
   var art=ArtLibrary.Load();if(art){foreach(Transform child in transform)if(child.name=="Cover"||child.name=="Pillar"||child.name=="Cover trim"||child.name=="Pillar strip")child.GetComponent<Renderer>().enabled=false;
    for(int x=-1;x<=1;x+=2)for(int z=-1;z<=1;z+=2){var cover=art.Spawn(art.Cover,transform);cover.transform.localPosition=new Vector3(x*12,0,z*10);cover.transform.localScale=new Vector3(1.2f,1.14f,1.33f);var pillar=art.Spawn(art.Pillar,transform);pillar.transform.localPosition=new Vector3(x*21,0,z*19);for(int i=0;i<2;i++){var crate=art.Spawn(art.Crate,transform);crate.transform.localPosition=new Vector3(x*(23+i*1.6f),0,z*24);}}}
   foreach(var box in GetComponentsInChildren<BoxCollider>())if(box.name=="Cover"||box.name=="Pillar")ExcludeObstacle(box);
   Surface=gameObject.AddComponent<NavMeshSurface>();Surface.collectObjects=CollectObjects.Children;Surface.useGeometry=UnityEngine.AI.NavMeshCollectGeometry.PhysicsColliders;Physics.SyncTransforms();Surface.BuildNavMesh();
   var sun=new GameObject("Key light").AddComponent<Light>();sun.type=LightType.Directional;sun.intensity=2;sun.color=new Color(.67f,.79f,1);sun.shadows=LightShadows.Soft;sun.transform.rotation=Quaternion.Euler(48,-32,0);
   RenderSettings.ambientMode=UnityEngine.Rendering.AmbientMode.Flat;RenderSettings.ambientLight=new Color(.36f,.43f,.54f);RenderSettings.fog=true;RenderSettings.fogColor=new Color(.025f,.04f,.075f);RenderSettings.fogDensity=.009f;
  }
  public static void ExcludeObstacle(BoxCollider box){var volume=box.GetComponent<NavMeshModifierVolume>();if(!volume)volume=box.gameObject.AddComponent<NavMeshModifierVolume>();var scale=box.transform.lossyScale;volume.area=1;volume.center=box.center-Vector3.up/scale.y;volume.size=box.size+new Vector3(1/scale.x,2/scale.y,1/scale.z);}
 }
}
