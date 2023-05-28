using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronBot.Braincase.ClockViews;

public class DropItem
{
    #region Members
    private Drop dropdata;
    private DropItem next;

    #endregion

    #region Properties
    public Drop Dropdata
    {
        get { return dropdata; }
        set { dropdata = value; }
    }
    public DropItem Next
    {
        get { return next; }
        set { next = value; }
    }

    #endregion

    #region Constructors
    public DropItem(Drop drop)
    {
        dropdata = drop;
        next = null;
    }

    #endregion

    #region public Methods
    public void Add(Drop drop)
    {
        var item = this;
        while (item.next != null)
        {
            item = item.next;
        }
        item.next = new DropItem(drop);
    }
    public void Remove(Drop drop)
    {
        DropItem item = this;
        DropItem prevItem = null;
        while (item.next != null)
        {
            prevItem = item;
            item = item.next;
            if (item.dropdata.Gid == drop.Gid)
            {
                prevItem.next = item.next;
            }
        }
    }
    #endregion

}



