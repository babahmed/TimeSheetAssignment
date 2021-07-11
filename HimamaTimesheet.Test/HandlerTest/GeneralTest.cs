
using AspNetCoreHero.Results;
using AutoMapper;
using FluentAssertions;
using HimamaTimesheet.Application.Features.Tracker.Commands.Create;
using HimamaTimesheet.Application.Features.Tracker.Queries.GetById;
using HimamaTimesheet.Application.Features.Tracker.Queries.GetOpen;
using HimamaTimesheet.Application.Interfaces.Repositories;
using HimamaTimesheet.Application.Interfaces.Shared;
using HimamaTimesheet.Application.Mappings;
using HimamaTimesheet.Domain.Entities.Catalog;
using HimamaTimesheet.Infrastructure.DbContexts;
using HimamaTimesheet.Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using PublicWorkflow.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static HimamaTimesheet.Application.Features.Tracker.Commands.Create.UpdateTrackerCommand;

namespace HimamaTimesheet.Test.HandlerTest
{
    public class GeneralTests
    {
        public GeneralTests()
        {
            string databaseName = Guid.NewGuid().ToString();

            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase(databaseName)
                    .Options;

            Dictionary<string, string> myConfiguration = new Dictionary<string, string>
                        {
                            {"Key1", "Value1"},
                            {"Nested:Key1", "NestedValue1"},
                            {"Nested:Key2", "NestedValue2"}
                        };

            var userId = Guid.NewGuid().ToString();

            var mock = new Mock<IAuthenticatedUserService>();
            mock.Setup(m => m.UserId).Returns(userId);

            var mockDatetime = new Mock<IDateTimeService>();
            mockDatetime.Setup(m => m.NowUtc).Returns(DateTime.UtcNow);

            _config = new ConfigurationBuilder()
                    .AddInMemoryCollection(myConfiguration)
                    .Build();

            _context = new ApplicationDbContext(_dbContextOptions, mockDatetime.Object, mock.Object);
            _uow = new UnitOfWork(_context, mock.Object);

            _timesheet = new GenericRepository<Timesheet>(_context, mock.Object);

            _mapper = new MapperConfiguration(cfg =>
                       cfg.AddProfile<TimeSheetProfile>())
                .CreateMapper();
        }

        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
        protected readonly IGenericRepository<Timesheet> _timesheet;
        protected readonly ApplicationDbContext _context;
        protected readonly UnitOfWork _uow;
        protected readonly IConfiguration _config;
        protected readonly IMapper _mapper;


        [Fact]
        public async Task CreatetTimeSheetAsync()
        {
            //Arange
            var mediator = new Mock<IMediator>();

            mediator.Setup(m => m.Send(It.IsAny<GetOpenTrackerQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<GetTrackerByIdResponse>.Fail());

            CreateTrackerCommand command = Mock.Mock.CreateTrackerFaker(Guid.NewGuid().ToString());
            CreateTrackerCommandHandler handler = new CreateTrackerCommandHandler(_timesheet, _uow, _mapper, mediator.Object);

            //Act
            var response = await handler.Handle(command, new System.Threading.CancellationToken());

            //Asert
            //Do the assertion
            var rec = await _timesheet.GetAsync(x => x.Id == response.Data);
            rec.Should().NotBeNull();

        }
        [Fact]
        public async Task CreatetTimeSheetInvalidAsync()
        {
            //Arange
            var mediator = new Mock<IMediator>();

            mediator.Setup(m => m.Send(It.IsAny<GetOpenTrackerQuery>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(Result<GetTrackerByIdResponse>.Fail());

            CreateTrackerCommand command = Mock.Mock.CreateTrackerFaker(Guid.NewGuid().ToString());
            CreateTrackerCommandHandler handler = new CreateTrackerCommandHandler(_timesheet, _uow, _mapper, mediator.Object);

            //Act
            var response = await handler.Handle(command, new System.Threading.CancellationToken());

            mediator.Setup(m => m.Send(It.IsAny<GetOpenTrackerQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<GetTrackerByIdResponse>.Success(new GetTrackerByIdResponse() { UserId="",TimeIn=DateTime.Now,TimeOut=null}));

            CreateTrackerCommand command2 = Mock.Mock.CreateTrackerFaker(Guid.NewGuid().ToString());
            CreateTrackerCommandHandler handler2 = new CreateTrackerCommandHandler(_timesheet, _uow, _mapper, mediator.Object);

            var response2 = await handler2.Handle(command2, new System.Threading.CancellationToken());

            //Asert
            //Do the assertion
            var rec = await _timesheet.GetAllAsync(x => x.UserId == command.UserId);
            rec.Count().Should().Be(1);
        }
        [Fact]
        public async Task UpdateTimeSheetAsync()
        {
            //Arange
            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.Send(It.IsAny<GetOpenTrackerQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<GetTrackerByIdResponse>.Fail());

            CreateTrackerCommand command = Mock.Mock.CreateTrackerFaker(Guid.NewGuid().ToString());
            CreateTrackerCommandHandler handler = new CreateTrackerCommandHandler(_timesheet, _uow, _mapper, mediator.Object);


            var createResponse = await handler.Handle(command, new System.Threading.CancellationToken());

            UpdateTrackerCommand UpdateCommand = Mock.Mock.UpdateTrackerFaker(createResponse.Data);
            UpdateCommand.TimeIn = DateTime.Now.AddDays(0.5);
            UpdateTrackerCommandHandler Updatehandler = new UpdateTrackerCommandHandler(_timesheet, _uow);

            var UpdateResponse = await Updatehandler.Handle(UpdateCommand, new System.Threading.CancellationToken());

            //Asert
            //Do the assertion
            var rec = await _timesheet.GetAsync(x => x.Id == createResponse.Data);
            rec.TimeIn.Should().Be(UpdateCommand.TimeIn.Value);
        }
        [Fact]
        public async Task UpdateTimeSheetInvalidAsync()
        {
            //Arange
            var mediator = new Mock<IMediator>();

            UpdateTrackerCommand UpdateCommand = Mock.Mock.UpdateTrackerFaker(2);
            UpdateCommand.TimeIn = DateTime.Now.AddDays(0.5);
            UpdateTrackerCommandHandler Updatehandler = new UpdateTrackerCommandHandler(_timesheet, _uow);

            var UpdateResponse = await Updatehandler.Handle(UpdateCommand, new System.Threading.CancellationToken());

            //Asert
            //Do the assertion
            var rec = await _timesheet.GetAsync(x => x.Id == UpdateResponse.Data);
            rec.Should().BeNull();
        }
        [Fact]
        public void GetTimesheetById()
        {

        }
        [Fact]
        public void GetInvalidTimesheetById()
        {

        }
        [Fact]
        public void FetchTimesheetById()
        {

        }
    }

}