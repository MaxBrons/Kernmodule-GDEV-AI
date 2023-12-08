using System.Collections.Generic;
using Unity.VisualScripting;

public class Blackboard
{
    private Dictionary<string, object> _values = new Dictionary<string, object>();

    public void Set(string id, object obj)
    {
        Set<object>(id, obj);
    }

    public void Set<T>(string id, T obj)
    {
        if (_values.ContainsKey(id)) {
            if (obj == null) {
                _values.Remove(id);
                return;
            }

            _values[id] = obj;
            return;
        }
        _values.Add(id, obj);
    }

    public T Get<T>(string id)
    {
        var obj = _values.GetValueOrDefault(id);

        return obj.IsConvertibleTo(typeof(T), true) ? (T)obj : default;
    }
}