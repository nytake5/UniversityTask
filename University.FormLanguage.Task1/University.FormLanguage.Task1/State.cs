namespace University.FormLanguage.Task1;

public class State
{
    public string Name { get; set; } = "";
    public List<KeyValuePair<string, State>> Transitions { get; set; } = new();
    public bool IsAcceptState { get; set; } = false;
    public bool IsStartState { get; set; }
    public List<State> EpsMove { get; set; } = new List<State>();

    protected bool Equals(State other)
    {
        return Name == other.Name;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((State) obj);
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
}