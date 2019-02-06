/// <summary>
/// Basic interface for all interactive items 
/// </summary>
public interface IBaseAction
{

    SimpleActionDelegate OnPointerLeft { get; set; }

    SimpleActionDelegate OnPointerHover { get; set; }

    SimpleActionDelegate OnActiveAction { get; set; }

}

/// <summary>
/// Simple delegate for basic functions
/// </summary>
public delegate void SimpleActionDelegate();