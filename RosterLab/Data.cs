using Microsoft.EntityFrameworkCore;

public enum Skill { 
    SeniorRN, 
    RN, 
    EN, 
    HCA 
}
public enum Slot { 
    AM, 
    PM, 
    Night 
}

public class Staff { 
    public int Id { get; set; } 
    public string Name { get; set; } = ""; 
    public Skill Skill { get; set; } }
public class Assignment { 
    public int Id { get; set; } 
    public DateOnly Date { get; set; } 
    public Slot Slot { get; set; } 
    public int StaffId { get; set; } 
}
public class RosterRun { 
    public int Id { get; set; } 
    public DateTimeOffset RanAt { get; set; } 
    public int Attempts { get; set; } 
    public bool UsedFallback { get; set; } 
}

public class RosterDb(DbContextOptions<RosterDb> o) : DbContext(o)
{
    public DbSet<Staff> Staff => Set<Staff>();
    public DbSet<Assignment> Assignments => Set<Assignment>();
    public DbSet<RosterRun> Runs => Set<RosterRun>();
}

public static class Seed
{
    public static void Run(RosterDb db)
    {
        if (db.Staff.Any()) return;
        string[] names = ["Ana","Ben","Cora","Dan","Eli","Fay","Gus","Hana","Ivy","Jon","Kai","Lea","Max","Nia","Omar","Pia","Quin","Rae"];
        Skill[] skills = [Skill.SeniorRN,Skill.SeniorRN,Skill.SeniorRN,
                          Skill.RN,Skill.RN,Skill.RN,Skill.RN,Skill.RN,Skill.RN,
                          Skill.EN,Skill.EN,Skill.EN,Skill.EN,Skill.EN,
                          Skill.HCA,Skill.HCA,Skill.HCA,Skill.HCA];
        for (int i = 0; i < names.Length; i++)
            db.Staff.Add(new Staff { Name = names[i], Skill = skills[i] });
        db.SaveChanges();
    }
}