using System.Linq;
using UnityEngine;
namespace Jimothy {
public partial class GameUI {
 void DenPageTitle(string title,string detail){Clear(new Color(.025f,.065f,.055f,.97f));Heading(page.transform,title,50,cream,new(.08f,.80f),new(.92f,.94f)).font=TitleDisplay;Placed(page.transform,detail,22,mint,new(.08f,.72f),new(.92f,.80f));}
 void ShowDenShop(int index){
  var entries=DenFurnishings.Catalog.Values.ToArray();int pages=(entries.Length+5)/6;index=Mathf.Clamp(index,0,pages-1);int current=index;
  DenPageTitle("MAKE IT YOUR DEN",$"{game.Data.coins} shinies · Buy once, show or hide anytime · {index+1}/{pages}");
  int unique=game.Data.bankedDiscoveries.Count;
  for(int i=0;i<6&&index*6+i<entries.Length;i++){var e=entries[index*6+i];bool owned=game.Data.decor.Contains(e.id),hidden=game.Data.hiddenDecor.Contains(e.id);float x=i%2==0?.08f:.53f,y=.55f-i/2*.18f;string label=owned?e.name+" · "+(hidden?"Show":"Hide"):unique<e.finds?e.name+"\nBank "+e.finds+" unique finds":e.name+" · "+e.cost+" shinies";
   var button=Button(page.transform,label,new(x,y),new(x+.39f,y+.14f),()=>{if(owned)game.ToggleDecoration(e.id);else game.BuyDecor(e.id,e.cost);ShowDenShop(current);},owned&&!hidden);button.interactable=owned||(unique>=e.finds&&game.Data.coins>=e.cost);
  }
  Button(page.transform,"Back",new(.08f,.055f),new(.31f,.16f),ShowDen);Button(page.transform,"Previous",new(.38f,.055f),new(.61f,.16f),()=>ShowDenShop(current-1)).interactable=index>0;Button(page.transform,"Next",new(.68f,.055f),new(.91f,.16f),()=>ShowDenShop(current+1)).interactable=index<pages-1;
 }
 void ShowDenFavorites(int index){
  var ids=game.Data.trophies.Concat(game.Data.pantry).Distinct().Where(game.Items.ContainsKey).OrderBy(id=>game.Items[id].name).ToArray();int pages=Mathf.Max(1,(ids.Length+5)/6);index=Mathf.Clamp(index,0,pages-1);int current=index;
  DenPageTitle("YOUR PRIZED FINDS",$"Pin up to 8 favorites to the front of your display · {game.Data.favoriteFinds.Count}/8 · {index+1}/{pages}");
  if(ids.Length==0)Placed(page.transform,"Unload finds in the Den to start your collection.",28,cream,new(.1f,.4f),new(.90f,.60f));
  for(int i=0;i<6&&index*6+i<ids.Length;i++){string id=ids[index*6+i];bool pinned=game.Data.favoriteFinds.Contains(id);float x=i%2==0?.08f:.53f,y=.55f-i/2*.18f;Button(page.transform,(pinned?"★ ":"")+game.Items[id].name,new(x,y),new(x+.39f,y+.14f),()=>{game.ToggleFavorite(id);ShowDenFavorites(current);},pinned);}
  Button(page.transform,"Back",new(.08f,.055f),new(.31f,.16f),ShowDen);Button(page.transform,"Previous",new(.38f,.055f),new(.61f,.16f),()=>ShowDenFavorites(current-1)).interactable=index>0;Button(page.transform,"Next",new(.68f,.055f),new(.91f,.16f),()=>ShowDenFavorites(current+1)).interactable=index<pages-1;
 }
}}
