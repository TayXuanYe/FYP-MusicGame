using Godot;
using System;

public partial class UserDataManager : Node
{
    private static UserDataManager _instance;
    public static UserDataManager Instance => _instance;

    public const string DEFAULT_USERNAME = "default_user";
    public UserData CurrentUser { get; set; } = new UserData(-1, DEFAULT_USERNAME, "DEFAULT", false, null);

    public override void _Ready()
    {
        _instance = this;
    }
}
