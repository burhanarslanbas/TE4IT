using System.ComponentModel;

namespace TE4IT.Domain.Enums;

public enum TaskRelationType
{
    [Description("Bu görev tamamlanmadan diğer görev ilerleyemez.")]
    Blocks = 1,

    [Description("Görevler birbiriyle bağlantılıdır fakat zorunlu bağımlılık yoktur.")]
    RelatesTo,

    [Description("Bu görev, belirli bir bug veya problemi düzeltir.")]
    Fixes,

    [Description("Bu görev veya bug başka bir görevle aynıdır (tekrardır).")]
    Duplicates
}
