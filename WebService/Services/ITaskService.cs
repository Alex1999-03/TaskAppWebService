using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebService.Models;
using WebService.Models.Request;
using WebService.Models.Response;
using Tasks = WebService.Models.Task;

namespace WebService.Services
{
    public interface ITaskService
    {
        Task<List<Tasks>> GetAllUncompleted();
        Task<List<Tasks>> GetAllCompleted();
        Task<Tasks> Add(TaskRequest model);

        Task<Tasks> Edit(TaskRequest model);

        Task<Tasks> Delete(int id);
    }

    public class TaskService : ITaskService
    {
        private readonly TaskAppDBContext _context;

        public TaskService(TaskAppDBContext context)
        {
            _context = context;
        }

        public async Task<Tasks> Add(TaskRequest model)
        {
            Tasks task = new Tasks();
            task.Description = model.Description;
            task.Date = model.Date;
            task.Completed = false;
            _context.Task.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<Tasks> Edit(TaskRequest model)
        {
            Tasks task = new Tasks();
            task = _context.Task.Find(model.Id);
            task.Completed = model.Completed;
            _context.Entry(task).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<Tasks> Delete(int id)
        {
            Tasks task = new Tasks();
            task = _context.Task.Find(id);
            _context.Remove(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<List<Tasks>> GetAllUncompleted()
        {
            return await _context.Task.OrderBy(t => t.Date).Where(x => x.Completed == false).ToListAsync();
        }

        public async Task<List<Tasks>> GetAllCompleted()
        {
            return await _context.Task.OrderBy(t => t.Date).Where(x => x.Completed == true).ToListAsync();
        }
    }
}
