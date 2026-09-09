from PIL import Image,ImageDraw,ImageFilter
from pathlib import Path
import random,math
random.seed(26);size=512
h=Image.new('L',(size,size),128);d=ImageDraw.Draw(h)
for i in range(2400):
 x=random.randrange(size);y=random.randrange(size);ln=random.randrange(8,48)
 pts=[(x+2*math.sin(j*.15+i),y+j) for j in range(ln)]
 d.line(pts,fill=random.randrange(70,190),width=random.randrange(1,3))
h=h.filter(ImageFilter.GaussianBlur(.65));a=h.load();im=Image.new('RGB',(size,size));p=im.load()
for y in range(size):
 for x in range(size):
  dx=(a[(x+1)%size,y]-a[(x-1)%size,y])/100;dy=(a[x,(y+1)%size]-a[x,(y-1)%size])/100;l=math.sqrt(dx*dx+dy*dy+1)
  p[x,y]=(int(127.5-127.5*dx/l),int(127.5-127.5*dy/l),int(127.5+127.5/l))
im.save(Path(__file__).resolve().parents[1]/'Unity/Assets/Jimothy/Resources/Surfaces/Jimothy_Undercoat_Normal.png')
