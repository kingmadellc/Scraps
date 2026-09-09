import bpy
from pathlib import Path
root=Path(__file__).resolve().parents[1]
p=root/'Art/Blender/Jimothy.blend'
bpy.ops.wm.open_mainfile(filepath=str(p))
rig=bpy.data.objects['Jimothy_Rig']
for im in bpy.data.images:
 if im.name.startswith('Jimothy_Albedo') and not im.packed_file:im.pack()
bpy.context.scene.frame_set(1)
bpy.ops.wm.save_as_mainfile(filepath=str(p),compress=True)
print('Packed character texture ; compressed source saved.')
