using FluentValidation;

namespace TE4IT.Application.Features.Education.Courses.Commands.DeleteCourse;

/// <summary>
/// DeleteCourseCommand için validation kuralları
/// CourseId route'dan geldiği için validation'a gerek yok (route constraint zaten kontrol ediyor)
/// </summary>
public sealed class DeleteCourseCommandValidator : AbstractValidator<DeleteCourseCommand>
{
    public DeleteCourseCommandValidator()
    {
        // CourseId route parameter'dan geldiği için validation'a gerek yok
    }
}

