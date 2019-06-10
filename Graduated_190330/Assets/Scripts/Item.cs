using System.Collections;
using System.Collections.Generic;

public class Item
{
    public string Name { get; private set;}
    public string Icon { get; private set; }
    public List<int> AddStatList { get; private set; }

    public Item(string name, string icon, List<int> addStatList)
    {
        Name = name;
        Icon = icon;
        AddStatList = addStatList;
    }
}
