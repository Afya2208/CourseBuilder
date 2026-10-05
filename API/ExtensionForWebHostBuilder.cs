using API.Features.Courses;
using API.Features.Feedbacks;
using API.Features.Groups;
using API.Features.Kits;
using API.Features.Learning;
using API.Features.LessonContent;
using API.Features.Lessons;
using API.Features.Modules;
using API.Features.Themes;
using API.Features.Users;

namespace API
{
    public static class ExtensionForWebHostBuilder
    {
        public static void AddRepositoriesAndServices(this IServiceCollection services)
        {
            services.AddTransient<IThemeRepository, ThemeRepository>();
            services.AddTransient<IThemeService, ThemeService>();
            
            services.AddTransient<ILessonTypeRepository, LessonTypeRepository>();
            services.AddTransient<ILessonRepository, LessonRepository>();
            services.AddTransient<ILessonService, LessonService>();
            
            services.AddTransient<IRoleRepository, RoleRepository>();
            services.AddTransient<IRoleService, RoleService>();
            
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IUserService, UserService>();
            
            services.AddTransient<IKitRepository, KitRepository>();
            services.AddTransient<IKitService, KitService>();
            
            services.AddTransient<IFeedbackRepository, FeedbackRepository>();
            services.AddTransient<IFeedbackService, FeedbackService>();

            services.AddTransient<ICorrelationRepository, CorrelationRepository>();
            
            services.AddTransient<IModuleRepository, ModuleRepository>();
            services.AddTransient<IModuleService, ModuleService>();
            
            services.AddTransient<ICourseRepository, CourseRepository>();
            services.AddTransient<ICourseService, CourseService>();
            
            services.AddTransient<ITaskAnswerRepository, TaskAnswerRepository>();
            services.AddTransient<ITaskTypeRepository, TaskTypeRepository>();
            services.AddTransient<ITaskRepository, TaskRepository>();
            
            services.AddTransient<IContentBlockRepository, ContentBlockRepository>();
            services.AddTransient<IContentBlockTypeRepository, ContentBlockTypeRepository>();
            
            services.AddTransient<IGroupRepository, GroupRepository>();
            services.AddTransient<IProgressService, ProgressService>();
            services.AddTransient<IProgressRepository, ProgressRepository>();
        }
    }
}