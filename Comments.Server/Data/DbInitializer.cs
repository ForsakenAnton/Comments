using Comments.Server.Data.Entities;
using Microsoft.EntityFrameworkCore;
using MoreLinq;

namespace Comments.Server.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(
        CommentsDbContext context,
        IWebHostEnvironment webHostEnvironment)
    {
        if (await context.Users.AnyAsync() || await context.Comments.AnyAsync())
        {
            return;
        }

        #region Files preparing section

        string applicationUrl = webHostEnvironment.EnvironmentName == "Development" ?
            "https://localhost:7092" :
            "https://qwerty123";

        string imageFolder = "images";
        string textFileFolder = "textFiles";

        string[] imageServerPaths = Directory.GetFiles(
            Path.Combine(webHostEnvironment.WebRootPath, imageFolder))
            .Select(file =>
            {
                string fileName = Path.GetFileName(file);

                return $"{applicationUrl}/{imageFolder}/{fileName}";
            })
            .ToArray();


        string textFileServerPath = $"{applicationUrl}/{textFileFolder}/lorem.txt";

        #endregion


        #region User creation

        var user1 = new User()
        {
            UserName = "John Doe",
            Email = "M0H6g@example.com",
            HomePage = "https://example.com",
        };

        var user2 = new User()
        {
            UserName = "Jane Doe",
            Email = "jane@me.com",
            HomePage = "https://example2.com",
        };

        var user3 = new User()
        {
            UserName = "Bob",
            Email = "bob@me.com",
            HomePage = "https://example3.com",
        };

        var user4 = new User()
        {
            UserName = "Alice",
            Email = "alice@me.com",
            HomePage = "https://example4.com",
        };

        var user5 = new User()
        {
            UserName = "Eve",
            Email = "eve@me.com",
            HomePage = "https://example5.com",
        };

        List<User> users = [user1, user2, user3, user4, user5];


        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();
        #endregion

        #region Comment creation
        //var comment1 = new Comment()
        //{
        //    Text = "Hello, world! This is the first comment.",
        //    CreationDate = DateTime.Now,
        //    ImageFile = imageServerPaths[0],
        //    TextFile = textFileServerPath,
        //    User = user1,
        //    Parent = null,
        //    Replies = new List<Comment>()
        //    {
        //        new Comment()
        //        {
        //            Text = "This is a reply to the first comment.",
        //            CreationDate = DateTime.Now.AddDays(1),
        //            ImageFile = imageServerPaths[1],
        //            TextFile = textFileServerPath,
        //            User = user2,
        //            Replies = new List<Comment>()
        //            {
        //                new Comment()
        //                {
        //                    Text = "This is a reply to the reply to the first comment.",
        //                    CreationDate = DateTime.Now.AddDays(2),
        //                    ImageFile = imageServerPaths[2],
        //                    User = user3,
        //                    Replies = new List<Comment>()
        //                    {
        //                        new Comment()
        //                        {
        //                            Text = "This is a reply to the reply to the reply to the first comment.",
        //                            CreationDate = DateTime.Now.AddDays(2.5),
        //                            TextFile = textFileServerPath,
        //                            User = user4,
        //                            Replies = new List<Comment>()
        //                            {
        //                                new Comment()
        //                                {
        //                                    Text = "This is a reply to the reply to the reply to the reply to the first comment.",
        //                                    CreationDate = DateTime.Now.AddDays(2.6),
        //                                    User = user5,
        //                                    Replies = new List<Comment>()
        //                                    {
        //                                        new Comment()
        //                                        {
        //                                            Text = "This is a reply to the reply to the reply to the reply to the reply to the first comment.",
        //                                            ImageFile = imageServerPaths[3],
        //                                            CreationDate = DateTime.Now.AddDays(2.7),
        //                                            User = user1,
        //                                            Replies = new List<Comment>()
        //                                            {
        //                                                new Comment()
        //                                                {
        //                                                    Text = "This is a reply to the reply to the reply to the reply to the reply to the reply to the first comment.",
        //                                                    CreationDate = DateTime.Now.AddDays(2.8),
        //                                                    ImageFile = imageServerPaths[5],
        //                                                    TextFile = textFileServerPath,
        //                                                    User = user2,
        //                                                }
        //                                            },
        //                                        }
        //                                    },
        //                                }
        //                            },
        //                        }
        //                    },
        //                },
        //                new Comment()
        //                {
        //                    Text = "This is a reply to the reply to the first comment.",
        //                    CreationDate = DateTime.Now.AddDays(1.5),
        //                    User = user4,
        //                    Replies = new List<Comment>()
        //                    {
        //                        new Comment()
        //                        {
        //                            Text = "This is a reply to the reply to the reply to the first comment.",
        //                            CreationDate = DateTime.Now.AddDays(4),
        //                            ImageFile = imageServerPaths[9],
        //                            TextFile = textFileServerPath,
        //                            User = user5,
        //                        }
        //                    },
        //                }
        //            },
        //        }
        //    }
        //};


        List<Comment> comments1 = new();
        //for (int i = 0; i < 10; i++)
        for (int i = 0; i < 10; i++)
        {
            int nextUserIndex = Random.Shared.Next(0, users.Count - 1);

            DateTime creationDate = CreateRandomDateTime();

            string? imageFile = i % 3 == 0 ? imageServerPaths[i % imageServerPaths.Length] : null;
            string? textFile = i % 2 == 0 ? textFileServerPath : null;

            comments1.Add(new Comment()
            {
                Text = $"Hello, world! This is comment number {i + 1}.",
                CreationDate = creationDate,
                ImageFile = imageFile,
                TextFile = textFile,
                User = users[nextUserIndex],
                Parent = null,
                Replies = new List<Comment>()
                {
                    new Comment()
                    {
                        Text = $"This is a reply to comment number {i + 1}.",
                        CreationDate = creationDate.AddMinutes(i * i),
                        User = users[nextUserIndex],
                        Replies = new List<Comment>()
                        {
                            new Comment()
                            {
                                Text = $"This is a reply to reply number {i + 1}.",
                                CreationDate = creationDate.AddMinutes(i * i),
                                User = users[nextUserIndex],
                            }
                        }
                    },
                    new Comment()
                    {
                        Text = $"This is a reply to comment number {i + 1}.",
                        CreationDate = creationDate.AddMinutes(i * i),
                        User = users[nextUserIndex],
                    }
                },
            });
        }

        //List<Comment> comments2 = new();
        //for (int i = 0; i < 10; i++)
        //{
        //    int nextUserIndex = Random.Shared.Next(0, users.Count - 1);

        //    DateTime creationDate = CreateRandomDateTime();

        //    string? imageFile = i % 4 == 0 ? imageServerPaths[i % imageServerPaths.Length] : null;
        //    string? textFile = i % 5 == 0 ? textFileServerPath : null;

        //    comments1.Add(new Comment()
        //    {
        //        Text = $"Hello, world! This is comment number {i + 1}.",
        //        CreationDate = creationDate,
        //        ImageFile = imageFile,
        //        TextFile = textFile,
        //        User = users[nextUserIndex],
        //        Parent = null,
        //        Replies = new List<Comment>()
        //        {
        //            new Comment()
        //            {
        //                Text = $"This is a reply to comment number {i + 1}.",
        //                CreationDate = creationDate.AddMinutes(i),
        //                User = users[nextUserIndex],
        //                Replies = new List<Comment>()
        //                {
        //                    new Comment()
        //                    {
        //                        Text = $"This is a reply to the reply to comment number {i + 1}.",
        //                        CreationDate = creationDate.AddHours(i + 1),
        //                        User = users[nextUserIndex],
        //                    },
        //                    new Comment()
        //                    {
        //                        Text = $"This is a reply to the reply to the reply to comment number {i + 1}.",
        //                        CreationDate = creationDate.AddHours(i + 2),
        //                        ImageFile = imageFile,
        //                        TextFile = textFile,
        //                        User = users[nextUserIndex],
        //                    },
        //                },
        //            }
        //        },
        //    });
        //}

        //List<Comment> comments3 = new();
        //for (int i = 0; i < 100; i++)
        //{
        //    int nextUserIndex = Random.Shared.Next(0, users.Count - 1);

        //    DateTime creationDate = CreateRandomDateTime();

        //    string? imageFile = i % 6 == 0 ? imageServerPaths[i % imageServerPaths.Length] : null;
        //    string? textFile = i % 7 == 0 ? textFileServerPath : null;

        //    comments3.Add(new Comment()
        //    {
        //        Text = $"Some Random comment {i + 1}.",
        //        CreationDate = creationDate,
        //        ImageFile = imageFile,
        //        TextFile = textFile,
        //        User = users[nextUserIndex],
        //        Parent = null,
        //    });
        //}


        List<Comment> allComments = comments1
            //.Concat(comments2)
            //.Concat(comments3)
            .ToList();

        allComments = allComments
            .Shuffle()
            .ToList();

        //allComments.Insert(0, comment1);

        await context.Comments.AddRangeAsync(allComments);
        await context.SaveChangesAsync();

        #endregion
    }

    private static DateTime CreateRandomDateTime()
    {
        return DateTime.Now
            .AddYears(Random.Shared.Next(-100, 100));
            //.AddDays(Random.Shared.Next(-100, 100))
            //.AddMinutes(Random.Shared.Next(-1000, 1000))
            //.AddSeconds(Random.Shared.Next(-1000, 1000));
    }
}
