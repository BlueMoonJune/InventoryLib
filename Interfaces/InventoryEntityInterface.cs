using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace InventoryLib.Interfaces
{
	internal class InventoryEntityInterface : ContainerInterface
    {
        public static Point GetTopLeftTileInMultitile(int x, int y, out int width, out int height)
        {
            Tile tile = Main.tile[x, y];

            int frameX = 0;
            int frameY = 0;
            width = 1;
            height = 1;

            if (tile.HasTile)
            {
                int style = 0, alt = 0;
                TileObjectData.GetTileInfo(tile, ref style, ref alt);
                TileObjectData data = TileObjectData.GetTileData(tile.TileType, style, alt);

                if (data != null)
                {
                    int size = 16 + data.CoordinatePadding;

                    frameX = tile.TileFrameX % (size * data.Width) / size;
                    frameY = tile.TileFrameY % (size * data.Height) / size;
                    width = data.Width;
                    height = data.Height;
                }
            }

            return new Point(x - frameX, y - frameY);
        }

        public InventoryEntityInterface(int i, int j)
        {
            x = i;
            y = j;
        }

        public static Point16 GetTopLeft(int i, int j)
        {
            if (!Main.tile[i, j].HasTile) return negOne;
            int type = Main.tile[i, j].TileType;
            int width, height;
            Point p = GetTopLeftTileInMultitile(i, j, out width, out height);
            if (TileEntity.ByPosition.ContainsKey(new Point16(p)))
                return new(p);
            return negOne;
        }

        public override List<Item> GetItems()
        {
            if (!TileEntity.ByPosition.TryGetValue(GetTopLeft(x, y), out TileEntity TE)) return new();
            IInventoryTileEntity tileEntity = TE as IInventoryTileEntity;
            if (tileEntity == null) return new List<Item>();
            return tileEntity.GetExtractableItemsForInterface(this).ToList();
        }

        public override bool InsertItem(Item item)
        {
            if (!TileEntity.ByPosition.TryGetValue(GetTopLeft(x, y), out TileEntity TE)) return new();
            IInventoryTileEntity tileEntity = TE as IInventoryTileEntity;
            if (tileEntity == null) return false;
            return tileEntity.InsertItem(item) || tileEntity.InsertItem(item, this);
        }

        public override bool ExtractItem(Item item)
        {
            if (!TileEntity.ByPosition.TryGetValue(GetTopLeft(x, y), out TileEntity TE)) return new();
            IInventoryTileEntity tileEntity = TE as IInventoryTileEntity;
            if (tileEntity == null) return false;
            return tileEntity.ExtractItem(item) || tileEntity.ExtractItem(item, this);
        }
    }
}
