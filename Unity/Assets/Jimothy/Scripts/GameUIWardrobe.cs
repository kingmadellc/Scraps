using UnityEngine;
using UnityEngine.UI;
namespace Jimothy {
public partial class GameUI {
 public void ShowWardrobe(int index=0){
  if(!game.AtHome)return;game.OpenDen();index=Mathf.Clamp(index,0,WardrobeRules.Looks.Length-1);int current=index;var look=WardrobeRules.Looks[index];bool unlocked=WardrobeRules.Unlocked(game.Data,look.id),equipped=game.Data.outfit==look.id;
  DenPageTitle("WARDROBE","");
  var portrait=Rect("Outfit preview",page.transform,new(.065f,.23f),new(.455f,.79f));var imageRect=Rect("Portrait image",portrait,Vector2.zero,Vector2.one);var raw=imageRect.gameObject.AddComponent<RawImage>();raw.raycastTarget=false;var fit=imageRect.gameObject.AddComponent<AspectRatioFitter>();fit.aspectMode=AspectRatioFitter.AspectMode.FitInParent;fit.aspectRatio=1;
  var preview=portrait.gameObject.AddComponent<WardrobePreview>();preview.Initialize(raw,look.id,game.Data.coat);
  Heading(page.transform,look.name,38,cream,new(.50f,.65f),new(.93f,.79f));Placed(page.transform,look.story,24,mint,new(.50f,.49f),new(.92f,.64f));
  Placed(page.transform,unlocked?(equipped?"Wearing this":"Unlocked"):look.requirement,24,unlocked?mint:coral,new(.50f,.39f),new(.92f,.48f));
  var wear=Button(page.transform,unlocked?(equipped?"Equipped":"Wear it"):"Not banked yet",new(.50f,.23f),new(.92f,.36f),()=>{if(game.Wear(look.id))ShowWardrobe(current);},unlocked);wear.interactable=unlocked&&!equipped;
  Button(page.transform,"Turn",new(.07f,.18f),new(.20f,.27f),preview.Turn);Button(page.transform,"Coat: "+WardrobeRules.Coats[game.Data.coat],new(.23f,.18f),new(.45f,.27f),()=>{game.ChangeCoat((game.Data.coat+1)%3);ShowWardrobe(current);});
  Button(page.transform,"Back",new(.07f,.045f),new(.30f,.14f),ShowDen);Button(page.transform,"Previous",new(.38f,.045f),new(.61f,.14f),()=>ShowWardrobe((current+WardrobeRules.Looks.Length-1)%WardrobeRules.Looks.Length));Button(page.transform,"Next",new(.69f,.045f),new(.92f,.14f),()=>ShowWardrobe((current+1)%WardrobeRules.Looks.Length));
 }
}
}
