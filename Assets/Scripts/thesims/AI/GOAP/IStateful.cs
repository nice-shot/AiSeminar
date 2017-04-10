namespace Ai.Goap {
/// <summary>
/// Any object that keeps a state for planning purposes should implement this
/// interface.
/// </summary>
public interface IStateful {
    /// <summary>
    /// Returns the current state of the object for planning purposes.
    /// </summary>
    State GetState();
}
}
