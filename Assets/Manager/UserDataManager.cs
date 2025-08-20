using Godot;
using System;

public partial class UserDataManager : Node
{
    private static UserDataManager _instance;
    public static UserDataManager Instance => _instance;

    public override void _Ready()
    {
            _instance = this;
    }
}
