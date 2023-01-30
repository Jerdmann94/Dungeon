using System.Collections.Generic;

[System.Serializable]
public class JsonListWrapper<T>
{
    public List<T> list;
    public JsonListWrapper(List<T> list) => this.list = list;
}