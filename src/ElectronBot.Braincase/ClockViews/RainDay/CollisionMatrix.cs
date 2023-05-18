using System;
using System.Collections.Generic;

namespace ElectronBot.Braincase.ClockViews;

public class CollisionMatrix
{
    #region Fileds
    private int xc;
    private int yc;
    private List<List<DropItem>> matrix;
    private float resolution;
    #endregion

    #region Contructor
    public CollisionMatrix(int x, int y, float r)
    {
        resolution = r;
        xc = x;
        yc = y;
        matrix = new List<List<DropItem>>();
        for (int i = 0; i <= x + 5; i++)
        {
            matrix.Add(new List<DropItem>());
            for (int j = 0; j <= y + 5; j++)
            {
                matrix[i].Add(new DropItem(null));
            }
        }
    }
    #endregion

    #region Private Methods

    private DropItem AddAll(DropItem to, int x, int y)
    {
        if (x > 0 && y > 0 && x < xc && y < yc)
        {
            var items = matrix[x][y];
            while (items.Next != null)
            {
                items = items.Next;
                to.Next = new DropItem(items.Dropdata);
                to = to.Next;
            }
        }
        return to;
    }
    #endregion

    #region Public Methods
    public DropItem Update(Drop drop, bool forceDelete)
    {
        if (drop.Gid != null)
        {
            if (matrix[drop.Gmx] == null || matrix[drop.Gmx][drop.Gmy] == null)
            {
                return null;
            }
            matrix[drop.Gmx][drop.Gmy].Remove(drop);
            if (forceDelete)
            {
                return null;
            }

            drop.Gmx = (int)Math.Floor(drop.X / resolution);
            drop.Gmy = (int)Math.Floor(drop.Y / resolution);
            if (matrix[drop.Gmx] == null || matrix[drop.Gmx][drop.Gmy] == null)
            {
                return null;
            }
            matrix[drop.Gmx][drop.Gmy].Add(drop);

            var collisions = Collisions(drop);
            if (collisions != null && collisions.Next != null)
            {
                return collisions.Next;
            }
        }
        else
        {
            drop.Gid = Guid.NewGuid().ToString().ToString().Substring(2, 9);
            drop.Gmx = (int)Math.Floor(drop.X / resolution);
            drop.Gmy = (int)Math.Floor(drop.Y / resolution);
            if (matrix[drop.Gmx] == null || matrix[drop.Gmx][drop.Gmy] == null)
            {
                return null;
            }

            matrix[drop.Gmx][drop.Gmy].Add(drop);
        }
        return null;
    }

    public DropItem Collisions(Drop drop)
    {
        var item = new DropItem(null);
        var first = item;

        item = AddAll(item, drop.Gmx - 1, drop.Gmy + 1);
        item = AddAll(item, drop.Gmx, drop.Gmy + 1);
        item = AddAll(item, drop.Gmx + 1, drop.Gmy + 1);

        return first;
    }

    public void Remove(Drop drop)
    {
        matrix[drop.Gmx][drop.Gmy].Remove(drop);
    }

    #endregion



}
