"""Reproducible NEON SIEGE art. Run with Blender --background --python this_file.
Original geometry and animation, no downloaded third-party assets.
"""
import bpy, math, os, json
from mathutils import Vector
ROOT=os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
OUT=os.path.join(ROOT,'Assets','_Project','Art','Models')
TEXTURES=os.path.join(ROOT,'Assets','_Project','Art','Textures')
os.makedirs(OUT,exist_ok=True);os.makedirs(TEXTURES,exist_ok=True)
COLORS=[(.055,.075,.105,1),(.18,.24,.3,1),(.48,.6,.64,1),(.8,.87,.88,1),(.02,.85,1,1),(1,.22,.045,1),(.8,.025,.25,1),(.24,.3,.37,1)]
image=bpy.data.images.new('SiegeAtlas',width=128,height=16)
image.pixels=[c for y in range(16) for x in range(128) for c in COLORS[x//16]]
image.filepath_raw=os.path.join(TEXTURES,'SiegeAtlas.png');image.file_format='PNG';image.save()
atlas=bpy.data.materials.new('SiegeAtlas');atlas.use_nodes=True
tex=atlas.node_tree.nodes.new('ShaderNodeTexImage');tex.image=image;tex.interpolation='Closest'
bs=atlas.node_tree.nodes.get('Principled BSDF');atlas.node_tree.links.new(tex.outputs['Color'],bs.inputs['Base Color']);bs.inputs['Metallic'].default_value=.65;bs.inputs['Roughness'].default_value=.35
glow=bpy.data.materials.new('SiegeGlow');glow.diffuse_color=(0,1,1,1)
parts=[]
def reset():
 bpy.ops.object.select_all(action='SELECT');bpy.ops.object.delete(use_global=False);parts.clear()
def box(name,loc,scale,color=1,bone=None,emission=False,bevel=.025):
 bpy.ops.mesh.primitive_cube_add(size=1,location=loc);o=bpy.context.object;o.name=name;o.scale=scale
 bpy.ops.object.transform_apply(location=False,rotation=False,scale=True)
 if bevel:
  m=o.modifiers.new('Machined edges','BEVEL');m.width=bevel;m.segments=1;bpy.ops.object.modifier_apply(modifier=m.name)
 o.data.materials.append(glow if emission else atlas)
 uv=o.data.uv_layers.active or o.data.uv_layers.new()
 for loop in uv.data:loop.uv=((color+.5)/8,.5)
 if bone:o.vertex_groups.new(name=bone).add(list(range(len(o.data.vertices))),1,'REPLACE')
 parts.append(o);return o
def export(name,rig=None):
 bpy.ops.object.select_all(action='DESELECT')
 for o in parts:o.select_set(True)
 bpy.context.view_layer.objects.active=parts[0];bpy.ops.object.join();mesh=bpy.context.object;mesh.name=name+'_Mesh';bpy.context.scene.cursor.location=(0,0,0);bpy.ops.object.origin_set(type='ORIGIN_CURSOR')
 if rig:
  modifier=mesh.modifiers.new('Armature','ARMATURE');modifier.object=rig;mesh.parent=rig;rig.select_set(True)
 bpy.ops.export_scene.fbx(filepath=os.path.join(OUT,name+'.fbx'),use_selection=True,axis_forward='-Z',axis_up='Y',add_leaf_bones=False,bake_anim=bool(rig),bake_anim_use_all_actions=False,bake_anim_use_nla_strips=False,bake_anim_simplify_factor=0,path_mode='AUTO',object_types={'MESH','ARMATURE'})
 return sum(len(p.vertices)-2 for p in mesh.data.polygons)
def character(name,kind):
 reset();bpy.ops.object.armature_add();rig=bpy.context.object;rig.name='Rig';bpy.ops.object.mode_set(mode='EDIT');rig.data.edit_bones.remove(rig.data.edit_bones[0]);bones={}
 def bone(n,head,tail,parent=None):
  b=rig.data.edit_bones.new(n);b.head=head;b.tail=tail
  if parent:b.parent=bones[parent]
  bones[n]=b
 bone('Hips',(0,0,.82),(0,0,1.05));bone('Spine',(0,0,1.05),(0,0,1.28),'Hips');bone('Chest',(0,0,1.28),(0,0,1.52),'Spine');bone('Neck',(0,0,1.52),(0,0,1.66),'Chest');bone('Head',(0,0,1.66),(0,0,1.98),'Neck')
 for side,sgn in [('Left',1),('Right',-1)]:
  bone(side+'Shoulder',(sgn*.12,0,1.5),(sgn*.38,0,1.5),'Chest');bone(side+'UpperArm',(sgn*.38,0,1.5),(sgn*.72,0,1.5),side+'Shoulder');bone(side+'LowerArm',(sgn*.72,0,1.5),(sgn*1.03,0,1.5),side+'UpperArm');bone(side+'Hand',(sgn*1.03,0,1.5),(sgn*1.18,0,1.5),side+'LowerArm')
  bone(side+'UpperLeg',(sgn*.18,0,.88),(sgn*.18,0,.49),'Hips');bone(side+'LowerLeg',(sgn*.18,0,.49),(sgn*.18,0,.13),side+'UpperLeg');bone(side+'Foot',(sgn*.18,0,.13),(sgn*.18,-.22,.09),side+'LowerLeg')
 bpy.ops.object.mode_set(mode='OBJECT');tone=2 if kind==0 else 1;wide=1.3 if kind in (3,4) else 1
 box('Pelvis',(0,0,.94),(.5,.32,.25),0,'Hips');box('Torso',(0,0,1.27),(.65*wide,.4,.5),tone,'Chest',bevel=.07);box('Chest plate',(0,-.23,1.32),(.45*wide,.08,.27),3 if kind==0 else 7,'Chest');box('Reactor',(0,-.285,1.34),(.16,.045,.19),4,'Chest',True)
 box('Helmet',(0,0,1.81),(.42,.38,.4),0,'Head',bevel=.065);box('Visor',(0,-.206,1.85),(.35,.045,.09),4,'Head',True);box('Helmet crown',(0,.015,2.02),(.28,.3,.08),tone,'Head')
 box('Backpack',(0,.28,1.31),(.42,.22,.42),0,'Chest');box('Spine cell',(0,.405,1.35),(.12,.06,.27),4,'Chest',True)
 for side,sgn in [('Left',1),('Right',-1)]:
  box('Pauldron',(sgn*.43,0,1.55),(.28*wide,.42,.3),tone,side+'UpperArm');box('Upper arm',(sgn*.61,0,1.5),(.23,.2,.22),0,side+'UpperArm');box('Bracer',(sgn*.88,0,1.5),(.3,.27,.28),tone,side+'LowerArm');box('Hand',(sgn*1.1,-.01,1.5),(.18,.19,.2),0,side+'Hand')
  box('Thigh',(sgn*.18,0,.68),(.25,.3,.36),tone,side+'UpperLeg');box('Knee',(sgn*.18,-.17,.46),(.23,.09,.17),0,side+'LowerLeg');box('Shin',(sgn*.18,0,.3),(.21,.25,.29),tone,side+'LowerLeg');box('Boot',(sgn*.18,-.08,.1),(.26,.44,.19),0,side+'Foot');box('Shin trim',(sgn*.18,-.145,.28),(.07,.03,.19),4,side+'LowerLeg',True)
 if kind==2:box('Antenna',(.22,.02,2.12),(.035,.04,.37),4,'Head',True)
 if kind==3:
  for sgn in [-1,1]:box('Tank plating',(sgn*.43,-.12,1.25),(.24,.38,.55),7,'Chest')
 if kind==4:
  for sgn in [-1,1]:box('Warden horn',(sgn*.24,0,2.11),(.13,.22,.42),3,'Head');box('Siege cannon',(sgn*.55,-.12,1.72),(.32,.55,.28),0,'Chest')
 bpy.context.view_layer.objects.active=rig;bpy.ops.object.mode_set(mode='POSE')
 # One baked take; Unity imports named frame ranges below.
 for frame in range(1,121):
  for p in rig.pose.bones:p.rotation_mode='XYZ';p.rotation_euler=(0,0,0);p.location=(0,0,0)
  for side,sgn in [('Left',1),('Right',-1)]:
   p=rig.pose.bones[side+'UpperArm'];rest=p.bone.matrix_local.to_3x3();delta=Vector((sgn,0,0)).rotation_difference(Vector((sgn*.2,-.3,-.93)));p.rotation_euler=(rest.inverted() @ delta.to_matrix() @ rest).to_euler()
  if frame<=24:
   phase=(frame-1)/24*math.tau
   for side,sgn in [('Left',1),('Right',-1)]:
    rig.pose.bones[side+'UpperLeg'].rotation_euler[0]=math.sin(phase)*sgn*.65
    rig.pose.bones[side+'LowerLeg'].rotation_euler[0]=max(0,-math.sin(phase)*sgn)*.85
   rig.pose.bones['Hips'].location.z=abs(math.sin(phase))*.04
  elif frame<=60:rig.pose.bones['Chest'].rotation_euler[0]=math.sin((frame-25)/35*math.tau)*.025
  elif frame<=72:rig.pose.bones['Chest'].rotation_euler[0]=math.sin((frame-61)/11*math.pi)*.09
  elif frame<=96:rig.pose.bones['RightLowerArm'].rotation_euler[0]=math.sin((frame-73)/23*math.pi)*.7
  else:rig.pose.bones['Hips'].rotation_euler[0]=min(1,(frame-97)/15)*1.4
  for p in rig.pose.bones:p.keyframe_insert('rotation_euler',frame=frame);p.keyframe_insert('location',frame=frame)
 bpy.ops.object.mode_set(mode='OBJECT');bpy.context.scene.frame_start=1;bpy.context.scene.frame_end=120;bpy.context.scene.frame_set(25)
 return export(name,rig)
stats={}
for name,kind in [('Operator',0),('Grunt',1),('Gunner',2),('Tank',3),('Warden',4)]:stats[name]=character(name,kind)
for index,name in enumerate(['Vanguard','Breacher','Longshot','Stinger','Aftershock']):
 reset();length=[.8,.7,1.25,.5,.72][index];box('Receiver',(0,0,0),(.15,length*.6,.18),1);box('Upper rail',(0,-.02,.115),(.1,length*.6,.045),2);box('Stock',(0,length*.4,-.03),(.12,.26,.16),0);box('Grip',(0,.08,-.16),(.105,.13,.22),0);box('Magazine',(0,-.12,-.16),(.11,.17,.2),2);box('Barrel',(0,-length*.5,0),(.075,length*.45,.07),0);box('Energy rail',(.083,-.1,.02),(.015,length*.36,.04),4,emission=True)
 if index==2:box('Scope',(0,-.02,.23),(.12,.28,.11),0);box('Scope lens',(0,-.17,.23),(.1,.015,.08),4,emission=True)
 if index==4:box('Drum',(0,-.1,-.09),(.3,.28,.28),7)
 stats[name]=export(name)
for name in ['Cover','Pillar','Crate','FloorTile','Wall']:
 reset()
 if name=='Cover':box('Cover body',(0,0,1.05),(5,1.5,2.1),1,bevel=.12);box('Cap',(0,0,2.12),(5.1,1.6,.12),2);box('Light strip',(0,-.81,1.8),(4.6,.06,.08),4,emission=True)
 elif name=='Pillar':box('Core',(0,0,2.8),(2,2,5.6),0,bevel=.18);box('Base',(0,0,.25),(2.7,2.7,.5),1);box('Cap',(0,0,5.6),(2.7,2.7,.5),1);box('Light strip',(0,-1.04,2.8),(.17,.05,4.7),4,emission=True)
 elif name=='Crate':box('Cargo',(0,0,.6),(1.3,1.3,1.2),2,bevel=.08);box('Inset',(0,-.67,.6),(.8,.06,.5),0);box('Lock',(0,-.72,.6),(.14,.05,.2),4,emission=True)
 elif name=='FloorTile':box('Tile',(0,0,-.1),(6,6,.2),1,bevel=.03)
 else:box('Wall',(0,0,2),(8,1,4),0,bevel=.12);box('Stripe',(0,-.55,3.6),(7.8,.1,.12),4,emission=True)
 stats[name]=export(name)
with open(os.path.join(ROOT,'Docs','asset-stats.json'),'w') as f:json.dump(stats,f,indent=2)
# Keep a source .blend showcasing the last generated module; script rebuilds every asset.
bpy.ops.wm.save_as_mainfile(filepath=os.path.join(ROOT,'Tools','SiegeAssets.blend'))
print('BLENDER_ASSETS_PASS',json.dumps(stats))

