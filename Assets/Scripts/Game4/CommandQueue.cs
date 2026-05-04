using UnityEngine;
using System.Collections.Generic;

public class CommandQueue : MonoBehaviour
{
    public const int MaxCommands = 12;

    readonly List<Direction> commands = new List<Direction>();
    public IReadOnlyList<Direction> Commands => commands;

    public void Add(Direction dir)
    {
        if (commands.Count < MaxCommands)
            commands.Add(dir);
    }

    public void Clear() => commands.Clear();

    public int Count => commands.Count;
}
