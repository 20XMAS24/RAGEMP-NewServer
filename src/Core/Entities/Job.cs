namespace Server.Core.Entities;

public class Job : BaseEntity
{
    /// <summary>Job name (taxi driver, mechanic, etc.)</summary>
    public required string Name { get; set; }

    /// <summary>Job description</summary>
    public string? Description { get; set; }

    /// <summary>Base salary per hour</summary>
    public long BaseSalary { get; set; }

    /// <summary>Required level to apply</summary>
    public int RequiredLevel { get; set; }

    /// <summary>Job color in HEX</summary>
    public string Color { get; set; } = "#FFFFFF";

    /// <summary>Is job available for hire</summary>
    public bool IsActive { get; set; } = true;
}