using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;

public class RosterAgent(RosterDb db, IChatClient chat, ILogger<RosterAgent> log)
{
    const string Example = "{\"AM\":[1,2,3,4,5],\"PM\":[6,7,8,9],\"Night\":[10,11,12]}";

    public async Task RunAsync(int windowDays, CancellationToken ct)
    {
        var staff = await db.Staff.ToListAsync(ct);
        var start = DateOnly.FromDateTime(DateTime.Today);
        db.Assignments.RemoveRange(db.Assignments);   // regenerate the window each tick
        int attempts = 0; bool fallback = false;

        for (int d = 0; d < windowDays; d++)
        {
            var day = start.AddDays(d);
            Dictionary<Slot, List<int>>? good = null;
            var problems = new List<string>();

            for (int t = 0; t < 3 && good is null; t++)
            {
                attempts++;
                var proposal = await Propose(staff, day, problems, ct);
                problems = proposal is null ? ["Reply was not valid JSON"] : Rules.Validate(proposal, staff);
                if (proposal is not null && problems.Count == 0) good = proposal;
                else log.LogWarning("{Day} try {T} failed: {P}", day, t + 1, string.Join("; ", problems));
            }
            if (good is null) { good = Rules.Greedy(d, staff); fallback = true; }

            foreach (var (slot, ids) in good)
                foreach (var id in ids)
                    db.Assignments.Add(new Assignment { Date = day, Slot = slot, StaffId = id });
        }

        db.Runs.Add(new RosterRun { RanAt = DateTimeOffset.Now, Attempts = attempts, UsedFallback = fallback });
        await db.SaveChangesAsync(ct);
        log.LogInformation("Roster updated. LLM attempts: {A}, fallback used: {F}", attempts, fallback);
    }

    async Task<Dictionary<Slot, List<int>>?> Propose(List<Staff> staff, DateOnly day, List<string> problems, CancellationToken ct)
    {
        var list = string.Join("\n", staff.Select(s => $"{s.Id} | {s.Name} | {s.Skill}"));
        var fix = problems.Count > 0 ? "\nYour last answer broke these rules. Fix them:\n" + string.Join("\n", problems) : "";
        var prompt = $"""
            You are rostering one hospital ward for {day:yyyy-MM-dd}.
            Staff (id | name | skill):
            {list}

            Rules:
            - AM needs exactly 5 staff, PM exactly 4, Night exactly 3.
            - Every shift needs at least one RN or SeniorRN.
            - Night needs at least one SeniorRN.
            - Each person works at most ONE shift today.
            Reply with JSON only, using staff ids, like this: {Example}
            {fix}
            """;
        try
        {
            var r = await chat.GetResponseAsync(prompt,
                new ChatOptions { ResponseFormat = ChatResponseFormat.Json, Temperature = 0.2f }, ct);
            var raw = JsonSerializer.Deserialize<Dictionary<string, List<int>>>(r.Text);
            return raw!.ToDictionary(k => Enum.Parse<Slot>(k.Key, true), k => k.Value);
        }
        catch (Exception e) when (e is JsonException or ArgumentException or NullReferenceException)
        { return null; }
    }
}