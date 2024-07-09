//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using System.Text.Json.Serialization;

namespace D20Tek.LowDb.UnitTests.Entities;

internal class TestDocument
{
    public int LastId { get; set; } = 0;

    public string Version { get; set; } = "1.0";

    [JsonPropertyName("tests")]
    public HashSet<TestEntity> Entities { get; init; } = [];

    public int GetNextId() => ++LastId;
}
