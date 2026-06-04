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

    public virtual DbSet<AdditionalTryForUser> AdditionalTryForUsers { get; set; }

    public virtual DbSet<ContentBlock> ContentBlocks { get; set; }

    public virtual DbSet<ContentBlockType> ContentBlockTypes { get; set; }

    public virtual DbSet<Correlation> Correlations { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<FeedbackCategory> FeedbackCategories { get; set; }

    public virtual DbSet<FeedbackSubmit> FeedbackSubmits { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<JoinStatus> JoinStatuses { get; set; }

    public virtual DbSet<Kit> Kits { get; set; }

    public virtual DbSet<Lesson> Lessons { get; set; }

    public virtual DbSet<LessonType> LessonTypes { get; set; }

    public virtual DbSet<Module> Modules { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Task> Tasks { get; set; }

    public virtual DbSet<TaskAnswer> TaskAnswers { get; set; }

    public virtual DbSet<TaskType> TaskTypes { get; set; }

    public virtual DbSet<Theme> Themes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserHasCourse> UserHasCourses { get; set; }

    public virtual DbSet<UserHasDoneCourse> UserHasDoneCourses { get; set; }

    public virtual DbSet<UserHasKit> UserHasKits { get; set; }

    public virtual DbSet<UserInGroup> UserInGroups { get; set; }

    public virtual DbSet<UserInformation> UserInformations { get; set; }

    public virtual DbSet<UserTrySolveDetail> UserTrySolveDetails { get; set; }

    public virtual DbSet<UserTrySolveTask> UserTrySolveTasks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("pg_trgm");

        modelBuilder.Entity<AdditionalTryForUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("additional_try_for_user_pkey");

            entity.ToTable("additional_try_for_user");

            entity.HasIndex(e => e.LessonId, "ix_additional_try_lesson_id");

            entity.HasIndex(e => e.UserId, "ix_additional_try_user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.IsUsed)
                .HasDefaultValue(false)
                .HasColumnName("is_used");
            entity.Property(e => e.LessonId).HasColumnName("lesson_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Lesson).WithMany(p => p.AdditionalTryForUsers)
                .HasForeignKey(d => d.LessonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("additional_try_for_user_lesson_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.AdditionalTryForUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("additional_try_for_user_user_id_fkey");
        });

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
            entity.Property(e => e.FileNameView)
                .HasMaxLength(256)
                .HasColumnName("file_name_view");
            entity.Property(e => e.FileNameStorage)
                .HasMaxLength(100)
                .HasColumnName("file_name_storage");
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
                .HasConstraintName("correlation_task_id_fkey");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("course_pkey");

            entity.ToTable("course");

            entity.HasIndex(e => e.AuthorId, "IX_course_author_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AuthorId).HasColumnName("author_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsPublic)
                .HasDefaultValue(false)
                .HasColumnName("is_public");
            entity.Property(e => e.LinkedGroupId).HasColumnName("linked_group_id");
            entity.Property(e => e.ModulesHaveOrder)
                .HasDefaultValue(true)
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

            entity.HasOne(d => d.LinkedGroup).WithMany(p => p.Courses)
                .HasForeignKey(d => d.LinkedGroupId)
                .HasConstraintName("course_linked_group_id_fkey");

            entity.HasMany(d => d.Groups).WithMany(p => p.CoursesNavigation)
                .UsingEntity<Dictionary<string, object>>(
                    "GroupHaveCourse",
                    r => r.HasOne<Group>().WithMany()
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("group_have_course_group_id_fkey"),
                    l => l.HasOne<Course>().WithMany()
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("group_have_course_course_id_fkey"),
                    j =>
                    {
                        j.HasKey("CourseId", "GroupId").HasName("group_have_course_pkey");
                        j.ToTable("group_have_course");
                        j.IndexerProperty<int>("CourseId").HasColumnName("course_id");
                        j.IndexerProperty<int>("GroupId").HasColumnName("group_id");
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

        modelBuilder.Entity<FeedbackCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("complaint_category_pkey");

            entity.ToTable("feedback_category");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('complaint_category_id_seq'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<FeedbackSubmit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("complaint_pkey");

            entity.ToTable("feedback_submit");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('complaint_id_seq'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.DateTimeSent)
                .HasDefaultValueSql("LOCALTIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date_time_sent");
            entity.Property(e => e.DateTimeSolved)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date_time_solved");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FeedbackCategoryId).HasColumnName("feedback_category_id");
            entity.Property(e => e.Text).HasColumnName("text");
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .HasColumnName("title");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.FeedbackCategory).WithMany(p => p.FeedbackSubmits)
                .HasForeignKey(d => d.FeedbackCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("complaint_complaint_category_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.FeedbackSubmits)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("complaint_user_id_fkey");
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("group_pkey");

            entity.ToTable("group");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CuratorFeedback)
                .HasMaxLength(100)
                .HasColumnName("curator_feedback");
            entity.Property(e => e.CuratorId).HasColumnName("curator_id");
            entity.Property(e => e.DateEnd).HasColumnName("date_end");
            entity.Property(e => e.DateStart)
                .HasDefaultValueSql("now()")
                .HasColumnName("date_start");
            entity.Property(e => e.MaxMembersCount)
                .HasDefaultValue(20)
                .HasColumnName("max_members_count");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");

            entity.HasOne(d => d.Curator).WithMany(p => p.Groups)
                .HasForeignKey(d => d.CuratorId)
                .HasConstraintName("group_curator_id_fkey");
        });

        modelBuilder.Entity<JoinStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("join_status_pkey");

            entity.ToTable("join_status");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Kit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("kit_pkey");

            entity.ToTable("kit");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AuthorId).HasColumnName("author_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasColumnName("price");

            entity.HasOne(d => d.Author).WithMany(p => p.Kits)
                .HasForeignKey(d => d.AuthorId)
                .HasConstraintName("kit_author_id_fkey");

            entity.HasMany(d => d.Courses).WithMany(p => p.Kits)
                .UsingEntity<Dictionary<string, object>>(
                    "CourseInKit",
                    r => r.HasOne<Course>().WithMany()
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("course_in_kit_course_id_fkey"),
                    l => l.HasOne<Kit>().WithMany()
                        .HasForeignKey("KitId")
                        .HasConstraintName("course_in_kit_kit_id_fkey"),
                    j =>
                    {
                        j.HasKey("KitId", "CourseId").HasName("course_in_kit_pkey");
                        j.ToTable("course_in_kit");
                        j.IndexerProperty<int>("KitId").HasColumnName("kit_id");
                        j.IndexerProperty<int>("CourseId").HasColumnName("course_id");
                    });
        });

        modelBuilder.Entity<Lesson>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("lesson_pkey");

            entity.ToTable("lesson");

            entity.HasIndex(e => e.LessonTypeId, "IX_lesson_lesson_type_id");

            entity.HasIndex(e => e.ModuleId, "IX_lesson_module_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ClosedUntil).HasColumnName("closed_until");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsRequired)
                .HasDefaultValue(false)
                .HasColumnName("is_required");
            entity.Property(e => e.LessonTypeId).HasColumnName("lesson_type_id");
            entity.Property(e => e.MaxTriesCount).HasColumnName("max_tries_count");
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

            entity.HasIndex(e => e.CourseId, "ix_module_course_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.LessonsHaveOrder)
                .HasDefaultValue(true)
                .HasColumnName("lessons_have_order");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Order)
                .HasDefaultValue(0)
                .HasColumnName("order");

            entity.HasOne(d => d.Course).WithMany(p => p.Modules)
                .HasForeignKey(d => d.CourseId)
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
            entity.Property(e => e.Score)
                .HasDefaultValue(1)
                .HasColumnName("score");
            entity.Property(e => e.TaskTypeId).HasColumnName("task_type_id");

            entity.HasOne(d => d.Lesson).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.LessonId)
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
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Task).WithMany(p => p.TaskAnswers)
                .HasForeignKey(d => d.TaskId)
                .HasConstraintName("task_answer_task_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.TaskAnswers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("task_answer_user_id_fkey");
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
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
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

        modelBuilder.Entity<UserHasCourse>(entity =>
        {
            entity.HasKey(e => new { e.CourseId, e.UserId }).HasName("user_has_course_pkey");

            entity.ToTable("user_has_course");

            entity.HasIndex(e => e.UserId, "ix_user_has_course_user_id");

            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");

            entity.HasOne(d => d.Course).WithMany(p => p.UserHasCourses)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_has_course_course_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.UserHasCourses)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("user_has_course_user_id_fkey");
        });

        modelBuilder.Entity<UserHasDoneCourse>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.CourseId }).HasName("user_has_done_course_pkey");

            entity.ToTable("user_has_done_course");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");

            entity.HasOne(d => d.Course).WithMany(p => p.UserHasDoneCourses)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_has_done_course_course_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.UserHasDoneCourses)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("user_has_done_course_user_id_fkey");
        });

        modelBuilder.Entity<UserHasKit>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.KitId }).HasName("user_has_kit_pkey");

            entity.ToTable("user_has_kit");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.KitId).HasColumnName("kit_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");

            entity.HasOne(d => d.Kit).WithMany(p => p.UserHasKits)
                .HasForeignKey(d => d.KitId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_has_kit_kit_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.UserHasKits)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("user_has_kit_user_id_fkey");
        });

        modelBuilder.Entity<UserInGroup>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.GroupId }).HasName("user_in_group_pkey");

            entity.ToTable("user_in_group");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.JoinStatusId).HasColumnName("join_status_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Group).WithMany(p => p.UserInGroups)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_in_group_group_id_fkey");

            entity.HasOne(d => d.JoinStatus).WithMany(p => p.UserInGroups)
                .HasForeignKey(d => d.JoinStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_in_group_join_status_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.UserInGroups)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("user_in_group_user_id_fkey");
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
                .HasConstraintName("user_information_user_id_fkey");
        });

        modelBuilder.Entity<UserTrySolveDetail>(entity =>
        {
            entity.HasKey(e => new { e.UserTryId, e.TaskId }).HasName("user_try_solve_detail_pkey");

            entity.ToTable("user_try_solve_detail");

            entity.Property(e => e.UserTryId).HasColumnName("user_try_id");
            entity.Property(e => e.TaskId).HasColumnName("task_id");
            entity.Property(e => e.IsSolved)
                .HasDefaultValue(false)
                .HasColumnName("is_solved");

            entity.HasOne(d => d.Task).WithMany(p => p.UserTrySolveDetails)
                .HasForeignKey(d => d.TaskId)
                .HasConstraintName("user_try_solve_detail_task_id_fkey");

            entity.HasOne(d => d.UserTry).WithMany(p => p.UserTrySolveDetails)
                .HasForeignKey(d => d.UserTryId)
                .HasConstraintName("user_try_solve_detail_user_try_id_fkey");
        });

        modelBuilder.Entity<UserTrySolveTask>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_try_solve_tasks_pkey");

            entity.ToTable("user_try_solve_tasks");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AllSolved)
                .HasDefaultValue(false)
                .HasColumnName("all_solved");
            entity.Property(e => e.DateTime)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date_time");
            entity.Property(e => e.LessonId).HasColumnName("lesson_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Lesson).WithMany(p => p.UserTrySolveTasks)
                .HasForeignKey(d => d.LessonId)
                .HasConstraintName("user_try_solve_tasks_lesson_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.UserTrySolveTasks)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("user_try_solve_tasks_user_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
