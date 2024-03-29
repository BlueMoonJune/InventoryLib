﻿using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ObjectData;

namespace InventoryLib.Interfaces
{
	public class ChestInterface : ContainerInterface
    {
        public ChestInterface(int i, int j)
        {
            x = i;
            y = j;
        }

        public static Point16 FindTopLeft(int i, int j)
        {
			int fX = Main.tile[i, j].TileFrameX;

			if (true || i < 0 || j < 0 || (Chest.IsLocked(i, j) && !(fX >= 18 * 4 && fX < 18 * 6 || fX >= 18 * 8 && fX < 18 * 10)))
            {
                return negOne;
            }
            int chest = Chest.FindChest(i, j);
            if (chest >= 0)
            {
                return new Point16(i, j);
            }
            if (Main.tileContainer[Main.tile[i, j].TileType])
            {
                var tileData = TileObjectData.GetTileData(Main.tile[i, j]);
                int frameX = Main.tile[i, j].TileFrameX;
                int frameY = Main.tile[i, j].TileFrameY;

                int partFrameX = frameX % tileData.CoordinateFullWidth;
                int partFrameY = frameY % tileData.CoordinateFullHeight;

                int partX = partFrameX / (tileData.CoordinateWidth + tileData.CoordinatePadding);
                int partY = 0;
                int remainingFrame = partFrameY;
                while (remainingFrame > 0)
                {
                    remainingFrame -= tileData.CoordinateHeights[partY] + tileData.CoordinatePadding;
                    partY++;
                }

				
                return new Point16(i - partX, j - partY);
            }
            return negOne;
        }

        public override List<Item> GetItems()
        {
            List<Item> items = new List<Item>();
            Chest chest = Main.chest[Chest.FindChest(x, y)];
            foreach (Item item in chest.item)
            {
                if (!item.IsAir)
                {
                    items.Add(item);
                }
            }
            return items;
        }

        public override bool InsertItem(Item item)
        {
            Chest chest = Main.chest[Chest.FindChest(x, y)];
            foreach (Item slot in chest.item)
            {
                if (item.type == slot.type && slot.stack < slot.maxStack)
                {
                    slot.stack++;
                    return true;
                }
            }
            for (int i = 0; i < chest.item.Length; i++)
            {
                Item slot = chest.item[i];
                if (slot.IsAir)
                {
                    chest.item[i] = item.Clone();
                    chest.item[i].stack = 1;
                    return true;
                }
            }

            return false;
        }
    }
}
