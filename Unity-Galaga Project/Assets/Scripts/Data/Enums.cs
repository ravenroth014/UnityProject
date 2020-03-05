//  Enums.cs
//  By Atid Puwatnuttasit

public enum EnemyType
{
    Blue        
    , Red
    , Green
}

// Enemy action state.
public enum EnemyStates
{
    PathFollowing
    , PathFormation
    , Idle
    , Diving
}

public enum CharacterType
{
    Player
    , Enemy
}

public enum InvisibleType
{
    Visible
    , BottomInvisible
    , TopInvisible
}

public enum MenuScene
{
    MainMenu
    , CustomMenu
    , Game
    , GameOver
    , None
}

public enum CustomMenuOrder
{
    Life = 0
    , Speed = 1
    , Bullet = 2
    , ConfirmMenu = 3
}