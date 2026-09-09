import bpy,json
from pathlib import Path
root=Path(__file__).resolve().parents[1];bpy.ops.wm.read_factory_settings(use_empty=True);bpy.ops.import_scene.fbx(filepath=str(root/'Unity/Assets/Jimothy/Resources/Jimothy.fbx'))
for o in bpy.data.objects:
 if o.type=='MESH':
  print(o.name,len(o.data.vertices),[(a.name,a.domain) for a in o.data.color_attributes]);
  for a in o.data.color_attributes:print(a.name,[list(a.data[i].color) for i in range(min(3,len(a.data)))])
