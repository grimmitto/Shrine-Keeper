using UnityEngine;

public interface IVisitor
{
    string VisitorName { get; }
    int DialogueID { get; }
    VisitorSchedule Schedule { get; }  // optional reference
}
