using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Repositories;

namespace API
{
    public static class ExtensionForBuilder
    {
        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<CorrelationRepository>();
            services.AddScoped<ThemeRepository>();
            services.AddScoped<UserRepository>();
            services.AddScoped<ModuleRepository>();
            services.AddScoped<CourseRepository>();
            services.AddScoped<RoleRepository>();
            services.AddScoped<LessonRepository>();
            services.AddScoped<TaskAnswerRepository>();
            services.AddScoped<TaskTypeRepository>();
            services.AddScoped<ContentBlockRepository>();
            services.AddScoped<ContentBlockTypeRepository>();
            services.AddScoped<FeedbackSubmitRepository>();
            services.AddScoped<TaskRepository>();
            services.AddScoped<GroupRepository>();
            services.AddScoped<LessonTypeRepository>();
        }
    }
}