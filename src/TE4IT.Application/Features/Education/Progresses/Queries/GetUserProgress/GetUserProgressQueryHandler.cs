using MediatR;
using TE4IT.Application.Abstractions.Auth;

namespace TE4IT.Application.Features.Education.Progresses.Queries.GetUserProgress;

public sealed class GetUserProgressQueryHandler(ICurrentUser currentUser) : IRequestHandler<GetUserProgressQuery, UserProgressResponse>
{
    public Task<UserProgressResponse> Handle(GetUserProgressQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.Id ?? throw new UnauthorizedAccessException("Kullanıcı kimliği bulunamadı.");

        // TODO: ICourseProgressService ile progress hesaplanacak
        // Şimdilik basit bir response döndürüyoruz
        return Task.FromResult(new UserProgressResponse
        {
            UserId = currentUserId.Value,
            CourseProgresses = new List<CourseProgressItem>()
        });
    }
}