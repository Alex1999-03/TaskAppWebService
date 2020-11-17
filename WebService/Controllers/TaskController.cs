using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebService.Models;
using WebService.Models.Request;
using WebService.Models.Response;
using WebService.Services;

namespace WebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet("tasks-uncompleted")]
        public async Task<ActionResult> GetAllTaskUncompleted()
        {
            DataResponse response = new DataResponse();
            try
            {
                var tasks = await _taskService.GetAllUncompleted();
                response.Data = tasks;
                response.Success = 1;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return Ok(response);
        }

        [HttpGet("tasks-completed")]
        public async Task<ActionResult> GetAllTaskCompleted()
        {
            DataResponse response = new DataResponse();
            try
            {
                var tasks = await _taskService.GetAllCompleted();
                response.Data = tasks;
                response.Success = 1;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult> AddTask(TaskRequest model)
        {
            DataResponse response = new DataResponse();
            try
            {
                var task = await _taskService.Add(model);
                response.Data = task;
                response.Success = 1;
            }
            catch (Exception ex)
            {

                response.Message = ex.Message;
            }
            return Ok(response);
        }

        [HttpPut]
        public async Task<ActionResult> EditTask(TaskRequest model)
        {
            DataResponse response = new DataResponse();
            try
            {
                var task = await _taskService.Edit(model);
                response.Data = task;
                response.Success = 1;
            }
            catch (Exception ex)
            {

                response.Message = ex.Message;
            }

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTask(int id)
        {
            DataResponse response = new DataResponse();
            try
            {
                var task = await _taskService.Delete(id);
                response.Data = task;
                response.Success = 1;
            }
            catch (Exception ex)
            {

                response.Message = ex.Message;
            }
            return Ok(response);
        }
 
       
    }
}
