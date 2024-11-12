using Scalar.AspNetCore;

namespace KubeCon.Sk.Debate.Host.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication MapDebateControllerEndpoints(this WebApplication app)
    {
        app.MapGet("start", (DebateEngineStateController debateEngineController) => debateEngineController.Start())
           .WithOpenApi(operation =>
           {
               operation.Summary = "Starts the Debate Loop";
               operation.OperationId = "startDebateLoop";
               return operation;
           });

        app.MapGet("stop", (DebateEngineStateController debateEngineController) => debateEngineController.Stop())
           .WithOpenApi(operation =>
           {
               operation.Summary = "Stops the Debate Loop";
               operation.OperationId = "stopDebateLoop";
               return operation;
           });

        app.MapPost("setDebateLoopDelay", (DebateEngineStateController debateEngineController, int debateLoopDelay) => debateEngineController.SetDebateLoopDelayInterval(debateLoopDelay))
           .WithOpenApi(operation =>
           {
               operation.Summary = "Set Debate Loop delay";
               operation.Description = "Sets the number of milliseconds between debate loop iterations. A lower number means the debates will be played quicker.";
               operation.OperationId = "setDebateLoopDelay";
               return operation;
           });

        app.MapPost("kickAgent", (DebateEngineStateController debateEngineController, string agentName) => debateEngineController.KickAgent(agentName))
           .WithOpenApi(operation =>
           {
               operation.Summary = "Kick agent";
               operation.Description = "Kick an individual agent out of the debate loop, but does not terminate the agent process or ban them from re-joining.";
               operation.OperationId = "kickAgent";
               return operation;
           });

        app.MapPost("unkickAgent", (DebateEngineStateController debateEngineController, string agentName) => debateEngineController.UnkickAgent(agentName))
           .WithOpenApi(operation =>
           {
               operation.Summary = "Unkick agent";
               operation.Description = "Let an individual agent into the debate loop.";
               operation.OperationId = "unkickAgent";
               return operation;
           });

        app.MapPost("addTopic", (DebateEngineStateController debateEngineController, string topic) => debateEngineController.AddTopic(topic))
           .WithOpenApi(operation =>
           {
               operation.Summary = "Add user topic";
               operation.Description = "Add a user supplied debate topic.";
               operation.OperationId = "addTopic";
               return operation;
           });

        return app;
    }
    public static WebApplication MapOpenApiDuringDevelopment(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        return app;
    }
}