from PIL import Image,ImageDraw
import random,math
from pathlib import Path
random.seed(64);im=Image.new('RGBA',(512,1024),(255,255,255,0));d=ImageDraw.Draw(im)
for i in range(72):
 x=random.uniform(12,500);end=random.uniform(15,340);lean=random.uniform(-25,25);phase=random.random()*6.28
 points=[(x+lean*(1-y/1024)+math.sin(y/150+phase)*5,y) for y in range(1023,int(end),-5)]
 d.line(points,fill=(255,255,255,random.randint(145,255)),width=random.choice([1,2,2,3]))
p=Path(__file__).resolve().parents[1]/'Unity/Assets/Jimothy/Resources/FurStrands.png';im.save(p)
