using Server.Core.Entities;
using Server.Infrastructure.Persistence;

namespace Server.Application.Services;

public interface IJobService
{
    Task<Job?> GetJobAsync(uint id);
    Task<Job?> GetJobByNameAsync(string name);
    Task<List<Job>> GetAllJobsAsync();
    Task<Job> CreateJobAsync(string name, long baseSalary, int requiredLevel = 1);
    Task CreateJobIfNotExistsAsync(string name, long baseSalary, int requiredLevel = 1);
    Task<bool> UpdateJobAsync(uint id, string? description = null, long? baseSalary = null);
    Task<bool> DeleteJobAsync(uint id);
}

public class JobService : IJobService
{
    private readonly IUnitOfWork _unitOfWork;

    public JobService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Job?> GetJobAsync(uint id)
    {
        return await _unitOfWork.Jobs.GetByIdAsync(id);
    }

    public async Task<Job?> GetJobByNameAsync(string name)
    {
        return await _unitOfWork.Jobs.FirstOrDefaultAsync(j => j.Name == name);
    }

    public async Task<List<Job>> GetAllJobsAsync()
    {
        return await _unitOfWork.Jobs.GetAllAsync();
    }

    public async Task<Job> CreateJobAsync(string name, long baseSalary, int requiredLevel = 1)
    {
        var existingJob = await _unitOfWork.Jobs.FirstOrDefaultAsync(j => j.Name == name);
        if (existingJob != null)
            throw new InvalidOperationException($"Job '{name}' already exists");

        var job = new Job
        {
            Name = name,
            BaseSalary = baseSalary,
            RequiredLevel = requiredLevel,
            IsActive = true
        };

        await _unitOfWork.Jobs.AddAsync(job);
        await _unitOfWork.SaveChangesAsync();

        return job;
    }

    public async Task CreateJobIfNotExistsAsync(string name, long baseSalary, int requiredLevel = 1)
    {
        var existingJob = await _unitOfWork.Jobs.FirstOrDefaultAsync(j => j.Name == name);
        if (existingJob != null)
            return;

        await CreateJobAsync(name, baseSalary, requiredLevel);
    }

    public async Task<bool> UpdateJobAsync(uint id, string? description = null, long? baseSalary = null)
    {
        var job = await _unitOfWork.Jobs.GetByIdAsync(id) ?? throw new KeyNotFoundException();

        if (description != null)
            job.Description = description;

        if (baseSalary.HasValue)
            job.BaseSalary = baseSalary.Value;

        _unitOfWork.Jobs.Update(job);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteJobAsync(uint id)
    {
        var job = await _unitOfWork.Jobs.GetByIdAsync(id) ?? throw new KeyNotFoundException();
        _unitOfWork.Jobs.Remove(job);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}