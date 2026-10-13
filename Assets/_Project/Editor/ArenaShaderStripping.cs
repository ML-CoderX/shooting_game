using System.Collections.Generic;
using UnityEditor.Build;
using UnityEditor.Rendering;
using UnityEngine;
namespace NeonSiege.Editor {
 // This arena uses mesh modules only. Restore these variants before adding terrain or vegetation.
 public sealed class ArenaShaderStripping : IPreprocessShaders {
  public int callbackOrder=>0;
  public void OnProcessShader(Shader shader,ShaderSnippetData snippet,IList<ShaderCompilerData> variants){
   if(shader.name.Contains("TerrainEngine")||shader.name.Contains("/Terrain/")||shader.name.Contains("SpeedTree"))variants.Clear();
  }
 }
}
