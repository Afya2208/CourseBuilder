using System;
using System.Collections.Generic;

namespace Models.Entities;

public partial class User
{
    public long Id { get; set; }

    public string Email { get; set; } = null!;

    public byte[] Password { get; set; } = null!;

    public int RoleId { get; set; }

    public byte[] Salt { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<AdditionalTryForUser> AdditionalTryForUsers { get; set; } = new List<AdditionalTryForUser>();

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    public virtual ICollection<FeedbackSubmit> FeedbackSubmits { get; set; } = new List<FeedbackSubmit>();

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    public virtual ICollection<Kit> Kits { get; set; } = new List<Kit>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<TaskAnswer> TaskAnswers { get; set; } = new List<TaskAnswer>();

    public virtual ICollection<UserHasCourse> UserHasCourses { get; set; } = new List<UserHasCourse>();

    public virtual ICollection<UserHasDoneCourse> UserHasDoneCourses { get; set; } = new List<UserHasDoneCourse>();

    public virtual ICollection<UserHasKit> UserHasKits { get; set; } = new List<UserHasKit>();

    public virtual ICollection<UserInGroup> UserInGroups { get; set; } = new List<UserInGroup>();

    public virtual UserInformation? UserInformation { get; set; }

    public virtual ICollection<UserTrySolveTask> UserTrySolveTasks { get; set; } = new List<UserTrySolveTask>();
}
