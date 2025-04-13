using HyperCube.Entities.Core.Entities.Base;

namespace HyperCube.Entities.Core.Entities;

/// <summary>
/// Base entity class with GUID as the primary key type.
/// </summary>
public abstract class BaseGuidEntity : BaseEntity<Guid>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseGuidEntity"/> class.
    /// </summary>
    protected BaseGuidEntity()
    {
        Id = Guid.NewGuid();
    }
}
