//using Xunit;
//using Moq;
//using DLForum.Service;
//using DLForum.Models;
//using DLForum.Models.Topic;
//using DLForum.Controllers;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Configuration;
//using System.Security.Claims;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using System.Linq.Expressions;
//using System;
//using System.Linq;

//namespace DLForum.Tests
//{
//    public class ForumTests
//    {
//        // Тесты для CommentService
//        public class CommentServiceTests
//        {
//            private readonly Mock<ISupabaseClientService> _mockClientService;
//            private readonly CommentService _commentService;

//            public CommentServiceTests()
//            {
//                _mockClientService = new Mock<ISupabaseClientService>();
//                _commentService = new CommentService(_mockClientService.Object);
//            }

//            [Fact]
//            public async Task AddCommentAsync_ValidData_ReturnsComment()
//            {
//                // Arrange
//                var userId = 1;
//                var topicId = 1;
//                var content = "Test comment";
//                var expectedComment = new createcomments
//                {
//                    id_users = userId,
//                    id_topics = topicId,
//                    comments = content
//                };

//                _mockClientService.Setup(x => x.AddCommentAsync(userId, topicId, content))
//                    .ReturnsAsync(expectedComment);

//                // Act
//                var result = await _commentService.AddCommentAsync(userId, topicId, content);

//                // Assert
//                Assert.NotNull(result);
//                Assert.Equal(userId, result.id_users);
//                Assert.Equal(topicId, result.id_topics);
//                Assert.Equal(content, result.comments);
//            }

//            [Fact]
//            public async Task GetCommentsByTopicIdAsync_ValidTopicId_ReturnsComments()
//            {
//                // Arrange
//                var topicId = 1;
//                var expectedComments = new List<comment>
//                {
//                    new comment { id = 1, comments = "Test comment 1" },
//                    new comment { id = 2, comments = "Test comment 2" }
//                };

//                _mockClientService.Setup(x => x.GetCommentsByTopicIdAsync(topicId))
//                    .ReturnsAsync(expectedComments);

//                // Act
//                var result = await _commentService.GetCommentsByTopicIdAsync(topicId);

//                // Assert
//                Assert.NotNull(result);
//                Assert.Equal(expectedComments.Count, result.Count);
//                Assert.Equal(expectedComments[0].comments, result[0].comments);
//            }
//        }

//        // Тесты для DetailsService
//        public class DetailsServiceTests
//        {
//            private readonly Mock<ISupabaseClientService> _mockClientService;
//            private readonly DetailsService _detailsService;

//            public DetailsServiceTests()
//            {
//                _mockClientService = new Mock<ISupabaseClientService>();
//                _detailsService = new DetailsService(_mockClientService.Object);
//            }

//            [Fact]
//            public async Task GetTopicByIdAsync_ValidId_ReturnsTopic()
//            {
//                // Arrange
//                var topicId = 1;
//                var expectedTopic = new Topic
//                {
//                    Id = topicId,
//                    Title = "Test Topic",
//                    Content = "Test Content"
//                };

//                _mockClientService.Setup(x => x.GetTopicByIdAsync(topicId))
//                    .ReturnsAsync(expectedTopic);

//                // Act
//                var result = await _detailsService.GetTopicByIdAsync(topicId);

//                // Assert
//                Assert.NotNull(result);
//                Assert.Equal(topicId, result.Id);
//                Assert.Equal(expectedTopic.Title, result.Title);
//            }

//            [Fact]
//            public async Task UpdateLikesCount_ValidData_UpdatesCount()
//            {
//                // Arrange
//                var topicId = 1;
//                var likeCount = 5;

//                _mockClientService.Setup(x => x.UpdateLikesCount(topicId, likeCount))
//                    .Returns(Task.CompletedTask);

//                // Act & Assert
//                await _detailsService.UpdateLikesCount(topicId, likeCount);
//                _mockClientService.Verify(x => x.UpdateLikesCount(topicId, likeCount), Times.Once);
//            }
//        }

//        // Тесты для FavoriteService
//        public class FavoriteServiceTests
//        {
//            private readonly Mock<ISupabaseClientService> _mockClientService;
//            private readonly FavoriteService _favoriteService;

//            public FavoriteServiceTests()
//            {
//                _mockClientService = new Mock<ISupabaseClientService>();
//                _favoriteService = new FavoriteService(_mockClientService.Object);
//            }

//            [Fact]
//            public async Task IsFavorite_ValidData_ReturnsTrue()
//            {
//                // Arrange
//                var userId = 1;
//                var topicId = 1;
//                var expectedFavorite = new favorite { UserId = userId, TopicId = topicId };

//                _mockClientService.Setup(x => x.IsFavorite(userId, topicId))
//                    .ReturnsAsync(true);

//                // Act
//                var result = await _favoriteService.IsFavorite(userId, topicId);

//                // Assert
//                Assert.True(result);
//            }

//            [Fact]
//            public async Task AddFavorite_ValidData_ReturnsFavorite()
//            {
//                // Arrange
//                var userId = 1;
//                var topicId = 1;
//                var expectedFavorite = new favorite { UserId = userId, TopicId = topicId };

//                _mockClientService.Setup(x => x.AddFavorite(userId, topicId))
//                    .ReturnsAsync(expectedFavorite);

//                // Act
//                var result = await _favoriteService.AddFavorite(userId, topicId);

//                // Assert
//                Assert.NotNull(result);
//                Assert.Equal(userId, result.UserId);
//                Assert.Equal(topicId, result.TopicId);
//            }
//        }

//        // Тесты для NotificationService
//        public class NotificationServiceTests
//        {
//            private readonly Mock<ISupabaseClientService> _mockClientService;
//            private readonly NotificationService _notificationService;

//            public NotificationServiceTests()
//            {
//                _mockClientService = new Mock<ISupabaseClientService>();
//                _notificationService = new NotificationService(_mockClientService.Object);
//            }

//            [Fact]
//            public async Task GetUserNotificationsAsync_ValidUserId_ReturnsNotifications()
//            {
//                // Arrange
//                var userId = 1;
//                var expectedNotifications = new List<notification>
//                {
//                    new notification { id = 1, message = "Test notification 1" },
//                    new notification { id = 2, message = "Test notification 2" }
//                };

//                _mockClientService.Setup(x => x.GetUserNotificationsAsync(userId))
//                    .ReturnsAsync(expectedNotifications);

//                // Act
//                var result = await _notificationService.GetUserNotificationsAsync(userId);

//                // Assert
//                Assert.NotNull(result);
//                Assert.Equal(expectedNotifications.Count, result.Count);
//                Assert.Equal(expectedNotifications[0].message, result[0].message);
//            }

//            [Fact]
//            public async Task AddNotificationAsync_ValidData_ReturnsNotification()
//            {
//                // Arrange
//                var userId = 1;
//                var message = "Test notification";
//                var fromTo = "System";
//                var read = false;
//                var created_at = DateTime.Now;
//                var type = "System";
//                var expectedNotification = new notification
//                {
//                    id_users = userId,
//                    message = message,
//                    from_to = fromTo,
//                    read = read,
//                    created_at = created_at,
//                    type = type
//                };

//                _mockClientService.Setup(x => x.AddNotificationAsync(userId, message, fromTo, read, created_at, type))
//                    .ReturnsAsync(expectedNotification);

//                // Act
//                var result = await _notificationService.AddNotificationAsync(userId, message, fromTo, read, created_at, type);

//                // Assert
//                Assert.NotNull(result);
//                Assert.Equal(userId, result.id_users);
//                Assert.Equal(message, result.message);
//                Assert.Equal(fromTo, result.from_to);
//                Assert.Equal(read, result.read);
//                Assert.Equal(type, result.type);
//            }
//        }

//        // Тесты для DetailsController
//        public class DetailsControllerTests
//        {
//            private readonly Mock<DetailsService> _mockDetailsService;
//            private readonly Mock<CommentService> _mockCommentService;
//            private readonly Mock<ImageService> _mockImageService;
//            private readonly Mock<FavoriteService> _mockFavoriteService;
//            private readonly DetailsController _controller;

//            public DetailsControllerTests()
//            {
//                _mockDetailsService = new Mock<DetailsService>();
//                _mockCommentService = new Mock<CommentService>();
//                _mockImageService = new Mock<ImageService>();
//                _mockFavoriteService = new Mock<FavoriteService>();
//                _controller = new DetailsController(
//                    _mockDetailsService.Object,
//                    _mockCommentService.Object,
//                    _mockImageService.Object,
//                    _mockFavoriteService.Object
//                );
//            }

//            [Fact]
//            public async Task Details_ValidId_ReturnsView()
//            {
//                // Arrange
//                var id = 1;
//                var expectedTopic = new Topic { Id = id, Title = "Test Topic" };
//                var expectedComments = new List<comment>();
//                var expectedImages = new List<images>();

//                _mockDetailsService.Setup(x => x.GetTopicByIdAsync(id))
//                    .ReturnsAsync(expectedTopic);
//                _mockCommentService.Setup(x => x.GetCommentsByTopicIdAsync(id))
//                    .ReturnsAsync(expectedComments);
//                _mockImageService.Setup(x => x.GetImagesByTopicIdAsync(id))
//                    .ReturnsAsync(expectedImages);

//                // Act
//                var result = await _controller.details(id);

//                // Assert
//                var viewResult = Assert.IsType<ViewResult>(result);
//                var model = Assert.IsType<Topic>(viewResult.Model);
//                Assert.Equal(id, model.Id);
//            }

//            [Fact]
//            public async Task CreateComment_ValidData_RedirectsToDetails()
//            {
//                // Arrange
//                var id_topic = 1;
//                var comment = "Test comment";
//                var userId = 1;

//                _mockCommentService.Setup(x => x.AddCommentAsync(userId, id_topic, comment))
//                    .ReturnsAsync(new createcomments());
//                _mockFavoriteService.Setup(x => x.GetCommentCount(id_topic))
//                    .ReturnsAsync(1);

//                // Act
//                var result = await _controller.CreateComment(id_topic, comment);

//                // Assert
//                var redirectResult = Assert.IsType<RedirectToActionResult>(result);
//                Assert.Equal("Details", redirectResult.ActionName);
//                Assert.Equal(id_topic, redirectResult.RouteValues["id"]);
//            }
//        }

//        // Тесты для HomeController
//        public class HomeControllerTests
//        {
//            private readonly Mock<ILogger<HomeController>> _mockLogger;
//            private readonly Mock<ISupabaseClientService> _mockSupabase;
//            private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
//            private readonly Mock<TopicService> _mockTopicService;
//            private readonly HomeController _controller;

//            public HomeControllerTests()
//            {
//                _mockLogger = new Mock<ILogger<HomeController>>();
//                _mockSupabase = new Mock<ISupabaseClientService>();
//                _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
//                _mockTopicService = new Mock<TopicService>();
//                _controller = new HomeController(
//                    _mockLogger.Object,
//                    _mockSupabase.Object,
//                    _mockHttpContextAccessor.Object,
//                    _mockTopicService.Object
//                );
//            }

//            [Fact]
//            public async Task Home_ValidPageNumber_ReturnsPartialView()
//            {
//                // Arrange
//                var pageNumber = 1;
//                var sortOrder = "newest";
//                var expectedTopics = new List<Topic>();
//                var totalCount = 0;

//                _mockTopicService.Setup(x => x.GetTopicsByPageAsync(pageNumber, 10, sortOrder))
//                    .ReturnsAsync((expectedTopics, totalCount));

//                // Act
//                var result = await _controller.Home(pageNumber, sortOrder);

//                // Assert
//                var partialViewResult = Assert.IsType<PartialViewResult>(result);
//                var model = Assert.IsType<List<Topic>>(partialViewResult.Model);
//                Assert.Equal(expectedTopics, model);
//            }

//            [Fact]
//            public async Task Index_ReturnsView()
//            {
//                // Arrange
//                var expectedTopics = new List<Topic>();

//                _mockSupabase.Setup(x => x.GetTopicsAsync())
//                    .ReturnsAsync(expectedTopics);

//                // Act
//                var result = await _controller.Index();

//                // Assert
//                var viewResult = Assert.IsType<ViewResult>(result);
//                var model = Assert.IsType<List<Topic>>(viewResult.Model);
//                Assert.Equal(expectedTopics, model);
//            }
//        }

//        // Тесты для ProfileController
//        public class ProfileControllerTests
//        {
//            private readonly Mock<ProfileUserService> _mockUserService;
//            private readonly Mock<NotificationService> _mockNotificationService;
//            private readonly Mock<IConfiguration> _mockConfiguration;
//            private readonly ProfileController _controller;

//            public ProfileControllerTests()
//            {
//                _mockUserService = new Mock<ProfileUserService>();
//                _mockNotificationService = new Mock<NotificationService>();
//                _mockConfiguration = new Mock<IConfiguration>();
//                _controller = new ProfileController(
//                    _mockUserService.Object,
//                    _mockNotificationService.Object,
//                    _mockConfiguration.Object
//                );
//            }

//            [Fact]
//            public async Task Profile_ValidUserId_ReturnsView()
//            {
//                // Arrange
//                var userId = 1;
//                var expectedTopics = new List<Topic>();
//                var expectedComments = new List<comment>();
//                var expectedFavorites = new List<favoritelist>();

//                _mockUserService.Setup(x => x.GetTopicsByAuthorAsync(userId))
//                    .ReturnsAsync(expectedTopics);
//                _mockUserService.Setup(x => x.GetCommentsByAuthorAsync(userId))
//                    .ReturnsAsync(expectedComments);
//                _mockUserService.Setup(x => x.GetFavorite(userId))
//                    .ReturnsAsync(expectedFavorites);

//                // Act
//                var result = await _controller.profile();

//                // Assert
//                var viewResult = Assert.IsType<ViewResult>(result);
//                var model = Assert.IsType<Profile.Profile>(viewResult.Model);
//                Assert.Equal(expectedTopics, model.TopicsList);
//                Assert.Equal(expectedComments, model.CommentsList);
//                Assert.Equal(expectedFavorites, model.FavoritesList);
//            }

//            [Fact]
//            public async Task UpdateProfile_ValidData_RedirectsToSettings()
//            {
//                // Arrange
//                var model = new ProfileSettingsViewModel();
//                var avatar = new Mock<IFormFile>().Object;
//                var banner = new Mock<IFormFile>().Object;

//                _mockConfiguration.Setup(x => x["Supabase:Url"]).Returns("https://test.com");
//                _mockConfiguration.Setup(x => x["Supabase:Key"]).Returns("test-key");

//                // Act
//                var result = await _controller.UpdateProfile(model, avatar, banner, null, null);

//                // Assert
//                var redirectResult = Assert.IsType<RedirectToActionResult>(result);
//                Assert.Equal("settings", redirectResult.ActionName);
//            }
//        }
//    }
//} 