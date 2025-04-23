using Entities.Models;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using Shared.Options;

namespace Repository;

public static class DbInitializer
{
    public static async Task InitializeAsync(
        RepositoryContext context,
        FileStorageOptions fileStorageOptions)
    {
        if (await context.Users.AnyAsync() || await context.Comments.AnyAsync())
        {
            return;
        }

        #region Files preparing section

        string imageFolder = "images";
        //string textFileFolder = "textFiles";

        string[] imageFileNames = Directory.GetFiles(
            Path.Combine(fileStorageOptions.WebRootPath, imageFolder))
            .Select(file =>
            {
                string fileName = Path.GetFileName(file);

                return fileName;
            })
            .ToArray();


        string textFileNameLorem = "lorem.txt";

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
        var comment1 = new Comment()
        {
            Text = "Hello, world! This is the first comment.",
            CreationDate = DateTime.Now,
            ImageFileName = imageFileNames[0],
            TextFileName = textFileNameLorem,
            User = users[0],
            Parent = null,
            Replies = new List<Comment>()
            {
                new Comment()
                {
                    Text = "This is a reply to the first comment.",
                    CreationDate = DateTime.Now.AddDays(1),
                    ImageFileName = imageFileNames[1],
                    TextFileName = textFileNameLorem,
                    User = users[1],
                    Replies = new List<Comment>()
                    {
                        new Comment()
                        {
                            Text = "This is a reply to the reply to the first comment.",
                            CreationDate = DateTime.Now.AddDays(2),
                            ImageFileName = imageFileNames[2],
                            User = users[2],
                            Replies = new List<Comment>()
                            {
                                new Comment()
                                {
                                    Text = "This is a reply to the reply to the reply to the first comment.",
                                    CreationDate = DateTime.Now.AddDays(2.5),
                                    TextFileName = textFileNameLorem,
                                    User = users[3],
                                    Replies = new List<Comment>()
                                    {
                                        new Comment()
                                        {
                                            Text = "This is a reply to the reply to the reply to the reply to the first comment.",
                                            CreationDate = DateTime.Now.AddDays(2.6),
                                            User = users[4],
                                            Replies = new List<Comment>()
                                            {
                                                new Comment()
                                                {
                                                    Text = "This is a reply to the reply to the reply to the reply to the reply to the first comment.",
                                                    ImageFileName = imageFileNames[3],
                                                    CreationDate = DateTime.Now.AddDays(2.7),
                                                    User = users[0],
                                                    Replies = new List<Comment>()
                                                    {
                                                        new Comment()
                                                        {
                                                            Text = "This is a reply to the reply to the reply to the reply to the reply to the reply to the first comment.",
                                                            CreationDate = DateTime.Now.AddDays(2.8),
                                                            ImageFileName = imageFileNames[5],
                                                            TextFileName = textFileNameLorem,
                                                            User = users[1],
                                                        }
                                                    },
                                                }
                                            },
                                        }
                                    },
                                }
                            }
                        },
                        new Comment()
                        {
                            Text = "This is a reply to the reply to the first comment.",
                            CreationDate = DateTime.Now.AddDays(1.5),
                            User = users[3],
                            Replies = new List<Comment>()
                            {
                                new Comment()
                                {
                                    Text = "Some other comment.",
                                    CreationDate = DateTime.Now.AddDays(4),
                                    ImageFileName = imageFileNames[0],
                                    TextFileName = textFileNameLorem,
                                    User = users[3],
                                }
                            },
                        }
                    },
                }
            }
        };


        List<Comment> comments1 = new();
        //for (int i = 0; i < 10; i++)
        for (int i = 0; i < 2; i++)
        {
            int nextUserIndex = Random.Shared.Next(0, users.Count - 1);

            DateTime creationDate = CreateRandomDateTime();

            string? imageFileName = i % 3 == 0 ? imageFileNames[i % imageFileNames.Length] : null;
            string? textFileName = i % 2 == 0 ? textFileNameLorem : null;

            comments1.Add(new Comment()
            {
                Text = $"Hello, world! This is comment number {i + 1}.",
                CreationDate = creationDate,
                ImageFileName = imageFileName,
                TextFileName = textFileName,
                User = users[nextUserIndex],
                Parent = null,
                Replies = new List<Comment>()
                {
                    new Comment()
                    {
                        Text = $"This is a reply to comment number {i + 1}.",
                        CreationDate = creationDate.AddHours(i * i),
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



        List<Comment> comments2 = new();
        for (int i = 0; i < 10; i++)
        {
            int nextUserIndex = Random.Shared.Next(0, users.Count - 1);

            DateTime creationDate = CreateRandomDateTime();

            string? imageFileName = i % 4 == 0 ? imageFileNames[i % imageFileNames.Length] : null;
            string? textFileName = i % 5 == 0 ? textFileNameLorem : null;

            comments1.Add(new Comment()
            {
                Text = $"Hello, world! This is comment number {i + 1}.",
                CreationDate = creationDate,
                ImageFileName = imageFileName,
                TextFileName = textFileName,
                User = users[nextUserIndex],
                Parent = null,
                Replies = new List<Comment>()
                {
                    new Comment()
                    {
                        Text = $"This is a reply to comment number {i + 1}.",
                        CreationDate = creationDate.AddMinutes(i),
                        User = users[nextUserIndex],
                        Replies = new List<Comment>()
                        {
                            new Comment()
                            {
                                Text = $"This is a reply to the reply to comment number {i + 1}.",
                                CreationDate = creationDate.AddHours(i + 1),
                                User = users[nextUserIndex],
                            },
                            new Comment()
                            {
                                Text = $"This is a reply to the reply to the reply to comment number {i + 1}.",
                                CreationDate = creationDate.AddHours(i + 2),
                                ImageFileName = imageFileName,
                                TextFileName = textFileName,
                                User = users[nextUserIndex],
                            },
                        },
                    }
                },
            });
        }

        List<Comment> comments3 = new();
        for (int i = 0; i < 100; i++)
        {
            int nextUserIndex = Random.Shared.Next(0, users.Count - 1);

            DateTime creationDate = CreateRandomDateTime();

            string? imageFileName = i % 6 == 0 ? imageFileNames[i % imageFileNames.Length] : null;
            string? textFileName = i % 7 == 0 ? textFileNameLorem : null;

            comments3.Add(new Comment()
            {
                Text = $"Some Random comment {i + 1}.",
                CreationDate = creationDate,
                ImageFileName = imageFileName,
                TextFileName = textFileName,
                User = users[nextUserIndex],
                Parent = null,
            });
        }


        List<Comment> allComments = comments1
            .Concat(comments2)
            .Concat(comments3)
            .ToList();

        allComments = allComments
            .Shuffle()
            .ToList();

        allComments.Insert(0, comment1);

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
