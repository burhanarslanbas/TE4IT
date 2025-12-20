using System.Reflection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using TE4IT.Domain.Entities.Education;

namespace TE4IT.Persistence.EducationManagement.Mappings;

/// <summary>
/// MongoDB için Guid ve embedded document mapping konfigürasyonları.
/// Guid'ler string olarak saklanır, embedded document'ler doğru şekilde serialize edilir.
/// </summary>
public static class EducationMongoMappings
{
    /// <summary>
    /// Tüm Education entity'leri için MongoDB mapping'lerini kaydeder.
    /// Uygulama başlangıcında bir kez çağrılmalıdır.
    /// </summary>
    public static void RegisterMappings()
    {
        if (BsonClassMap.IsClassMapRegistered(typeof(Course)))
        {
            return; // Zaten kayıtlı, tekrar kaydetme
        }

        // Guid serializer'ı string olarak ayarla (global)
        BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));

        RegisterCourseMapping();
        RegisterCourseRoadmapMapping();
        RegisterRoadmapStepMapping();
        RegisterCourseContentMapping();
        RegisterEnrollmentMapping();
        RegisterProgressMapping();
    }

    private static void RegisterCourseMapping()
    {
        BsonClassMap.RegisterClassMap<Course>(cm =>
        {
            cm.AutoMap();
            cm.SetIdMember(cm.GetMemberMap(c => c.Id));
            // Guid zaten string olarak serialize edilecek (global serializer)
        });
    }

    private static void RegisterCourseRoadmapMapping()
    {
        BsonClassMap.RegisterClassMap<CourseRoadmap>(cm =>
        {
            cm.AutoMap();
            cm.SetIdMember(cm.GetMemberMap(c => c.Id));
            
            // Steps property'sini ignore et (read-only collection, backing field kullanılacak)
            cm.UnmapMember(c => c.Steps);
            
            // Backing field'ı map et - MongoDB'deki "Steps" array'ini backing field'a yaz
            // MapField string field name bekler
            cm.MapField("steps").SetElementName("Steps");
            
            // Embedded document, Course içinde tutulur
        });
    }

    private static void RegisterRoadmapStepMapping()
    {
        BsonClassMap.RegisterClassMap<RoadmapStep>(cm =>
        {
            cm.AutoMap();
            cm.SetIdMember(cm.GetMemberMap(s => s.Id));
            
            // Contents property'sini ignore et (read-only collection, backing field kullanılacak)
            cm.UnmapMember(s => s.Contents);
            
            // Backing field'ı map et - MongoDB'deki "Contents" array'ini backing field'a yaz
            // MapField string field name bekler
            cm.MapField("contents").SetElementName("Contents");
            
            // Embedded document, CourseRoadmap içinde tutulur
        });
    }

    private static void RegisterCourseContentMapping()
    {
        BsonClassMap.RegisterClassMap<CourseContent>(cm =>
        {
            cm.AutoMap();
            cm.SetIdMember(cm.GetMemberMap(c => c.Id));
            // Embedded document, RoadmapStep içinde tutulur
        });
    }

    private static void RegisterEnrollmentMapping()
    {
        BsonClassMap.RegisterClassMap<Enrollment>(cm =>
        {
            cm.AutoMap();
            cm.SetIdMember(cm.GetMemberMap(e => e.Id));
            // CourseId, UserId Guid'leri string olarak saklanacak
        });
    }

    private static void RegisterProgressMapping()
    {
        BsonClassMap.RegisterClassMap<Progress>(cm =>
        {
            cm.AutoMap();
            cm.SetIdMember(cm.GetMemberMap(p => p.Id));
            // CourseId, StepId, ContentId, EnrollmentId, UserId Guid'leri string olarak saklanacak
        });
    }
}


