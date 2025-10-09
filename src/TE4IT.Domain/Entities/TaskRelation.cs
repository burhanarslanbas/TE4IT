using TE4IT.Domain.Entities.Common;
using TE4IT.Domain.Enums;

namespace TE4IT.Domain.Entities;

public class TaskRelation : BaseEntity
{
    public Guid SourceTaskId { get; set; } // Kaynak Görev ID
    public Guid TargetTaskId { get; set; } // Hedef Görev ID
    public TaskRelationType RelationType { get; set; } // Görev İlişki Türü (Parent, Child, Blocks, CausedBy, RelatedTo, DuplicateOf, FollowUp, LinkedTo, ReferenceTo, Other)
}