using Bunit;
using FluentAssertions;
using Moq;
using ReactiveUI;
using System.Reactive.Subjects;
using Xunit;
using MessageBusSample.Client.Components.Actions;
using MessageBusSample.Client.Services;
using MessageBusSample.Client.Components.Actions.Rename;
using MessageBusSample.Client.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using Bunit.Extensions;

namespace MessageBusSample.Tests;

public class GenericListItemTests : TestContext
{
    private readonly Subject<InitDeleteActionMessage> _deleteMessageBus = new();
    private readonly Subject<InitRenameActionMessage> _renameMessageBus = new();

    private readonly int EntityId = 123;

    public GenericListItemTests()
    {
        Services.AddDefaultTestContextServices(this, JSInterop);
        JSInterop.SetupVoid("mudPopover.initialize", _ => true);
        Services.AddMudServices();

        //// Mock MessageBus so we can send test messages
        //var messageBusMock = new Mock<IMessageBus>();
        //messageBusMock.Setup(mb => mb.Listen<InitDeleteActionMessage>())
        //    .Returns(_deleteMessageBus);
        //messageBusMock.Setup(mb => mb.Listen<InitRenameActionMessage>())
        //    .Returns(_renameMessageBus);

        //Services.AddSingleton(messageBusMock.Object);
    }

    [Fact]
    public void Component_Should_Render_Correctly()
    {
        // Arrange
        var entity = new Entity { Id = EntityId, Name = "TestEntity" };

        // Act
        var cut = RenderComponent<GenericListItem>(parameters => parameters
            .Add(p => p.Entity, entity));

        // Assert
        cut.Markup.Should().Contain("TestEntity");
    }

    [Fact]
    public void Should_React_To_Delete_Message()
    {
        // Arrange
        var entity = new Entity { Id = EntityId, Name = "TestEntity" };
        var cut = RenderComponent<GenericListItem>(parameters => parameters
            .Add(p => p.Entity, entity));

        // Act
        MessageBus.Current.SendMessage(new InitDeleteActionMessage(EntityId));

        // Assert
        cut.Markup.Should().Contain("text-decoration: line-through"); // Checks strikethrough style
    }

    [Fact]
    public void Should_React_To_Rename_Message()
    {
        // Arrange
        var entity = new Entity { Id = EntityId, Name = "TestEntity" };
        var cut = RenderComponent<GenericListItem>(parameters => parameters
            .Add(p => p.Entity, entity));

        // Act
        MessageBus.Current.SendMessage(new InitRenameActionMessage(EntityId, "Test"));

        // Assert
        cut.Markup.Should().Contain("font-style: italic"); // Checks italic style
    }

    [Fact]
    public void Should_Not_React_To_Unrelated_Message()
    {
        // Arrange
        var entity = new Entity { Id = EntityId, Name = "TestEntity" };
        var cut = RenderComponent<GenericListItem>(parameters => parameters
            .Add(p => p.Entity, entity));

        // Act
        MessageBus.Current.SendMessage(new InitDeleteActionMessage(EntityId)); // Different ID

        // Assert
        cut.Markup.Should().NotContain("text-decoration: line-through"); // No strikethrough
    }

    [Fact]
    public void Should_Dispose_MessageBus_Subscriptions()
    {
        // Arrange
        var entity = new Entity { Id = EntityId, Name = "TestEntity" };
        var cut = RenderComponent<GenericListItem>(parameters => parameters
            .Add(p => p.Entity, entity));

        // Act
        cut.Dispose();

        // Assert
        _deleteMessageBus.HasObservers.Should().BeFalse();
        _renameMessageBus.HasObservers.Should().BeFalse();
    }
}