public interface IInteractable
{
    string InteractionText { get; }
    void Interact();

    bool StaysInteractable { get; }
}
