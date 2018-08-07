// Used to store state names to avoid relying on strings
public static class States {
    // Agent Basic States
    public const string
    X = "x",
    Y = "y",
    Z = "z",
    ESCAPED = "escaped",
    EXPLORED = "explored",
    HELD_ITEM = "heldItem",

    // States for actions
    ESCAPE_ROUTE_AVAILABLE = "escapeRouteAvailable",

    // States for points of interest

    SEARCHED = "searched",

    // Door States
    OPEN = "open",
    LOCKED = "locked",

    // Item States
    TYPE = "type",
    VISIBLE = "visible"
    ;
}
