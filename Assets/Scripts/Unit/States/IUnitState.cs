// IUnitState.cs
public interface IUnitState
{
    // Appelé une seule fois lors de l'entrée dans l'état
    void OnEnter(Unit unit);

    // Appelé à chaque frame ou à intervalles réguliers (moins souvent que Update, de préférence)
    void OnExecute(Unit unit);

    // Appelé une seule fois lors de la sortie de l'état
    void OnExit(Unit unit);
}