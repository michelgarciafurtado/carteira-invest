using System;
using System.Collections.Generic;
using System.Text;

namespace CarteiraInvest.Domain.Aggregates;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }

    protected BaseEntity()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }

}
