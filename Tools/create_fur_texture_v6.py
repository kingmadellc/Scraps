from PIL import Image,ImageDraw
from pathlib import Path
import random
random.seed(61)
im=Image.new('RGBA',(256,512));d=ImageDraw.Draw(im)
for i in range(18):
 x=8+i*14;end=random.randrange(0,170);tip=x+random.randrange(-12,13);w=random.randrange(5,11);shade=random.randrange(130,240)
 d.polygon([(x-w,511),(x+w,511),(tip+w*.3,end+55),(tip,end),(tip-w*.3,end+55)],fill=(shade,shade,shade,255))
# Fade buried roots and soften individual strands so cards do not read as rectangles.
a=im.load()
for y in range(512):
 fade=min(1,(511-y)/95)
 for x in range(256):
  rr,gg,bb,aa=a[x,y];a[x,y]=(rr,gg,bb,int(aa*fade))
im.save(Path(__file__).resolve().parents[1]/'Unity/Assets/Jimothy/Resources/FurStrands.png')
