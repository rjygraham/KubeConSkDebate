﻿using System.ClientModel.Primitives;

namespace KubeCon.Sk.Debate.Infrastructure.PipelinePolicies;

/// <summary>
/// Bug fix: Remove duplicate Authorization headers from the request.
/// See https://github.com/Azure/azure-sdk-for-net/issues/46109
/// </summary>
public sealed class SingleAuthorizationHeaderPolicy : PipelinePolicy
{
    public override void Process(PipelineMessage message, IReadOnlyList<PipelinePolicy> pipeline, int currentIndex)
    {
        RemoveDuplicateHeader(message.Request.Headers);
        ProcessNext(message, pipeline, currentIndex);
    }

    public override async ValueTask ProcessAsync(PipelineMessage message, IReadOnlyList<PipelinePolicy> pipeline, int currentIndex)
    {
        RemoveDuplicateHeader(message.Request.Headers);
        await ProcessNextAsync(message, pipeline, currentIndex).ConfigureAwait(false);
    }

    private static void RemoveDuplicateHeader(PipelineRequestHeaders headers)
    {
        if (!headers.TryGetValues("Authorization", out var headerValues) || headerValues == null)
        {
            return;
        }

        using var enumerator = headerValues.GetEnumerator();

        if (!enumerator.MoveNext()) { return; }

        var firstValue = enumerator.Current;

        // Check if there’s more than one value
        if (enumerator.MoveNext())
        {
            headers.Set("Authorization", firstValue);
        }
    }
}