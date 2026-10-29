public static class Rules
{
    public static readonly Dictionary<Slot, int> Capacity = new() { 
        [Slot.AM] = 5, [Slot.PM] = 4, [Slot.Night] = 3 
    };
    static bool IsRn(Staff s) => s.Skill is Skill.SeniorRN or Skill.RN;

    public static List<string> Validate(Dictionary<Slot, List<int>> plan, List<Staff> staff)
    {
        var v = new List<string>();
        var byId = staff.ToDictionary(s => s.Id);
        foreach (var (slot, cap) in Capacity)
        {
            var ids = plan.GetValueOrDefault(slot) ?? [];
            if (ids.Count != cap) v.Add($"{slot} has {ids.Count} staff, needs exactly {cap}");
            if (ids.Any(i => !byId.ContainsKey(i))) { v.Add($"{slot} has an unknown staff id"); continue; }
            var people = ids.Select(i => byId[i]).ToList();
            if (!people.Any(IsRn)) v.Add($"{slot} needs at least one RN or SeniorRN");
            if (slot == Slot.Night && !people.Any(p => p.Skill == Skill.SeniorRN)) v.Add("Night needs a SeniorRN");
        }
        foreach (var g in plan.Values.SelectMany(x => x).GroupBy(x => x).Where(g => g.Count() > 1))
            v.Add($"Staff {g.Key} is on more than one shift today");
        return v;
    }

    // Backup plan: no AI, always valid
    public static Dictionary<Slot, List<int>> Greedy(int dayIndex, List<Staff> staff)
    {
        var pool = staff.OrderBy(s => (s.Id + dayIndex * 5) % staff.Count).ToList();
        var plan = new Dictionary<Slot, List<int>>();
        foreach (var slot in new[] { Slot.Night, Slot.PM, Slot.AM })
        {
            var pick = new List<Staff>();
            void Take(Func<Staff, bool> f) { var s = pool.First(f); pool.Remove(s); pick.Add(s); }
            if (slot == Slot.Night) Take(p => p.Skill == Skill.SeniorRN);
            if (!pick.Any(IsRn)) Take(IsRn);
            while (pick.Count < Capacity[slot]) Take(_ => true);
            plan[slot] = pick.Select(p => p.Id).ToList();
        }
        return plan;
    }
}