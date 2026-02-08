using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace AgentFramework.ChatWithPlugin.Controllers;

[ApiController]
[Route("api/agent/[controller]")]
public class LocationFinderController(
    [FromKeyedServices("location_finder")] AIAgent locationFinderAgent,
    [FromKeyedServices("location_finder")] AgentSessionStore sessionStore) : ControllerBase
{
    [HttpPost("find-location")]
    public async Task<IActionResult> FindLocation([FromBody] string eventDetails)
    {
        // In a real-world scenario conversationId should be a unique conversation id tied to the current user.
        AgentSession session = await sessionStore.GetSessionAsync(
            agent: locationFinderAgent,
            conversationId: locationFinderAgent.Id);

        AgentResponse response = await locationFinderAgent.RunAsync(
            message: eventDetails,
            session: session);

        await sessionStore.SaveSessionAsync(
            agent: locationFinderAgent,
            conversationId: locationFinderAgent.Id,
            session: session);

        return Ok(response.Text);
    }
}