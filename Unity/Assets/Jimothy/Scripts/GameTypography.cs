using UnityEngine;
namespace Jimothy {
/// <summary>Shared identity fonts for menus and discovery headings; HUD stays independently legible.</summary>
public static class GameTypography {
 static Font display,secondary,fallback;
 static Font Fallback=>fallback?fallback:(fallback=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"));
 public static Font Display=>display?display:(display=Resources.Load<Font>("TitleFonts/FugazOne-Regular")??Fallback);
 public static Font Secondary=>secondary?secondary:(secondary=Resources.Load<Font>("TitleFonts/BarlowCondensed-SemiBold")??Fallback);
 public static readonly Color Forest=new(.025f,.075f,.060f),Cream=new(.97f,.92f,.79f),Brass=new(.88f,.65f,.34f);
}
}
