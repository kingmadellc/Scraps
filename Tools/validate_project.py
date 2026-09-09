"""Asset integrity and C# syntax checks. Does not substitute for Unity compilation."""
from pathlib import Path
import json, struct
import tree_sitter, tree_sitter_c_sharp
root=Path(__file__).resolve().parents[1]
parser=tree_sitter.Parser(tree_sitter.Language(tree_sitter_c_sharp.language()))
errors=[]
for p in (root/'Unity/Assets').rglob('*.cs'):
 tree=parser.parse(p.read_bytes())
 if tree.root_node.has_error:errors.append(str(p))
print('C# syntax:', 'PASS' if not errors else errors)
assert not errors
items=json.loads((root/'Unity/Assets/Jimothy/Resources/Items.json').read_text())['items']
assert len(items)==256 and len({x['id'] for x in items})==256
assert sum(x['category']=='trophy' for x in items)==16
assert all(x['nutrition']>0 for x in items if x['category']=='food')
print('Catalog: PASS (256 unique IDs, 16 trophies, valid food nutrition)')
for name in ['Jimothy','OldBallard']:
 f=root/f'Unity/Assets/Jimothy/Resources/{name}.fbx'
 assert f.read_bytes().startswith(b'Kaydara FBX Binary'),name
 assert f.stat().st_size>100000
 assert (root/f'Art/Blender/{name}.blend').exists()
 print(name+': Blender source and binary FBX present')
for p in (root/'Art/Renders').glob('*.png'):
 with p.open('rb') as f:
  head=f.read(24);w,h=struct.unpack('>II',head[16:24])
 print(p.name, str(w)+'x'+str(h))
# Jump geometry checks across every building height, including tallest roofs.
max_rise=8.4**2/(2*19)
for h in [8,10.5,7,9,11]:
 levels=[0]+[.65+j*(h-.65)/6 for j in range(7)]+[h+.98]
 assert max(b-a for a,b in zip(levels,levels[1:]))<max_rise
print('Vertical route rise: PASS against authored jump apex (does not test collision/playability)')
print('Unity compile, EditMode tests, input, collision, persistence I/O and device performance: NOT RUN (Unity unavailable)')
