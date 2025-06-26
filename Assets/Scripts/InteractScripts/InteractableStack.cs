using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class InteractableStack<T>
{
    private List<T> _stack = new();
    public int Count = 0;

    // Always insert at the head of the list, no duplicates
    public void Push(T value)
    {
        // Move duplicate to the start if it is already there
        if (Contains(value))
        {
            Remove(value);
        }

        _stack.Insert(0, value);
        Count++;
    }

    // Pop from the head of the list if the list has things in it
    public T Pop()
    {
        if (_stack.Count <= 0)
        {
            return default(T);
        }

        Count--;

        T retVal = _stack[0];
        _stack.RemoveAt(0);

        return retVal;
    }

    public T Peek()
    {
        if (_stack.Count <= 0)
        {
            return default(T);
        }

        return _stack[0];
    }

    // removes a value if it exists
    public void Remove(T value)
    {
        if (!_stack.Contains(value))
        {
            return;
        }

        Count--;

        _stack.Remove(value);
    }

    public bool Contains(T value)
    {
        return _stack.Contains(value);
    }
}
