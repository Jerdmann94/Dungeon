using System;
using System.Collections.Generic;

[Serializable]
public class JsonListWrapper<T>
{
    public List<T> list;

    public JsonListWrapper(List<T> list)
    {
        this.list = list;
    }
}