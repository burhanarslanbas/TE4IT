namespace TE4IT.Domain.Constants;

public static class Permissions
{
    public static class Project
    {
        public const string Create = "Project.Create";
        public const string View = "Project.View";
        public const string Update = "Project.Update";
        public const string Delete = "Project.Delete";
    }

    public static class Module
    {
        public const string Create = "Module.Create";
        public const string View = "Module.View";
        public const string Update = "Module.Update";
        public const string Delete = "Module.Delete";
    }

    public static class UseCase
    {
        public const string Create = "UseCase.Create";
        public const string View = "UseCase.View";
        public const string Update = "UseCase.Update";
        public const string Delete = "UseCase.Delete";
    }

    public static class Task
    {
        public const string Create = "Task.Create";
        public const string View = "Task.View";
        public const string Update = "Task.Update";
        public const string Assign = "Task.Assign";
        public const string StateChange = "Task.StateChange";
        public const string Delete = "Task.Delete";
    }

    public static class TaskRelation
    {
        public const string Create = "TaskRelation.Create";
        public const string View = "TaskRelation.View";
        public const string Delete = "TaskRelation.Delete";
    }

    public static class Education
    {
        public const string CourseCreate = "Education.Course.Create";
        public const string CourseUpdate = "Education.Course.Update";
        public const string CourseDelete = "Education.Course.Delete";
        public const string CourseView = "Education.Course.View";
        
        public const string RoadmapCreate = "Education.Roadmap.Create";
        public const string RoadmapUpdate = "Education.Roadmap.Update";
        public const string RoadmapDelete = "Education.Roadmap.Delete";
        public const string RoadmapView = "Education.Roadmap.View";
        
        public const string ContentCreate = "Education.Content.Create";
        public const string ContentUpdate = "Education.Content.Update";
        public const string ContentDelete = "Education.Content.Delete";
        public const string ContentAccess = "Education.Content.Access";
        
        public const string EnrollmentCreate = "Education.Enrollment.Create";
        public const string EnrollmentView = "Education.Enrollment.View";
        
        public const string ProgressView = "Education.Progress.View";
        public const string ProgressUpdate = "Education.Progress.Update";
    }
}


