namespace PlanningPoker.Core.SharedKernel;

public class BaseEntityWithGuid : BaseEntity
{
    public BaseEntityWithGuid()
    {
        Id = Guid.NewGuid().ToString();
    }
}
