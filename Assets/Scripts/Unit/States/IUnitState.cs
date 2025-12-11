public interface IUnitState
{
    // Called on state enter
    void OnEnter(Unit unit);

    // Called each tick
    void OnExecute(Unit unit);

    // Called on state exit
    void OnExit(Unit unit);
}