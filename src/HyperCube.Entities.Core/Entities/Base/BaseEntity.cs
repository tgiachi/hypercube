using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HyperCube.Entities.Core.Entities.Base;

/// <summary>
///     Base class for all entities in the HyperCube framework.
/// </summary>
/// <typeparam name="TKey">The type of the primary key.</typeparam>
public abstract class BaseEntity<TKey> where TKey : IEquatable<TKey>
{
    /// <summary>
    ///     Gets or sets the unique identifier for the entity.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public TKey Id { get; set; } = default!;

    /// <summary>
    ///     Gets or sets the date when this entity was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    ///     Gets or sets the date when this entity was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    ///     Determines whether this entity is equal to another object.
    /// </summary>
    /// <param name="obj">The object to compare with the current entity.</param>
    /// <returns>true if the specified object is equal to the current entity; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != GetType())
        {
            return false;
        }

        if (obj is not BaseEntity<TKey> other)
        {
            return false;
        }

        if (EqualityComparer<TKey>.Default.Equals(Id, default) ||
            EqualityComparer<TKey>.Default.Equals(other.Id, default))
        {
            return false;
        }

        return EqualityComparer<TKey>.Default.Equals(Id, other.Id);
    }

    /// <summary>
    ///     Gets the hash code for this entity.
    /// </summary>
    /// <returns>A hash code for the current entity.</returns>
    public override int GetHashCode()
    {
        return Id?.GetHashCode() ?? 0;
    }

    /// <summary>
    ///     Compares two entities for equality.
    /// </summary>
    /// <param name="left">The first entity to compare.</param>
    /// <param name="right">The second entity to compare.</param>
    /// <returns>true if the entities are equal; otherwise, false.</returns>
    public static bool operator ==(BaseEntity<TKey>? left, BaseEntity<TKey>? right)
    {
        if (left is null && right is null)
        {
            return true;
        }

        if (left is null || right is null)
        {
            return false;
        }

        return left.Equals(right);
    }

    /// <summary>
    ///     Compares two entities for inequality.
    /// </summary>
    /// <param name="left">The first entity to compare.</param>
    /// <param name="right">The second entity to compare.</param>
    /// <returns>true if the entities are not equal; otherwise, false.</returns>
    public static bool operator !=(BaseEntity<TKey>? left, BaseEntity<TKey>? right)
    {
        return !(left == right);
    }
}
