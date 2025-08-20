using Godot;
using System;

public partial class UserDataManager : Node
{
    private static UserDataManager _instance;
    public static UserDataManager Instance => _instance;

    public const string DEFAULT_USERNAME = "default_user";
    public UserData CurrentUser { get; set; } = new UserData(DEFAULT_USERNAME, "DEFAULT", false);

    public override void _Ready()
    {
        _instance = this;
    }
}
