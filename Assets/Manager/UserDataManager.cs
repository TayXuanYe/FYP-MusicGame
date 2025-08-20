using Godot;
using System;

public partial class UserDataManager : Node
{
    private static UserDataManager _instance;
    public static UserDataManager Instance => _instance;
    public UserData CurrentUser { get; set; }

    public override void _Ready()
    {
        _instance = this;
    }
}
