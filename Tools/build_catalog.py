import json
from pathlib import Path
root=Path(__file__).resolve().parents[1]
families={
'food': {'Bagel':['Salmon','Sesame','Poppy Seed','Everything','Onion','Rye','Blueberry','Cinnamon'], 'Pastry':['Cardamom Knot','Bear Claw','Cinnamon Roll','Kringle','Apple Danish','Lemon Scone','Blackberry Scone','Almond Croissant'], 'Market':['Apple','Pear','Blackberries','Raspberries','Strawberries','Carrot','Peach','Plum'], 'Leftovers':['Fish Taco','Salmon Skin','Fries','Pizza Crust','Rice Ball','Cheese Cube','Sandwich Corner','Clam Chowder']},
'valuable': {'Pocket':['Quarter','Dime','Nickel','Arcade Token','Bus Token','Laundry Token','Dollar Coin','Foreign Coin'], 'Metal':['Brass Key','Copper Washer','Silver Button','Bronze Nut','Enamel Pin','Bottle Opener','Tiny Bell','Watch Back'], 'Glass':['Amber Marble','Blue Marble','Green Marble','Sea Glass','Red Bead','Crystal Bead','Prism Shard','Glass Stopper'], 'Paper':['Gift Voucher','Market Coupon','Record Voucher','Coffee Card','Book Coupon','Arcade Ticket','Ferry Token','Trade Stamp']},
'curio': {'Harbor':['Cork Float','Net Float','Net Needle','Rope Coil','Lure','Bobber','Shell','Boat Tag'], 'Street':['Bottle Cap','Lost Mitten','Shoelace','Tiny Umbrella','Bike Reflector','Sunglass Lens','Rubber Duck','Toy Wheel'], 'Garden':['Acorn','Pine Cone','Fern Print','Flowerpot','Seed Packet','Garden Gnome','Wind Chime','Smooth Stone'], 'Music':['Guitar Pick','Record Sleeve','Drum Key','Ticket Stub','Harmonica','Kazoo','Cassette','Tambourine Bell'], 'Nordic':['Wooden Horse','Knit Patch','Rune Bead','Mini Oar','Wool Tassel','Carved Fish','Painted Tile','Small Pennant']}}
items=[]
# 104 base objects with two authored condition/style variants = 208 types.
for cat,groups in families.items():
 for family,names in groups.items():
  for name in names:
   for variant in (['Fresh','Day-old'] if cat=='food' else ['Classic','Unusual']):
    title=f'{variant} {name}';items.append(dict(id='item_%03d'%len(items),name=title,category=cat,rarity='uncommon' if variant=='Unusual' else 'common',value=4 if cat=='valuable' else 2,nutrition=22 if variant=='Fresh' else 12 if cat=='food' else 0))
# 32 more distinct collectibles.
for name in ['Mini Crab Pot','Ferry Schedule','Dock Cleat','Ship Compass','Signal Flag','Rope Fender','Lobster Patch','Salmon Scale Charm','Coffee Spoon','Espresso Cup','Tea Strainer','Picnic Fork','Tiny Thermos','Biscuit Tin','Jam Jar','Honey Dipper','Postcard of Rainier','Ballard Map','Library Bookmark','Old Street Number','Bicycle Bell','Brass Door Knocker','Ceramic Raccoon','Wooden Seaplane','Mini Sailboat','Tin Lighthouse','Pocket Telescope','Cedar Box','Quilt Square','Garden Lantern','Den Welcome Mat','Patchwork Cushion']:
 items.append(dict(id='item_%03d'%len(items),name=name,category='curio',rarity='rare',value=12,nutrition=0))
for name in ['The First Salmon','Marvin’s Lost Clapper','Rainier at Dawn','The Golden Bagel','Captain’s Last Compass','The Tiny Troll','The Ballard Crown','Ghost of the Old Trolley','The Nordic Star','Midnight Market Medal','The Uncatchable Floatplane','King of the Alley','The Glass Kraken','The Lockkeeper’s Key','The Perfect Pinecone','The Mayor of Trash']:
 items.append(dict(id='trophy_%02d'%sum(x['category']=='trophy' for x in items),name=name,category='trophy',rarity='legendary',value=0,nutrition=0))
assert len(items)==256 and len({i['id'] for i in items})==256
(root/'Unity/Assets/Jimothy/Resources/Items.json').write_text(json.dumps({'items':items},indent=2,ensure_ascii=False))
print('256 collectible definitions generated; 16 named trophies.')
