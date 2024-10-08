using PlanningPoker.Core.SharedKernel;

namespace PlanningPoker.Core.Entities;

public class Property : BaseEntity
{
    private readonly IDictionary<string, string> data = null!;
    public required PropertyType Type { get; init; }

    public required IDictionary<string, string> Data
    {
        get => data;
        init
        {
            if (value.Count == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Property must have at least one data value.");
            }

            data = value;
        }
    }

    public bool IsEqualProperty(Property? other)
    {
        if (other is null)
        {
            return false;
        }

        if (Data.Count != other.Data.Count)
        {
            return false;
        }

        if (Type != other.Type)
        {
            return false;
        }

        return !Data.Except(other.Data).Any();
    }
}

public enum PropertyType
{
    Undefined,
    Label
}
