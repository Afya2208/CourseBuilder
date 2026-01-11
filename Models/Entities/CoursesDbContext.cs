using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Models.Entities;

public partial class CoursesDbContext : DbContext
{
    public CoursesDbContext()
    {
    }

    public CoursesDbContext(DbContextOptions<CoursesDbContext> options)
        : base(options)
    {
    }
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        
    }


    public virtual DbSet<ContentBlock> ContentBlocks { get; set; }

    public virtual DbSet<ContentBlockType> ContentBlockTypes { get; set; }

    public virtual DbSet<Correlation> Correlations { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<Lesson> Lessons { get; set; }

    public virtual DbSet<LessonType> LessonTypes { get; set; }

    public virtual DbSet<Module> Modules { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Task> Tasks { get; set; }

    public virtual DbSet<TaskAnswer> TaskAnswers { get; set; }

    public virtual DbSet<TaskType> TaskTypes { get; set; }

    public virtual DbSet<Theme> Themes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserInformation> UserInformations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ContentBlock>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("content_block_pkey");

            entity.ToTable("content_block");

            entity.HasIndex(e => e.ContentBlockTypeId, "IX_content_block_content_block_type_id");

            entity.HasIndex(e => e.LessonId, "IX_content_block_lesson_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ContentBlockTypeId)
                .HasDefaultValue(1)
                .HasColumnName("content_block_type_id");
            entity.Property(e => e.File).HasColumnName("file");
            entity.Property(e => e.FileName)
                .HasMaxLength(256)
                .HasColumnName("file_name");
            entity.Property(e => e.LessonId).HasColumnName("lesson_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Order)
                .HasDefaultValue(0)
                .HasColumnName("order");
            entity.Property(e => e.TextValue).HasColumnName("text_value");

            entity.HasOne(d => d.ContentBlockType).WithMany(p => p.ContentBlocks)
                .HasForeignKey(d => d.ContentBlockTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("content_block_content_block_type_id_fkey");

            entity.HasOne(d => d.Lesson).WithMany(p => p.ContentBlocks)
                .HasForeignKey(d => d.LessonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("content_block_lesson_id_fkey");
        });

        modelBuilder.Entity<ContentBlockType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("content_block_type_pkey");

            entity.ToTable("content_block_type");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Correlation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("correlation_pkey");

            entity.ToTable("correlation");

            entity.HasIndex(e => e.TaskId, "IX_correlation_task_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Left)
                .HasMaxLength(500)
                .HasColumnName("left");
            entity.Property(e => e.Right)
                .HasMaxLength(500)
                .HasColumnName("right");
            entity.Property(e => e.TaskId).HasColumnName("task_id");

            entity.HasOne(d => d.Task).WithMany(p => p.Correlations)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("correlation_task_id_fkey");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("course_pkey");

            entity.ToTable("course");

            entity.HasIndex(e => e.AuthorId, "IX_course_author_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AuthorId).HasColumnName("author_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.MinimalCompletionPercentage)
                .HasDefaultValue(100)
                .HasColumnName("minimal_completion_percentage");
            entity.Property(e => e.ModulesHaveOrder)
                .HasDefaultValue(false)
                .HasColumnName("modules_have_order");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasColumnName("price");

            entity.HasOne(d => d.Author).WithMany(p => p.Courses)
                .HasForeignKey(d => d.AuthorId)
                .HasConstraintName("course_author_id_fkey");

            entity.HasMany(d => d.ForCourses).WithMany(p => p.NeededCourses)
                .UsingEntity<Dictionary<string, object>>(
                    "CourseOrder",
                    r => r.HasOne<Course>().WithMany()
                        .HasForeignKey("ForCourseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("course_order_for_course_id_fkey"),
                    l => l.HasOne<Course>().WithMany()
                        .HasForeignKey("NeededCourseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("course_order_needed_course_id_fkey"),
                    j =>
                    {
                        j.HasKey("NeededCourseId", "ForCourseId").HasName("course_order_pkey");
                        j.ToTable("course_order");
                        j.HasIndex(new[] { "ForCourseId" }, "IX_course_order_for_course_id");
                        j.IndexerProperty<int>("NeededCourseId").HasColumnName("needed_course_id");
                        j.IndexerProperty<int>("ForCourseId").HasColumnName("for_course_id");
                    });

            entity.HasMany(d => d.NeededCourses).WithMany(p => p.ForCourses)
                .UsingEntity<Dictionary<string, object>>(
                    "CourseOrder",
                    r => r.HasOne<Course>().WithMany()
                        .HasForeignKey("NeededCourseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("course_order_needed_course_id_fkey"),
                    l => l.HasOne<Course>().WithMany()
                        .HasForeignKey("ForCourseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("course_order_for_course_id_fkey"),
                    j =>
                    {
                        j.HasKey("NeededCourseId", "ForCourseId").HasName("course_order_pkey");
                        j.ToTable("course_order");
                        j.HasIndex(new[] { "ForCourseId" }, "IX_course_order_for_course_id");
                        j.IndexerProperty<int>("NeededCourseId").HasColumnName("needed_course_id");
                        j.IndexerProperty<int>("ForCourseId").HasColumnName("for_course_id");
                    });

            entity.HasMany(d => d.Themes).WithMany(p => p.Courses)
                .UsingEntity<Dictionary<string, object>>(
                    "CourseTheme",
                    r => r.HasOne<Theme>().WithMany()
                        .HasForeignKey("ThemeId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("course_theme_theme_id_fkey"),
                    l => l.HasOne<Course>().WithMany()
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("course_theme_course_id_fkey"),
                    j =>
                    {
                        j.HasKey("CourseId", "ThemeId").HasName("course_theme_pkey");
                        j.ToTable("course_theme");
                        j.HasIndex(new[] { "ThemeId" }, "IX_course_theme_theme_id");
                        j.IndexerProperty<int>("CourseId").HasColumnName("course_id");
                        j.IndexerProperty<int>("ThemeId").HasColumnName("theme_id");
                    });
        });

        modelBuilder.Entity<Lesson>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("lesson_pkey");

            entity.ToTable("lesson");

            entity.HasIndex(e => e.LessonTypeId, "IX_lesson_lesson_type_id");

            entity.HasIndex(e => e.ModuleId, "IX_lesson_module_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.LessonTypeId).HasColumnName("lesson_type_id");
            entity.Property(e => e.ModuleId).HasColumnName("module_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Order)
                .HasDefaultValue(0)
                .HasColumnName("order");

            entity.HasOne(d => d.LessonType).WithMany(p => p.Lessons)
                .HasForeignKey(d => d.LessonTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("lesson_lesson_type_id_fkey");

            entity.HasOne(d => d.Module).WithMany(p => p.Lessons)
                .HasForeignKey(d => d.ModuleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("lesson_module_id_fkey");
        });

        modelBuilder.Entity<LessonType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("lesson_type_pkey");

            entity.ToTable("lesson_type");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(300)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Module>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("module_pkey");

            entity.ToTable("module");

            entity.HasIndex(e => e.CourseId, "IX_module_course_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.LessonsHaveOrder)
                .HasDefaultValue(false)
                .HasColumnName("lessons_have_order");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Order)
                .HasDefaultValue(0)
                .HasColumnName("order");

            entity.HasOne(d => d.Course).WithMany(p => p.Modules)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("module_course_id_fkey");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("role_pkey");

            entity.ToTable("role");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Task>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("task_pkey");

            entity.ToTable("task");

            entity.HasIndex(e => e.LessonId, "IX_task_lesson_id");

            entity.HasIndex(e => e.TaskTypeId, "IX_task_task_type_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.LessonId).HasColumnName("lesson_id");
            entity.Property(e => e.Order)
                .HasDefaultValue(0)
                .HasColumnName("order");
            entity.Property(e => e.Question).HasColumnName("question");
            entity.Property(e => e.TaskTypeId).HasColumnName("task_type_id");

            entity.HasOne(d => d.Lesson).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.LessonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("task_lesson_id_fkey");

            entity.HasOne(d => d.TaskType).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.TaskTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("task_task_type_id_fkey");
        });

        modelBuilder.Entity<TaskAnswer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("task_answer_pkey");

            entity.ToTable("task_answer");

            entity.HasIndex(e => e.TaskId, "IX_task_answer_task_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.File).HasColumnName("file");
            entity.Property(e => e.FileName)
                .HasMaxLength(256)
                .HasColumnName("file_name");
            entity.Property(e => e.IsRight).HasColumnName("is_right");
            entity.Property(e => e.TaskId).HasColumnName("task_id");
            entity.Property(e => e.TextValue).HasColumnName("text_value");

            entity.HasOne(d => d.Task).WithMany(p => p.TaskAnswers)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("task_answer_task_id_fkey");
        });

        modelBuilder.Entity<TaskType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("task_type_pkey");

            entity.ToTable("task_type");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Theme>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("theme_pkey");

            entity.ToTable("theme");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_pkey");

            entity.ToTable("user");

            entity.HasIndex(e => e.RoleId, "IX_user_role_id");

            entity.HasIndex(e => e.Email, "user_email_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Password).HasColumnName("password");
            entity.Property(e => e.RoleId)
                .HasDefaultValue(1)
                .HasColumnName("role_id");
            entity.Property(e => e.Salt).HasColumnName("salt");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_role_id_fkey");
        });

        modelBuilder.Entity<UserInformation>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("user_information_pk");

            entity.ToTable("user_information");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("user_id");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("last_name");
            entity.Property(e => e.MiddleName)
                .HasMaxLength(100)
                .HasColumnName("middle_name");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.Position)
                .HasMaxLength(100)
                .HasColumnName("position");

            entity.HasOne(d => d.User).WithOne(p => p.UserInformation)
                .HasForeignKey<UserInformation>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_information_user_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
