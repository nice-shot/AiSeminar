// Used to store state names to avoid relying on strings
public 
    class States {
    // Agent Basic States
    public const string
    X = "x",
    Y = "y",
    Z = "z",
    ESCAPED = "escaped",
    EXPLORED = "explored",
    HELD_ITEM = "heldItem",
    
    // Zombie states
    EAT_BRAINS = "eatBrains",
    WANDER = "wander",

    // States for actions
    ESCAPE_ROUTE_AVAILABLE = "escapeRouteAvailable",

    // States for points of interest
    SEARCHED = "searched",
    CLEAR = "clear", // Used in safe spots

    // Door States
    OPEN = "open",
    LOCKED = "locked",

    // Item States
    TYPE = "type",
    VISIBLE = "visible"
    ;
}
