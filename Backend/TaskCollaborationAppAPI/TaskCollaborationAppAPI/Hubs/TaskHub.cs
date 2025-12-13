using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using TaskCollaborationAppAPI.Models;
using TaskCollaborationAppAPI.Repositories;

namespace TaskCollaborationAppAPI.Hubs
{
    [Authorize]
    public class TaskHub : Hub
    {
    }
}
