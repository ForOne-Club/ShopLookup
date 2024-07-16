global using static ReitsKit.Helper.MiscHelper;
global using ReitsKit.UI.Basic;
global using ReitsKit.UI.Origin;
global using ReitsKit.UI.Sys;
global using static ReitsKit.Helper.DrawExtend;
global using ReitsKit.Core;
global using Microsoft.Xna.Framework;
global using Microsoft.Xna.Framework.Graphics;
global using ReLogic.Content;
global using Terraria.ModLoader;
global using Terraria;
global using System.Collections.Generic;

namespace ReitsKit.Helper;

public static class MiscHelper
{
    public static Vector2 Resolution => UISystem.Resolution;

    public static readonly RasterizerState CullNoneAndScissor =
        new() { CullMode = CullMode.None, ScissorTestEnable = true };

    public static Texture2D T2D(string path) =>
        ModContent.Request<Texture2D>(path, AssetRequestMode.ImmediateLoad).Value;

    public static bool Between(float l, float v, float r) => l <= v && v <= r;

    public static Rectangle[] Rect3X3(int w, int h, int offset = 2) =>
    [
        new(0, 0, w, h),
        new(w + offset, 0, w, h),
        new((w + offset) * 2, 0, w, h),
        new(0, h + offset, w, h),
        new(w + offset, h + offset, w, h),
        new((w + offset) * 2, h + offset, w, h),
        new(0, (h + offset) * 2, w, h),
        new(w, (h + offset) * 2, w, h),
        new((w + offset) * 2, (h + offset) * 2, w, h)
    ];

}