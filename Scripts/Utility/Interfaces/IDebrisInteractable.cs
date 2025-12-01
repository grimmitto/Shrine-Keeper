public interface IDebrisInteractable : IInteractable
{
    int cleaningValue { get; }  // how much to restore maintenance
    string resourceType { get; } // wood, vine, dirt etc
}
