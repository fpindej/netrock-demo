using System.Net;
using System.Net.Http.Json;
using Netrock.Api.Tests.Contracts;
using Netrock.Api.Tests.Fixtures;
using Netrock.Application.Features.Jobs.Dtos;
using Netrock.Application.Identity.Constants;
using Netrock.Shared;
using JobExecutionDetailResponse = Netrock.Api.Tests.Contracts.JobExecutionDetailResponse;
using ListExecutionsResponse = Netrock.Api.Tests.Contracts.ListExecutionsResponse;

namespace Netrock.Api.Tests.Controllers;

public class JobsControllerTests : IClassFixture<CustomWebApplicationFactory>, IDisposable
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public JobsControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _factory.ResetMocks();
        _client = factory.CreateClient();
    }

    public void Dispose() => _client.Dispose();

    private HttpRequestMessage Get(string url, string auth) =>
        new(HttpMethod.Get, url) { Headers = { { "Authorization", auth } } };

    private HttpRequestMessage Post(string url, string auth) =>
        new(HttpMethod.Post, url) { Headers = { { "Authorization", auth } } };

    private HttpRequestMessage Delete(string url, string auth) =>
        new(HttpMethod.Delete, url) { Headers = { { "Authorization", auth } } };

    #region ListJobs

    [Fact]
    public async Task ListJobs_WithPermission_Returns200()
    {
        _factory.JobManagementService.GetRecurringJobsAsync()
            .Returns(new List<RecurringJobOutput>
            {
                new("cleanup-job", "0 0 * * *", null, null, null, false, null)
            });

        var response = await _client.SendAsync(
            Get("/api/v1/admin/jobs", TestAuth.WithPermissions(AppPermissions.Jobs.View)));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<List<RecurringJobResponse>>();
        Assert.NotNull(body);
        Assert.Single(body);
        Assert.Equal("cleanup-job", body[0].Id);
        Assert.Equal("0 0 * * *", body[0].Cron);
    }

    [Fact]
    public async Task ListJobs_WithoutPermission_Returns403()
    {
        var response = await _client.SendAsync(
            Get("/api/v1/admin/jobs", TestAuth.User()));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task ListJobs_Unauthenticated_Returns401()
    {
        using var anonClient = _factory.CreateClient();

        var response = await anonClient.GetAsync("/api/v1/admin/jobs");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region GetJob

    [Fact]
    public async Task GetJob_WithPermission_Returns200()
    {
        _factory.JobManagementService.GetRecurringJobDetailAsync("test-job")
            .Returns(Result<RecurringJobDetailOutput>.Success(
                new RecurringJobDetailOutput("test-job", "0 * * * *", null, null, null, false, null, [])));

        var response = await _client.SendAsync(
            Get("/api/v1/admin/jobs/test-job", TestAuth.WithPermissions(AppPermissions.Jobs.View)));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<RecurringJobDetailResponse>();
        Assert.NotNull(body);
        Assert.Equal("test-job", body.Id);
        Assert.Equal("0 * * * *", body.Cron);
    }

    [Fact]
    public async Task GetJob_NotFound_Returns404()
    {
        _factory.JobManagementService.GetRecurringJobDetailAsync("nonexistent")
            .Returns(Result<RecurringJobDetailOutput>.Failure(ErrorMessages.Jobs.NotFound, ErrorType.NotFound));

        var response = await _client.SendAsync(
            Get("/api/v1/admin/jobs/nonexistent", TestAuth.WithPermissions(AppPermissions.Jobs.View)));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region TriggerJob

    [Fact]
    public async Task TriggerJob_WithPermission_Returns204()
    {
        _factory.JobManagementService.TriggerJobAsync("test-job")
            .Returns(Result.Success());

        var response = await _client.SendAsync(
            Post("/api/v1/admin/jobs/test-job/trigger", TestAuth.WithPermissions(AppPermissions.Jobs.Manage)));

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task TriggerJob_WithoutPermission_Returns403()
    {
        var response = await _client.SendAsync(
            Post("/api/v1/admin/jobs/test-job/trigger", TestAuth.WithPermissions(AppPermissions.Jobs.View)));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    #endregion

    #region RemoveJob

    [Fact]
    public async Task RemoveJob_WithPermission_Returns204()
    {
        _factory.JobManagementService.RemoveJobAsync("test-job")
            .Returns(Result.Success());

        var response = await _client.SendAsync(
            Delete("/api/v1/admin/jobs/test-job", TestAuth.WithPermissions(AppPermissions.Jobs.Manage)));

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    #endregion

    #region PauseJob

    [Fact]
    public async Task PauseJob_WithPermission_Returns204()
    {
        _factory.JobManagementService.PauseJobAsync("test-job")
            .Returns(Result.Success());

        var response = await _client.SendAsync(
            Post("/api/v1/admin/jobs/test-job/pause", TestAuth.WithPermissions(AppPermissions.Jobs.Manage)));

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    #endregion

    #region ResumeJob

    [Fact]
    public async Task ResumeJob_WithPermission_Returns204()
    {
        _factory.JobManagementService.ResumeJobAsync("test-job")
            .Returns(Result.Success());

        var response = await _client.SendAsync(
            Post("/api/v1/admin/jobs/test-job/resume", TestAuth.WithPermissions(AppPermissions.Jobs.Manage)));

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    #endregion

    #region RestoreJobs

    [Fact]
    public async Task RestoreJobs_WithPermission_Returns204()
    {
        _factory.JobManagementService.RestoreJobsAsync()
            .Returns(Result.Success());

        var response = await _client.SendAsync(
            Post("/api/v1/admin/jobs/restore", TestAuth.WithPermissions(AppPermissions.Jobs.Manage)));

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task RestoreJobs_Failure_Returns400()
    {
        _factory.JobManagementService.RestoreJobsAsync()
            .Returns(Result.Failure(ErrorMessages.Jobs.RestoreFailed));

        var response = await _client.SendAsync(
            Post("/api/v1/admin/jobs/restore", TestAuth.WithPermissions(AppPermissions.Jobs.Manage)));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task RestoreJobs_WithoutPermission_Returns403()
    {
        var response = await _client.SendAsync(
            Post("/api/v1/admin/jobs/restore", TestAuth.User()));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    #endregion

    #region ListExecutions

    [Fact]
    public async Task ListExecutions_WithPermission_Returns200()
    {
        var executionId = Guid.NewGuid();
        _factory.JobExecutionService.GetExecutionsAsync("test-job", 1, 10, null, Arg.Any<CancellationToken>())
            .Returns(new JobExecutionListOutput(
                [new JobExecutionSummaryOutput(executionId, "test-job", "Succeeded",
                    DateTimeOffset.UtcNow, DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5), null, "Schedule")],
                1, 1, 10));

        var response = await _client.SendAsync(
            Get("/api/v1/admin/jobs/test-job/executions?pageNumber=1&pageSize=10",
                TestAuth.WithPermissions(AppPermissions.Jobs.View)));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ListExecutionsResponse>();
        Assert.NotNull(body);
        Assert.Single(body.Items);
        Assert.Equal(1, body.TotalCount);
    }

    [Fact]
    public async Task ListExecutions_WithoutPermission_Returns403()
    {
        var response = await _client.SendAsync(
            Get("/api/v1/admin/jobs/test-job/executions", TestAuth.User()));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    #endregion

    #region GetExecutionDetail

    [Fact]
    public async Task GetExecutionDetail_WithPermission_Returns200()
    {
        var executionId = Guid.NewGuid();
        _factory.JobExecutionService.GetExecutionDetailAsync(executionId, Arg.Any<CancellationToken>())
            .Returns(Result<JobExecutionDetailOutput>.Success(
                new JobExecutionDetailOutput(executionId, "test-job", null, "Succeeded",
                    DateTimeOffset.UtcNow, DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5), null, "Schedule", [])));

        var response = await _client.SendAsync(
            Get($"/api/v1/admin/jobs/executions/{executionId}",
                TestAuth.WithPermissions(AppPermissions.Jobs.View)));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JobExecutionDetailResponse>();
        Assert.NotNull(body);
        Assert.Equal(executionId, body.Id);
        Assert.Equal("Succeeded", body.Status);
    }

    [Fact]
    public async Task GetExecutionDetail_NotFound_Returns404()
    {
        var executionId = Guid.NewGuid();
        _factory.JobExecutionService.GetExecutionDetailAsync(executionId, Arg.Any<CancellationToken>())
            .Returns(Result<JobExecutionDetailOutput>.Failure(ErrorMessages.Jobs.ExecutionNotFound, ErrorType.NotFound));

        var response = await _client.SendAsync(
            Get($"/api/v1/admin/jobs/executions/{executionId}",
                TestAuth.WithPermissions(AppPermissions.Jobs.View)));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetExecutionDetail_WithoutPermission_Returns403()
    {
        var executionId = Guid.NewGuid();
        var response = await _client.SendAsync(
            Get($"/api/v1/admin/jobs/executions/{executionId}", TestAuth.User()));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    #endregion
}
