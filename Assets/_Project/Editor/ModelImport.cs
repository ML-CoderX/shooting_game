using UnityEditor;
namespace NeonSiege.Editor {
 public sealed class ModelImport : AssetPostprocessor {
  public override uint GetVersion()=>3; void OnPreprocessModel(){if(!assetPath.StartsWith("Assets/_Project/Art/Models/"))return;var model=(ModelImporter)assetImporter;model.materialImportMode=ModelImporterMaterialImportMode.ImportStandard;model.globalScale=1;model.useFileScale=true;model.importCameras=false;model.importLights=false;model.isReadable=false;
   bool character=assetPath.EndsWith("Operator.fbx")||assetPath.EndsWith("Grunt.fbx")||assetPath.EndsWith("Gunner.fbx")||assetPath.EndsWith("Tank.fbx")||assetPath.EndsWith("Warden.fbx");model.importAnimation=character;model.generateSecondaryUV=!character;
   if(character){model.animationType=assetPath.EndsWith("Operator.fbx")?ModelImporterAnimationType.Human:ModelImporterAnimationType.Generic;model.avatarSetup=ModelImporterAvatarSetup.CreateFromThisModel;model.clipAnimations=new[]{Clip("Run",1,24,true),Clip("Idle",25,60,true),Clip("Shoot",61,72,false),Clip("Reload",73,96,false),Clip("Death",97,120,false)};}}
  static ModelImporterClipAnimation Clip(string name,int start,int end,bool loop)=>new ModelImporterClipAnimation{name=name,firstFrame=start,lastFrame=end,loopTime=loop,lockRootRotation=true,lockRootHeightY=true,lockRootPositionXZ=true};
 }
}


