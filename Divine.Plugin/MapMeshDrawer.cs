using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Divine.SDK.Extensions;

using SharpDX;

namespace Divine.Plugin
{
    public static class MapMeshDrawer
    {
        public static void Draw()
        {
            var position = EntityManager.LocalHero.Position;

            const int CellCount = 130;
            for (var i = -130; i < CellCount; ++i)
            {
                for (var j = -130; j < CellCount; ++j)
                {
                    Color c;
                    var cell = MapManager.GetMeshCell(i, j);
                    var flag = cell.Flags;
                    if (flag.HasFlag(MapMeshCellFlags.Walkable))
                    {
                        c = flag.HasFlag(MapMeshCellFlags.Tree) ? Color.Purple : Color.Green;
                        if (flag.HasFlag(MapMeshCellFlags.GridFlagObstacle))
                        {
                            c = Color.Pink;
                        }
                    }
                    else if (flag.HasFlag(MapMeshCellFlags.MovementBlocker))
                    {
                        c = Color.Green;
                    }
                    else
                    {
                        c = Color.Red;
                    }

                    /*if (flag.HasFlag(CellFlags.Visible))
                    {
                        c = Color.White;
                        DrawRectange(cell, c, 10);
                        continue;
                    }
                    else
                    {
                        continue;
                    }*/
                    
                    DrawRectange(cell, c);
                }
            }

            var mousePositionCell = MapManager.GetMeshCell(GameManager.MousePosition);
            DrawRectange(mousePositionCell, Color.Aqua, 10);

            DrawRectange(MapManager.GetMeshCell(position), Color.Yellow, 5);
        }

        private static void DrawRectange(MapMeshCell cell, Color color, int size = 1)
        {
            var meshCellSize = MapManager.MeshCellSize;
            var position = cell.Position;
            if (position.Z < -5000)
            {
                return;
            }

            for (var i = 0; i < size; i++)
            {
                var a = position + new Vector3(4 + i, 4 + i, 0);
                var b = a + new Vector3(0, meshCellSize - 8 - (i * 2), 0);
                var c = a + new Vector3(meshCellSize - 8 - (i * 2), 0, 0);
                var d = c + new Vector3(0, meshCellSize - 8 - (i * 2), 0);

                if (!a.IsOnScreen() || !b.IsOnScreen() || !c.IsOnScreen() || !d.IsOnScreen())
                {
                    return;
                }

                var a2 = RendererManager.WorldToScreen(a);
                var b2 = RendererManager.WorldToScreen(b);
                var c2 = RendererManager.WorldToScreen(c);
                var d2 = RendererManager.WorldToScreen(d);

                RendererManager.DrawLine(a2, b2, color);
                RendererManager.DrawLine(b2, d2, color);
                RendererManager.DrawLine(d2, c2, color);
                RendererManager.DrawLine(c2, a2, color);
            }
        }
    }
}